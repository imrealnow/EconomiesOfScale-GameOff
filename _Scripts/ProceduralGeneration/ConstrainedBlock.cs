using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "ConstrainedBlock", menuName = "TileBlock/Constrained Block")]
public class ConstrainedBlock : ScriptableObject, IEquatable<ConstrainedBlock>
{
    public List<ConstrainedBlock> westBlocks = new List<ConstrainedBlock>();
    public List<ConstrainedBlock> northBlocks = new List<ConstrainedBlock>();
    public List<ConstrainedBlock> eastBlocks = new List<ConstrainedBlock>();
    public List<ConstrainedBlock> southBlocks = new List<ConstrainedBlock>();

    public TileBlockData blockData;

    public HashSet<ConstrainedBlock> GetAdjacenyBlocksFromDirection(Vector2 direction)
    {
        if (direction == Vector2.left)
        {
            return new HashSet<ConstrainedBlock>(westBlocks);
        }
        else if (direction == Vector2.up)
        {
            return new HashSet<ConstrainedBlock>(northBlocks);
        }
        else if (direction == Vector2.right)
        {
            return new HashSet<ConstrainedBlock>(eastBlocks);
        }
        else if (direction == Vector2.down)
        {
            return new HashSet<ConstrainedBlock>(southBlocks);
        }
        else
        {
            return null;
        }
    }

    public bool Equals(ConstrainedBlock other)
    {
        return this.blockData.Equals(other.blockData);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ConstrainedBlock);
    }

    public override int GetHashCode()
    {
        return blockData.GetHashCode();
    }

    public override string ToString()
    {
        return blockData.ToString();
    }
}
