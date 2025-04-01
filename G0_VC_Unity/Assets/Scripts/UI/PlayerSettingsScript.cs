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
    private int frameRateInt;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown graphicsDropdown;
    public TMP_Dropdown frameRateDropdown;
    public TMP_Dropdown vsyncDropdown;
    public TMP_Dropdown upscalingDropdown;
    public TMP_Dropdown upscalingOptionsDropdown;
    public Slider SensitivitySlider;
    public Slider audioSlider;
    public Button resetButton;
    public Button saveButton;

   
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //loads playerprefs data
        vsyncDropdown.value = PlayerPrefs.GetInt("VSync");
        resolutionDropdown.value = PlayerPrefs.GetInt("Resolution", 0);
        graphicsDropdown.value = PlayerPrefs.GetInt("Graphics", 0);
        SensitivitySlider.value = PlayerPrefs.GetFloat("Sens", 0f);
        frameRateDropdown.value = PlayerPrefs.GetInt("Frames", 0);
        audioSlider.value = PlayerPrefs.GetFloat("Audio", 0f);

        resetColor = saveButton.colors;
        resetColor.normalColor = new UnityEngine.Color(1, 0, 1, 0);
        saveButton.colors = resetColor;
    }



    //saves playerprefs data with a button
    public void saveData()
    {
        PlayerPrefs.SetFloat("Sens", SensitivitySlider.value);
        PlayerPrefs.SetInt("Frames", frameRateDropdown.value);
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
        frameRateDropdown.value = 0;
        SensitivitySlider.value = 0f;
        audioSlider.value = 0f;
        resolutionDropdown.value = 0;
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
    public void dropdownvalueResolution(int index)
    {
        if (Screen.height <= 0)
        {
            
        }
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


    public void frameRateLimit(int index)
    {
        frameRateInt = frameRateDropdown.value;
        switch (frameRateInt)
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
            case 12: break;
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
                frameRateDropdown.gameObject.SetActive(true); break;

            case 1:
                frameRateDropdown.gameObject.SetActive(false); break;
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


    public void audioSliderValue()
    {
        saveButtonColor = saveButton.colors;
        saveButtonColor.normalColor = new UnityEngine.Color(1, 0, 1, 1);
        saveButton.colors = saveButtonColor;
    }






}
