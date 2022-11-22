using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveBlockController : MonoBehaviour
{
    [SerializeField]
    private string value;

    // Start is called before the first frame update
    void Start()
    {
        //SetupSaveBlock("default");
    }

    public void ToggleSaveBlock()
    {
        GetComponentInParent<SavesContentController>().SetSelectedSave(value);
    }

    public void SetupSaveBlock(string value, string activeSpectacle)
    {
        this.value = value;

        // Try loading texture
        try
        {
            byte[] FileBytes = File.ReadAllBytes("Saves/" + value + "/" + activeSpectacle + ".png");
            Texture2D tex = new Texture2D(2, 2);
            if (ImageConversion.LoadImage(tex, FileBytes))
            {
                gameObject.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }

        gameObject.GetComponentInChildren<TMP_Text>().text = value;
    }
}
