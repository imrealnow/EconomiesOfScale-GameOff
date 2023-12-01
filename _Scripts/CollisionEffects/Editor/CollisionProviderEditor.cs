using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CollisionProvider))]
public class CollisionProviderEditor : Editor
{
    private List<Type> collisionEffectTypes;
    private string[] collisionEffectTypeNames;
    private List<bool> foldouts = new List<bool>(); // To keep track of foldouts for each CollisionEffect

    private void OnEnable()
    {
        // Find all types that are subclasses of CollisionEffect and are not abstract
        collisionEffectTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(CollisionEffect)))
            .ToList();

        // Convert the list of types to a list of their names to display in the dropdown
        collisionEffectTypeNames = collisionEffectTypes.Select(t => t.Name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CollisionProvider provider = (CollisionProvider)target;

        if (provider.EffectSet == null)
        {
            if (GUILayout.Button("Create Effect Set"))
            {
                CreateNewEffectSet(provider);
            }
        }
        else
        {
            DrawEffects(provider);
            EditorGUILayout.Space();

            if (GUILayout.Button("Add Collision Effect"))
            {
                ShowAddEffectMenu(provider);
            }
        }

        // Ensure any changes to the list are saved
        if (GUI.changed)
        {
            EditorUtility.SetDirty(provider);
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void DrawEffects(CollisionProvider provider)
    {
        for (int i = 0; i < provider.EffectSet.effects.Count; i++)
        {
            if (foldouts.Count <= i)
            {
                foldouts.Add(false);
            }

            EditorGUILayout.BeginVertical("box"); // Start box

            EditorGUILayout.BeginHorizontal();
            foldouts[i] = EditorGUILayout.Foldout(foldouts[i], provider.EffectSet.effects[i].name);

            // Remove CollisionEffect
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_TreeEditor.Trash"), GUILayout.Width(30)))
            {
                RemoveEffect(provider, i);
                continue;
            }
            EditorGUILayout.EndHorizontal(); // End horizontal layout

            if (foldouts[i])
            {
                Editor collisionEffectEditor = CreateEditor(provider.EffectSet.effects[i]);
                collisionEffectEditor.OnInspectorGUI();
            }

            EditorGUILayout.EndVertical(); // End box
        }
    }

    private void ShowAddEffectMenu(CollisionProvider provider)
    {
        var menu = new GenericMenu();

        foreach (var type in collisionEffectTypes)
        {
            menu.AddItem(new GUIContent(type.Name), false, () => AddNewEffect(provider, type));
        }

        menu.ShowAsContext();
    }

    private void AddNewEffect(CollisionProvider provider, Type effectType)
    {
        ScriptableObject newEffect = CreateInstance(effectType) as CollisionEffect;
        newEffect.name = effectType.Name;

        if (provider.EffectSet.effects == null)
            provider.EffectSet.effects = new List<CollisionEffect>();

        provider.EffectSet.effects.Add(newEffect as CollisionEffect);

        AssetDatabase.AddObjectToAsset(newEffect, provider.EffectSet);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void RemoveEffect(CollisionProvider provider, int index)
    {
        var effectToRemove = provider.EffectSet.effects[index];
        provider.EffectSet.effects.RemoveAt(index);
        foldouts.RemoveAt(index);

        DestroyImmediate(effectToRemove, true); // Remove the sub-asset
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void CreateNewEffectSet(CollisionProvider provider)
    {
        // Prompt the user for a location to save the new EffectSet
        string path = EditorUtility.SaveFilePanelInProject(
            "Save New Effect Set",
            "NewEffectSet",
            "asset",
            "Please enter a file name to save the new effect set to"
        );

        // If the user cancels, the path will be empty
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        // Create a new instance of the EffectSet
        CollisionEffectSet effectSet = CreateInstance<CollisionEffectSet>();

        // Create the new asset at the specified path
        AssetDatabase.CreateAsset(effectSet, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Assign the new EffectSet to the CollisionProvider
        provider.EffectSet = effectSet;
        EditorUtility.SetDirty(provider);
        EditorUtility.SetDirty(effectSet); // Mark the new asset as dirty to ensure it gets saved
    }
}
