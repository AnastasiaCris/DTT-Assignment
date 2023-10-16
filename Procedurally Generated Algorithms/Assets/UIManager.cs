using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Scrollbar camPanning;
    private float maxZoom = 5.5f;
    private float minZoom;
    
    /// <summary>
    /// Setting up camera position and size to always be in the middle of the maze and always be able to see the whole maze
    /// </summary>
    public void SetUpCamera()
    {
        Camera cam = Camera.main;
        
        int biggestNr = DFSMaze.Height > DFSMaze.Width ? DFSMaze.Height : DFSMaze.Width; //get the biggest nr between height and width
        
        minZoom = DFSMaze.Height >= DFSMaze.Width? 5 + biggestNr / 2 : biggestNr / 2;  
        
        if (minZoom <= maxZoom) minZoom = maxZoom; // size can't be smaller then 5.5
        
        cam.orthographicSize = minZoom; //set the size

        if(camPanning.value > 0)
            camPanning.value = 0; //reset the scrollbar in case it's been used
    }

    /// <summary>
    /// Using the scrollbar it zooms the camera in and out
    /// </summary>
    public void ZoomCamera()
    {
        float currentZoom = Camera.main.orthographicSize;

        currentZoom = minZoom + (maxZoom - minZoom) * camPanning.value;

        // For Orthographic Camera
        Camera.main.orthographicSize = currentZoom;
    }

    public void OnInstGenValueChanged(Toggle toggle)
    {
        DFSMaze.instantGen = toggle.isOn;
    }
    
    /// <summary>
    /// Change Width in the input field
    /// </summary>
    public void OnWidthValueChanged(TMP_InputField textField)
    {
        int parseW = 0;
        if (int.TryParse(textField.text, out parseW))
        {
            parseW = Int32.Parse(textField.text);
        }
        if (parseW <= 2 && textField.text.Length > 0) //width can't be smaller then 2
        {
            parseW = 2;
            textField.text = parseW.ToString();
        }
        if (parseW >= 250) //width can't be bigger then 250
        {
            parseW = 250;
            textField.text = parseW.ToString();
        }

        DFSMaze.Width = parseW;
    }
    
    /// <summary>
    /// Change Height in the input field
    /// </summary>
    public void OnHeightValueChanged(TMP_InputField textField)
    {
        int parseH = 0;
        if (int.TryParse(textField.text, out parseH))
        {
            parseH = Int32.Parse(textField.text);
        }
        if (parseH <= 2 && textField.text.Length > 0) //height can't be smaller then 2
        {
            parseH = 2;
            textField.text = parseH.ToString();
        }
        if (parseH >= 250) //height can't be bigger then 250
        {
            parseH = 250;
            textField.text = parseH.ToString();
        }

        DFSMaze.Height = parseH;
    }
}
