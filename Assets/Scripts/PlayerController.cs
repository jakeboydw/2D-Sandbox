using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;

    private bool onGround;
    private Vector2 inputDirection;
    private Rigidbody2D rb;

    private InputControls control;

    private Animator anim;

    private void Awake()
    {
        control = new InputControls();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        control.Player.Jump.started += Jump;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            onGround = false;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (onGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
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
        inputDirection = control.Player.Move.ReadValue<Vector2>();

        anim.SetFloat("horizontalSpeed", inputDirection.x);
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
