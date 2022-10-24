using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ManagerControl : MonoBehaviour
{
    /*
    public bool isUsingCellularAutomata;
    public bool isSimpleCA;
    public bool isUsingLazyFloodFill;

    public int Size = 10;
    public float Decay = 0.9f;
    public uint CellularAutomataLoops = 2;*/
    public bool isReading;
    public string fileName;

    /*public static bool isExisting;
    public static string fileType;*/
    private Spectacle spectacle;

    // Start is called before the first frame update
    void Start()
    {
        Run();
    }

    void Run()
    {
        // Create Spectacle
        spectacle = new Spectacle();

        // Read or Create

        // Make algo
        string[] fileDetails = fileName.Split(".");
        spectacle.algorithm = GetAlgorithmByFileType(fileDetails[1]);

        // Make Grid
        if (isReading && File.Exists("Saves/" + fileName))
        {
            spectacle.GenerateGridFromFile(fileDetails[0]);
        } else
        {
            spectacle.CreateNewGrid(fileDetails[0]);
        }

        // Make Texture
        spectacle.GenerateTexture();

        // Complete
        spectacle.grid.Build(spectacle.texture);

        // Player
            // Make Player
            // Apply Player
            // Play...?
    }

    private Algorithm GetAlgorithmByFileType(string fileType)
    {
        Algorithm algorithm;

        switch (fileType)
        {
            case "lff":
                algorithm = new LFF();
                break;
            case "cas":
                algorithm = new CellularAutomata_Simple();
                break;
            case "cac":
                algorithm = new CellularAutomata_Complex();
                break;
            case "dsq":
                algorithm = new DS();
                break;
            default:
                throw new System.FormatException("File type of \"." + fileType + "\" not found.");
        }

        return algorithm;
    }
}
