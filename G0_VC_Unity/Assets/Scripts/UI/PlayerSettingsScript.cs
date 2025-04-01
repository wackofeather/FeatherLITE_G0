using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks.Data;
using System.Drawing;
using UnityEditor;
public class PlayerSettingsScript : MonoBehaviour
{
    public ColorBlock resetColor;
    public ColorBlock saveButtonColor;
    private int vSyncint;
    private int dropDownInt;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown graphicsDropdown;
    public Slider SensitivitySlider;
    public Slider frameLimitSlider;
    public Slider audioSlider;
    public Button resetButton;
    public Button saveButton;
    public Toggle vSyncToggle;
   
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //loads playerprefs data
        vSyncint = PlayerPrefs.GetInt("VSync");
        resolutionDropdown.value = PlayerPrefs.GetInt("Resolution", 0);
        graphicsDropdown.value = PlayerPrefs.GetInt("Graphics", 0);
        SensitivitySlider.value = PlayerPrefs.GetFloat("Sens", 0f);
        frameLimitSlider.value = PlayerPrefs.GetFloat("Frames", 0f);
        audioSlider.value = PlayerPrefs.GetFloat("Audio", 0f);
        //sets the toggle button
        if (vSyncint == 1)
        {
            vSyncToggle.isOn = false;
        }
        else
        {
            vSyncToggle.isOn = true;
        }
        resetColor = saveButton.colors;
        resetColor.normalColor = new UnityEngine.Color(1, 0, 1, 0);
        saveButton.colors = resetColor;
    }

    public void VsyncToggle()
    {
        if (vSyncToggle.isOn == true)
        {
            vSyncint = 0;
        }
        if (vSyncToggle.isOn == false)
        {
            vSyncint = 1;
        }
    }


    //saves playerprefs data with a button
    public void saveData()
    {
        PlayerPrefs.SetFloat("Sens", SensitivitySlider.value);
        PlayerPrefs.SetFloat("Frames", frameLimitSlider.value);
        PlayerPrefs.SetInt("Resolution", resolutionDropdown.value);
        PlayerPrefs.SetInt("Graphics", graphicsDropdown.value);
        PlayerPrefs.SetFloat("Audio", audioSlider.value);
        PlayerPrefs.SetInt("VSync",vSyncint);
        PlayerPrefs.Save();

    }


    //resets all PlayerPrefs data

    public void deleteData()
    {
        PlayerPrefs.DeleteAll();
        frameLimitSlider.value = 0f;
        SensitivitySlider.value = 0f;
        audioSlider.value = 0f;
        resolutionDropdown.value = 0;
        graphicsDropdown.value = 0;
        vSyncToggle.isOn = true;
    }

  
    
    
    
    public void saveButtonColorChange()
    {
        resetColor = saveButton.colors;
        resetColor.normalColor = new UnityEngine.Color(1, 0, 1, 0);
        saveButton.colors = resetColor;
    }




    //all the funtions called for each menu option

    //dropdown for resolution 
    public void dropdownvalueResolution(int index)
    {
        switch (index)
        {

            case 0: dropDownInt = 0; break;
            case 1: dropDownInt = 1; break;
            case 2: dropDownInt = 2; break;
            case 3: dropDownInt = 3; break;
        }
        saveButtonColor = saveButton.colors;
        saveButtonColor.normalColor = new UnityEngine.Color(1, 0, 1, 1);
        saveButton.colors = saveButtonColor;
    }

    public void dropdownvalueGraphics(int index)
    {
        switch (index)
        {

            case 0: dropDownInt = 0; break;
            case 1: dropDownInt = 1; break;
            case 2: dropDownInt = 2; break;
        }
        saveButtonColor = saveButton.colors;
        saveButtonColor.normalColor = new UnityEngine.Color(1, 0, 1, 1);
        saveButton.colors = saveButtonColor;
    }





    //Sensitivity bar funtion
    public void sensitivityBarValueChange()
    {
        saveButtonColor = saveButton.colors;
        saveButtonColor.normalColor = new UnityEngine.Color(1, 0, 1, 1);
        saveButton.colors = saveButtonColor;
    }

    //Framelimit bar funtion
    public void frameRateLimitSlider()
    {
        saveButtonColor = saveButton.colors;
        saveButtonColor.normalColor = new UnityEngine.Color(1, 0, 1, 1);
        saveButton.colors = saveButtonColor;
    }

    //Vsync button function
    public void VsyncValueChange()
    {
        saveButtonColor = saveButton.colors;
        saveButtonColor.normalColor = new UnityEngine.Color(1, 0, 1, 1);
        saveButton.colors = saveButtonColor;
    }

    public void audioSliderValue()
    {
        saveButtonColor = saveButton.colors;
        saveButtonColor.normalColor = new UnityEngine.Color(1, 0, 1, 1);
        saveButton.colors = saveButtonColor;
    }






}
