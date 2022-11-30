using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Spectacle
{
    public GameObject gameObject { get; set; }
    public Algorithm algorithm { get; set; }
    public Grid grid { get; set; }
    public Texture2D texture { get; set; }

    public Spectacle()
    {
        GenerateGameObject();

        grid = new Grid(128, 128);

        grid.gameObject.transform.parent = gameObject.transform;
    }

    public void CreateNewGrid(string fileName)
    {
        if (algorithm.GetType() == typeof(CellularAutomata_Simple)
            || algorithm.GetType() == typeof(CellularAutomata_Complex))
        {
            grid.GenerateRandomGrid();
        } else if (algorithm.GetType() == typeof(DS))
        {
            grid.GenerateTwoNSquareGrid();

            GameObject.Find("Manager").GetComponent<ManagerControl>().CreateWater();
        } else if (algorithm.GetType() == typeof(BSPR))
        {
            grid.GenerateTwoNSquareGrid();
        }

        algorithm.Tiles = grid.Tiles;

        grid.Tiles = algorithm.Run();

        algorithm.Write(fileName);
    }

    public void GenerateGridFromFile(string fileName)
    {
        if (algorithm.GetType() == typeof(DS))
        {
            GameObject.Find("Manager").GetComponent<ManagerControl>().CreateWater();
        }

        grid.Tiles = algorithm.Read(fileName);
    }

    private void GenerateGameObject()
    {
        gameObject = new GameObject();
        gameObject.name = "Spectacle";
        gameObject.transform.position = new Vector3(0, 0, 0);
    }

    internal void GenerateTexture()
    {
        if (algorithm.GetType() == typeof(DS))
        {
            DS ds = (DS)algorithm;
            float Height = (float)ds.Height.Length;
            texture = TextureGeneration.ValueMapTextureDynamic(grid.Tiles, Height);
        } else
        {
            texture = TextureGeneration.ValueMapTextureBinary(grid.Tiles);
        }
    }
}
