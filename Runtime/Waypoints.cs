using System.Collections.Generic;
using UnityEngine;

namespace DumbTrollface.Waypoints
{
    public class Waypoints : MonoBehaviour
    {
        /// <summary>
        /// Determines if the path is displayed as closed (line from the last to first waypoint) or open.
        /// </summary>
        [SerializeField][Tooltip("Should the path is displayed as closed (line from the last to first waypoint) or open")]
        private bool closed = false;

        /// <summary>
        /// List of all waypoints filled with some initial waypoints.
        /// </summary>
        [SerializeField]
        private List<Vector3> waypoints = new()
        {
            new Vector3(-2f, 0f, -2f),
            new Vector3( 2f, 0f, -2f),
            new Vector3( 2f, 0f,  2f),
            new Vector3(-2f, 0f,  2f)
        };

        public IReadOnlyList<Vector3> WaypointsList => waypoints;
        public int Count => waypoints.Count;
        public bool Closed => closed;

        private void OnDrawGizmos()
        {
            // Skip if there is nothing to visualize
            if (waypoints == null || waypoints.Count == 0)
                return;

            // Setting the color of the gizmos
            Gizmos.color = Color.yellow;

            // Draw spheres for the waypoints
            for (int i = 0; i < waypoints.Count; i++)
            {
                // Transform local to world space
                Vector3 p = transform.TransformPoint(waypoints[i]);
                Gizmos.DrawSphere(p, 0.15f);
            }

            // Connect waypoints with lines
            int segmentCount = closed ? waypoints.Count : waypoints.Count - 1;
            for (int i = 0; i < segmentCount; i++)
            {
                // Draw a line from this waypoint to the next
                int next = (i + 1) % waypoints.Count;
                Vector3 a = transform.TransformPoint(waypoints[i]);
                Vector3 b = transform.TransformPoint(waypoints[next]);
                Gizmos.DrawLine(a, b);
            }
        }
    }

}
