using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Required when Using UI elements.
using UnityEngine.SceneManagement;
using CI.QuickSave;

public class SelectionControls : MonoBehaviour
{

    public GameObject continuePanel;
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    public string mapSaveName;
    public string resourcesSaveName;

    private GameObject continueButton;

    public void Awake()
    {
        continueButton = GameObject.Find("ContinueButton");

        if (QuickSaveRoot.Exists(mapSaveName)|| QuickSaveRoot.Exists(resourcesSaveName))
        {
            continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
        }
    }
   
    public void NewGame()
    {
        DeleteSave();
        loadLevelStart();
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
    public void SetCreditsPanelState()
    {
        if (creditsPanel.activeInHierarchy)
        {
            creditsPanel.SetActive(false);
        }
        else
        {
            creditsPanel.SetActive(true);
        }
        
    }



    public void loadLevelStart()
    {

        SceneManager.LoadScene(1);
    }
    public void Quit()
    {
        if (Application.isEditor)
        {
            //UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }
    public void DeleteSave()
    {
        if (QuickSaveRoot.Exists(mapSaveName))
        {
            QuickSaveRoot.Delete(mapSaveName);
            print("Deleted data file " + mapSaveName);
        }
        else
        {
            print("No map to delete");
        }

        if (QuickSaveRoot.Exists(resourcesSaveName))
        {
            QuickSaveRoot.Delete(resourcesSaveName);
            print("Deleted data file " + resourcesSaveName);
        }
        else
        {
            print("No resources to delete");
        }
    }

    public void RunTutorial()
    {
        if (QuickSaveRoot.Exists("TutorialFile"))
        {
            QuickSaveRoot.Delete("TutorialFile");
        }
        
        if (QuickSaveRoot.Exists("tutorialResourceAndUpgradeDataSaveFile"))
        {
            QuickSaveRoot.Delete("tutorialResourceAndUpgradeDataSaveFile");
        }
        SceneManager.LoadScene(2);
    }

}
