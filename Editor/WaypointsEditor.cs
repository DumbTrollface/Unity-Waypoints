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
                Handles.Label(worldPos, $"P{i}");

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
    }
}
