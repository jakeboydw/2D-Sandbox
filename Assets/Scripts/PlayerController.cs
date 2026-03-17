using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public int selectedSlotIndex = 0;
    public GameObject hotbarSelector;

    public Inventory inventory;
    public bool inventoryShowing = false;

    public TerrainGeneration terrainGenerator;
    public ItemClass selectedItem;
    public float playerRange;

    public Vector2 spawnPos;
    public Vector2Int mousePos;

    public float moveSpeed;
    public float jumpForce;

    public bool onGround;

    private Vector2 inputDirection;
    private Rigidbody2D rb;

    private InputControls control;

    private Animator anim;

    private void Awake()
    {
        control = new InputControls();
        control.Player.Jump.started += Jump;
        control.Player.Hit.started += Hit;
        control.Player.Place.started += Place;
    }

    public void Spawn()
    {
        transform.position = spawnPos;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        inventory = GetComponent<Inventory>();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (onGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void Hit(InputAction.CallbackContext context)
    {
        if (Vector2.Distance(transform.position, mousePos) <= playerRange)
        {
            anim.SetTrigger("hit");
            terrainGenerator.RemoveTile(mousePos.x, mousePos.y);
        }
    }

    private void Place(InputAction.CallbackContext context)
    {
        if (Vector2.Distance(transform.position, mousePos) <= playerRange && Vector2.Distance(transform.position, mousePos) > 1f)
        {
            anim.SetTrigger("hit");
            if (selectedItem != null)
            {
                if (selectedItem.itemType == ItemClass.ItemType.block)
                {
                    bool isPlaced = terrainGenerator.PlaceTile(selectedItem.tileClass, mousePos.x, mousePos.y);
                    if (isPlaced)
                    {
                        inventory.Remove(selectedItem);
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        control.Enable();
    }

    private void OnDisable()
    {
        control.Disable();
    }

    private void Update()
    {
        mousePos.x = (int) Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()).x;
        mousePos.y = (int) Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()).y;

        inputDirection = control.Player.Move.ReadValue<Vector2>();

        anim.SetFloat("horizontalSpeed", inputDirection.x);

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            inventoryShowing = !inventoryShowing;

            if (inventoryShowing)
            {
                control.Player.Disable();
            }
            else
            {
                control.Player.Enable();
            }
        }

        inventory.inventoryUI.SetActive(inventoryShowing);

        if (Mouse.current.scroll.ReadValue().y > 0)
        {
            if (selectedSlotIndex < inventory.inventoryWidth - 1)
            {
                selectedSlotIndex += 1;
            }
        }
        else if (Mouse.current.scroll.ReadValue().y < 0)
        {
            if (selectedSlotIndex > 0)
            {
                selectedSlotIndex -= 1;
            }
        }

        hotbarSelector.transform.position = inventory.hotbarUISlots[selectedSlotIndex].transform.position;

        if (inventory.inventorySlots[selectedSlotIndex, inventory.inventoryHeight - 1] != null)
        {
            selectedItem = inventory.inventorySlots[selectedSlotIndex, inventory.inventoryHeight - 1].item;
        }
        else
        {
            selectedItem = null;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(inputDirection.x * moveSpeed * Time.deltaTime, rb.linearVelocity.y);

        if (inputDirection.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (inputDirection.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
