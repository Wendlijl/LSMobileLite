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
    public GameObject newGameMessage; //variable to hold the upgrade panel
    public GameObject laserChargeToken1;
    public GameObject laserChargeToken2;
    public GameObject laserChargeToken3;
    public GameObject jumpChargeToken1;
    public GameObject jumpChargeToken2;
    public GameObject jumpChargeToken3;
    public GameObject jumpChargeToken4;
    public GameObject jumpChargeToken5;

    private Button landOnPlanet; //contextual button used for landing on planets
    private Button endPlayerTurn; //contextual button used for landing on planets
    private Button endEnemyTurn; //contextual button used for landing on planets
    private bool isPaused; //boolean used to track if the game is paused 
    private int sceneIndex; //variable used to hold the current scene index so that level can be restarted at any time
    private ManageMap mapManager;
    private AbilityController abilityController;
    private MovementController movementController;

    private List<GameObject> healthList = new List<GameObject>();
    private List<GameObject> emptyHealthList = new List<GameObject>();
    private List<GameObject> shieldList = new List<GameObject>();
    private List<GameObject> emptyShieldList = new List<GameObject>();

    private GameObject healthPanel;
    private GameObject emptyHealthPanel;
    private GameObject shieldPanel;
    private GameObject emptyShieldPanel;

    private void Awake()
    {
        landOnPlanet = GameObject.Find("LandingButton").GetComponent<Button>(); //get a reference to the planet landing button
        endPlayerTurn = GameObject.Find("EndPlayerTurnButton").GetComponent<Button>(); //get a reference to the planet landing button
        endEnemyTurn = GameObject.Find("EndEnemyTurnButton").GetComponent<Button>(); //get a reference to the planet landing button
        mapManager = GameObject.Find("GameController").GetComponent<ManageMap>();
        abilityController = GameObject.Find("Player").GetComponent<AbilityController>();
        movementController = GameObject.Find("Player").GetComponent<MovementController>();
        
        healthPanel = GameObject.Find("HealthPanel");
        emptyHealthPanel = GameObject.Find("EmptyHealthPanel");
        shieldPanel = GameObject.Find("ShieldPanel");
        emptyShieldPanel = GameObject.Find("EmptyShieldPanel");
        landOnPlanet.gameObject.SetActive(false); //disable the planet landing button so it cannot be clicked until desired
        endPlayerTurn.gameObject.SetActive(false); //disable the planet landing button so it cannot be clicked until desired
        endEnemyTurn.gameObject.SetActive(false); //disable the planet landing button so it cannot be clicked until desired
        isPaused = false; //set the game pause state to false
        sceneIndex = SceneManager.GetActiveScene().buildIndex; //get a reference to the current scene index

        Transform[] allTransforms = healthPanel.GetComponentsInChildren<Transform>();
        foreach (Transform child in allTransforms)
        {
            healthList.Add(child.gameObject);
        }

        allTransforms = shieldPanel.GetComponentsInChildren<Transform>();
        foreach (Transform child in allTransforms)
        {
            shieldList.Add(child.gameObject);
        }

        allTransforms = emptyHealthPanel.GetComponentsInChildren<Transform>();
        foreach (Transform child in allTransforms)
        {
            emptyHealthList.Add(child.gameObject);
        }

        allTransforms = emptyShieldPanel.GetComponentsInChildren<Transform>();
        foreach (Transform child in allTransforms)
        {
            emptyShieldList.Add(child.gameObject);
        }

        if (QuickSaveRoot.Exists(mapManager.saveName))
        {
            //Do nothing
        }
        else
        {
            newGameMessage.SetActive(true);
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

    public void SetHealthState(int maxHealth, int currentHealth, int maxShields, int currentShields)
    {
        for (int i = 0; i <= 6; i++)
        {
            healthList[i].SetActive(false);
            emptyHealthList[i].SetActive(false);
            shieldList[i].SetActive(false);
            emptyShieldList[i].SetActive(false);
        }


        for (int i = 0; i <= maxHealth; i++)
        {
            emptyHealthList[i].SetActive(true);
        }

        for (int i = 0; i <= maxShields; i++)
        {
            emptyShieldList[i].SetActive(true);
        }

        for (int i = 0; i <= currentHealth; i++)
        {
            healthList[i].SetActive(true);
        }

        for (int i = 0; i <= currentShields; i++)
        {
            shieldList[i].SetActive(true);
        }
    }

    public void SetEndTurnButtonState()
    {
        if (mapManager.spawnedEnemies.Count <= 0)
        {
            endEnemyTurn.gameObject.SetActive(false);
            endPlayerTurn.gameObject.SetActive(false);
            mapManager.combatActive = false;
            mapManager.enemyTurn = true;
            mapManager.playerTurn = false;
            abilityController.laserRange = abilityController.maxLaserRange;
        }
        else
        {
            if (mapManager.playerTurn)
            {
                endEnemyTurn.gameObject.SetActive(true);
                endPlayerTurn.gameObject.SetActive(false);
                movementController.hasMoved = false;
                abilityController.abilityUsed = false;
                mapManager.enemyTurn = true;
                mapManager.playerTurn = false;
                mapManager.OrderEnemyTurns();

            }
            else
            {
                if (abilityController.laserRange < abilityController.maxLaserRange)
                {
                    abilityController.laserRange++;
                }
                if (abilityController.jumpRange < abilityController.maxJumpRange)
                {
                    abilityController.jumpRange++;
                }
                endEnemyTurn.gameObject.SetActive(false);
                endPlayerTurn.gameObject.SetActive(true);
                mapManager.enemyTurn = false;
                mapManager.playerTurn = true;
                SetLaserCharge();
                SetJumpCharge();
                //Debug.Log(abilityController.laserRange);
            }
        }
    }

    public void SetLaserCharge()
    {
        switch (abilityController.laserRange)
        {
            case 0:
                laserChargeToken1.SetActive(false);
                laserChargeToken2.SetActive(false);
                laserChargeToken3.SetActive(false);
                break;
            case 1:
                laserChargeToken1.SetActive(true);
                laserChargeToken2.SetActive(false);
                laserChargeToken3.SetActive(false);
                break;
            case 2:
                laserChargeToken1.SetActive(true);
                laserChargeToken2.SetActive(true);
                laserChargeToken3.SetActive(false);
                break;
            case 3:
                laserChargeToken1.SetActive(true);
                laserChargeToken2.SetActive(true);
                laserChargeToken3.SetActive(true);
                break;
        }
    }
    public void SetJumpCharge()
    {
        switch (abilityController.jumpRange)
        {
            case 0:
                jumpChargeToken1.SetActive(false);
                jumpChargeToken2.SetActive(false);
                jumpChargeToken3.SetActive(false);
                jumpChargeToken4.SetActive(false);
                jumpChargeToken5.SetActive(false);
                break;
            case 1:
                jumpChargeToken1.SetActive(true);
                jumpChargeToken2.SetActive(false);
                jumpChargeToken3.SetActive(false);
                jumpChargeToken4.SetActive(false);
                jumpChargeToken5.SetActive(false);
                break;
            case 2:
                jumpChargeToken1.SetActive(true);
                jumpChargeToken2.SetActive(true);
                jumpChargeToken3.SetActive(false);
                jumpChargeToken4.SetActive(false);
                jumpChargeToken5.SetActive(false);
                break;
            case 3:
                jumpChargeToken1.SetActive(true);
                jumpChargeToken2.SetActive(true);
                jumpChargeToken3.SetActive(true);
                jumpChargeToken4.SetActive(false);
                jumpChargeToken5.SetActive(false);
                break;
            case 4:
                jumpChargeToken1.SetActive(true);
                jumpChargeToken2.SetActive(true);
                jumpChargeToken3.SetActive(true);
                jumpChargeToken4.SetActive(true);
                jumpChargeToken5.SetActive(false);
                break;
            case 5:
                jumpChargeToken1.SetActive(true);
                jumpChargeToken2.SetActive(true);
                jumpChargeToken3.SetActive(true);
                jumpChargeToken4.SetActive(true);
                jumpChargeToken5.SetActive(true);
                break;
        }
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
