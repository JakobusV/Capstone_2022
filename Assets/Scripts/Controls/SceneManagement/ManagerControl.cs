using Assets.Scripts.PlayerControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public static bool isPaused;
    public LayerMask Ground;

    /*public static bool isExisting;
    public static string fileType;*/
    private Spectacle spectacle;
    private GameObject player;
    private List<IEnemy> enemyList = new List<IEnemy>();
    private System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        Run();
    }

    internal void CreateWater()
    {
        GameObject water = Instantiate(Resources.Load("Prefabs/Water") as GameObject);

        water.transform.position = new Vector3(spectacle.grid.Tiles.GetLength(0) / 2, -131, spectacle.grid.Tiles.GetLength(1) / 2);

        water.transform.localScale = new Vector3(spectacle.grid.Tiles.GetLength(0), 100, spectacle.grid.Tiles.GetLength(1));
    }

    private void Update()
    {
        PauseControl();

        GetNumberInput();
    }

    private void GetNumberInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            string Spectacle_Index = "1";
            string Spectacle_Extension = ".lff";

            SceneChange(Spectacle_Index, Spectacle_Extension);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            string Spectacle_Index = "2";
            string Spectacle_Extension = ".cas";

            SceneChange(Spectacle_Index, Spectacle_Extension);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            string Spectacle_Index = "3";
            string Spectacle_Extension = ".dsq";

            SceneChange(Spectacle_Index, Spectacle_Extension);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            string Spectacle_Index = "4";
            string Spectacle_Extension = ".bsp";

            SceneChange(Spectacle_Index, Spectacle_Extension);
        }
    }

    /// <summary>
    /// This method should be called by other objects or things in the scene to change what spectacle is currently loaded and playable.
    /// When called, SceneChange should receive an index to change to and a file extension it should be expecting to use.
    /// If an index does not exist when sent to SceneChange, it will create the file with the default setup for that file.
    /// </summary>
    /// <param name="Spectacle_Index">File name. ex: 1, 2, or 3</param>
    /// <param name="Spectacle_Extension">File extension. ex: .lff, .cas, or .dsq</param>
    private void SceneChange(string Spectacle_Index, string Spectacle_Extension)
    {
        if (string.IsNullOrEmpty(Spectacle_Index))
        {
            // Get next index
            Spectacle_Index = PlayerStatus.GetNextIndex();
        }

        SaveAndWrite();

        //FadeToBlack();

        // Swap status
        PlayerStatus.SwapStatus(Spectacle_Index, Spectacle_Extension);


        // Get algo file & setup manager for reload
        FileInfo file = new FileInfo(PlayerStatus.GetSavePath() + Spectacle_Index + Spectacle_Extension);
        // reading if file exists
        isReading = file.Exists;
        fileName = Spectacle_Index + Spectacle_Extension;

        // Reload scene with new settings
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void PauseControl()
    {
        if (player != null && Input.GetKeyDown(KeyCode.Tab))
        {
            if (isPaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        player.GetComponent<PlayerPartManager>().Resume();
        Time.timeScale = 1f;
        isPaused = false;
    }

    private void Pause()
    {
        player.GetComponent<PlayerPartManager>().Pause();
        SaveAndWrite();
        Time.timeScale = 0f;
        isPaused = true;
    }

    private void SaveAndWrite()
    {
        player.GetComponent<PlayerPartManager>().UpdateStatus();
        PlayerStatus.Write();
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
        spectacle.grid.Build(spectacle.texture, spectacle.algorithm);

        // Make Player
        player = Instantiate(Resources.Load("Prefabs/Player_Camera_Bundle") as GameObject);
        player.name = "PlayerCamBundle";

        // Create enemies
        if (!isReading)
        {
            // Create all enemies
            enemyList = GenerateListOfEnemies();
        }
    }

    private List<IEnemy> GenerateListOfEnemies()
    {
        int x_len_cap = spectacle.grid.Tiles.GetLength(0);
        int y_len_cap = spectacle.grid.Tiles.GetLength(1);

        while (enemyList.Count < 5)
        {
            int x_ran = random.Next(0, x_len_cap);
            int y_ran = random.Next(0, y_len_cap);

            IEnemy new_enemy = CreateEnemy(y_ran, x_ran);

            enemyList.Add(new_enemy);
        }

        return enemyList;
    }

    private IEnemy CreateEnemy(int y_ran, int x_ran)
    {
        GameObject enemy_obj = Instantiate(Resources.Load("Prefabs/Enemy/BasicEnemy") as GameObject);

        RaycastHit hit;
        Vector3 SpawnPoint = new Vector3(x_ran, 1000f, y_ran);
        Physics.Raycast(SpawnPoint, Vector3.down, out hit, 5000f, Ground);

        enemy_obj.transform.position = hit.point + Vector3.up;

        return enemy_obj.GetComponent<BasicEnemyControl>();
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

    private string FindFileType(string PathToSpectacleIndex)
    {
        string[] fileTypes = { ".lff", ".cas", ".cac", ".dsq", ".bsp" };
        FileInfo file = null;

        foreach (string fileType in fileTypes)
        {
            file = new FileInfo(PathToSpectacleIndex + fileType);
            if (file.Exists)
            {
                return fileType;
            }
        }
        
        return "";
    }

    internal void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
