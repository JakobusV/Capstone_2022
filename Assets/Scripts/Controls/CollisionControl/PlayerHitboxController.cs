using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitboxController : MonoBehaviour
{
    private PlayerAttack player;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerAttack>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            player.Knockback(other.gameObject);
            player.Attack(other.gameObject);
        }
    }
}
