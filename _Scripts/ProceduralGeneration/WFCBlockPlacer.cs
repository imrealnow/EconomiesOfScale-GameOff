using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WFCBlockPlacer : MonoBehaviour
{
    [Tooltip("The dimensions of each block")]
    public Vector2 blockDimensions;
    [Tooltip("The dimensions of the grid(how many blocks to place)")]
    public Vector2 gridDimensions;
    [Tooltip("The pool of blocks to choose from")]
    public List<ConstrainedBlock> blockPool = new List<ConstrainedBlock>();
    public GameObject tileBlockPrefab;

    private PlaceholderBlock[,] placeholderBlocks;
    private Stack<GuessRecord> guessRecords = new Stack<GuessRecord>();
    private HashSet<PlaceholderBlock> minimumEntropyBlocks = new HashSet<PlaceholderBlock>();
    private readonly Vector2[] adjacentDirections = new Vector2[]
    {
        Vector2.left,
        Vector2.up,
        Vector2.right,
        Vector2.down
    };

    private Tilemap tilemap;
    public Tilemap TileMap
    {
        get
        {
            if (tilemap == null)
            {
                tilemap = GetComponent<Tilemap>();
            }
            return tilemap;
        }
    }

    private void CreatePlaceholders()
    {
        placeholderBlocks = new PlaceholderBlock[(int)gridDimensions.x, (int)gridDimensions.y];
        for (int x = 0; x < gridDimensions.x; x++)
        {
            for (int y = 0; y < gridDimensions.y; y++)
            {
                PlaceholderBlock placeholderBlock = new PlaceholderBlock(new HashSet<ConstrainedBlock>(blockPool), x, y);
                minimumEntropyBlocks.Add(placeholderBlock);
                placeholderBlocks[x, y] = placeholderBlock;
            }
        }
    }

    // searches through the placeholderBlocks for the minimum entropy value
    // add the placeholder blocks with the minimum entropy to the minimumEntropyBlocksSet
    // and clear the set if a lower entropy is found
    private bool GetMinimumEntropyBlocks()
    {
        minimumEntropyBlocks.Clear();
        int minimumEntropy = int.MaxValue;
        for (int x = 0; x < gridDimensions.x; x++)
        {
            for (int y = 0; y < gridDimensions.y; y++)
            {
                if (placeholderBlocks[x, y].HasDecided)
                    continue;

                // if you find a cell with no options, stop checking and revert last guess
                if (placeholderBlocks[x, y].Entropy == 0)
                {
                    Debug.Log(x + "," + y + " has no options");
                    // if no guesses have been made so far, then the blocks might not have been
                    // configured correctly
                    if (guessRecords.Count == 0)
                        throw new Exception(placeholderBlocks[x, y].ToString() + " placeholder has no options, and no guesses have been made");
                    RevertLastGuess();
                    // say that the method failed
                    return false;
                }
                if (placeholderBlocks[x, y].Entropy < minimumEntropy)
                {
                    minimumEntropy = placeholderBlocks[x, y].Entropy;
                    minimumEntropyBlocks.Clear();
                    minimumEntropyBlocks.Add(placeholderBlocks[x, y]);
                }
                else if (placeholderBlocks[x, y].Entropy == minimumEntropy)
                {
                    minimumEntropyBlocks.Add(placeholderBlocks[x, y]);
                }
            }
        }
        // say that the method succeeded
        return true;
    }

    private bool AllBlocksDecided()
    {
        bool allDecided = true;
        foreach (PlaceholderBlock pb in placeholderBlocks)
        {
            allDecided = allDecided && pb.HasDecided;
        }
        return allDecided;
    }

    private PlaceholderBlock[,] ClonePlaceholderBlocks()
    {
        PlaceholderBlock[,] clone = new PlaceholderBlock[(int)gridDimensions.x, (int)gridDimensions.y];
        for (int x = 0; x < gridDimensions.x; x++)
        {
            for (int y = 0; y < gridDimensions.y; y++)
            {
                clone[x, y] = (PlaceholderBlock)placeholderBlocks[x, y].Clone();
            }
        }
        return clone;
    }

    public void CollapseBlock(int x, int y)
    {
        PlaceholderBlock block = placeholderBlocks[x, y];
        if (block.PossibleBlocks.Count == 0)
        {
            throw new Exception("CollapseBlock called on block with no possible blocks");
        }
        ConstrainedBlock choice = block.DecideBlock();
        if (block.Entropy > 1)
        {
            // if the choice wasn't definite, record it so it can be reverted
            GuessRecord guessRecord = new GuessRecord(ClonePlaceholderBlocks(), choice, x, y);
            guessRecords.Push(guessRecord);
        }

        Debug.Log("Collapsing block at " + x + ", " + y + " with " + choice.blockData.name.ToString());
        // reduce possibilities of adjacent blocks
        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            int adjacentX = x + (int)adjacentDirections[i].x;
            int adjacentY = y + (int)adjacentDirections[i].y;
            if (adjacentX >= 0 && adjacentX < gridDimensions.x && adjacentY >= 0 && adjacentY < gridDimensions.y)
            {
                PlaceholderBlock adjacentBlock = placeholderBlocks[adjacentX, adjacentY];
                adjacentBlock.SetPossibleBlocks(choice.GetAdjacenyBlocksFromDirection(adjacentDirections[i]));
                PropogateConstraints(adjacentX, adjacentY);
            }
        }
    }

    private void PropogateConstraints(int x, int y)
    {
        PlaceholderBlock block = placeholderBlocks[x, y];
        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            HashSet<ConstrainedBlock> adjacentPossibilities = new HashSet<ConstrainedBlock>();
            foreach (ConstrainedBlock possibleBlock in block.PossibleBlocks)
            {
                adjacentPossibilities.UnionWith(possibleBlock.GetAdjacenyBlocksFromDirection(adjacentDirections[i]));
            }
            int adjacentX = x + (int)adjacentDirections[i].x;
            int adjacentY = y + (int)adjacentDirections[i].y;
            if (adjacentX >= 0 && adjacentX < gridDimensions.x && adjacentY >= 0 && adjacentY < gridDimensions.y)
            {
                PlaceholderBlock adjacentBlock = placeholderBlocks[adjacentX, adjacentY];
                if (adjacentBlock.HasDecided)
                    continue;
                if (adjacentBlock.PossibleBlocks.SetEquals(adjacentPossibilities))
                {
                    continue;
                }
                adjacentBlock.SetPossibleBlocks(adjacentPossibilities);
                PropogateConstraints(adjacentX, adjacentY);
            }
        }
    }

    private void RevertLastGuess()
    {
        GuessRecord record = guessRecords.Pop();
        placeholderBlocks = record.previousState;
        placeholderBlocks[record.guessedX, record.guessedY].RemovePossibleBlocks(new HashSet<ConstrainedBlock>() { record.blockChoice });
    }

    [ContextMenu("Solve")]
    public void Solve()
    {
        CreatePlaceholders();
        int guessCount = 0;
        while (!AllBlocksDecided() && guessCount < 1000)
        {
            guessCount++;
            while (!GetMinimumEntropyBlocks())
                continue;
            System.Random rng = new System.Random();
            PlaceholderBlock firstBlock = minimumEntropyBlocks.ElementAt(rng.Next(minimumEntropyBlocks.Count));
            CollapseBlock(firstBlock.x, firstBlock.y);
        }

        foreach (PlaceholderBlock block in placeholderBlocks)
        {
            Debug.Log(block.x + " " + block.y + " " + block.DecidedBlock.blockData.name);
        }
    }

    [ContextMenu("Test")]
    public void RenderTiles()
    {
        if (!AllBlocksDecided())
            return;
        for (int x = 0; x < gridDimensions.x; x++)
        {
            for (int y = 0; y < gridDimensions.y; y++)
            {
                TileBlockData blockData = placeholderBlocks[x, y].DecidedBlock.blockData;
                BoundsInt dataBounds = blockData.bounds;
                Vector3Int newMin = new Vector3Int(
                    dataBounds.xMin + (int)(x * blockDimensions.x),
                    dataBounds.yMin + (int)(y * blockDimensions.y),
                    0
                );
                BoundsInt newBounds = new BoundsInt(newMin, dataBounds.size);
                //TileMap.SetTilesBlock(newBounds, blockData.tiles);
            }
        }
    }
}

public struct GuessRecord
{
    public PlaceholderBlock[,] previousState;
    public ConstrainedBlock blockChoice;
    public int guessedX;
    public int guessedY;

    public GuessRecord(PlaceholderBlock[,] previousState, ConstrainedBlock blockChoice, int guessedX, int guessedY)
    {
        this.previousState = previousState;
        this.blockChoice = blockChoice;
        this.guessedX = guessedX;
        this.guessedY = guessedY;
    }
}