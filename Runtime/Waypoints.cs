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
    }

}
