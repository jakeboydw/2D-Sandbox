using UnityEngine;

[System.Serializable]
public class BiomeClass
{
    public string name;
    public Color biomeColor;
    public TileAtlas tileAtlas;

    public float terrainFreq;
    public int heightMultiplier;
    public int dirtLayerHeight;

    public OreClass[] ores;

    public int tallGrassChance;
    public int treeChance;
    public int minTreeHeight;
    public int maxTreeHeight;
}
