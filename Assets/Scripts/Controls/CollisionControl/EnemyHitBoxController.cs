using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBoxController : MonoBehaviour
{
    private BasicEnemyControl enemyControl;
    private void Start()
    {
        enemyControl = GetComponentInParent<BasicEnemyControl>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PlayerModel")
        {
            enemyControl.shouldAttack = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "PlayerModel")
        {
            enemyControl.shouldAttack = false;
        }
    }
}
