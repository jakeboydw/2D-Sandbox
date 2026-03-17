using UnityEngine;

[System.Serializable]
public class ItemClass
{
    public enum ItemType
    {
        block,
        tool
    };

    public enum ToolType
    {
        axe,
        pickaxe,
        hammer
    };

    public ItemType itemType;
    public ToolType toolType;

    public TileClass tileClass;
    public ToolClass toolClass;

    public string name;
    public Sprite sprite;

    public ItemClass(TileClass tile)
    {
        name = tile.tileName;
        sprite = tile.tileSprites[0];
        itemType = ItemType.block;
        tileClass = tile;
    }

    public ItemClass(ToolClass tool)
    {
        name = tool.toolName;
        sprite = tool.sprite;
        itemType = ItemType.tool;
        toolType = tool.toolType;
        toolClass = tool;
    }
}
