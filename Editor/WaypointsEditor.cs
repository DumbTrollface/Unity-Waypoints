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

        /// <summary>
        /// The property that holds the closed bool
        /// </summary>
        private SerializedProperty closedProp;

        /// <summary>
        /// The list that is displayed in the inspector
        /// </summary>
        private ReorderableList waypointList;

        /// <summary>
        /// The object that this editor is displaying and modifying
        /// </summary>
        private Waypoints wp;

        /// <summary>
        /// Should only the handles of the currently selected waypoint be shown?
        /// </summary>
        private bool showOnlySelected = false;

        private void OnEnable()
        {
            // Initialize all objects and properties
            wp = target as Waypoints;
            waypointsProp = serializedObject.FindProperty("waypoints");
            closedProp = serializedObject.FindProperty("closed");

            // Create a reorderable list that shows the waypoints
            waypointList = new ReorderableList(serializedObject, waypointsProp, true, true, true, true)
            {
                multiSelect = false,
                drawHeaderCallback = rect =>
                    {
                        EditorGUI.LabelField(rect, "Waypoints");
                    },
                drawElementCallback = DrawWaypointListElement,
                onAddCallback = list =>
                    {
                        int selected = GetSelectedIndex();
                        if (selected == -1)
                        {
                            AddWaypointAt(0);
                        }
                        else
                        {
                            AddWaypointAt(selected + 1);
                        }
                    },
                onSelectCallback = list => { SceneView.RepaintAll(); }
            };
        }

        private void OnSceneGUI()
        {
            serializedObject.Update();
            int count = waypointsProp.arraySize;
            int selected = GetSelectedIndex();

            // Iterate over all waypoints
            for (int i = 0; i < count; i++)
            {
                if (showOnlySelected && i != selected) continue;

                // Read the property and convert the position from local to world space
                SerializedProperty pointProp = waypointsProp.GetArrayElementAtIndex(i);
                Vector3 worldPos = wp.transform.TransformPoint(pointProp.vector3Value);

                // Create a label to make it easier to see which point is which
                float offset = HandleUtility.GetHandleSize(worldPos) * 0.2f;
                Handles.Label(worldPos + Vector3.up * offset + Vector3.right * offset, $"P{i}");

                EditorGUI.BeginChangeCheck();
                // Create a handle that can move an individual waypoint in the scene view
                Vector3 newPos = Handles.PositionHandle(worldPos, Tools.pivotRotation == PivotRotation.Local ? wp.transform.rotation : Quaternion.identity);

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

            EditorGUILayout.LabelField("Display Settings", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            showOnlySelected = EditorGUILayout.Toggle("show only selected", showOnlySelected);
            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }

            EditorGUILayout.Space();

            waypointList.DoLayoutList();

            EditorGUILayout.Space();

            if (GUILayout.Button("Deselect"))
            {
                waypointList.ClearSelection();
                SceneView.RepaintAll();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Insert Front"))
            {
                AddWaypointAt(0);
            }
            if (GUILayout.Button("Insert Back"))
            {
                AddWaypointAt(waypointList.count);
            }

            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws a single element in the waypoint list inspector
        /// </summary>
        /// <param name="rect">the rectangle in which the element is rendered</param>
        /// <param name="index">index of the list element being rendered</param>
        /// <param name="isActive">true if the user has clicked on this element</param>
        /// <param name="isFocused">true if the inspector window currently has active mouse/keyboard focus</param>
        private void DrawWaypointListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = waypointsProp.GetArrayElementAtIndex(index);

            const float labelWidth = 70f;
            const float spacing = 4f;

            Rect labelRect = new(
                rect.x,
                rect.y,
                labelWidth,
                rect.height);

            Rect fieldRect = new(
                labelRect.xMax + spacing,
                rect.y,
                rect.width - labelWidth - 2f * spacing,
                rect.height);

            EditorGUI.LabelField(labelRect, $"Point {index}");
            EditorGUI.PropertyField(fieldRect, element, GUIContent.none);
        }

        /// <summary>
        /// Inserts a waypoint at the specified index into the list.
        /// </summary>
        /// <param name="index">index of the new element</param>
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

        private int GetSelectedIndex()
        {
            return waypointList.selectedIndices.Count == 1 ? waypointList.selectedIndices[0] : -1;
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
