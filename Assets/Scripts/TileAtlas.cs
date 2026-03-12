using UnityEngine;

[CreateAssetMenu(fileName = "Tile Atlas", menuName = "TileAtlas")]
public class TileAtlas : ScriptableObject
{
    public TileClass grass;
    public TileClass dirt;
    public TileClass stone;
}
