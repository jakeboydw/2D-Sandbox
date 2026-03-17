using UnityEngine;

[CreateAssetMenu(fileName = "newtoolclass", menuName = "ToolClass")]
public class ToolClass : ScriptableObject
{
    public string toolName;
    public Sprite sprite;
    public ItemClass.ToolType toolType;
}
