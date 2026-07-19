using UnityEngine;
using UnityEngine.AI;

namespace DumbTrollface.Waypoints
{
    public class WaypointTraveler : MonoBehaviour
    {
        public enum TravelMode
        {
            Simple,
            NavMeshAgent
        }

        public enum EndMode
        {
            Stop,
            Loop,
            PingPong
        }

        public enum TravelDirection
        {
            Forward = 1,
            Backward = -1
        }

        [Header("Path")]

        [SerializeField]
        private Waypoints _waypoints;

        [SerializeField]
        [Tooltip("What should the traveler do when it reached the end.")]
        private EndMode _endMode = EndMode.Stop;

        [SerializeField]
        [Tooltip("This field shows the current waypoint index for debugging. It can also be used to set the starting target waypoint.")]
        private int _currentWaypointIndex = 0;

        [Header("Movement")]

        [SerializeField]
        private TravelMode _travelMode = TravelMode.Simple;

        [SerializeField]
        [Tooltip("The NavMeshAgent to be used. Only needed when travel mode is set to NavMeshAgent\n" +
            "NOTE: This Script does not override any values of the agent. Values like the movement speed need to be set in the agent.")]
        private NavMeshAgent _agent;

        [SerializeField]
        private float _moveSpeed = 2.0f;

        [SerializeField]
        [Tooltip("Determines the radius for when a waypoint is considered as reached.")]
        private float _waypointRadius = 0.5f;

        [SerializeField]
        [Tooltip("Turning speed of the traveler to face the next waypoint.")]
        private float _turnSpeedDegrees = 180.0f;

        [SerializeField]
        [Tooltip("Threshold for determining when to start walking when turning")]
        private float _facingThresholdDegrees = 3.0f;

        [SerializeField]
        [Tooltip("The initial travel direction over the waypoints. Also shows the current direction during play mode.")]
        private TravelDirection _travelDirection = TravelDirection.Forward;

        void Update()
        {
            if (_waypoints == null || _waypoints.Count == 0)
                return;

            Vector3 target = _waypoints.GetWorldPoint(_currentWaypointIndex);
            Vector3 toTarget = target - transform.position;

            if (HasReachedTarget(toTarget))
            {
                AdvanceToNextWaypoint();
                return;
            }

            if (_travelMode == TravelMode.Simple)
            {
                Quaternion targetRotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);

                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    _turnSpeedDegrees * Time.deltaTime);

                float angle = Quaternion.Angle(transform.rotation, targetRotation);

                if (angle <= _facingThresholdDegrees)
                {
                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        target,
                        _moveSpeed * Time.deltaTime);
                }
            }
        }

        /// <summary>
        /// Determines if the current waypoint target has been reached
        /// </summary>
        /// <param name="toTarget">Vector from the traveler to the current target</param>
        /// <returns>true if the current target has been reached, otherwise false</returns>
        private bool HasReachedTarget(Vector3 toTarget)
        {
            switch (_travelMode)
            {
                case TravelMode.Simple:
                    return toTarget.sqrMagnitude <= _waypointRadius * _waypointRadius;
                case TravelMode.NavMeshAgent:
                    // Check if the agent is actively calculating or moving on a path
                    if (!_agent.pathPending)
                    {
                        // Check if the remaining distance is less than or equal to the stopping distance
                        if (_agent.remainingDistance <= _agent.stoppingDistance)
                        {
                            // Check if the agent has completely stopped moving or doesn't have a path
                            if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                default: return false;
            }
            
        }

        /// <summary>
        /// Sets the next waypoint as the target, disables the component if there is no next waypoint
        /// </summary>
        private void AdvanceToNextWaypoint()
        {
            if (_waypoints == null || _waypoints.Count == 0)
                return;

            int nextIndex = _currentWaypointIndex + (int)_travelDirection;

            if (nextIndex >= _waypoints.Count || nextIndex < 0)
            {
                switch (_endMode)
                {
                    case EndMode.Stop:
                        if (_travelMode == TravelMode.NavMeshAgent)
                        {
                            _agent.isStopped = true;
                        }

                        enabled = false;
                        break;
                    case EndMode.Loop:
                        _currentWaypointIndex = (nextIndex + _waypoints.Count) % _waypoints.Count;
                        break;
                    case EndMode.PingPong:
                        _travelDirection = (TravelDirection)(-(int)_travelDirection); // TODO is there a better way to do this?
                        _currentWaypointIndex += (int)_travelDirection;
                        break;
                }
            }
            else
            {
                _currentWaypointIndex = nextIndex;
            }

            if (_travelMode == TravelMode.NavMeshAgent)
            {
                _agent.SetDestination(_waypoints.GetWorldPoint(_currentWaypointIndex));
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_waypoints == null || _waypoints.Count == 0)
                return;

            Vector3 target = _waypoints.GetWorldPoint(_currentWaypointIndex);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(target, _waypointRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target);
        }
#endif
    }
}
