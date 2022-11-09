using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SavesContentController : MonoBehaviour
{
    private string filePath = "Saves/";

    [SerializeField]
    private string selectedSave = "";

    // Start is called before the first frame update
    void Start()
    {
        foreach (string fullPath in Directory.GetDirectories(filePath))
        {
            // Get just directory name
            string path = fullPath.Split("/")[1];

            if (path != "default")
            {
                // Create prefab
                GameObject SaveBlock = Instantiate(Resources.Load("Prefabs/SaveBlock") as GameObject);

                // Set prefab as child
                SaveBlock.transform.SetParent(transform);

                // Set name as directory value
                SaveBlock.name = path + "_SB";

                // Get current spectacle
                string activeSpectacle = GetActiveSpectacle(fullPath);

                // Setup SaveBlock with directory value
                SaveBlock.GetComponent<SaveBlockController>().SetupSaveBlock(path, activeSpectacle);
            }
        }
    }

    public void SetSelectedSave(string value)
    {
        if (selectedSave != "")
        {
            // Get current selected save and disable it
            GameObject.Find(selectedSave + "_SB").GetComponent<Toggle>().isOn = false;

            // Set selectedSave as value
            selectedSave = value;
        } else
        {
            // Set selectedSave as value
            selectedSave = value;
        }
    }

    public string GetSelectedSave()
    {
        // Validate String
        if (string.IsNullOrEmpty(selectedSave))
        {
            throw new System.Exception("selectedSave:" + selectedSave);
        }

        return selectedSave;
    }

    private string GetActiveSpectacle(string saveToDirectoryPath)
    {
        string statusPath = saveToDirectoryPath + "/status.mgn";

        Regex re = new Regex(@"Spectacle_Index=(.+?)$", RegexOptions.Multiline);
        StreamReader sr = new StreamReader(statusPath);
        string currentLine;
        string result = null;

        while ((currentLine = sr.ReadLine()) != null)
        {
            Match match = re.Match(currentLine);
            if (match.Success)
            {
                result = match.Groups[1].Value;
            }
        }

        sr.Close();

        return result;
    }
}
