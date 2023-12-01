using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomEditor(typeof(TileBlock))]
public class TileBlockEditor : Editor
{
    BoxBoundsHandle bbh;
    TileBlock tileBlock;

    private void OnEnable()
    {
        bbh = new BoxBoundsHandle();
        bbh.axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y;
        bbh.handleColor = Color.green;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        tileBlock = (TileBlock)target;
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button(tileBlock.lockBounds ? "Edit bounds" : "Lock Bounds"))
        {
            tileBlock.lockBounds = !tileBlock.lockBounds;
            EditorUtility.SetDirty(target);
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Block"))
        {
            tileBlock.blockData = SaveTileBlock(tileBlock);
        }
        if (GUILayout.Button("Load Block"))
        {
            LoadTileBlock(tileBlock, tileBlock.blockData);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }


    protected virtual void OnSceneGUI()
    {

        // get initial variables
        tileBlock = (TileBlock)target;
        Grid grid = tileBlock.TileGrid;
        Bounds bounds = tileBlock.Bounds;
        Vector3 center = tileBlock.transform.position + (bounds.center * grid.cellSize.x);
        if (tileBlock.lockBounds)
        {
            Handles.DrawWireCube(center, bounds.size);
            return;
        }

        // set initial handle values
        bbh.center = center;
        bbh.size = bounds.size;

        // draw handle
        EditorGUI.BeginChangeCheck();
        bbh.DrawHandle();
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(tileBlock, "Change Bounds");

            // snap values to grid cells
            Vector3 newSize = new Vector3(
                    SnapFloatToStep(bbh.size.x, grid.cellSize.x),
                    SnapFloatToStep(bbh.size.y, grid.cellSize.y),
                    1
                );
            Vector3 centerDelta = (bbh.center - center).normalized;
            // keeps center in place unless size has change
            // if size has changed then move center in the same direction
            // the handle's center moved when it was resized.
            Vector3 newCenter = bounds.center + (newSize - bounds.size).magnitude * 0.5f * centerDelta;
            tileBlock.Bounds = new Bounds(newCenter, newSize);

        }
    }

    protected float SnapFloatToStep(float value, float step)
    {
        // rounds value to the nearest multiple of step
        float mod = value % step;
        if (mod < step * 0.5f)
            return value - mod;
        else
            return value + step - mod;
    }

    public static TileBlockData SaveTileBlock(TileBlock tileBlock)
    {
        if (!AssetDatabase.IsValidFolder("Assets/TileBlocks"))
            AssetDatabase.CreateFolder("Assets", "TileBlocks");

        bool isNew = tileBlock.blockData == null;
        // make new data file if it doesn't exist
        TileBlockData data = tileBlock.blockData ?? CreateInstance<TileBlockData>();

        data.bounds = tileBlock.ConvertBounds(tileBlock.Bounds);
        data.tiles = tileBlock.TileBases.ToList();

        if (isNew)
        {
            string name = tileBlock.name + data.bounds.size.x + "x" + data.bounds.size.y;
            string constrainedName = tileBlock.name;
            if (AssetDatabase.LoadAssetAtPath("Assets/TileBlocks/" + name + ".asset", typeof(TileBlockData)) != null)
            {
                int i = 1;
                while (AssetDatabase.LoadAssetAtPath("Assets/TileBlocks/" + name + i + ".asset", typeof(TileBlockData)) != null)
                {
                    i++;
                }
                name += "(" + i + ")";
            }
            if (AssetDatabase.LoadAssetAtPath("Assets/TileBlocks/" + constrainedName + ".asset", typeof(ConstrainedBlock)) == null)
            {
                ConstrainedBlock constrainedBlock = CreateInstance<ConstrainedBlock>();
                constrainedBlock.blockData = data;
                AssetDatabase.CreateAsset(constrainedBlock, "Assets/TileBlocks/" + constrainedName + ".asset");
            }
            AssetDatabase.CreateAsset(data, "Assets/TileBlocks/" + name + ".asset");
        }
        AssetDatabase.SaveAssets();
        return data;
    }

    public static void LoadTileBlock(TileBlock tileBlock, TileBlockData data)
    {
        if (data == null || tileBlock == null)
            return;

        tileBlock.blockData = data;
        tileBlock.Bounds = new Bounds(data.bounds.center, data.bounds.size);
        tileBlock.tileBases = data.tiles.ToArray();
        tileBlock.TileMap.ClearAllTiles();
        tileBlock.TileMap.SetTilesBlock(data.bounds, data.tiles.ToArray());
    }
}
