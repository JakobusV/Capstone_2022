using Assets.Scripts.PlayerControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Cinemachine;

public class PlayerPartManager : MonoBehaviour
{
    [SerializeField]
    private float Health = 10f;

    public void Start()
    {
        SetupPreferences();
        SetupStatus();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            UpdateStatus();
            PlayerStatus.Write();
        }
    }

    public void SetupPreferences()
    {
        foreach (FieldInfo field in typeof(PlayerPreferences).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            // Get name of field
            string name = field.Name;

            // Get value rom field
            var value = field.GetValue(null);

            // Switch to option appropriate for applying the value to the correct part
            switch (name)
            {
                case "Vertical_Sensitivity":
                    gameObject.GetComponentInChildren<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = (float)value;
                    break;
                case "Horizontal_Sensitivity":
                    gameObject.GetComponentInChildren<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = (float)value * 100;
                    break;
                case "Sound":
                    break;
            }
        }
    }

    public void SetupStatus()
    {
        foreach (FieldInfo field in typeof(PlayerStatus).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            // Get name of field
            string name = field.Name;

            // Get value from field
            var value = field.GetValue(null);

            // Get player transform for xyz cases
            Transform tf = GameObject.Find("Player").transform;

            // Switch to option appropriate for applying the value to the correct part
            switch (name)
            {
                case "Health":
                    Health = (float)value;
                    break;
                case "Move_Speed":
                    gameObject.GetComponentInChildren<PlayerMovement>().moveSpeed = (float)value;
                    break;
                case "Jump_Force":
                    gameObject.GetComponentInChildren<PlayerMovement>().jumpForce = (float)value;
                    break;
                case "Position_X":
                    tf.position = new Vector3((float)value, tf.position.y, tf.position.z);
                    break;
                case "Position_Y":
                    tf.position = new Vector3(tf.position.x, (float)value, tf.position.z);
                    break;
                case "Position_Z":
                    tf.position = new Vector3(tf.position.x, tf.position.y, (float)value);
                    break;
                default:
                    Debug.Log("IGNORED: " + name + "=" + value);
                    break;
            }
        }
    }

    public void UpdateStatus()
    {
        foreach (FieldInfo field in typeof(PlayerStatus).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            // Get name of field
            string name = field.Name;

            // Get player transform for xyz cases
            Transform tf = GameObject.Find("Player").transform;

            // Switch to option appropriate for applying the value from the correct part
            switch (name)
            {
                case "Health":
                    field.SetValue(null, Health);
                    break;
                case "Move_Speed":
                    field.SetValue(null, gameObject.GetComponentInChildren<PlayerMovement>().moveSpeed);
                    break;
                case "Jump_Force":
                    field.SetValue(null, gameObject.GetComponentInChildren<PlayerMovement>().jumpForce);
                    break;
                case "Position_X":
                    field.SetValue(null, tf.position.x);
                    break;
                case "Position_Y":
                    field.SetValue(null, tf.position.y);
                    break;
                case "Position_Z":
                    field.SetValue(null, tf.position.z);
                    break;
                default:
                    Debug.Log("IGNORED: " + name);
                    break;
            }
        }
    }
}
