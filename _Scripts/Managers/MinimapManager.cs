using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "MinimapManager", menuName = "SO/Managers/MinimapManager")]
public class MinimapManager : SManager
{

    public Color defaultColor = Color.white;
    public Color playerColor = Color.red;
    public SVector3 playerPosition; // Player's transform
    public SEvent regenerationTrigger;
    public int minimapScale = 2; // Scale factor (e.g., 2, 4, 8)

    private RawImage minimapDisplay; // UI element for the minimap
    private Texture2D minimapTexture;
    private Texture2D dynamicTexture;
    private Texture2D scaledTexture;
    private List<Tilemap> tilemaps;
    private Vector2 minimapCenter; // Center position for the minimap

    public override void OnEnabled()
    {
        base.OnEnabled();
        GameObject minimapObject = GameObject.FindWithTag("Minimap");
        if (minimapObject)
        {
            minimapDisplay = minimapObject.GetComponent<RawImage>();
        }
        if (regenerationTrigger != null)
        {
            regenerationTrigger.sharedEvent += GenerateMinimap;
        }
        GenerateMinimap();
        InitialiseScaledTexture();
    }

    public override void OnDisabled()
    {
        base.OnDisabled();
        if (regenerationTrigger != null)
        {
            regenerationTrigger.sharedEvent -= GenerateMinimap;
        }
    }

    public override void Update()
    {
        ResetDynamicTexture();
        ScaleMinimap();
    }

    private void CollectTilemaps()
    {
        tilemaps = new List<Tilemap>();
        Tilemap[] allTilemaps = FindObjectsOfType<Tilemap>();
        foreach (var tilemap in allTilemaps)
        {
            if (tilemap.gameObject.tag.Equals("MinimapWall"))
            {
                tilemaps.Add(tilemap);
            }
        }
    }

    private void GenerateMinimap()
    {
        CollectTilemaps();
        minimapTexture = new Texture2D(100, 100);

        foreach (var tilemap in tilemaps)
        {
            foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = tilemap.origin + new Vector3Int(pos.x, pos.y, pos.z);
                if (!tilemap.HasTile(localPlace)) continue;

                // Convert tilemap position to texture position
                int x = localPlace.x + minimapTexture.width / 2;
                int y = localPlace.y + minimapTexture.height / 2;
                minimapTexture.SetPixel(x, y, defaultColor);
            }
        }

        minimapTexture.Apply();
        ResetDynamicTexture();
        minimapDisplay.texture = dynamicTexture;
    }

    private void ResetDynamicTexture()
    {
        dynamicTexture = new Texture2D(minimapTexture.width, minimapTexture.height);
        dynamicTexture.SetPixels(minimapTexture.GetPixels());
        minimapDisplay.texture = dynamicTexture;
    }

    private void InitialiseScaledTexture()
    {
        scaledTexture = new Texture2D(minimapTexture.width / minimapScale, minimapTexture.height / minimapScale);
    }

    private void ScaleMinimap()
    {
        Vector2 playerPos = playerPosition.Value;

        int centerX = Mathf.Clamp((int)playerPos.x - scaledTexture.width / 2, 0, minimapTexture.width - scaledTexture.width);
        int centerY = Mathf.Clamp((int)playerPos.y - scaledTexture.height / 2, 0, minimapTexture.height - scaledTexture.height);

        for (int x = 0; x < scaledTexture.width; x++)
        {
            for (int y = 0; y < scaledTexture.height; y++)
            {
                Color pixelColor = minimapTexture.GetPixel(centerX + x * minimapScale, centerY + y * minimapScale);
                scaledTexture.SetPixel(x, y, pixelColor);
            }
        }

        // Draw the player's position on the scaled map
        int playerScaledX = Mathf.Clamp((int)((playerPos.x - centerX) / minimapScale), 0, scaledTexture.width - 1);
        int playerScaledY = Mathf.Clamp((int)((playerPos.y - centerY) / minimapScale), 0, scaledTexture.height - 1);

        // Set a block of pixels for the player to make it more visible at scale
        int playerSize = minimapScale; // Adjust size as needed
        for (int i = -playerSize / 2; i <= playerSize / 2; i++)
        {
            for (int j = -playerSize / 2; j <= playerSize / 2; j++)
            {
                int drawX = playerScaledX + i;
                int drawY = playerScaledY + j;
                if (drawX >= 0 && drawX < scaledTexture.width && drawY >= 0 && drawY < scaledTexture.height)
                {
                    scaledTexture.SetPixel(drawX, drawY, playerColor);
                }
            }
        }

        scaledTexture.Apply();
        minimapDisplay.texture = scaledTexture;
    }
}