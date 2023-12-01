using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(SceneLoader))]
public class SceneLoaderEditor : Editor
{
    private SerializedProperty sceneLoadTriggers;

    private ReorderableList reorderableList;

    private void OnEnable()
    {
        sceneLoadTriggers = serializedObject.FindProperty("sceneLoadTriggers");

        reorderableList = new ReorderableList(serializedObject, sceneLoadTriggers, true, true, true, true)
        {
            drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Scene Load Triggers");
            },

            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    element,
                    GUIContent.none);
            },

            elementHeightCallback = (int index) =>
            {
                return EditorGUI.GetPropertyHeight(reorderableList.serializedProperty.GetArrayElementAtIndex(index));
            }
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, "sceneLoadTriggers", "m_Script");
        reorderableList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}


[CustomPropertyDrawer(typeof(SceneLoadTrigger))]
public class SceneLoadTriggerDrawer : PropertyDrawer
{
    private float propertyHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return property.isExpanded ? EditorGUIUtility.singleLineHeight * 3 : EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int oldIndentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw foldout
        Rect foldoutRect = new Rect(position.x + 10, position.y, 14, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none);

        // Draw label
        Rect labelRect = new Rect(position.x + 15, position.y, position.width - 15, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, GetSceneName(property.FindPropertyRelative("sceneIndex").intValue));

        if (property.isExpanded)
        {
            // Draw properties
            Rect fieldRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("triggerEvent"), new GUIContent("Trigger Event"));

            fieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative("sceneIndex"), new GUIContent("Scene Index"));

            propertyHeight = EditorGUIUtility.singleLineHeight * 3;
        }
        else
        {
            propertyHeight = EditorGUIUtility.singleLineHeight;
        }

        EditorGUI.EndProperty();
        EditorGUI.indentLevel = oldIndentLevel;
    }

    private string GetSceneName(int index)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(index);
        return System.IO.Path.GetFileNameWithoutExtension(path);
    }
}
