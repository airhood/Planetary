using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class BlockDistribution
{
    public Matter matter;
    public float rarity;
    public float size;
}

public enum SpawnType
{
    Specify, Random
}

[System.Serializable]
public class Spawn
{
    public SpawnType positionType;
    public List<Vector2Int> positions;
    public int minAmount, maxAmount;
}

public class TerrainGeneration : MonoBehaviour
{
    [Header("World")]
    public Main main;
    public Tilemap collidableBlock;
    public Tilemap nonCollidableBlock;
    public Tilemap ladder;

    [Header("Generation Settings")]
    public int worldSize;
    public int dirtLayerHeight = 5;
    public float surfaceValue = 0.25f;
    public float heightMultiplier = 4f;
    public int heightAddition = 25;
    public float terrainFreq = 0.05f;
    public float caveFreq = 0.05f;
    public float seed;
    public Texture2D caveNoiseTexture;

    public List<BlockDistribution> blockDistributions;
    public List<Spawn> spawns;

    [Header("Spread")] public List<Texture2D> spreads;
    
    public Texture2D coalSpread;
    public Texture2D copperSpread;
    public Texture2D ironSpread;
    public Texture2D goldSpread;
    public Texture2D diamondSpread;

    [Header("Output")]
    public bool isLoaded;
    public Texture2D colorMap;

    [Header("Player")]
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        Load();
    }

    public async void Load()
    {
        await GenerateWorld();
        print("map loaded");
        SetPlayer();
        StartPlayer();
        print("load done");
    }

    public async Task GenerateWorld()
    {
        if (caveNoiseTexture == null)
        {
            caveNoiseTexture = new Texture2D(worldSize, worldSize);
            
            coalSpread = new Texture2D(worldSize, worldSize);
            copperSpread = new Texture2D(worldSize, worldSize);
            ironSpread = new Texture2D(worldSize, worldSize);
            goldSpread = new Texture2D(worldSize, worldSize);
            diamondSpread = new Texture2D(worldSize, worldSize);
        }

        GenerateNoiseTexture(seed, caveFreq, surfaceValue, caveNoiseTexture);

        GenerateNoiseTexture(seed + (100f * (int)Matters.Coal), Main.data.matterList[(int)Matters.Coal].rarity, Main.data.matterList[(int)Matters.Coal].size, coalSpread);
        GenerateNoiseTexture(seed + (100f * (int)Matters.Copper), Main.data.matterList[(int)Matters.Copper].rarity, Main.data.matterList[(int)Matters.Copper].size, copperSpread);
        GenerateNoiseTexture(seed + (100f * (int)Matters.Iron), Main.data.matterList[(int)Matters.Iron].rarity, Main.data.matterList[(int)Matters.Iron].size, ironSpread);
        GenerateNoiseTexture(seed + (100f * (int)Matters.Gold), Main.data.matterList[(int)Matters.Gold].rarity, Main.data.matterList[(int)Matters.Gold].size, goldSpread);
        GenerateNoiseTexture(seed + (100f * (int)Matters.Diamond), Main.data.matterList[(int)Matters.Diamond].rarity, Main.data.matterList[(int)Matters.Diamond].size, diamondSpread);

        int k = blockDistributions.Count - spreads.Count;

        for (int i = k; i > 0; i--)
        {
            spreads.Add(new Texture2D(worldSize, worldSize));
        }
        
        for (int i = 0; i < blockDistributions.Count; i++)
        {
            GenerateNoiseTexture(seed + (100f * (int)blockDistributions[i].matter.matterType), blockDistributions[i].rarity, blockDistributions[i].size, spreads[i]);
        }
        
        await Task.Delay(100);

        await GenerateTerrain();
        
        isLoaded = true;
    }

    public void StartPlayer()
    {
        player.GetComponent<Rigidbody2D>().gravityScale = 2;
        player.GetComponent<Collider2D>().enabled = this;
    }

    public void GenerateNoiseTexture(float seed, float frequency, float limit, Texture2D noiseTexture)
    {        
        for(int x = 0; x < noiseTexture.width; x++)
        {
            for(int y = 0; y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
                if (v > limit)
                    noiseTexture.SetPixel(x, y, Color.white);
                else
                    noiseTexture.SetPixel(x, y, Color.black);
            }
        }

        noiseTexture.Apply();
    }



    public async Task GenerateTerrain()
    {
        if (caveNoiseTexture == null) return;
        for (int x = 0; x < caveNoiseTexture.width; x++)
        {
            float height = (Mathf.PerlinNoise((x + seed) * terrainFreq, seed * terrainFreq) * heightMultiplier) + heightAddition;
            for (int y = 0; y < height; y++)
            {
                Matters tile;
                if (y < height - dirtLayerHeight)
                {
                    if (coalSpread.GetPixel(x, y).r > 0.5f)
                        tile = Matters.Coal;
                    else if (copperSpread.GetPixel(x, y).r > 0.5f)
                        tile = Matters.Copper;
                    else if (ironSpread.GetPixel(x, y).r > 0.5f)
                        tile = Matters.Iron;
                    else if (goldSpread.GetPixel(x, y).r > 0.5f)
                        tile = Matters.Gold;
                    else if (diamondSpread.GetPixel(x, y).r > 0.5f)
                        tile = Matters.Diamond;
                    else
                        tile = Matters.stone;
                }
                else if (y < height - 1)
                {
                    tile = Matters.Dirt;
                }
                else
                {
                    tile = Matters.Dirt;
                }

                if (caveNoiseTexture.GetPixel(x, y).r > 0.5f)
                {
                    collidableBlock.SetTile(new Vector3Int(x, y, 0), Main.data.matterList[(int)tile].tile);
                    main.world.planet[0].map.map[x, y] = (short)((short)tile * (-1));
                }
            }
            await Task.Delay(25);
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
