using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickupControl : MonoBehaviour, IPickup
{
    public string obj;

    public void Pickup()
    {
        GameObject.Find("PlayerCamBundle").GetComponent<PlayerPartManager>().WeaponPickup(obj);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PlayerModel")
        {
            GameObject.Find("PlayerCamBundle").GetComponent<PlayerPartManager>().pickupQue.Add(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "PlayerModel")
        {
            GameObject.Find("PlayerCamBundle").GetComponent<PlayerPartManager>().pickupQue.Remove(this);
        }
    }
}
