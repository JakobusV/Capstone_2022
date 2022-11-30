using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilizeWaterFilter : MonoBehaviour
{
    void Start()
    {
        if (GameObject.Find("Water(Clone)") == null && GameObject.Find("Water") == null)
        {
            GameObject UW_Filter = GameObject.Find("UW_Filter");

            UW_Filter.SetActive(false);
        }
    }
}
