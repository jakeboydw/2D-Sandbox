using UnityEngine;

[CreateAssetMenu(fileName = "Tile Atlas", menuName = "TileAtlas")]
public class TileAtlas : ScriptableObject
{
    public TileClass grass;
    public TileClass dirt;
    public TileClass stone;

    //ores
    public TileClass coal;
    public TileClass iron;
    public TileClass gold;
    public TileClass diamond;

    //plants
    public TileClass log;
    public TileClass leaf;
    public TileClass tallGrass;
}
