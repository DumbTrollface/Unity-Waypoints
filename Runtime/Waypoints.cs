using System.Collections.Generic;
using UnityEngine;

namespace DumbTrollface.Waypoints
{
    public class Waypoints : MonoBehaviour
    {
        /// <summary>
        /// Determines if the path is considered as closed (connection from the last to first waypoint) or open.
        /// </summary>
        [SerializeField]
        [Tooltip("Should the path be considered as closed (connection from the last to first waypoint) or open.")]
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

        /// <summary>
        /// Return a readonly List of the waypoints.
        /// </summary>
        public IReadOnlyList<Vector3> WaypointsList => waypoints;

        /// <summary>
        /// Returns the number of waypoints in this path
        /// </summary>
        public int Count => waypoints.Count;

        /// <summary>
        /// Returns the state of the closed setting
        /// </summary>
        public bool Closed => closed;

        /// <summary>
        /// Returns the position of a waypoint in local space
        /// </summary>
        /// <param name="index">index of the waypoint</param>
        /// <returns>position in local space</returns>
        public Vector3 GetLocalPoint(int index) => waypoints[index];

        /// <summary>
        /// Returns the position of a waypoint in world space
        /// </summary>
        /// <param name="index">index of the waypoint</param>
        /// <returns>position in world space</returns>
        public Vector3 GetWorldPoint(int index) => transform.TransformPoint(waypoints[index]);
    }

}
