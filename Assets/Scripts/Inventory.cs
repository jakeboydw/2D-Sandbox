using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Vector2 inventoryOffset;
    public Vector2 hotbarOffset;
    public Vector2 multiplier;

    public GameObject inventoryUI;
    public GameObject hotbarUI;
    public GameObject inventorySlotPrefab;

    public int itemLimit = 64; 
    public int inventoryWidth;
    public int inventoryHeight;
    public InventorySlot[,] inventorySlots;
    public InventorySlot[] hotbarSlots;
    public GameObject[,] uiSlots;
    public GameObject[] hotbarUISlots;

    private void Start()
    {
        inventorySlots = new InventorySlot[inventoryWidth, inventoryHeight];
        uiSlots = new GameObject[inventoryWidth, inventoryHeight];

        hotbarSlots = new InventorySlot[inventoryWidth];
        hotbarUISlots = new GameObject[inventoryWidth];

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
                inventorySlot.transform.localPosition = new Vector3(x * multiplier.x + inventoryOffset.x, y * multiplier.y + inventoryOffset.y);
                uiSlots[x, y] = inventorySlot;
                inventorySlots[x, y] = null;
            }
        }

        for (int x = 0; x < inventoryWidth; x++)
        {
            GameObject hotbarSlot = Instantiate(inventorySlotPrefab, hotbarUI.transform.GetChild(0).transform);
            hotbarSlot.transform.localPosition = new Vector3(x * multiplier.x + hotbarOffset.x, hotbarOffset.y);
            hotbarUISlots[x] = hotbarSlot;
            hotbarSlots[x] = null;
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

                    uiSlots[x, y].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "0";
                    uiSlots[x, y].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
                }
                else
                {
                    uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().enabled = true;
                    uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().sprite = inventorySlots[x, y].item.sprite;

                    uiSlots[x, y].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = true;
                    uiSlots[x, y].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = inventorySlots[x, y].quantity.ToString();
                }
            }
        }

        for (int x = 0; x < inventoryWidth; x++)
        {
            if (inventorySlots[x, inventoryHeight - 1] == null)
            {
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().sprite = null;
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().enabled = false;

                hotbarUISlots[x].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "0";
                hotbarUISlots[x].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = false;
            }
            else
            {
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().enabled = true;
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().sprite = inventorySlots[x, inventoryHeight - 1].item.sprite;

                hotbarUISlots[x].transform.GetChild(1).GetComponent<TextMeshProUGUI>().enabled = true;
                hotbarUISlots[x].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = inventorySlots[x, inventoryHeight - 1].quantity.ToString();
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
                    newSlot.quantity = 1;
                    added = true;
                    break;
                }
                else if (inventorySlots[x, y].item.sprite == item.sprite && inventorySlots[x, y].quantity <= itemLimit)
                {
                    inventorySlots[x, y].quantity += 1;
                    added = true;
                    break;
                }
                else
                {
                    continue;
                }
            }
        }

        updateInventoryUI();
    }

    public bool Remove(ItemClass item)
    {
        for (int y = inventoryHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < inventoryWidth; x++)
            {
                if (inventorySlots[x, y] == null) continue;
                if (inventorySlots[x, y].item == item)
                {
                    inventorySlots[x, y].quantity -= 1;

                    if (inventorySlots[x, y].quantity == 0)
                    {
                        inventorySlots[x, y] = null;
                    }

                    updateInventoryUI();
                    return true;
                }
            }
        }
        return false;
    }
}
