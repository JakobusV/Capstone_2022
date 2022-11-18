using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject hitbox;
    public float attackCooldown = 0.5f;
    public float attackLinger = 0.2f;
    private bool isAttackAvailable = true;
    public bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        hitbox.SetActive(false);
    }

    public void TryAttack()
    {
        if (isAttackAvailable)
        {
            isAttackAvailable = false;

            Attack();

            Invoke(nameof(ResetAttack), attackCooldown);
        }
    }

    private void ResetAttack()
    {
        isAttackAvailable = true;
    }

    private void Attack()
    {
        isAttacking = true;
        hitbox.SetActive(true);

        Invoke(nameof(StopAttack), attackLinger);
    }

    private void StopAttack()
    {
        isAttacking = false;
        hitbox.SetActive(false);
    }
}
