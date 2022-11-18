using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerModel;
    public Rigidbody rb;

    public float rotationSpeed;

    public Transform combatLookAt;

    public CameraStyle currentStyle;

    private Color playerColor;
    private CinemachineFreeLook CineFreeLook;
    private float SetFOV;
    public enum CameraStyle
    {
        Basic,
        Combat
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CineFreeLook = GameObject.Find("BasicCam").GetComponent<CinemachineFreeLook>();
        SetFOV = CineFreeLook.m_Lens.FieldOfView;
    }

    public void Update()
    {
        FaceTarget();

        DoCameraMovement();

        CheckOpacity();

        SpeedToFOV();
    }

    private void FaceTarget()
    {
        if (Input.GetMouseButton(0))
        {
            currentStyle = CameraStyle.Combat;
        } else
        {
            currentStyle = CameraStyle.Basic;
        }
    }

    private void SpeedToFOV()
    {
        // get player
        GameObject player_obj = player.gameObject;

        // get avr speed which in this case is walk speed
        float avr_speed = player_obj.GetComponent<PlayerMovement>().moveSpeed_Walk;
        // get current move speed
        float cur_speed = player_obj.GetComponent<PlayerMovement>().moveSpeed;
        // get current FOV
        float cur_FOV = CineFreeLook.m_Lens.FieldOfView;

        // get FOV scale
        float FOV_scale = (cur_speed - avr_speed) * 4;

        // set FOV perportionally to set walk speed
        CineFreeLook.m_Lens.FieldOfView = Mathf.Lerp(cur_FOV, SetFOV + FOV_scale, Time.deltaTime * 8);
    }

    private void CheckOpacity()
    {
        float y_axis = CineFreeLook.m_YAxis.Value;

        playerColor = playerModel.gameObject.GetComponent<MeshRenderer>().material.color;

        if (0.18 < y_axis && y_axis < 0.33)
        {
            float y_value = y_axis - 0.165f;
            float correction = 6.06f;

            playerColor.a = y_value * correction;
        } else if (y_axis < 0.18)
        {
            playerColor.a = 0.1f;
        } else
        {
            playerColor.a = 1;
        }

        playerModel.gameObject.GetComponent<MeshRenderer>().material.color = playerColor;
    }

    private void DoCameraMovement()
    {
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        if (currentStyle == CameraStyle.Basic)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
            {
                playerModel.forward = Vector3.Slerp(playerModel.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }
        }
        else if (currentStyle == CameraStyle.Combat)
        {

            Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirToCombatLookAt.normalized;

            playerModel.forward = Vector3.Slerp(playerModel.forward, dirToCombatLookAt.normalized, Time.deltaTime * 24);
        }
    }
}
