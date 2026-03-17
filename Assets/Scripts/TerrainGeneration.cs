using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    public PlayerController player;
    public CameraController cameraController;
    public GameObject dropTile;

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
    private Dictionary<Vector2Int, GameObject> worldTileObjects = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, TileClass> worldTileClasses = new Dictionary<Vector2Int, TileClass>();

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

        cameraController.Spawn(new Vector3(player.spawnPos.x, player.spawnPos.y, cameraController.transform.position.z));
        player.Spawn();
    }

    public void GenerateTerrain()
    {
        TileClass tile;
        for (int x = 0; x < worldSize; x++)
        {
            curBiome = GetCurrentBiome(x, 0);
            float height = Mathf.PerlinNoise((x + seed) * curBiome.terrainFreq, seed * curBiome.terrainFreq) * curBiome.heightMultiplier + heightAddition;

            if (x == worldSize / 2)
            {
                player.spawnPos = new Vector2(x, height + 2);
            }

            for (int y = 0; y < height; y++)
            {
                curBiome = GetCurrentBiome(x, y);
                if (y < height - curBiome.dirtLayerHeight)
                {
                    tile = curBiome.tileAtlas.stone;

                    if (curBiome.ores[0].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > curBiome.ores[0].minSpawnDepth)
                    {
                        tile = curBiome.tileAtlas.coal;
                    }
                    if (curBiome.ores[1].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > curBiome.ores[1].minSpawnDepth)
                    {
                        tile = curBiome.tileAtlas.iron;
                    }
                    if (curBiome.ores[2].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > curBiome.ores[2].minSpawnDepth)
                    {
                        tile = curBiome.tileAtlas.gold;
                    }
                    if (curBiome.ores[3].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > curBiome.ores[3].minSpawnDepth)
                    {
                        tile = curBiome.tileAtlas.diamond;
                    }
                }
                else if (y < height - 1)
                {
                    tile = curBiome.tileAtlas.dirt;
                }
                else
                {
                    tile = curBiome.tileAtlas.grass;
                }

                if (caveNoiseTexture.GetPixel(x, y).r > 0.5f)
                {
                    PlaceTile(tile, x, y);
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
                                PlaceTile(curBiome.tileAtlas.tallGrass, x, y + 1);
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
            PlaceTile(curBiome.tileAtlas.log, x, y + i);
        }

        PlaceTile(curBiome.tileAtlas.leaf, x, y + treeHeight);
        PlaceTile(curBiome.tileAtlas.leaf, x, y + treeHeight + 1);
        PlaceTile(curBiome.tileAtlas.leaf, x, y + treeHeight + 2);
        PlaceTile(curBiome.tileAtlas.leaf, x - 1, y + treeHeight);
        PlaceTile(curBiome.tileAtlas.leaf, x - 1, y + treeHeight + 1);
        PlaceTile(curBiome.tileAtlas.leaf, x + 1, y + treeHeight);
        PlaceTile(curBiome.tileAtlas.leaf, x + 1, y + treeHeight + 1);
    }

    public void RemoveTile(int x, int y)
    {
        if (worldTiles.Contains(new Vector2Int(x, y)))
        {
            Destroy(worldTileObjects[new Vector2Int(x, y)]);
            if (worldTileClasses[new Vector2Int(x, y)].canDrop)
            {
                GameObject newDropTile = Instantiate(dropTile, new Vector2(x, y + 0.5f), Quaternion.identity);
                newDropTile.GetComponent<SpriteRenderer>().sprite = worldTileClasses[new Vector2Int(x, y)].tileSprites[0];

                ItemClass dropTileItem = new ItemClass(worldTileClasses[new Vector2Int(x, y)]);
                newDropTile.GetComponent<DropTileController>().item = dropTileItem;
            }

            worldTiles.RemoveAt(worldTiles.IndexOf(new Vector2Int(x, y)));
            worldTileObjects.Remove(new Vector2Int(x, y));
            worldTileClasses.Remove(new Vector2Int(x, y));
        }
    }

    public bool PlaceTile(TileClass tile, int x, int y)
    {
        if (!worldTiles.Contains(new Vector2Int(x, y)))
        {
            GameObject newTile = new GameObject();
            Sprite[] tileSprites = tile.tileSprites;
            bool isSolid = tile.isSolid;

            newTile.AddComponent<SpriteRenderer>();
            int spriteIndex = Random.Range(0, tileSprites.Length);
            Sprite tileSprite = tileSprites[spriteIndex];
            newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
            newTile.GetComponent<SpriteRenderer>().sortingOrder = -5;

            if (isSolid)
            {
                newTile.AddComponent<BoxCollider2D>();
                newTile.GetComponent<BoxCollider2D>().size = Vector2.one;
                newTile.tag = "Ground";
            }

            newTile.name = tileSprites[0].name;
            newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

            worldTiles.Add(new Vector2Int(x, y));
            worldTileObjects[new Vector2Int(x, y)] = newTile;
            worldTileClasses[new Vector2Int(x, y)] = tile;

            return true;
        }
        return false;
    }
}
