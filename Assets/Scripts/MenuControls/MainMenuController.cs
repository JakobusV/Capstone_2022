using Assets.Scripts.PlayerControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject CreateSpecErrorLabel;
    [SerializeField]
    private GameObject LoadSpecErrorLabel;

    private string fileType = ".lff";

    public void Start()
    {
        if (File.Exists("Saves/Prefs/PlayerPreferences.txt"))
        {
            PlayerPreferences.Read();
        } else
        {
            PlayerPreferences.Write();
        }
    }

    public void CreateGame(GameObject InputTextObject)
    {
        string file = InputTextObject.GetComponent<TMP_InputField>().text;

        if (file.Contains("/"))
        {
            CreateSpecErrorLabel.GetComponent<TMP_Text>().text = "Naming option not allowed.";
        } else if (Directory.Exists("Saves/" + file))
        {
            CreateSpecErrorLabel.GetComponent<TMP_Text>().text = "File already exists with that name.";
        }
        else
        {
            // Create directory
            Directory.CreateDirectory("Saves/" + file);

            // Create first spectacle file
            StringBuilder sb = new StringBuilder("0");
            sb.Append(fileType);

            // Setup manager
            ManagerControl.isReading = false;
            ManagerControl.fileName = sb.ToString();

            // Setup player status
            PlayerStatus.Spectacle_Index = sb.ToString();
            PlayerStatus.Spectacle_Path = file;
            PlayerStatus.Write();

            // Load Scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void LoadGame(GameObject InputContentObject)
    {
        string file = "";

        try
        {
            file = InputContentObject.GetComponent<SavesContentController>().GetSelectedSave();
        } catch (Exception e)
        {
            Debug.LogError(e.Message);
            LoadSpecErrorLabel.GetComponent<TMP_Text>().text = "Please select a world first.";
            return;
        }

        if (file.Contains("/"))
        {
            LoadSpecErrorLabel.GetComponent<TMP_Text>().text = "File naming not allowed.";
        } else if (Directory.Exists("Saves/" + file))
        {
            // Setup player status
            PlayerStatus.Read(file);

            // Setup manager
            ManagerControl.isReading = true;
            ManagerControl.fileName = PlayerStatus.Spectacle_Index;

            // Load Scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        } else
        {
            LoadSpecErrorLabel.GetComponent<TMP_Text>().text = "File doesn't exist.";
        }
    }

    public void ResetLabel(GameObject ErrorLabel)
    {
        ErrorLabel.GetComponent<TMP_Text>().text = "";
    }

    public void SavePreferences()
    {
        PlayerPreferences.Write();
    }

    public void SetFileType(GameObject InputDropDown)
    {
        int value = InputDropDown.GetComponent<TMP_Dropdown>().value;

        switch (value)
        {
            case 0:
                fileType = ".lff";
                break;
            case 1:
                fileType = ".cas";
                break;
            case 2:
                fileType = ".dsq";
                break;
            case 3:
                fileType = ".bsp";
                break;
            default:
                fileType = ".lff";
                break;
        }
    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
