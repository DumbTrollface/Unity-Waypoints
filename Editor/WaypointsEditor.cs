using UnityEditor;
using UnityEngine;

namespace DumbTrollface.Waypoints
{
    [CustomEditor(typeof(Waypoints))]
    public class WaypointsEditor : Editor
    {
        /// <summary>
        /// The property that holds the waypoint list
        /// </summary>
        private SerializedProperty waypointsProp;

        private void OnEnable()
        {
            waypointsProp = serializedObject.FindProperty("waypoints");
        }

        private void OnSceneGUI()
        {
            serializedObject.Update();
            int count = waypointsProp.arraySize;
            Waypoints waypoints = (Waypoints) target;

            // Iterate over all waypoints
            for (int i = 0; i < count; i++)
            {
                // Read the property and convert the position from local to world space
                SerializedProperty pointProp = waypointsProp.GetArrayElementAtIndex(i);
                Vector3 worldPos = waypoints.transform.TransformPoint(pointProp.vector3Value);

                // Create a label to make it easier to see which point is which
                float offset = HandleUtility.GetHandleSize(worldPos) * 0.2f;
                Handles.Label(worldPos + Vector3.up * offset + Vector3.right * offset, $"P{i}");

                EditorGUI.BeginChangeCheck();
                // Create a handle that can move an individual waypoint in the scene view
                Vector3 newPos = Handles.PositionHandle(worldPos, Quaternion.identity);

                // If the handle has been manipulated, we write the new value back to the property and apply the changes to the object
                if (EditorGUI.EndChangeCheck())
                {
                    Vector3 newLocal = waypoints.transform.InverseTransformPoint(newPos);
                    pointProp.vector3Value = newLocal;

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
        private static void DrawGizmos(Waypoints waypoints, GizmoType type)
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
                Vector3 p = waypoints.transform.TransformPoint(waypoints.WaypointsList[i]);
                Gizmos.DrawSphere(p, 0.15f);
            }

            // Connect waypoints with lines
            int segmentCount = waypoints.Closed ? waypoints.Count : waypoints.Count - 1;
            for (int i = 0; i < segmentCount; i++)
            {
                // Draw a line from this waypoint to the next
                int next = (i + 1) % waypoints.Count;
                Vector3 a = waypoints.transform.TransformPoint(waypoints.WaypointsList[i]);
                Vector3 b = waypoints.transform.TransformPoint(waypoints.WaypointsList[next]);
                Gizmos.DrawLine(a, b);
            }
        }
    }
}
