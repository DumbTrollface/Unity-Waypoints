using UnityEngine;

namespace DumbTrollface.Waypoints
{
    public class WaypointTraveler : MonoBehaviour
    {
        [Header("Path")]

        [SerializeField]
        private Waypoints _waypoints;

        [SerializeField]
        [Tooltip("Should the traveler consider the path as a loop even if the path is not closed?\n" +
            "If the path is declared as closed, this option has no effect.")]
        private bool _loop = true;

        [SerializeField]
        [Tooltip("This field shows the current waypoint index for debugging. It can also be used to set the starting target waypoint.")]
        private int _currentWaypointIndex = 0;

        [Header("Movement")]

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

        /// <summary>
        /// Determines if the current waypoint target has been reached
        /// </summary>
        /// <param name="toTarget">Vector from the traveler to the current target</param>
        /// <returns>true if the current target has been reached, otherwise false</returns>
        private bool HasReachedTarget(Vector3 toTarget)
        {
            return toTarget.sqrMagnitude <= _waypointRadius * _waypointRadius;
        }

        /// <summary>
        /// Sets the next waypoint as the target, disables the component if there is no next waypoint
        /// </summary>
        private void AdvanceToNextWaypoint()
        {
            if (_waypoints == null || _waypoints.Count == 0)
                return;

            int nextIndex = _currentWaypointIndex + 1;

            if (nextIndex >= _waypoints.Count)
            {
                if (_loop || _waypoints.Closed)
                {
                    nextIndex = 0;
                }
                else
                {
                    nextIndex = _waypoints.Count - 1;

                    enabled = false;
                    return;
                }
            }

            _currentWaypointIndex = nextIndex;
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
