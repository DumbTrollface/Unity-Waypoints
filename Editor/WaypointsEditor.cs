using UnityEditor;
using UnityEditorInternal;
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
        private SerializedProperty closedProp;

        private ReorderableList waypointList;

        private Waypoints wp;

        private void OnEnable()
        {
            wp = target as Waypoints;
            waypointsProp = serializedObject.FindProperty("waypoints");
            closedProp = serializedObject.FindProperty("closed");

            waypointList = new ReorderableList(serializedObject, waypointsProp, true, true, true, true);
            waypointList.multiSelect = false;
            waypointList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Waypoints");
            };
            waypointList.drawElementCallback = DrawWaypointListElement;
            waypointList.onAddCallback = list =>
            {
                if (waypointList.selectedIndices.Count == 1)
                {
                    AddWaypointAt(waypointList.selectedIndices[0] + 1);
                }
                else
                {
                    Debug.LogWarning("No waypoint selected");
                }
            };
        }

        private void OnSceneGUI()
        {
            serializedObject.Update();
            int count = waypointsProp.arraySize;

            // Iterate over all waypoints
            for (int i = 0; i < count; i++)
            {
                // Read the property and convert the position from local to world space
                SerializedProperty pointProp = waypointsProp.GetArrayElementAtIndex(i);
                Vector3 worldPos = wp.transform.TransformPoint(pointProp.vector3Value);

                // Create a label to make it easier to see which point is which
                float offset = HandleUtility.GetHandleSize(worldPos) * 0.2f;
                Handles.Label(worldPos + Vector3.up * offset + Vector3.right * offset, $"P{i}");

                EditorGUI.BeginChangeCheck();
                // Create a handle that can move an individual waypoint in the scene view
                Vector3 newPos = Handles.PositionHandle(worldPos, Quaternion.identity);

                // If the handle has been manipulated, we write the new value back to the property and apply the changes to the object
                if (EditorGUI.EndChangeCheck())
                {
                    Vector3 newLocal = wp.transform.InverseTransformPoint(newPos);
                    pointProp.vector3Value = newLocal;

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);

            serializedObject.Update();

            EditorGUILayout.LabelField("Path Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(closedProp);

            EditorGUILayout.Space();

            waypointList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawWaypointListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = waypointsProp.GetArrayElementAtIndex(index);

            const float labelWidth = 70f;
            const float spacing = 4f;

            Rect labelRect = new Rect(
                rect.x,
                rect.y,
                labelWidth,
                rect.height);

            Rect fieldRect = new Rect(
                labelRect.xMax + spacing,
                rect.y,
                rect.width - labelWidth - 2f * spacing,
                rect.height);

            EditorGUI.LabelField(labelRect, $"Point {index}");
            EditorGUI.PropertyField(fieldRect, element, GUIContent.none);
        }

        private void AddWaypointAt(int index)
        {
            serializedObject.Update();

            waypointsProp.InsertArrayElementAtIndex(index);
            SerializedProperty inserted = waypointsProp.GetArrayElementAtIndex(index);

            Vector3 newPosition;
            int count = waypointsProp.arraySize;

            // Insert into empty or single element list
            if (count == 0 || count == 1)
            {
                newPosition = Vector3.forward * (count + 1);
            }

            // Insert between existing elements
            else if (index > 0 && index < count - 1)
            {
                Vector3 a = waypointsProp.GetArrayElementAtIndex(index - 1).vector3Value;
                Vector3 b = waypointsProp.GetArrayElementAtIndex(index + 1).vector3Value;
                newPosition = 0.5f * (a + b);
            }

            // Insert at the end
            else if (index > 0)
            {
                Vector3 last = waypointsProp.GetArrayElementAtIndex(index - 1).vector3Value;
                Vector3 prev = waypointsProp.GetArrayElementAtIndex(index - 2).vector3Value;
                newPosition = last + (last - prev);
            }

            // Insert at the front
            else
            {
                Vector3 next = waypointsProp.GetArrayElementAtIndex(1).vector3Value;
                newPosition = next - Vector3.forward;
            }

            inserted.vector3Value = newPosition;

            serializedObject.ApplyModifiedProperties();

            waypointList.Select(index);
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
