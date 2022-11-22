using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject hitbox;
    public IWeapon weapon;
    public Transform WeaponSwingSpawnPosition;
    public Transform SwingPosition;
    private float swingTime;
    private bool isAttackAvailable = true;
    public bool isAttacking = false;
    public Transform dropPosition;
    private GameObject weapon_model;
    private Vector3 swing_s { get; } = new Vector3(0f, -55f, 0f);
    private Vector3 swing_e { get; } = new Vector3(0f, 55f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        hitbox.SetActive(false);
    }

    private void Update()
    {
        if (isAttacking && weapon_model != null)
        {
            swingTime += Time.deltaTime / weapon.LingerTime;

            SwingPosition.localEulerAngles = new Vector3(
                Mathf.LerpAngle(swing_s.x, swing_e.x, swingTime),
                Mathf.LerpAngle(swing_s.y, swing_e.y, swingTime),
                Mathf.LerpAngle(swing_s.z, swing_e.z, swingTime)
                );
        }
    }

    public void TryAttack()
    {
        if (isAttackAvailable && weapon != null)
        {
            SwingModel();

            isAttackAvailable = false;
            isAttacking = true;

            hitbox.SetActive(true);

            Invoke(nameof(StopAttack), weapon.LingerTime);
            Invoke(nameof(ResetAttack), weapon.CoolDownTime);
        }
    }

    private void SwingModel()
    {
        // Reset swing
        swingTime = 0;
        SwingPosition.localEulerAngles = swing_s;

        // Setup model
        weapon_model = Instantiate(Resources.Load("Models/Weapons/" + weapon.ModelName) as GameObject);
        weapon_model.transform.position = WeaponSwingSpawnPosition.transform.position;
        weapon_model.transform.forward = SwingPosition.forward;
        weapon_model.transform.parent = WeaponSwingSpawnPosition;
    }

    private void ResetAttack()
    {
        isAttackAvailable = true;
    }

    public void Attack(GameObject enemyObject)
    {
        try
        {
            enemyObject.GetComponentInParent<IEnemy>().TakeDamage(weapon.Damage, weapon.Knockback);
        }
        catch (Exception ex)
        {
            Debug.Log(enemyObject.name + " was not an enemy.");
            Debug.Log(ex.Message);
        }
    }

    public void Knockback(GameObject enemyObject)
    {
        Rigidbody enemy_rb = enemyObject.GetComponentInParent<Rigidbody>();

        if (enemy_rb != null)
        {
            // Variables
            Vector3 Heading = enemyObject.transform.position - gameObject.transform.position;
            // Flatten angle
            Heading = new Vector3(Heading.x, 0.1f, Heading.z);

            enemy_rb.velocity = Vector3.zero;
            enemy_rb.AddForce(Heading.normalized * weapon.Knockback, ForceMode.Impulse);
        }
    }

    private void StopAttack()
    {
        isAttacking = false;
        hitbox.SetActive(false);
        if (weapon_model)
        {
            Destroy(weapon_model);
        }
    }
}
