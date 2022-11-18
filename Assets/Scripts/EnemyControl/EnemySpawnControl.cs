using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnControl : MonoBehaviour
{
    public LayerMask Ground;
    private RaycastHit hit;

    // Start is called before the first frame update
    public void Spawn()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 5000, Ground))
        {
            GameObject enemy = Instantiate(Resources.Load("Prefabs/BasicEnemy") as GameObject);
            enemy.transform.position = new Vector3(hit.point.x, hit.point.y + 2, hit.point.z);
        }

        Destroy(gameObject);
    }
}
