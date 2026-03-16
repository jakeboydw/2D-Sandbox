using UnityEngine;

[CreateAssetMenu(fileName = "newtileclass", menuName = "TileClass")]
public class TileClass : ScriptableObject
{
    public string tileName;
    public Sprite[] tileSprites;
    public bool isSolid = true;
    public bool canDrop = true;
}
