using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCollision : MonoBehaviour
{
    bool underwater = false;
    GameObject UW_Filter = null;
    Collider PlayerModelCollider = null;
    Collider collider = null;

    private void Start()
    {
        collider = gameObject.GetComponent<Collider>();
    }

    private void Update()
    {
        if (UW_Filter != null && PlayerModelCollider != null)
        {
            underwater = collider.bounds.Intersects(PlayerModelCollider.bounds);
            
            UW_Filter.SetActive(underwater);
        }
        else
        {
            // Try to get filter
            try
            {
                UW_Filter = GameObject.Find("UW_Filter");
            }
            catch { }

            // Try to get collider
            try
            {
                PlayerModelCollider = GameObject.Find("PlayerModel").GetComponent<Collider>();
            }
            catch { }
        }
    }
}
