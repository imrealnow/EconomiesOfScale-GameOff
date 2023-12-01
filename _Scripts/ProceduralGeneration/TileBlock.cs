using UnityEngine;
using UnityEngine.Tilemaps;

public class TileBlock : MonoBehaviour
{
    [SerializeField] Bounds blockBounds;

    private Tilemap tileMap;
    private Grid tileGrid;
    public TileBase[] tileBases;
    public TileBlockData blockData;

    public bool lockBounds { get; set; }

    public Bounds Bounds
    {
        get
        {
            return blockBounds;
        }
        set
        {
            blockBounds = value;
        }
    }

    public Tilemap TileMap
    {
        get
        {
            if (tileMap == null)
                tileMap = GetComponent<Tilemap>();
            return tileMap;
        }
    }

    public Grid TileGrid
    {
        get
        {
            if (tileGrid == null)
                tileGrid = GetComponent<Tilemap>().layoutGrid;
            return tileGrid;
        }
    }

    public TileBase[] TileBases
    {
        get
        {
            return TileMap.GetTilesBlock(ConvertBounds(blockBounds));
        }
    }

    public BoundsInt ConvertBounds(Bounds bounds)
    {
        return new BoundsInt(Mathf.RoundToInt(bounds.min.x), Mathf.RoundToInt(bounds.min.y), 0, Mathf.RoundToInt(bounds.size.x), Mathf.RoundToInt(bounds.size.y), 1);
    }

    private void Awake()
    {
        tileMap = GetComponent<Tilemap>();
        tileGrid = tileMap.layoutGrid;
        tileBases = TileBases;
    }
}
