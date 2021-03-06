﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.
using UnityEngine.SceneManagement;
using CI.QuickSave;

public class SelectionControls : MonoBehaviour
{
    public Slider mainSlider;
    public GameObject continuePanel;
    public GameObject optionsPanel;
    bool hasMoved; //Create a check for whether movement has happened yet
    bool hasSelected; //Create check for whether player has made a selection
    float upDownMovement; //Create a variable to define vertical movement of selector
    public string saveName;

    public void Awake()
    {
        //Get the slider object
        mainSlider = GameObject.Find("SelectionSlider").GetComponent<Slider>();
        //continuePanel = GameObject.Find("ContinuePanel");
        //optionsPanel = GameObject.Find("OptionsPanel");
        Debug.Log(continuePanel);
        Debug.Log(optionsPanel);
    }

    public void SubmitSliderSetting()
    {
        //Move the slider based on player input. Bound the slider to min and max values
        if (upDownMovement < 0 && mainSlider.value<4)
        {
            mainSlider.value++;
        }
        else if (upDownMovement > 0 && mainSlider.value > 0)
        {
            mainSlider.value--;
        }

    }
    public void Update()
    {
        upDownMovement = Input.GetAxis("Vertical");
        //Determine if the selector has moved yet for a given keypress.
        if (upDownMovement == 0)
        {
            hasMoved = false;
        }
        else if (upDownMovement != 0 && !hasMoved) //If up or down input is detected and the selector has not moved, then call the movement function and set hasMoved to true
        {
            hasMoved = true;
            SubmitSliderSetting();
        }
        if (Input.GetKeyDown("return") || Input.GetKeyDown("space"))
        {
            switch (mainSlider.value)
            {
                case 0:
                    print("Continue Selected");
                    if (QuickSaveRoot.Exists(saveName))
                    {
                        loadLevelStart();
                    }
                    else
                    {
                        SetContinuePanelActive();
                    }
                    //SetContinuePanelActive();
                    break;
                case 1:
                    print("New Selected");
                    DeleteSave();
                    loadLevelStart();
                    break;
                case 2:
                    print("Tutorial Selected");
                    break;
                case 3:
                    print("Options Selected");
                    SetOptionsPanelActive();
                    break;
                case 4:
                    print("Quit Selected");
                    Quit();
                    break;
            }
        }
    }

    public void SetContinuePanelActive()
    {
        continuePanel.SetActive(true);
    }
    public void SetContinuePanelInActive()
    {
        continuePanel.SetActive(false);
    }

    public void SetOptionsPanelActive()
    {
        optionsPanel.SetActive(true);
    }
    public void SetOptionsPanelInActive()
    {
        optionsPanel.SetActive(false);
    }

    public void loadLevelStart()
    {

        SceneManager.LoadScene(1);
    }
    public void Quit()
    {
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }
    public void DeleteSave()
    {
        if (QuickSaveRoot.Exists(saveName))
        {
            QuickSaveRoot.Delete(saveName);
            print("Deleted data file " + saveName);
        }
        else
        {
            print("Nothing to delete");
        }
    }

}
