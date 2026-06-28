using UnityEngine;

namespace DumbTrollface.Waypoints
{
    public class WaypointTraveler : MonoBehaviour
    {
        [Header("Path")]
        [SerializeField] private Waypoints waypoints;
        [SerializeField] private bool loop = true;
        [SerializeField] private int currentWaypointIndex = 0;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2.0f;
        [SerializeField] private float waypointRadius = 0.2f;

        void Update()
        {
            Vector3 target = waypoints.GetWorldPoint(currentWaypointIndex);
            Vector3 toTarget = target - transform.position;

            if (HasReachedTarget(toTarget))
            {
                AdvanceToNextWaypoint();
                return;
            }

            if (toTarget.sqrMagnitude < 0.0001f)
                return;

            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        }

        private bool HasReachedTarget(Vector3 toTarget)
        {
            return toTarget.sqrMagnitude <= waypointRadius * waypointRadius;
        }

        private void AdvanceToNextWaypoint()
        {
            if (waypoints == null || waypoints.Count == 0)
                return;

            int nextIndex = currentWaypointIndex + 1;

            if (nextIndex >= waypoints.Count)
            {
                if (loop || waypoints.Closed)
                {
                    nextIndex = 0;
                }
                else
                {
                    nextIndex = waypoints.Count - 1;

                    enabled = false;
                    return;
                }
            }

            currentWaypointIndex = nextIndex;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (waypoints == null || waypoints.Count == 0)
                return;

            Vector3 target = waypoints.GetWorldPoint(currentWaypointIndex);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(target, waypointRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target);
        }
#endif
    }
}
