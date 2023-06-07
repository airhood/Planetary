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
    public Tilemap ladder;

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
    public Texture2D colorMap;
    public int grassMinHeight;

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

    public Texture2D addNoise(Texture2D noise1, Texture2D noise2)
    {
        if (noise1.width != noise2.width) return null;
        if (noise1.height != noise2.height) return null;
        int width = noise1.width;
        int height = noise1.height;
        Texture2D resultTexture = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float r = noise1.GetPixel(x, y).r + noise2.GetPixel(x, y).r;
                float g = noise1.GetPixel(x, y).g + noise2.GetPixel(x, y).g;
                float b = noise1.GetPixel(x, y).b + noise2.GetPixel(x,y).b;
                if (r > 1) r = 1;
                if (g > 1) g = 1;
                if (b > 1) b = 1;
                if (r < 0) r = 0;
                if (g < 0) g = 0;
                if (b < 0) b = 0;
                resultTexture.SetPixel(x, y, new Color(r, g, b));
            }
        }
        resultTexture.Apply();
        return resultTexture;
    }

    public Texture2D substractNoise(Texture2D noise1, Texture2D noise2)
    {
        if (noise1.width != noise2.width) return null;
        if (noise1.height != noise2.height) return null;
        int width = noise1.width;
        int height = noise1.height;
        Texture2D resultTexture = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float r = noise1.GetPixel(x, y).r - noise2.GetPixel(x, y).r;
                float g = noise1.GetPixel(x, y).g - noise2.GetPixel(x, y).g;
                float b = noise1.GetPixel(x, y).b - noise2.GetPixel(x, y).b;
                if (r > 1) r = 1;
                if (g > 1) g = 1;
                if (b > 1) b = 1;
                if (r < 0) r = 0;
                if (g < 0) g = 0;
                if (b < 0) b = 0;
                resultTexture.SetPixel(x, y, new Color(r, g, b));
            }
        }
        resultTexture.Apply();
        return resultTexture;
    }

    public Texture2D multiplyNoise(Texture2D noise1, Texture2D noise2)
    {
        if (noise1.width != noise2.width) return null;
        if (noise1.height != noise2.height) return null;
        int width = noise1.width;
        int height = noise1.height;
        Texture2D resultTexture = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float r = noise1.GetPixel(x, y).r * noise2.GetPixel(x, y).r;
                float g = noise1.GetPixel(x, y).g * noise2.GetPixel(x, y).g;
                float b = noise1.GetPixel(x, y).b * noise2.GetPixel(x, y).b;
                if (r > 1) r = 1;
                if (g > 1) g = 1;
                if (b > 1) b = 1;
                if (r < 0) r = 0;
                if (g < 0) g = 0;
                if (b < 0) b = 0;
                resultTexture.SetPixel(x, y, new Color(r, g, b));
            }
        }
        resultTexture.Apply();
        return resultTexture;
    }

    public void GenerateTerrain(Texture2D worldPerlinNoise)
    {
        for (int x = 0; x < worldPerlinNoise.width; x++)
        {
            float height = (Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier) + heightAddition;
            short maxHeight = 0;
            for (int y = 0; y < height; y++)
            {
                if (noiseTexture.GetPixel(x, y).r > surfaceValue)
                {
                    collidableBlock.SetTile(new Vector3Int(x, y, 0), Main.matterList[(int)Matters.stone].tile);
                    main.world.planet[0].map.map[x, y] = (short)Matters.stone * (-1);
                    maxHeight = (short)y;
                }
            }

            /*
            if (maxHeight < grassMinHeight) continue;

            float dirtNoise = Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq);
            byte dirtHeight;
            if (dirtNoise > 0 && dirtNoise <= 0.3) dirtHeight = 1;
            else if (dirtNoise > 0.3 && dirtNoise <= 0.55) dirtHeight = 2;
            else if (dirtNoise > 0.5 && dirtNoise <= 0.8) dirtHeight = 3;
            else if (dirtNoise > 8 && dirtNoise <= 1) dirtHeight = 4;
            else dirtHeight = 0;

            for (int d = 1; d <= dirtHeight; d++)
            {
                collidableBlock.SetTile(new Vector3Int(x, maxHeight + d, 0), Main.matterList[(int)Matters.Dirt].tile);
                main.world.planet[0].map.map[x, maxHeight + d] = (short)Matters.Dirt * (-1);
            }
            */
        }
    }

    private void SetPlayer()
    {
        float gravityScale = player.GetComponent<Rigidbody2D>().gravityScale;
        player.GetComponent<Rigidbody2D>().gravityScale = 0;
        Vector2Int pos = (Vector2Int)collidableBlock.WorldToCell(player.transform.position);
        //float height = ((Mathf.PerlinNoise((pos.x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier) + heightAddition) + (Mathf.RoundToInt(Mathf.PerlinNoise((pos.x + seed) * terrainFreq, seed * terrainFreq) * 4));
        float height = (Mathf.PerlinNoise((pos.x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier) + heightAddition;
        Vector2 newPos = new Vector2(pos.x + 0.5f, Mathf.CeilToInt(height));
        player.transform.position = newPos;
        player.GetComponent<Rigidbody2D>().gravityScale = gravityScale;
    }
}
