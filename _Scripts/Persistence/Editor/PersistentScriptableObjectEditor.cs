using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PersistentScriptableObject))]
public class PersistentScriptableObjectEditor : Editor
{
    private bool foldoutFields = false;

    public override void OnInspectorGUI()
    {
        PersistentScriptableObject pso = (PersistentScriptableObject)target;
        if (!pso.IsLoaded)
            pso.LoadData();

        // header buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Data"))
            pso.SaveData();
        if (GUILayout.Button("Load Data"))
            pso.LoadData();
        EditorGUILayout.EndHorizontal();

        // draws data template
        base.OnInspectorGUI();

        // draws data contents
        object[] changedValues = new object[pso.DataTemplate.Count];
        foldoutFields = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutFields, "Data");
        EditorGUI.BeginChangeCheck();
        if (pso.DataContainer.IsAssigned && foldoutFields)
        {
            for (int i = 0; i < pso.DataTemplate.Count; i++)
            {
                changedValues[i] = pso.DataContainer.data.Single(pair => pair.Key.Equals(pso.DataTemplate[i].key)).Value;
                changedValues[i] = DrawAssociatedField(pso.DataTemplate[i].type, pso.DataTemplate[i].key, changedValues[i]);
            }
        }
        if (EditorGUI.EndChangeCheck())
        {
            for (int i = 0; i < pso.DataTemplate.Count; i++)
            {
                KeyValuePair<string, object> pair = pso.DataContainer.data.Single(pair => pair.Key.Equals(pso.DataTemplate[i].key));
                for (int j = 0; j < pso.DataContainer.data.Count; j++)
                {
                    if (pair.Key.Equals(pso.DataContainer.data[j].Key))
                    {
                        Type valueType = SystemType.GetTypeFromEnum(pso.DataTemplate[j].type);
                        MethodInfo setDataMethod = pso.GetType().GetMethod("TrySetValue").MakeGenericMethod(valueType);
                        object[] parameters = { pso.DataTemplate[j].key, changedValues[j], false };
                        bool result = (bool)setDataMethod.Invoke(pso, parameters);
                        if (!result)
                            Debug.LogError("Data save failed");
                    }
                }
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private object DrawAssociatedField(SystemType.TypeEnum fieldType, string fieldName, object value)
    {
        if (value == null)
            return null;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(fieldName);
        switch (fieldType)
        {
            case SystemType.TypeEnum.Boolean:
                value = EditorGUILayout.Toggle((bool)value);
                break;
            case SystemType.TypeEnum.Integer:
                value = EditorGUILayout.IntField((int)value);
                break;
            case SystemType.TypeEnum.Float:
                value = EditorGUILayout.FloatField((float)value);
                break;
            case SystemType.TypeEnum.String:
                value = EditorGUILayout.TextField((string)value);
                break;
            case SystemType.TypeEnum.Vector2:
                value = EditorGUILayout.Vector2Field(fieldName, (Vector2)value);
                break;
            case SystemType.TypeEnum.Vector3:
                value = EditorGUILayout.Vector3Field(fieldName, (Vector3)value);
                break;
        }
        EditorGUILayout.EndHorizontal();
        return value;
    }
}
