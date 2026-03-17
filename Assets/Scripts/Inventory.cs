using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Vector2 offset;
    public Vector2 multiplier;

    public GameObject inventoryUI;
    public GameObject inventorySlotPrefab;

    public int inventoryWidth;
    public int inventoryHeight;
    public InventorySlot[,] inventorySlots;
    public GameObject[,] uiSlots;

    private void Start()
    {
        inventorySlots = new InventorySlot[inventoryWidth, inventoryHeight];
        uiSlots = new GameObject[inventoryWidth, inventoryHeight];

        SetupUI();
        updateInventoryUI();
    }

    void SetupUI()
    {
        for (int x = 0; x < inventoryWidth; x++)
        {
            for (int y = 0; y < inventoryHeight; y++)
            {
                GameObject inventorySlot = Instantiate(inventorySlotPrefab, inventoryUI.transform.GetChild(0).transform);
                inventorySlot.transform.localPosition = new Vector3(x * multiplier.x + offset.x, y * multiplier.y + offset.y);
                uiSlots[x, y] = inventorySlot;
                inventorySlots[x, y] = null;
            }
        }
    }

    void updateInventoryUI()
    {
        for (int x = 0; x < inventoryWidth; x++)
        {
            for (int y = 0; y < inventoryHeight; y++)
            {
                if (inventorySlots[x, y] == null)
                {
                    uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().sprite = null;
                    uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().enabled = false;

                    uiSlots[x, y].transform.GetChild(1).GetComponent<Text>().text = "0";
                    uiSlots[x, y].transform.GetChild(1).GetComponent<Text>().enabled = false;
                }
                else
                {
                    uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().enabled = true;
                    uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().sprite = inventorySlots[x, y].item.sprite;

                    uiSlots[x, y].transform.GetChild(1).GetComponent<Text>().enabled = true;
                    uiSlots[x, y].transform.GetChild(1).GetComponent<Text>().text = inventorySlots[x, y].quantity.ToString();
                }
            }
        }
    }

    public void Add(ItemClass item)
    {
        bool added = false;
        for (int y = inventoryHeight - 1; y >= 0; y--)
        {
            if (added) break;
            for (int x = 0;x < inventoryWidth; x++)
            {
                if (inventorySlots[x, y] == null)
                {
                    InventorySlot newSlot = new InventorySlot();
                    inventorySlots[x, y] = newSlot;
                    newSlot.item = item;
                    newSlot.quantity += 1;
                    added = true;
                    break;
                }
            }
        }

        updateInventoryUI();
    }

    public void Remove(ItemClass item)
    {

    }
}
