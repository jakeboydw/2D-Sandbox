using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    public float seed;

    [Header("Generation Settings")]
    public int worldSize;
    public int heightAddition;
    public Texture2D caveNoiseTexture;
    public float caveFreq;
    public float caveLimit;

    [Header("BiomeSettings")]
    public Gradient biomeGradient;
    public Texture2D biomeMap;
    public float biomeFreq;
    public BiomeClass[] biomes;

    private BiomeClass curBiome;
    private List<Vector2Int> worldTiles = new List<Vector2Int>();

    private void Start()
    {
        seed = Random.Range(-10000, 10000);

        //biomes
        biomeMap = new Texture2D(worldSize, worldSize);
        DrawBiomeMap();

        //caves
        caveNoiseTexture = new Texture2D(worldSize, worldSize);
        DrawNoiseTexture(caveNoiseTexture, caveFreq, caveLimit);

        //ores
        for (int i = 0; i < biomes.Length; i++)
        {
            for (int j = 0; j < biomes[i].ores.Length; j++)
            {
                biomes[i].ores[j].spreadTexture = new Texture2D(worldSize, worldSize);
                DrawNoiseTexture(biomes[i].ores[j].spreadTexture, biomes[i].ores[j].frequency, biomes[i].ores[j].size);
            }
        }

        GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        Sprite[] tileSprites;
        for (int x = 0; x < worldSize; x++)
        {
            curBiome = GetCurrentBiome(x, 0);
            float height = Mathf.PerlinNoise((x + seed) * curBiome.terrainFreq, seed * curBiome.terrainFreq) * curBiome.heightMultiplier + heightAddition;
            for (int y = 0; y < height; y++)
            {
                curBiome = GetCurrentBiome(x, y);
                if (y < height - curBiome.dirtLayerHeight)
                {
                    tileSprites = curBiome.tileAtlas.stone.tileSprites;

                    if (curBiome.ores[0].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > curBiome.ores[0].minSpawnDepth)
                    {
                        tileSprites = curBiome.tileAtlas.coal.tileSprites;
                    }
                    if (curBiome.ores[1].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > curBiome.ores[1].minSpawnDepth)
                    {
                        tileSprites = curBiome.tileAtlas.iron.tileSprites;
                    }
                    if (curBiome.ores[2].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > curBiome.ores[2].minSpawnDepth)
                    {
                        tileSprites = curBiome.tileAtlas.gold.tileSprites;
                    }
                    if (curBiome.ores[3].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > curBiome.ores[3].minSpawnDepth)
                    {
                        tileSprites = curBiome.tileAtlas.diamond.tileSprites;
                    }
                }
                else if (y < height - 1)
                {
                    tileSprites = curBiome.tileAtlas.dirt.tileSprites;
                }
                else
                {
                    tileSprites = curBiome.tileAtlas.grass.tileSprites;
                }

                if (caveNoiseTexture.GetPixel(x, y).r > 0.5f)
                {
                    PlaceTile(tileSprites, x, y);
                }

                if (y >= height - 1)
                {
                    int t = Random.Range(0, curBiome.treeChance);
                    if (t == 1)
                    {
                        if (worldTiles.Contains(new Vector2Int(x, y)))
                        {
                            GenerateTree(Random.Range(curBiome.minTreeHeight, curBiome.maxTreeHeight), x, y + 1);
                        }
                    }
                    else
                    {
                        int i = Random.Range(0, curBiome.tallGrassChance);
                        if (i == 1)
                        {
                            if (worldTiles.Contains(new Vector2Int(x, y)))
                            {
                                PlaceTile(curBiome.tileAtlas.tallGrass.tileSprites, x, y + 1);
                            }
                        }
                    }
                }
            }
        }
    }

    public void DrawBiomeMap()
    {
        for (int x = 0; x < biomeMap.width; x++)
        {
            for (int y = 0; y < biomeMap.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * biomeFreq, (y + seed) * biomeFreq);
                Color col = biomeGradient.Evaluate(v);
                biomeMap.SetPixel(x, y, col);
            }
        }
        biomeMap.Apply();
    }

    public BiomeClass GetCurrentBiome(int x, int y)
    {
        for (int i = 0; i < biomes.Length; i++)
        {
            if (biomes[i].biomeColor == biomeMap.GetPixel(x, y))
            {
                return biomes[i];
            }
        }
        return null;
    }

    public void DrawNoiseTexture(Texture2D noiseTexture, float frequency, float limit)
    {
        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0;y < noiseTexture.height; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
                if (v > limit)
                {
                    noiseTexture.SetPixel(x, y, Color.white);
                }
                else
                {
                    noiseTexture.SetPixel(x, y, Color.black);
                }
            }
        }
        noiseTexture.Apply();
    }

    public void GenerateTree(int treeHeight, int x, int y)
    {
        for (int i = 0; i < treeHeight; i++)
        {
            PlaceTile(curBiome.tileAtlas.log.tileSprites, x, y + i);
        }

        PlaceTile(curBiome.tileAtlas.leaf.tileSprites, x, y + treeHeight);
        PlaceTile(curBiome.tileAtlas.leaf.tileSprites, x, y + treeHeight + 1);
        PlaceTile(curBiome.tileAtlas.leaf.tileSprites, x, y + treeHeight + 2);
        PlaceTile(curBiome.tileAtlas.leaf.tileSprites, x - 1, y + treeHeight);
        PlaceTile(curBiome.tileAtlas.leaf.tileSprites, x - 1, y + treeHeight + 1);
        PlaceTile(curBiome.tileAtlas.leaf.tileSprites, x + 1, y + treeHeight);
        PlaceTile(curBiome.tileAtlas.leaf.tileSprites, x + 1, y + treeHeight + 1);
    }

    public void PlaceTile(Sprite[] tileSprites, int x, int y)
    {
        if (!worldTiles.Contains(new Vector2Int(x, y)))
        {
            GameObject tile = new GameObject();

            tile.AddComponent<SpriteRenderer>();
            int spriteIndex = Random.Range(0, tileSprites.Length);
            Sprite tileSprite = tileSprites[spriteIndex];
            tile.GetComponent<SpriteRenderer>().sprite = tileSprite;

            tile.name = tileSprites[0].name;
            tile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

            worldTiles.Add(new Vector2Int(x, y));
        }
    }
}
