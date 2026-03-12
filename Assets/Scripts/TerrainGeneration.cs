using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    public float seed;
    public int worldSize;
    public int heightAddition;

    public Gradient biomeGradient;
    public Texture2D biomeMap;
    public float biomeFreq;
    public BiomeClass[] biomes;

    private BiomeClass curBiome;

    private void Start()
    {
        seed = Random.Range(-10000, 10000);

        biomeMap = new Texture2D(worldSize, worldSize);
        DrawBiomeMap();
    }

    public void GenerateTerrain()
    {
        Sprite[] tileSprites;
        for (int x = 0; x < worldSize; x++)
        {
            SetCurrentBiome(x, 0);
            float height = Mathf.PerlinNoise((x + seed) * curBiome.terrainFreq, seed * curBiome.terrainFreq) * curBiome.heightMultiplier + heightAddition;
            for (int y = 0; y < height; y++)
            {

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

    public void SetCurrentBiome(int x, int y)
    {
        for (int i = 0; i < biomes.Length; i++)
        {
            if (biomes[i].biomeColor == biomeMap.GetPixel(x, y))
            {
                curBiome = biomes[i];
                break;
            }
        }

        curBiome = null;
    }
}
