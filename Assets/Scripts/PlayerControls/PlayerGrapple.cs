using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGrapple : MonoBehaviour
{
    private GameObject canvas;
    private GameObject knob;
    private GameObject growCircle;
    private GameObject basicCam;
    private GameObject player;
    private GameObject grapple_model;
    public bool isGrappleEquipped;
    public bool isCharging;
    public bool isGrappling;
    public float chargeAmount = 0.8f;
    private float chargeTotal;
    public int grappleLayer = 6;
    private int layerMask;
    public Vector3? attachedPoint;
    public RaycastHit hit;
    private RaycastHit UIhit;

    // Start is called before the first frame update
    void Start()
    {
        knob = GameObject.Find("Knob");
        growCircle = GameObject.Find("GrowCircle");

        basicCam = GameObject.Find("BasicCam");

        player = GameObject.Find("Player");

        growCircle.SetActive(false);

        layerMask = 1 << grappleLayer;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();

        if (!isCharging)
        {
            chargeTotal = 0f;
            growCircle.SetActive(false);
        } else
        {
            chargeTotal += chargeAmount;
            //chargeTotal = Mathf.Lerp(chargeTotal, chargeTotal + chargeAmount, Time.deltaTime * 16);
        }

        GrappleModelManager();
    }

    private void GrappleModelManager()
    {
        if (attachedPoint != null)
        {
            if (grapple_model == null)
            {
                grapple_model = Instantiate(Resources.Load("Prefabs/GrappleModel") as GameObject);
                grapple_model.transform.parent = player.transform;
                
                // Set rotation to face attached point
                var look_position = (Vector3)attachedPoint - player.transform.position;
                grapple_model.transform.localRotation = Quaternion.LookRotation(look_position);

                // Set position between two points
                grapple_model.transform.position = Vector3.Lerp(player.transform.position, (Vector3)attachedPoint, 0.5f);

                // Get distance from attach to player
                var distance_difference = Vector3.Distance(player.transform.position, (Vector3)attachedPoint);
                grapple_model.transform.localScale = new Vector3(grapple_model.transform.localScale.x, grapple_model.transform.localScale.y, distance_difference);
            } else
            {
                // Set position between two points
                grapple_model.transform.position = Vector3.Lerp(player.transform.position, (Vector3)attachedPoint, 0.5f);

                // Get distance from attach to player
                var distance_difference = Vector3.Distance(player.transform.position, (Vector3)attachedPoint);
                grapple_model.transform.localScale = new Vector3(grapple_model.transform.localScale.x, grapple_model.transform.localScale.y, distance_difference);
            }
        }
        else
        {
            if (grapple_model != null)
            {
                Destroy(grapple_model);
            }
        }
    }

    public void TryCharge()
    {
        if (chargeTotal < 100)
        {
            isCharging = true;

            growCircle.SetActive(true);
        } else if (Input.GetMouseButton(1))
        {
            AttachGrapple();

            isGrappling = true;
        }
    }

    public void TryGrapple()
    {

    }

    private void AttachGrapple()
    {
        if (Physics.Raycast(basicCam.transform.position, Camera.main.transform.forward, out hit, 5000, layerMask))
        {
            attachedPoint = hit.point;
            //Debug.Log(hit.point);
        }
        else
        {
            attachedPoint = null;
        }
    }

    private void UpdateUI()
    {
        if (isGrappleEquipped)
        {
            Debug.DrawRay(basicCam.transform.position, Camera.main.transform.forward, Color.green);

            if (Physics.Raycast(basicCam.transform.position, Camera.main.transform.forward, out UIhit, 5000, layerMask))
            {
                Debug.DrawLine(basicCam.transform.position, UIhit.point, Color.yellow);
                knob.GetComponent<Image>().color = Color.yellow;
            } else
            {
                Debug.DrawLine(basicCam.transform.position, UIhit.point, Color.red);
                knob.GetComponent<Image>().color = Color.red;
            }

            float growCircleValue = (100 - chargeTotal) / 2;

            growCircle.GetComponent<RectTransform>().sizeDelta = new Vector2(growCircleValue, growCircleValue);
        } else
        {
            knob.GetComponent<Image>().color = Color.white;
        }
    }
}
