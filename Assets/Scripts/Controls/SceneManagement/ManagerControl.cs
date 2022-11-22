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
    public static bool isNew;

    /*public static bool isExisting;
    public static string fileType;*/
    private Spectacle spectacle;
    private GameObject player;
    private List<IEnemy> enemyList;
    private System.Random random;

    // Start is called before the first frame update
    void Start()
    {
        Run();
    }

    private void Update()
    {
        PauseControl();
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
        player.GetComponent<PlayerPartManager>().UpdateStatus();
        PlayerStatus.Write();
        Time.timeScale = 0f;
        isPaused = true;
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
        player = Instantiate(Resources.Load("Prefabs/Player_Camera_Bundle") as GameObject);
        player.name = "PlayerCamBundle";

        // Apply Player

        // Find all Spawners and activate them.
        /*GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
        foreach (GameObject spawner in spawners)
        {
            spawner.GetComponent<EnemySpawnControl>().Spawn();
        }*/

        if (isNew)
        {
            // Create all enemies
            enemyList = GenerateListOfEnemies();
        }
        else
        {
            // Place all enemies

        }

        // Resume

    }

    private List<IEnemy> GenerateListOfEnemies()
    {
        int x_len_cap = spectacle.grid.Tiles.GetLength(0);
        int y_len_cap = spectacle.grid.Tiles.GetLength(1);

        while (enemyList.Count < 5)
        {
            int x_ran = random.Next(0, x_len_cap);
            int y_ran = random.Next(0, y_len_cap);

            if (spectacle.grid.Tiles[y_ran, x_ran].Value == 0)
            {
                //TODO: Spawn enemys to specific designated areas
            }

            IEnemy new_enemy = CreateEnemy(y_ran, x_ran);
        }

        return null;
    }

    private IEnemy CreateEnemy(int y_ran, int x_ran)
    {
        GameObject enemy_obj = Instantiate(Resources.Load("Prefabs/Enemy/BasicEnemy") as GameObject);
        

        return null;
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

    internal void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
