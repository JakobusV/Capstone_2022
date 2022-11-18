using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float moveSpeed_Walk = 6;
    public float moveSpeed_Sprint = 8;
    public float moveSpeed_Attack = 5;
    public float moveSpeed_Charge = 3;
    public float moveSpeed_Stop = 0;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool isJumpAvailable = true;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask Ground;
    bool isGrounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    internal bool isGrappling;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);

        GetInput();
        SpeedControl();

        if (isGrounded)
        {
            rb.drag = groundDrag;
        } else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && isJumpAvailable && isGrounded)
        {
            isJumpAvailable = false;

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        if (!isGrappling)
        {
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (isGrounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            } else
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            }
        }
    }

    private void SpeedControl()
    {
        if (!isGrappling)
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    public void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        isJumpAvailable = true;
    }

    public void SetSpeed(int action)
    {
        switch (action)
        {
            case 0:
                Walk();
                break;
            case 1:
                Attack();
                break;
            case 2:
                Charge();
                break;
            case 3:
                Sprint();
                break;
        }
    }

    public void Sprint()
    {
        moveSpeed = moveSpeed_Sprint;
    }

    public void Walk()
    {
        moveSpeed = moveSpeed_Walk;
    }

    public void Charge()
    {
        moveSpeed = moveSpeed_Charge;
    }

    public void Attack()
    {
        moveSpeed = moveSpeed_Attack;
    }

    public void Stop()
    {
        moveSpeed = moveSpeed_Stop;
    }
}
