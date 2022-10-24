using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateScene : MonoBehaviour
{
    public int Size = 10;
    public float Decay = 0.9f;
    public float Chance = 100;
    public Material material;
    public bool isReading;
/*
    void Start()
    {
        if (isReading)
        {
            ReadLFF();
        } else
        {
            NewLFF();
        }
    }

    void NewLFF()
    {

        // Display Grid
        //lff.Display(gameObject, material);
        lff.Build(gameObject);

        // Write new output to file
        lff.Write(name); // NOTE: currently take a name alone, adds .txt and correct pathing. Please enter just a name
    }

    void ReadLFF()
    {
        LFF lff = new LFF();

        lff.Read(name);

        //lff.Display(gameObject, material);
        
    }*/
}
