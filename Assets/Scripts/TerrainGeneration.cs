using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGeneration : MonoBehaviour
{
    [Header("World")]
    public Main main;
    public Tilemap collidableBlock;
    public Tilemap nonCollidableBlock;

    [Header("World Generation")]
    public float surfaceValue = 0.7f;
    public int worldSize;
    public float terrainFreq = 0.05f;
    public float caveFreq = 0.05f;
    public float heightMultiplier = 4f;
    public int heightAddition = 25;
    public float seed;
    public Texture2D noiseTexture;
    public bool isLoaded;

    [Header("Player")]
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        GenerateNoiseTexture();
        GenerateTerrain(noiseTexture);
        SetPlayer();
        isLoaded = true;
    }

    public void GenerateNoiseTexture()
    {
        noiseTexture = new Texture2D(worldSize, worldSize);
        
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * caveFreq, (y + seed) * caveFreq);
                noiseTexture.SetPixel(x, y, new Color(v, v, v));
            }
        }

        noiseTexture.Apply();
    }

    public void GenerateTerrain(Texture2D worldPerlinNoise)
    {
        for (int x = 0; x < worldPerlinNoise.width; x++)
        {
            float height = (Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier) + heightAddition;

            for (int y = 0; y < height; y++)
            {
                if (noiseTexture.GetPixel(x, y).r > surfaceValue)
                {
                    collidableBlock.SetTile(new Vector3Int(x, y, 0), Main.matterList[(int)Matters.stone].tile);
                    main.world.planet[0].map.map[x, y] = (short)Matters.stone * (-1);
                }
            }
        }
    }

    private void SetPlayer()
    {
        float gravityScale = player.GetComponent<Rigidbody2D>().gravityScale;
        player.GetComponent<Rigidbody2D>().gravityScale = 0;
        Vector2Int pos = (Vector2Int)collidableBlock.WorldToCell(player.transform.position);
        float height = (Mathf.PerlinNoise((pos.x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier) + heightAddition;
        Vector2 newPos = new Vector2(pos.x, Mathf.CeilToInt(height));
        player.transform.position = newPos;
        player.GetComponent<Rigidbody2D>().gravityScale = gravityScale;
    }
}
