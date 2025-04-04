using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks.Data;
using System.Drawing;
using UnityEditor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
public class PlayerSettingsScript : MonoBehaviour
{
    public ColorBlock resetColor;
    public ColorBlock saveButtonColor;
    private int vSyncint;
    private int resolutiondropDownInt;
    private int graphicsdropDownInt;
    private int framerateInt;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown graphicsDropdown;
    public TMP_Dropdown vsyncDropdown;
    public TMP_Dropdown frameRateDropdown;
    public Slider SensitivitySlider;
    public Slider audioSlider;
    public Button resetButton;
    public Button saveButton;
    public TMP_Text frameRateText;
    public TMP_Text sensitivityvalueText;
    public TMP_Text audioValueText;
    HashSet<string> selectedResolutionHashset = new HashSet<string>();
    Resolution[] Allresolutions;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //loads playerprefs data
        
        vsyncDropdown.value = PlayerPrefs.GetInt("VSync", 0);
        VSyncDropdown();
        graphicsDropdown.value = PlayerPrefs.GetInt("Graphics", 0);
        dropdownvalueGraphics();
        SensitivitySlider.value = PlayerPrefs.GetFloat("Sens", 50f);
        sensitivityBarValueChange();
        frameRateDropdown.value = PlayerPrefs.GetInt("Frames", 11);
        framerateDropdown();
        audioSlider.value = PlayerPrefs.GetFloat("Audio", 100f);
        audioSliderValue();
        //sets save button color
        resetColor = saveButton.colors;
        resetColor.normalColor = new UnityEngine.Color(1, 0, 1, 0);
        saveButton.colors = resetColor;
        int x = Screen.currentResolution.width;
        int y = Screen.currentResolution.height;
        Screen.SetResolution(x, y, true);        



        Allresolutions = Screen.resolutions;
        string newRes;
        
        foreach (Resolution res in Allresolutions)  
        {

            newRes = res.width.ToString() + " x " + res.height.ToString();
             
                if ( res.width >= 1280 && res.height >= 720)
                {
                    selectedResolutionHashset.Add(newRes);
                }
        }
        
        List<string> selectedResolutionList = new List<string>(selectedResolutionHashset);
        resolutionDropdown.AddOptions(selectedResolutionList);

        resolutionDropdown.value = PlayerPrefs.GetInt("Resolution", selectedResolutionHashset.Count - 1);
        dropdownvalueResolution();

    }





    //saves playerprefs data with a button
    public void saveData()
    {
        PlayerPrefs.SetFloat("Sens", SensitivitySlider.value);
        PlayerPrefs.SetFloat("Frames", frameRateDropdown.value);
        PlayerPrefs.SetInt("Resolution", resolutionDropdown.value);
        PlayerPrefs.SetInt("Graphics", graphicsDropdown.value);
        PlayerPrefs.SetFloat("Audio", audioSlider.value);
        PlayerPrefs.SetInt("VSync",vsyncDropdown.value);
        PlayerPrefs.Save();
    }


    //resets all PlayerPrefs data

    public void deleteData()
    {
        PlayerPrefs.DeleteAll();
        frameRateDropdown.value = 11;
        SensitivitySlider.value = 50f;
        audioSlider.value = 100f;
        resolutionDropdown.value = selectedResolutionHashset.Count - 1;
        graphicsDropdown.value = 0;
        vsyncDropdown.value = 0;
    }

  
    
    
    
    public void saveButtonColorChange()
    {
        resetColor = saveButton.colors;
        resetColor.normalColor = new UnityEngine.Color(1, 0, 1, 0);
        saveButton.colors = resetColor;
    }




    //all the funtions called for each menu option

    //dropdown for resolution 
    public void dropdownvalueResolution()
    {
        resolutiondropDownInt = resolutionDropdown.value;
        switch (resolutiondropDownInt)
        {

            case 0: break;
            case 1: break;
            case 2: break;
            case 3: break;


        }
        saveButtonColor = saveButton.colors;
        saveButtonColor.normalColor = new UnityEngine.Color(1, 0, 1, 1);
        saveButton.colors = saveButtonColor;
    }

    public void dropdownvalueGraphics()
    {
        graphicsdropDownInt = graphicsDropdown.value;
        switch (graphicsdropDownInt)
        {

            case 0: break;
            case 1: break;
            case 2: break;
        }
        saveButtonColor = saveButton.colors;
        saveButtonColor.normalColor = new UnityEngine.Color(1, 0, 1, 1);
        saveButton.colors = saveButtonColor;
    }

    public void VSyncDropdown()
    {
        vSyncint = vsyncDropdown.value;
        switch (vSyncint)
        {
            case 0:
                frameRateText.gameObject.SetActive(true);
                frameRateDropdown.gameObject.SetActive(true); break;

            case 1:
                frameRateText.gameObject.SetActive(false);
                frameRateDropdown.gameObject.SetActive(false); break;
        }
        saveButtonColor = saveButton.colors;
        saveButtonColor.normalColor = new UnityEngine.Color(1, 0, 1, 1);
        saveButton.colors = saveButtonColor;
    }



    //Sensitivity bar funtion
    public void sensitivityBarValueChange()
    {
        sensitivityvalueText.text = SensitivitySlider.value.ToString("0");
        saveButtonColor = saveButton.colors;
        saveButtonColor.normalColor = new UnityEngine.Color(1, 0, 1, 1);
        saveButton.colors = saveButtonColor;
    }

    //Framelimit bar funtion
    public void framerateDropdown()
    {
        framerateInt = frameRateDropdown.value;
        switch (framerateInt)
        {

            case 0: break;
            case 1: break;
            case 2: break;
            case 3: break;
            case 4: break;
            case 5: break;
            case 6: break;
            case 7: break;
            case 8: break;
            case 9: break;
            case 10: break;
            case 11: break;
        }
        saveButtonColor = saveButton.colors;
        saveButtonColor.normalColor = new UnityEngine.Color(1, 0, 1, 1);
        saveButton.colors = saveButtonColor;
    }


    public void audioSliderValue()
    {
        audioValueText.text = audioSlider.value.ToString("0");
        saveButtonColor = saveButton.colors;
        saveButtonColor.normalColor = new UnityEngine.Color(1, 0, 1, 1);
        saveButton.colors = saveButtonColor;
    }






}
