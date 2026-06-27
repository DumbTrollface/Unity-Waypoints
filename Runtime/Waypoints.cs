using System.Collections.Generic;
using UnityEngine;

namespace DumbTrollface.Waypoints
{
    public class Waypoints : MonoBehaviour
    {
        /// <summary>
        /// List of all waypoints filled with some initial waypoints.
        /// </summary>
        [SerializeField]
        private List<Vector3> waypoints = new List<Vector3>
        {
            new Vector3(-2f, 0f, -2f),
            new Vector3( 2f, 0f, -2f),
            new Vector3( 2f, 0f,  2f),
            new Vector3(-2f, 0f,  2f)
        };
    }

}
