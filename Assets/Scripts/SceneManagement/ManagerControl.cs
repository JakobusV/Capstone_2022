using Assets.Scripts.PlayerControls;
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
    public static bool isReading;
    public static string fileName;

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
        // Pause

        // Create Spectacle
        spectacle = new Spectacle();

        // Read or Create

        // Make algo
        string[] fileDetails = fileName.Split(".");
        spectacle.algorithm = GetAlgorithmByFileType(fileDetails[1]);

        // Make Grid
        if (isReading && File.Exists(PlayerStatus.GetSavePath() + fileName))
        {
            spectacle.GenerateGridFromFile(fileDetails[0]);
        } else
        {
            spectacle.CreateNewGrid(fileDetails[0]);
        }

        // Make Texture
        spectacle.GenerateTexture();

        // Create PNG bytes
        byte[] pngBytes = spectacle.texture.EncodeToPNG();

        // Create PNG
        File.WriteAllBytes(PlayerStatus.GetSavePath() + fileName + ".png", pngBytes);

        // Complete
        spectacle.grid.Build(spectacle.texture);

        // Make Player
        GameObject Player_Camera = Instantiate(Resources.Load("Prefabs/Player_Camera_Bundle") as GameObject);

        // Apply Player


        // Resume
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
            case "bsp":
                algorithm = new BSPR();
                break;
            default:
                throw new System.FormatException("File type of \"." + fileType + "\" not found.");
        }

        return algorithm;
    }

    public static bool FileTypeExist(string fileType)
    {
        bool doesExist = false;
        
        switch (fileType)
        {
            case "lff":
                doesExist = true;
                break;
            case "cas":
                doesExist = true;
                break;
            case "cac":
                doesExist = true;
                break;
            case "dsq":
                doesExist = true;
                break;
            case "bsp":
                doesExist = true;
                break;
            default:
                break;
        }

        return doesExist;
    }
}
