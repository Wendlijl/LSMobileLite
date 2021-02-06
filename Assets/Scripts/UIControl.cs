using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CI.QuickSave;

public class UIControl : MonoBehaviour
{
    //This script is intended to control all of the UI elements presented to the player. 

    public int levelIndex; //variable used to set the scene to be loaded when landing on a planet
    public GameObject hologramMenu; //variable to hold the hologram upgrade menu
    public GameObject pausePanel; //variable to hold the game pause screen
    public GameObject upgradePanel; //variable to hold the upgrade panel
    public GameObject newGamePanel; //variable to hold the upgrade panel

    private Button landOnPlanet; //contextual button used for landing on planets
    private bool isPaused; //boolean used to track if the game is paused 
    private int sceneIndex; //variable used to hold the current scene index so that level can be restarted at any time
    private ManageMap mapManager;


    private void Awake()
    {
        landOnPlanet = GameObject.Find("LandingButton").GetComponent<Button>(); //get a reference to the planet landing button
        mapManager = GameObject.Find("GameController").GetComponent<ManageMap>();
        landOnPlanet.gameObject.SetActive(false); //disable the planet landing button so it cannot be clicked until desired
        isPaused = false; //set the game pause state to false
        sceneIndex = SceneManager.GetActiveScene().buildIndex; //get a reference to the current scene index

        if (QuickSaveRoot.Exists(mapManager.saveName))
        {
            //Do nothing
        }
        else
        {
            newGamePanel.SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused) //listen for the escape key to be pressed and then activate the pause screen if it is not active
        {
            Pause();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused) //listen for the escape key to be pressed and then deactivate the pause screen if it is active
        {
            UnPause();
        }
        if (Input.GetKeyDown(KeyCode.Q) && isPaused) //if the game is paused and the Q key is pressed, then quit the game
        {
            Quit();
        }
        if (Input.GetKeyDown(KeyCode.R) && isPaused) //if the game is paused and the R key is pressed then restart the level
        {
            Restart();
        }
    }

    public void ActivateLandOnPlanet() 
    {
        //this function will enable the landing button game object when called
        landOnPlanet.gameObject.SetActive(true);
    }
    
    public void DeactivateLandOnPlanet()
    {
        //this function will disable the landing button game object when called
        landOnPlanet.gameObject.SetActive(false); 
    }

    public void LoadLevelByIndex()
    {
        //This function will load a given scene based on the scene index
        Debug.Log(levelIndex);
        //SceneManager.LoadScene(levelIndex);
        SceneManager.LoadSceneAsync(levelIndex, LoadSceneMode.Single);
    }

    public void SetPausePanelActive()
    {
        //this function will enable the pause panel when called 
        pausePanel.SetActive(true);
    }
    public void SetPausePanelInActive()
    {
        //this function will disable the pause panel when called
        pausePanel.SetActive(false);
    }

    public void SetUpgradePanelActive()
    {
        //this function will enable the upgrade panel when called 
        //upgradePanel.SetActive(true);
        hologramMenu.GetComponent<Animator>().Play("UpgradePanelOpen");
    }
    public void SetUpgradePanelInActive()
    {
        //this function will disable the upgrade panel when called 
        //upgradePanel.SetActive(false);
        hologramMenu.GetComponent<Animator>().Play("UpgradePanelClose");

    }

    public void Pause()
    {
        //this function controls the pause logic for the game, setting the pause boolean, calling the pause panel activation function, and freezing all action in the game
        isPaused = true;
        pausePanel.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }
    public void UnPause()
    {
        //this function controls the unpause logic for the game, setting the pause boolean, calling the pause panel deactivation function, and unfreezing all action in the game
        isPaused = false;
        pausePanel.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }
    public void Restart()
    {
        //this function controls the reset level logic for the game, setting the pause boolean, pause panel activation, and game time appropriately, and then reloading the level using the scene manager
        isPaused = false;
        pausePanel.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(sceneIndex);
    }
    public void Quit()
    {
        //this function controls the quit 
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

}
