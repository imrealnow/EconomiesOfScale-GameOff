using NavMeshPlus.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
public class NavMeshFixer : MonoBehaviour
{
    public NavMeshData navMeshReference;
    private NavMeshSurface navSurface;
    private NavMeshData navData = null;

    private void Awake()
    {
        navSurface = GetComponent<NavMeshSurface>();
        navSurface.navMeshData = navMeshReference;
    }

    [ContextMenu("Fix")]
    public void Fix()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            navSurface.BuildNavMesh();
            navData = navSurface.navMeshData;
            // save the navData as an asset
            string path = "Assets/" + transform.parent.name + "-NavMesh.asset";
#if UNITY_EDITOR
            AssetDatabase.CreateAsset(navData, path);
            //set navData to reference the asset
            navMeshReference = AssetDatabase.LoadAssetAtPath<NavMeshData>(path);
            // apply overrides from this component to the parent prefab
            PrefabUtility.ApplyPrefabInstance(transform.parent.gameObject, InteractionMode.AutomatedAction);
            AssetDatabase.SaveAssets();
#endif
        }
    }

    [ContextMenu("Set Nav Data")]
    public void SetNavData()
    {
        navSurface.navMeshData = navData;
    }

    [ContextMenu("Set Nav Data to null")]
    public void SetNavDataToNull()
    {
        navData = null;
        navSurface.navMeshData = null;
    }
}
