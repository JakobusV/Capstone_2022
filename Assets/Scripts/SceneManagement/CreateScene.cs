using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateScene : MonoBehaviour
{
    public int Size = 10;
    public float Chance = 100;
    public float Decay = 0.9f;
    public Material material;
    public string name;
    public bool isReading;

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
        // Create Grid
        Tile[,] Grid = new Tile[Size, Size];

        // Populate Grid
        for (int i = 0; i < Grid.Length; i++)
        {
            int X = i % Size;
            int Y = i / Size;
            Tile tile = new Tile()
            {
                X = X,
                Y = Y
            };
            Grid[Y, X] = tile;
        }

        // Setup Lazy Flood Fill
        LFF lff = new LFF()
        {
            Height = Size,
            Width = Size,
            Chance = Chance,
            Decay = Decay,
            Grid = Grid,
            Debug = false
        };

        // Get start cordinates
        int Start = Size / 2;

        // Run Lazy Flood Fill
        lff.Run(Start, Start);

        // Display Grid
        lff.Display(gameObject, material);

        // Write new output to file
        lff.Write(name); // NOTE: currently take a name alone, adds .txt and correct pathing. Please enter just a name
    }

    void ReadLFF()
    {
        LFF lff = new LFF();

        lff.Read(name);

        lff.Display(gameObject, material);
    }
}
