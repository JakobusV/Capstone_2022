using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBarMovement : MonoBehaviour
{
    void Update()
    {
        try
        {
            // Try get player and face towards them
            gameObject.transform.forward = Camera.main.transform.position - gameObject.transform.root.position;
        }
        catch
        {
            
        }
    }
}
