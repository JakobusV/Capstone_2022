using Assets.Scripts.PlayerControls;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PreferenceMenuController : MonoBehaviour
{
    void Awake()
    {
        // Read file
        try
        {
            PlayerPreferences.Read();
        }
        catch (Exception ex)
        {
            Debug.Log("Could not read from file.");
            Debug.LogException(ex);
        }

        // Set UI sliders to the preferences current settings
        SetSliders();
    }

    public void SetSliders()
    {
        // Get array of all field names in player preferences
        string[] fieldNames = typeof(PlayerPreferences).GetFields().Select(fi => fi.Name).ToArray();

        // for each field in player preference
        foreach (Transform child in transform)
        {
            // if the name of the child matches any field name
            if (fieldNames.Any(fi => fi == child.name))
            {
                // Get slider from child
                Slider slider = child.GetComponent<Slider>();

                // Use childs name to get the value from player preferences
                var value = typeof(PlayerPreferences).GetField(child.name).GetValue(null);

                // Convert it to a float and set it
                slider.value = (float)value;
            }
        }
    }

    public void UpdatePreference(string preferenceName)
    {
        // Get child
        Transform child = gameObject.transform.Find(preferenceName);

        // Get slider
        Slider slider = child.GetComponent<Slider>();

        // Get value
        float value = slider.value;

        // Get field
        FieldInfo field = typeof(PlayerPreferences).GetField(preferenceName, BindingFlags.Public | BindingFlags.Static);

        // Set preference
        field.SetValue(null, value);
    }
}
