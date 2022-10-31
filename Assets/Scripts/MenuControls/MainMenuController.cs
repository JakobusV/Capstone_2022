using Assets.Scripts.PlayerControls;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public void CreateGame(GameObject InputTextObject)
    {
        string file = InputTextObject.GetComponent<TMP_InputField>().text;

        if (File.Exists("Saves/" + file))
        {
            CreateSpecErrorLabel.GetComponent<TMP_Text>().text = "File already exists.";
        }
        else
        {
            string[] fileSplit = file.Split(".");
            if (ManagerControl.FileTypeExist(fileSplit[fileSplit.Length - 1]))
            {
                ManagerControl.isReading = false;
                ManagerControl.fileName = file;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            } else
            {
                CreateSpecErrorLabel.GetComponent<TMP_Text>().text = "File type not accepted.";
            }
        }
    }

    public void LoadGame(GameObject InputTextObject)
    {
        string file = InputTextObject.GetComponent<TMP_InputField>().text;

        if (File.Exists("Saves/" + file))
        {
            string[] fileSplit = file.Split(".");
            if (ManagerControl.FileTypeExist(fileSplit[fileSplit.Length - 1]))
            {
                ManagerControl.isReading = true;
                ManagerControl.fileName = file;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            } else
            {
                LoadSpecErrorLabel.GetComponent<TMP_Text>().text = "File type not accepted.";
            }
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

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
