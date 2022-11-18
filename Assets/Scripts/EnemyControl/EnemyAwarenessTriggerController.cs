using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAwarenessTriggerController : MonoBehaviour
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
            try
            {
                enemyControl.BecomeAware();
            }
            catch (NullReferenceException)
            {
                Start();
                enemyControl.BecomeAware();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "PlayerModel")
        {
            enemyControl.BecomeUnaware();
        }
    }
}
