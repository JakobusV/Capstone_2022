using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicEnemyControl : MonoBehaviour, IEnemy
{
    public GameObject enemy_model;
    [Header("Movement")]
    public float moveSpeed;
    public float rotationSpeed;
    public float jumpForce;
    public float dragValue;
    [Header("Misc. Controls")]
    public float maxHealth;
    public bool isEmoting;
    public float waitTime;
    public bool isAwareOfPlayer;
    public Collider awareness_collider;
    [Header("Ground Check")]
    public bool isGrounded;
    public float enemyHeight;
    public float enemyRadius;
    public LayerMask Ground;
    [Header("Attack")]
    public float damageValue;
    public float knockback;
    public bool shouldAttack;
    public bool isAttacking;
    public float attackCooldown;

    private GameObject player;
    private Rigidbody rb;
    private Slider health_slider;
    private bool isHit;
    private bool isDead;

    public float Health { get; set; }

    void Start()
    {
        // Setup private components
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        health_slider = GetComponentInChildren<Slider>();

        // Setup Slider
        Health = maxHealth;
        health_slider.maxValue = Health;
        health_slider.value = Health;

        health_slider.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();

        if (isGrounded)
        {
            rb.drag = dragValue;
        } else
        {
            rb.drag = 0f;
        }

        if (isAwareOfPlayer && !isAttacking)
        {
            FacePlayer();

            if (shouldAttack)
            {
                TryAttack();
            }
            else if (!isEmoting)
            {
                if (awareness_collider.bounds.Contains(player.transform.position))
                {
                    Move();

                    CheckSpeed();

                    CheckForJump();
                } 
                else
                {
                    isAwareOfPlayer = false;
                }
            }
        } else if (!isAwareOfPlayer)
        {
            SlowDown();
        }
    }

    private void UpdateHealthBar()
    {
        if (health_slider != null)
        {
            health_slider.value = Health;

            if (Health <= 0 || Health == maxHealth)
            {
                health_slider.gameObject.SetActive(false);
            }
            else
            {
                health_slider.gameObject.SetActive(true);
            }
        }
    }

    public void TryAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;

            Attack();

            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    private void ResetAttack()
    {
        isAttacking = false;
        BecomeAware();
    }

    public void Attack()
    {
        isAwareOfPlayer = false;
        Rigidbody player_rb = player.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        Vector3 launchDirection = new Vector3(
            enemy_model.transform.forward.normalized.x
            , enemy_model.transform.forward.normalized.y + 0.4F
            , enemy_model.transform.forward.normalized.z);
        player_rb.velocity = launchDirection * knockback;
        rb.AddForce(enemy_model.transform.forward.normalized * -4, ForceMode.Impulse);
        player.GetComponentInParent<PlayerPartManager>().TakeDamage(damageValue);
    }

    private void CheckGround()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, enemyHeight * 0.5f + 0.3f, Ground);
    }

    private void CheckForJump()
    {
        if (isGrounded)
        {
            Vector3 aboveGround = new Vector3(transform.position.x, transform.position.y - enemyHeight * 0.45f, transform.position.z);

            Debug.DrawRay(aboveGround, transform.forward, Color.green);

            if (Physics.Raycast(aboveGround, enemy_model.transform.forward, enemyRadius + 0.2f, Ground))
            {
                Jump();
            }
        }
    }

    public void BecomeAware()
    {
        // Become aware
        isAwareOfPlayer = true;

        // Emote
        isEmoting = true;
        // Instead of calling Jump, we use a specific jumpForce and ForceMode here for the emote
        rb.AddForce(transform.up * 4, ForceMode.Impulse);

        // Apply wait time to finish emote
        Invoke(nameof(FinishEmoting), waitTime);
    }

    private void FinishEmoting()
    {
        isEmoting = false;
    }

    private void FinishHit()
    {
        isHit = false;
    }

    public void BecomeUnaware()
    {
        isAwareOfPlayer = false;
    }

    private void SlowDown()
    {
        Vector3 stopHorizontal = new Vector3(rb.velocity.x * -1, 0f, rb.velocity.z * -1);

        rb.AddForce(stopHorizontal, ForceMode.Acceleration);
    }

    private void FacePlayer()
    {
        Vector3 dirToPlayer = player.transform.position - new Vector3(transform.position.x, player.transform.position.y, transform.position.z);

        enemy_model.transform.forward = Vector3.Slerp(enemy_model.transform.forward, dirToPlayer, Time.deltaTime * rotationSpeed);
    }

    private void Move()
    {
        rb.AddForce(enemy_model.transform.forward.normalized * moveSpeed, ForceMode.Force);
    }

    private void CheckSpeed()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (!isHit && flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Force);
    }

    public void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }

    public void TakeDamage(float Damage, float Knockback)
    {
        // Allow knockback
        isHit = true;

        // Do damage 
        Health -= Damage;

        // Correct Health if it goes out of bounds
        Health = (Health < 0) ? 0 : Health;
        Health = (Health > maxHealth) ? maxHealth : Health;

        // Update Health Bar
        UpdateHealthBar();

        // Check if enemy needs to die
        if (Health <= 0)
        {
            // KILL
            Die();
        } 
        else
        {
            // Stop being hit
            Invoke(nameof(FinishHit), knockback * 0.01f);
        }
    }
}
