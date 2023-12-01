using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileBlock", menuName = "SO/TileBlock")]
public class TileBlockData : ScriptableObject
{
    [SerializeField] public BoundsInt bounds;
    [SerializeField] public List<TileBase> tiles;

    public bool Equals(TileBlockData other)
    {
        return bounds.Equals(other.bounds);
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType() == typeof(TileBlockData))
        {
            return Equals((TileBlockData)obj);
        }
        return false;
    }

    public override int GetHashCode()
    {
        int hash = 7;
        hash += (int)bounds.size.magnitude * 11;
        foreach (var tile in tiles)
        {
            if (tile != null)
                hash += tile.name.GetHashCode() * 7;
            else
                hash += "null".GetHashCode() * 5;
        }
        return hash;
    }
}
