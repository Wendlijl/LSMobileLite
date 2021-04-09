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

    private Button landOnPlanet; //contextual button used for landing on planets
    private GameObject endPlayerTurn; //contextual button used for landing on planets
    private Button endEnemyTurn; //contextual button used for landing on planets
    private bool isPaused; //boolean used to track if the game is paused 
    private bool upgradeHologramActive;
    private int sceneIndex; //variable used to hold the current scene index so that level can be restarted at any time
    private ManageMap mapManager;
    private AbilityController abilityController;
    private MovementController movementController;
    private TurnManager turnManager;

    private GameObject laserHolder;
    private GameObject emptyLaserHolder;
    private GameObject jumpHolder;
    private GameObject emptyJumpHolder;

    private List<GameObject> laserList = new List<GameObject>();
    private List<GameObject> emptyLaserList = new List<GameObject>();
    private List<GameObject> jumpList = new List<GameObject>();
    private List<GameObject> emptyJumpList = new List<GameObject>();


    private List<GameObject> healthList = new List<GameObject>();
    private List<GameObject> emptyHealthList = new List<GameObject>();
    private List<GameObject> shieldList = new List<GameObject>();
    private List<GameObject> emptyShieldList = new List<GameObject>();

    private GameObject healthPanel;
    private GameObject emptyHealthPanel;
    private GameObject shieldPanel;
    private GameObject emptyShieldPanel;

    private Slider rocketSlider;
    private GameObject rocketReloadingImage;
    private Slider shieldSlider;
    private GameObject shieldRechargingingImage;

    private void Awake()
    {
        landOnPlanet = GameObject.Find("LandingButton").GetComponent<Button>(); //get a reference to the planet landing button
        endPlayerTurn = GameObject.Find("PlayerButtonBackground"); //get a reference to the planet landing button
        endEnemyTurn = GameObject.Find("EndEnemyTurnButton").GetComponent<Button>(); //get a reference to the planet landing button
        mapManager = GameObject.Find("GameController").GetComponent<ManageMap>();
        turnManager = GameObject.Find("GameController").GetComponent<TurnManager>();
        abilityController = GameObject.Find("Player").GetComponent<AbilityController>();
        movementController = GameObject.Find("Player").GetComponent<MovementController>();
        
        healthPanel = GameObject.Find("HealthPanel");
        emptyHealthPanel = GameObject.Find("EmptyHealthPanel");
        shieldPanel = GameObject.Find("ShieldPanel");
        emptyShieldPanel = GameObject.Find("EmptyShieldPanel");

        emptyLaserHolder = GameObject.Find("EmptyLaserCharge");
        laserHolder = GameObject.Find("FullLaserCharge");
        emptyJumpHolder = GameObject.Find("EmptyJumpCharge");
        jumpHolder = GameObject.Find("FullJumpCharge");
        
        landOnPlanet.gameObject.SetActive(false); //disable the planet landing button so it cannot be clicked until desired
        endPlayerTurn.SetActive(false); //disable the planet landing button so it cannot be clicked until desired
        endEnemyTurn.gameObject.SetActive(false); //disable the planet landing button so it cannot be clicked until desired
        isPaused = false; //set the game pause state to false
        sceneIndex = SceneManager.GetActiveScene().buildIndex; //get a reference to the current scene index

        rocketSlider = GameObject.Find("RocketReloadMeter").GetComponent<Slider>();
        rocketReloadingImage = GameObject.Find("RocketReloadingImage");
        shieldSlider = GameObject.Find("ShieldBoostRechargeMeter").GetComponent<Slider>();
        shieldRechargingingImage = GameObject.Find("ShieldRechargingImage");

        rocketReloadingImage.SetActive(false);
        shieldRechargingingImage.SetActive(false);

        upgradeHologramActive = false;

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

        allTransforms = emptyLaserHolder.GetComponentsInChildren<Transform>();
        foreach (Transform child in allTransforms)
        {
            emptyLaserList.Add(child.gameObject);
        }
        allTransforms = laserHolder.GetComponentsInChildren<Transform>();
        foreach (Transform child in allTransforms)
        {
            laserList.Add(child.gameObject);
        }
        allTransforms = emptyJumpHolder.GetComponentsInChildren<Transform>();
        foreach (Transform child in allTransforms)
        {
            emptyJumpList.Add(child.gameObject);
        }
        allTransforms = jumpHolder.GetComponentsInChildren<Transform>();
        foreach (Transform child in allTransforms)
        {
            jumpList.Add(child.gameObject);
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
        if (!turnManager.combatActive)
        {
            landOnPlanet.gameObject.SetActive(true);
        }
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

    public void SetUpgradePanelState()
    {
        if (!turnManager.combatActive)
        {
            if (upgradeHologramActive)
            {
                upgradeHologramActive = false;
                hologramMenu.GetComponent<Animator>().Play("UpgradePanelClose");
            }
            else
            {
                upgradeHologramActive = true;
                hologramMenu.GetComponent<Animator>().Play("UpgradePanelOpen");
            }
        }
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

    public void SetLaserCharge(int currentCharge, int maxCharge)
    {
        //Debug.Log("Current charge: " + currentCharge + ". Max charge: " + maxCharge);
        for (int i = 0; i <= 3; i++)
        {
            laserList[i].SetActive(false);
            emptyLaserList[i].SetActive(false);
        }
        for (int i = 0; i <= maxCharge; i++)
        {
            emptyLaserList[i].SetActive(true);
        }
        for (int i = 0; i <= currentCharge; i++)
        {
            laserList[i].SetActive(true);
        }
    }
    public void SetJumpCharge(int currentCharge, int maxCharge)
    {
        //Debug.Log("Current charge: " + currentCharge + ". Max charge: " + maxCharge);
        for (int i = 0; i <= 5; i++)
        {
            jumpList[i].SetActive(false);
            emptyJumpList[i].SetActive(false);
        }
        for (int i = 0; i <= maxCharge; i++)
        {
            emptyJumpList[i].SetActive(true);
        }
        for (int i = 0; i <= currentCharge; i++)
        {
            jumpList[i].SetActive(true);
        }
    }
    
    public void SetRocketReloadState(int currentRocketReloadAmount, int rocketReloadTime)
    {
        rocketSlider.maxValue = rocketReloadTime;
        rocketSlider.value = currentRocketReloadAmount;
        if (currentRocketReloadAmount < rocketReloadTime)
        {
            rocketReloadingImage.SetActive(true);
        }
        else
        {
            rocketReloadingImage.SetActive(false);
        }
    }

    public void SetShieldBoostRechargeState(int currentShieldBoostCharge, int shieldBoostRechargeTime)
    {
        shieldSlider.maxValue = shieldBoostRechargeTime;
        shieldSlider.value = currentShieldBoostCharge;

        if (currentShieldBoostCharge < shieldBoostRechargeTime)
        {
            shieldRechargingingImage.SetActive(true);
        }
        else
        {
            shieldRechargingingImage.SetActive(false);
        }
    }

    public void beginButtonStateCoroutine()
    {
        StartCoroutine("SetEndTurnButtonStateCo");
    }

    private IEnumerator SetEndTurnButtonStateCo()
    {
        yield return null;
    }
    public void SetEndTurnButtonState()
    {
        if (!turnManager.combatActive)
        {
            endEnemyTurn.gameObject.SetActive(false);
            endPlayerTurn.gameObject.SetActive(false);
        }
        else
        {
            if (turnManager.playerTurn)
            {
                endEnemyTurn.gameObject.SetActive(false);
                endPlayerTurn.gameObject.SetActive(true);

            }
            else
            {
                endEnemyTurn.gameObject.SetActive(true);
                endPlayerTurn.gameObject.SetActive(false);
            }
            SetLaserCharge(abilityController.laserRange, abilityController.maxLaserRange);
            SetJumpCharge(abilityController.jumpRange, abilityController.maxJumpRange);
            SetShieldBoostRechargeState(abilityController.currentShieldBoostCharge, abilityController.shieldBoostRechargeTime);
            SetRocketReloadState(abilityController.currentRocketReloadAmount, abilityController.rocketReloadTime);
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
