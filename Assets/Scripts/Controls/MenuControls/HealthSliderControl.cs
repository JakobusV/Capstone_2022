using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSliderControl : MonoBehaviour
{
    public float transitionSpeed = 8;

    private RectTransform rect;
    private Slider slider;
    private bool showing;
    private Vector3 targetPosition;
    private Vector3 startingPosition;
    private float tar_val;

    private void Start()
    {
        rect = gameObject.GetComponent<RectTransform>();
        slider = gameObject.GetComponent<Slider>();

        targetPosition = new Vector3(rect.position.x, rect.position.y - 55, rect.position.z);
        startingPosition = rect.position;

        tar_val = slider.value;
    }

    private void Update()
    {
        if (rect != null)
        {
            if (showing)
            {
                DropDown();

                Invoke(nameof(StopShowing), 2f);
            }
            else
            {
                ReturnUp();
            }
/*
            if (slider.value != tar_val)
            {
                UpdateValue();
            }*/
        }
    }

    private void UpdateValue()
    {
        // We allow a hard coded value here to ensuer the updated health shows up twice as fast as the animation
        slider.value = Mathf.Lerp(slider.value, tar_val, Time.deltaTime * transitionSpeed * 2);
    }

    public void SetValue(float value)
    {
        try
        {
            slider.value = value;
        }
        catch (NullReferenceException)
        {
            slider = gameObject.GetComponent<Slider>();
            slider.value = value;
        }
    }

    public float GetValue()
    {
        return slider.value;
    }

    private void StopShowing()
    {
        showing = false;
    }

    public void ShowSelf()
    {
        showing = true;
    }

    private void DropDown()
    {
        rect.position = Vector3.Lerp(rect.position, targetPosition, Time.deltaTime * transitionSpeed);
    }

    private void ReturnUp()
    {
        rect.position = Vector3.Lerp(rect.position, startingPosition, Time.deltaTime * transitionSpeed); 
    }
}
