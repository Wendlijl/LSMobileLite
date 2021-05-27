using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // Required when Using UI elements.
using UnityEngine.SceneManagement;
using CI.QuickSave;
using TMPro;

public class SelectionControls : MonoBehaviour
{

    public GameObject continuePanel;
    public GameObject optionsPanel;
    public GameObject creditsPanel;
    public GameObject highScorePanel;
    public Animator animator;

    public string mapSaveName;
    public string resourcesSaveName;
    public float transitionTime = 1f;

    private GameObject continueButton;
    private AudioSource musicSource;
    private float musicVolume;
    private bool quietMusic;

    private List<HighScoreObject> highScoreObjectList;
    //private Transform[] allChildren;
    private List<TMP_Text> nameList;
    private List<TMP_Text> scoresList;
    private TMP_Text[] allChildren;

    public void Awake()
    {
        continueButton = GameObject.Find("ContinueButton");
        musicSource = GameObject.Find("Music").GetComponent<AudioSource>();
        musicVolume = musicSource.volume;

        if (QuickSaveRoot.Exists(mapSaveName)|| QuickSaveRoot.Exists(resourcesSaveName))
        {
            continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
        }
        if (QuickSaveRoot.Exists("HighScoreFile")) //if a save file exists, load data from that file
        {
            QuickSaveReader instReader = QuickSaveReader.Create("HighScoreFile"); //create an instance of the quick save reader to pull in the save file
            highScoreObjectList = instReader.Read<List<HighScoreObject>>("HighScoreObjectList");
            highScorePanel.SetActive(true);
            allChildren = GameObject.Find("HighScoreVerticalLayoutGroup2").GetComponentsInChildren<TMP_Text>();
            int i= 0;
            foreach (TMP_Text child in allChildren)
            {
                if (i > 0)
                {
                    child.text = highScoreObjectList[i-1].scoreString;
                }
                i++;
            }
            i = 0;
            allChildren = GameObject.Find("HighScoreVerticalLayoutGroup3").GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text child in allChildren)
            {
                if (i > 0)
                {
                    child.text = highScoreObjectList[i-1].scoreValue.ToString();
                }
                i++;
            }
            highScorePanel.SetActive(false);
        }



    }

    private void Update()
    {
        if (quietMusic)
        {
            if (musicSource.volume > 0)
            {
                musicSource.volume -= musicVolume * Time.deltaTime / transitionTime;
            }
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

    public void SetHighScorePanelState()
    {
        if (highScorePanel.activeInHierarchy)
        {
            highScorePanel.SetActive(false);
        }
        else
        {
            highScorePanel.SetActive(true);
        }

    }



    public void loadLevelStart()
    {
        StartCoroutine(MakeSceneTransition(1));
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
        StartCoroutine(MakeSceneTransition(2));
    }

    public IEnumerator MakeSceneTransition(int sceneIndex)
    {
        animator.SetTrigger("Start");
        quietMusic = true;

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneIndex);
    }
}
