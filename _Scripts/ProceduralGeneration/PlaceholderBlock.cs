using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaceholderBlock : ICloneable
{

    HashSet<ConstrainedBlock> possibleBlocks = new HashSet<ConstrainedBlock>();
    ConstrainedBlock decidedBlock = null;
    public int x { get; private set; }
    public int y { get; private set; }

    public bool HasDecided { get { return decidedBlock != null; } }
    public ConstrainedBlock DecidedBlock { get { return decidedBlock; } }
    public int Entropy { get { return possibleBlocks.Count; } }
    public HashSet<ConstrainedBlock> PossibleBlocks { get { return possibleBlocks; } }

    public PlaceholderBlock(HashSet<ConstrainedBlock> possibleBlocks, int x, int y)
    {
        foreach (ConstrainedBlock block in possibleBlocks)
        {
            this.possibleBlocks.Add(block);
        }
        this.x = x;
        this.y = y;
    }

    public PlaceholderBlock(PlaceholderBlock source)
    {
        this.x = source.x;
        this.y = source.y;
        this.decidedBlock = source.DecidedBlock;
        foreach(ConstrainedBlock block in source.PossibleBlocks)
        {
            this.possibleBlocks.Add(block);
        }
    }

    public void AddPossibleBlocks(HashSet<ConstrainedBlock> blocks)
    {
        possibleBlocks.UnionWith(blocks);
    }

    public void RemovePossibleBlocks(HashSet<ConstrainedBlock> blocks)
    {
        possibleBlocks.ExceptWith(blocks);
    }

    public void SetPossibleBlocks(HashSet<ConstrainedBlock> blocks)
    {
        possibleBlocks = blocks;
    }

    public ConstrainedBlock DecideBlock()
    {
        if (possibleBlocks.Count == 0)
            return null;
        
        if (possibleBlocks.Count == 1)
        {
            decidedBlock = possibleBlocks.First();
            return decidedBlock;
        }
        else
        {
            System.Random rng = new System.Random();
            int randomIndex = rng.Next(0, possibleBlocks.Count);
            decidedBlock = possibleBlocks.ElementAt(randomIndex);
            return decidedBlock;
        }
    }
    
    public object Clone()
    {
        PlaceholderBlock clone = new PlaceholderBlock(this);
        return clone;
    }

    public override string ToString()
    {
        return string.Format("[PlaceholderBlock: x={0}, y={1}, decidedBlock={2}, possibleBlocks={3}]", x, y, decidedBlock, possibleBlocks.Count);
    }
}
