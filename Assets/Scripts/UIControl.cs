using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CI.QuickSave;
using TMPro;

public class UIControl : MonoBehaviour
{
    //This script is intended to control all of the UI elements presented to the player. 

    public int levelIndex; //variable used to set the scene to be loaded when landing on a planet
    public GameObject hologramMenu; //variable to hold the hologram upgrade menu
    public GameObject pausePanel; //variable to hold the game pause screen
    //public GameObject upgradePanel; //variable to hold the upgrade panel
    public GameObject newGameMessage; //variable to hold the upgrade panel
    private GameObject resourceWarningMessage;
    public GameObject starGateMessage;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;


    private TMP_Text resourceTextDisplay;
    private Button landOnPlanet; //contextual button used for landing on planets
    private GameObject endPlayerTurn; //contextual button used for landing on planets
    private GameObject fogOfWar;
    private Button endEnemyTurn; //contextual button used for landing on planets
    private GameObject playerTurnIcon;
    private GameObject enemyTurnIcon;
    private bool isPaused; //boolean used to track if the game is paused 
    private bool upgradeHologramActive;
    private bool upgradeState;
    public bool UpgradeState { get { return upgradeState; } set { upgradeState = value; } }
    private int sceneIndex; //variable used to hold the current scene index so that level can be restarted at any time
    private ManageMap mapManager;
    private AbilityController abilityController;
    private MovementController movementController;
    private TutorialManager tutorialManager;
    private ResourceAndUpgradeManager resourceAndUpgradeManager;
    private TurnManager turnManager;
    private PlayerHealthControl playerHealthControl;
    private GameObject gameController;
    private GameObject player;

    private GameObject laserChargeHolder;
    private GameObject emptyLaserChargeHolder;
    private GameObject jumpChargeHolder;
    private GameObject emptyJumpChargeHolder;

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
    private Slider threatLevelSlider;

    private GameObject masterUpgradeLayoutGroup;
    private GameObject healthUpgradeObject;
    private GameObject shieldUpgradeObject;
    private GameObject laserRangeUpgradeObject;
    private GameObject laserRechargeUpgradeObject;
    private GameObject rocketRangeUpgradeObject;
    private GameObject rocketReloadUpgradeObject;
    private GameObject rocketYieldUpgradeObject;
    private GameObject jumpRangeUpgradeObject;
    private GameObject jumpRechargeUpgradeObject;
    private GameObject shieldBoostUpgradeObject;
    private GameObject shieldBoostRechargeUpgradeObject;
    private GameObject shieldOverboostUpgradeObject;
    private GameObject healthRepairObject;
    private GameObject sensorRangeUpgradeObject;

    private GameObject rocketButtonHolder;
    private GameObject shieldButtonHolder;
    private GameObject jumpButtonHolder;

    private GameObject starGateMessageObject;

    private GameObject abilityUsedIcon;
    private GameObject moveUsedIcon;



    private void Awake()
    {
        upgradeState = true;
        fogOfWar = GameObject.Find("TilemapFogOfWar");
        landOnPlanet = GameObject.Find("LandingButton").GetComponent<Button>(); //get a reference to the planet landing button
        endPlayerTurn = GameObject.Find("PlayerButtonBackground"); //get a reference to the planet landing button
        endEnemyTurn = GameObject.Find("EndEnemyTurnButton").GetComponent<Button>(); //get a reference to the planet landing button
        playerTurnIcon = GameObject.Find("PlayerTurnIconBackground");
        enemyTurnIcon = GameObject.Find("EnemyTurnIconBackground");
        gameController = GameObject.Find("GameController");
        mapManager = gameController.GetComponent<ManageMap>();
        turnManager = gameController.GetComponent<TurnManager>();
        resourceAndUpgradeManager = gameController.GetComponent<ResourceAndUpgradeManager>();
        tutorialManager = gameController.GetComponent<TutorialManager>();
        player = GameObject.Find("Player");
        abilityController = player.GetComponent<AbilityController>();
        playerHealthControl = player.GetComponent<PlayerHealthControl>();
        movementController = player.GetComponent<MovementController>();

        resourceWarningMessage = GameObject.Find("AllResourcesCollectedWarningBackgroundImage");
        resourceWarningMessage.SetActive(false);

        healthPanel = GameObject.Find("HealthPanel");
        emptyHealthPanel = GameObject.Find("EmptyHealthPanel");
        shieldPanel = GameObject.Find("ShieldPanel");
        emptyShieldPanel = GameObject.Find("EmptyShieldPanel");

        emptyLaserChargeHolder = GameObject.Find("EmptyLaserCharge");
        laserChargeHolder = GameObject.Find("FullLaserCharge");
        emptyJumpChargeHolder = GameObject.Find("EmptyJumpCharge");
        jumpChargeHolder = GameObject.Find("FullJumpCharge");

        resourceTextDisplay = GameObject.Find("ResourceCountText (TMP)").GetComponent<TMP_Text>();

        landOnPlanet.gameObject.SetActive(false); //disable the planet landing button so it cannot be clicked until desired
        endPlayerTurn.SetActive(false); //disable the planet landing button so it cannot be clicked until desired
        endEnemyTurn.gameObject.SetActive(false); //disable the planet landing button so it cannot be clicked until desired
        abilityUsedIcon = GameObject.Find("AbilityUsedToken");
        moveUsedIcon = GameObject.Find("HasMovedToken");

        abilityUsedIcon.SetActive(false);
        moveUsedIcon.SetActive(false);
        playerTurnIcon.SetActive(false);
        enemyTurnIcon.SetActive(false);
        isPaused = false; //set the game pause state to false
        sceneIndex = SceneManager.GetActiveScene().buildIndex; //get a reference to the current scene index

        rocketSlider = GameObject.Find("RocketReloadMeter").GetComponent<Slider>();
        rocketReloadingImage = GameObject.Find("RocketReloadingImage");
        shieldSlider = GameObject.Find("ShieldBoostRechargeMeter").GetComponent<Slider>();
        shieldRechargingingImage = GameObject.Find("ShieldRechargingImage");

        rocketReloadingImage.SetActive(false);
        shieldRechargingingImage.SetActive(false);

        upgradeHologramActive = false;

        threatLevelSlider = GameObject.Find("ThreatLevelIndicator").GetComponentInChildren<Slider>();


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

        allTransforms = emptyLaserChargeHolder.GetComponentsInChildren<Transform>();
        foreach (Transform child in allTransforms)
        {
            emptyLaserList.Add(child.gameObject);
        }
        allTransforms = laserChargeHolder.GetComponentsInChildren<Transform>();
        foreach (Transform child in allTransforms)
        {
            laserList.Add(child.gameObject);
        }
        allTransforms = emptyJumpChargeHolder.GetComponentsInChildren<Transform>();
        foreach (Transform child in allTransforms)
        {
            emptyJumpList.Add(child.gameObject);
        }
        allTransforms = jumpChargeHolder.GetComponentsInChildren<Transform>();
        foreach (Transform child in allTransforms)
        {
            jumpList.Add(child.gameObject);
        }


        if (QuickSaveRoot.Exists(resourceAndUpgradeManager.ResourceAndUpgradeDataSaveFileName)||mapManager.saveName=="TutorialFile")
        {
            //Do nothing
        }
        else
        {
            newGameMessage.SetActive(true);
        }


        masterUpgradeLayoutGroup = GameObject.Find("MasterUpgradeHorizontalLayoutGroup");
        healthUpgradeObject = GameObject.Find("HealthUpgradeObject");
        shieldUpgradeObject = GameObject.Find("ShieldUpgradeObject");
        laserRangeUpgradeObject = GameObject.Find("LaserRangeUpgradeObject");
        laserRechargeUpgradeObject = GameObject.Find("LaserRechargeUpgradeObject");
        rocketRangeUpgradeObject = GameObject.Find("RocketRangeUpgradeObject");
        rocketReloadUpgradeObject = GameObject.Find("RocketReloadUpgradeObject");
        rocketYieldUpgradeObject = GameObject.Find("RocketYieldUpgradeObject");
        jumpRangeUpgradeObject = GameObject.Find("JumpRangeUpgradeObject");
        jumpRechargeUpgradeObject = GameObject.Find("JumpRechargeUpgradeObject");
        shieldBoostUpgradeObject = GameObject.Find("ShieldBoostUpgradeObject");
        shieldBoostRechargeUpgradeObject = GameObject.Find("ShieldBoostRechargeUpgradeObject");
        shieldOverboostUpgradeObject = GameObject.Find("ShieldOverboostUpgradeObject");
        healthRepairObject = GameObject.Find("HealthRepairObject");
        sensorRangeUpgradeObject = GameObject.Find("SensorRangeUpgradeObject");

        masterUpgradeLayoutGroup.SetActive(false);

        rocketButtonHolder = GameObject.Find("RocketButtonHolder");
        shieldButtonHolder = GameObject.Find("ShieldButtonHolder");
        jumpButtonHolder = GameObject.Find("JumpButtonHolder");

        rocketButtonHolder.SetActive(resourceAndUpgradeManager.RocketsInstalled);
        shieldButtonHolder.SetActive(resourceAndUpgradeManager.ShieldBoostInstalled);
        jumpButtonHolder.SetActive(resourceAndUpgradeManager.JumpDriveInstalled);

        SetThreatLevelSlider(resourceAndUpgradeManager.ThreatLevel);
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
            mapManager.Save();
            resourceAndUpgradeManager.SaveResourceAndUpgradeData();
            Quit();
        }
        if (Input.GetKeyDown(KeyCode.R) && isPaused) //if the game is paused and the R key is pressed then restart the level
        {
            Restart();
        }
    }

    public void SetFogOfWarState()
    {
        if (fogOfWar.activeInHierarchy)
        {
            fogOfWar.SetActive(false);
        }
        else
        {
            fogOfWar.SetActive(true);
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

    public void ResourcesCollectedWarning()
    {
        if (resourceWarningMessage.activeInHierarchy)
        {
            resourceWarningMessage.SetActive(false);
        }
        else
        {
            resourceWarningMessage.SetActive(true);
            ShortWarning[] warningScripts = resourceWarningMessage.GetComponentsInChildren<ShortWarning>();
            foreach(ShortWarning warningScript in warningScripts)
            {
                warningScript.StartCoroutine("FadeOut");
            }
            resourceWarningMessage.GetComponentInChildren<ShortWarningText>().StartCoroutine("FadeOut");
        }
    }

    public void DisableResourceCollectedWarning()
    {
        resourceWarningMessage.SetActive(false);
    }
    
    public void DeactivateLandOnPlanet()
    {
        //this function will disable the landing button game object when called
        landOnPlanet.gameObject.SetActive(false); 
    }

    public void LoadLevelByIndex()
    {
        //This function will load a given scene based on the scene index
        //Debug.Log(levelIndex);
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
        if (!turnManager.combatActive && upgradeState)
        {
            if (upgradeHologramActive)
            {
                upgradeHologramActive = false;
                hologramMenu.GetComponent<Animator>().Play("UpgradePanelClose");
                masterUpgradeLayoutGroup.SetActive(false);
            }
            else
            {
                upgradeHologramActive = true;
                hologramMenu.GetComponent<Animator>().Play("UpgradePanelOpen");
                StartCoroutine("SetButtonsActive");
            }

            if (upgradeHologramActive && movementController.MovementState && mapManager.saveName != "TutorialFile") 
            {
                tutorialManager.SetMovementState();
            }else if(!upgradeHologramActive && !movementController.MovementState)
            {
                tutorialManager.SetMovementState();
            }
            
        }
        if (mapManager.saveName == "TutorialFile")
        {
            tutorialManager.ExplainUpgrades();
        }
    }

    public void DisplayStarGateMessage()
    {
        starGateMessageObject =  Instantiate(starGateMessage);
        starGateMessageObject.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>());
        starGateMessageObject.transform.localScale = new Vector3(1, 1, 1);
        starGateMessageObject.GetComponent<RectTransform>().localPosition = new Vector3(1, 1, 0);
        starGateMessageObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        starGateMessageObject.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);

        Button[] buttons = starGateMessageObject.GetComponentsInChildren<Button>();
        foreach(Button button in buttons)
        {
            Debug.Log(button.gameObject.name);
            if(button.gameObject.name == "ContinueQuestButton")
            {
                if(mapManager.saveName == "TutorialFile")
                {
                    button.onClick.AddListener(delegate { Quit(); });
                }
                else
                {
                    button.onClick.AddListener(delegate { StartNewLevel(); });
                }
                
            }
            if(button.gameObject.name == "ReturnHomeButton")
            {
                if (mapManager.saveName == "TutorialFile")
                {
                    button.onClick.AddListener(delegate { Quit(); });
                }
                else
                {
                    button.onClick.AddListener(delegate { DisplayVictoryText(); });
                }
                
            }
            if(button.gameObject.name == "StayHereButton")
            {
                button.onClick.AddListener(delegate { tutorialManager.SetMovementState(); });
            }
            button.onClick.AddListener(delegate { DestroyStarGateMessage(); });
            
        }
    }

    public void StartNewLevel()
    {
        resourceAndUpgradeManager.SolarSystemNumber++;
        resourceAndUpgradeManager.SaveResourceAndUpgradeData();
        mapManager.Delete();
        SceneManager.LoadScene(1);
    }

    public void DestroyStarGateMessage()
    {
        if (starGateMessageObject)
        {
            Destroy(starGateMessageObject);
        }
    }

    private IEnumerator SetButtonsActive()
    {
        yield return new WaitForSeconds(0.36f);
        masterUpgradeLayoutGroup.SetActive(true);
    }


    public void SetHealthState(int maxHealth, int currentHealth, int maxShields, int currentShields)
    {
        //Debug.Log("Max Health "+ maxHealth+". Current health "+currentHealth+". Max Shields "+ maxShields+". Current Shields "+ currentShields);

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
        //Debug.Log("Laser charge set as "+currentCharge+" and "+ maxCharge);
        currentCharge = (currentCharge > 6) ? 6:currentCharge;
        maxCharge = (maxCharge > 6) ? 6: maxCharge;
        //Debug.Log("Current charge: " + currentCharge + ". Max charge: " + maxCharge);
        for (int i = 0; i <= 6; i++)
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
        for (int i = 0; i <= 6; i++)
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
        //Debug.Log("Shield boost recharge set to " + currentRocketReloadAmount + " and " + rocketReloadTime);
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

    public void SetThreatLevelSlider(float value)
    {
        //Debug.Log(movementController.ThreatLevel);
        threatLevelSlider.value = value;
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

            playerTurnIcon.SetActive(false);
            enemyTurnIcon.SetActive(false);
        }
        else
        {
            if (turnManager.playerTurn)
            {
                endPlayerTurn.gameObject.SetActive(true);

                playerTurnIcon.SetActive(true);
                enemyTurnIcon.SetActive(false);

            }
            else
            {

                endPlayerTurn.gameObject.SetActive(false);
                playerTurnIcon.SetActive(false);
                enemyTurnIcon.SetActive(true);
            }
            SetLaserCharge(abilityController.laserRange, abilityController.maxLaserRange);
            SetJumpCharge(abilityController.jumpRange, abilityController.maxJumpRange);
            SetShieldBoostRechargeState(abilityController.currentShieldBoostCharge, abilityController.shieldBoostRechargeTime);
            SetRocketReloadState(abilityController.currentRocketReloadAmount, abilityController.rocketReloadTime);
        }
    }

    public void SetPlayerTurnIndicators()
    {
        abilityUsedIcon.SetActive(abilityController.abilityUsed);
        moveUsedIcon.SetActive(movementController.hasMoved);
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

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SetGameOverPanelState()
    {
        if (gameOverPanel.activeInHierarchy)
        {
            gameOverPanel.SetActive(false);
        }
        else
        {
            gameOverPanel.SetActive(true);
        }
    }

    //public void SetResourceCount(int resources, bool addToCurrent)
    public void SetResourceCount(int resources)
    {
        resourceTextDisplay.text = resources.ToString();
    }

    public void SetUpgradeButtons()
    {

        healthUpgradeObject.SetActive((resourceAndUpgradeManager.CurrentMaxHealth < 6) ? true : false);
        shieldUpgradeObject.SetActive((resourceAndUpgradeManager.CurrentMaxShields < 6) ? true : false);

        laserRangeUpgradeObject.SetActive(resourceAndUpgradeManager.CurrentMaxLaserRange < 6 ? true : false);
        laserRechargeUpgradeObject.SetActive(resourceAndUpgradeManager.CurrentMaxLaserRecharge < 3 ? true : false);

        sensorRangeUpgradeObject.SetActive(resourceAndUpgradeManager.CurrentMaxSensorRange < 10 ? true : false);

        if (resourceAndUpgradeManager.RocketsInstalled)
        {
            rocketButtonHolder.SetActive(resourceAndUpgradeManager.RocketsInstalled);

            rocketRangeUpgradeObject.SetActive(resourceAndUpgradeManager.CurrentMaxRocketRange < 6 ? true : false);
            rocketReloadUpgradeObject.SetActive(resourceAndUpgradeManager.CurrentMaxRocketReload > 2 ? true : false);
            rocketYieldUpgradeObject.SetActive(resourceAndUpgradeManager.CurrentMaxRocketYield < 3 ? true : false);
            TMP_Text[] rocketText = rocketRangeUpgradeObject.GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text thisText in rocketText)
            {
                if (thisText.text == "Install Rockets")
                {
                    thisText.text = "+1 Rocket Range";
                }
            }
        }
        else
        {
            rocketRangeUpgradeObject.SetActive(true);
            
            TMP_Text[] rocketText = rocketRangeUpgradeObject.GetComponentsInChildren<TMP_Text>();
            foreach(TMP_Text thisText in rocketText)
            {
                if(thisText.text=="+1 Rocket Range")
                {
                    thisText.text = "Install Rockets";
                }
            }
            
            rocketReloadUpgradeObject.SetActive(false);
            rocketYieldUpgradeObject.SetActive(false);
        }

        if (resourceAndUpgradeManager.JumpDriveInstalled)
        {
            jumpButtonHolder.SetActive(resourceAndUpgradeManager.JumpDriveInstalled);
            TMP_Text[] jumpText = jumpRangeUpgradeObject.GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text thisText in jumpText)
            {
                if (thisText.text == "Install Jump Drive")
                {
                    thisText.text = "+1 Jump Range";
                }
            }
            jumpRangeUpgradeObject.SetActive(resourceAndUpgradeManager.CurrentMaxJumpRange < 6 ? true : false);
            jumpRechargeUpgradeObject.SetActive(resourceAndUpgradeManager.CurrentMaxJumpRecharge < 3 ? true : false);
        }
        else
        {
            TMP_Text[] jumpText = jumpRangeUpgradeObject.GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text thisText in jumpText)
            {
                if (thisText.text == "+1 Jump Range")
                {
                    thisText.text = "Install Jump Drive";
                }
            }
            jumpRangeUpgradeObject.SetActive(true);
            jumpRechargeUpgradeObject.SetActive(false);
        }

        if (resourceAndUpgradeManager.ShieldBoostInstalled)
        {
            shieldButtonHolder.SetActive(resourceAndUpgradeManager.ShieldBoostInstalled);

            TMP_Text[] shieldBoostText = shieldBoostUpgradeObject.GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text thisText in shieldBoostText)
            {
                if (thisText.text == "Install Shield Boost")
                {
                    thisText.text = "+1 Shield Boost";
                }
            }
            shieldBoostUpgradeObject.SetActive(resourceAndUpgradeManager.CurrentMaxShieldBoost < 3 ? true : false);
            shieldBoostRechargeUpgradeObject.SetActive(resourceAndUpgradeManager.CurrentMaxShieldBoostRecharge > 2 ? true : false);
            shieldOverboostUpgradeObject.SetActive(resourceAndUpgradeManager.CurrentShieldOverboostActive ? false : true);
        }
        else
        {
            TMP_Text[] shieldBoostText = shieldBoostUpgradeObject.GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text thisText in shieldBoostText)
            {
                if (thisText.text == "+1 Shield Boost")
                {
                    thisText.text = "Install Shield Boost";
                }
            }

            shieldBoostUpgradeObject.SetActive(true);
            shieldBoostRechargeUpgradeObject.SetActive(false);
            shieldOverboostUpgradeObject.SetActive(false);
        }


        healthRepairObject.SetActive((playerHealthControl.currentPlayerHealth < resourceAndUpgradeManager.CurrentMaxHealth ? true : false));
        
        


        healthUpgradeObject.GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = resourceAndUpgradeManager.HealthMaxUpgradeCost.ToString();
        shieldUpgradeObject.GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = resourceAndUpgradeManager.ShieldMaxUpgradeCost.ToString();

        laserRangeUpgradeObject.GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = resourceAndUpgradeManager.LaserRangeUpgradeCost.ToString();
        laserRechargeUpgradeObject.GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = resourceAndUpgradeManager.LaserRechargeUpgradeCost.ToString();

        sensorRangeUpgradeObject.GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = resourceAndUpgradeManager.SensorRangeUpgradeCost.ToString();

        rocketRangeUpgradeObject.GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = resourceAndUpgradeManager.RocketRangeUpgradeCost.ToString();
        rocketReloadUpgradeObject.GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = resourceAndUpgradeManager.RocketReloadUpgradeCost.ToString();
        rocketYieldUpgradeObject.GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = resourceAndUpgradeManager.RocketYieldUpgradeCost.ToString();

        jumpRangeUpgradeObject.GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = resourceAndUpgradeManager.JumpRangeUpgradeCost.ToString();
        jumpRechargeUpgradeObject.GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = resourceAndUpgradeManager.JumpRechargeUpgradeCost.ToString();

        shieldBoostUpgradeObject.GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = resourceAndUpgradeManager.ShieldBoostUpgradeCost.ToString();
        shieldBoostRechargeUpgradeObject.GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = resourceAndUpgradeManager.ShieldBoostRechargeUpgradeCost.ToString();
        shieldOverboostUpgradeObject.GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = resourceAndUpgradeManager.ShieldOverboostUpgradeCost.ToString();

        healthRepairObject.GetComponentInChildren<Button>().GetComponentInChildren<TMP_Text>().text = resourceAndUpgradeManager.HealthRepairCost.ToString();
    }

    public void DisplayVictoryText()
    {
        TMP_Text[] victoryTextFields = victoryPanel.GetComponentsInChildren<TMP_Text>(); ;
        if (resourceAndUpgradeManager.Resources < 1000)
        {
            
            foreach(TMP_Text textField in victoryTextFields)
            {
                if(textField.name== "VicotryText(TMP)")
                {
                    textField.text = "You leave the gate network and return home. You sell the resources you collected but don't even make enough to recoup your expenses. In the end, you have to sell your ship to get by. Your days of adventure and dreams of riches are over.";
                }
            }
            victoryPanel.SetActive(true);
        }
        else if(resourceAndUpgradeManager.Resources > 1000&& resourceAndUpgradeManager.Resources<2500)
        {
            foreach (TMP_Text textField in victoryTextFields)
            {
                if (textField.name == "VicotryText(TMP)")
                {
                    textField.text = "You leave the gate network and return home. You sell the resources you collected, but only barely cover the expenses of the trip. You get a job running cargo in some of the safer systems for a few years, but its a hard living and in the end, you have to sell your ship to get by. Your days of adventure and dreams of riches are over.";
                }
            }
            victoryPanel.SetActive(true);
        }
        else if(resourceAndUpgradeManager.Resources > 2500&& resourceAndUpgradeManager.Resources<5000)
        {
            foreach (TMP_Text textField in victoryTextFields)
            {
                if (textField.name == "VicotryText(TMP)")
                {
                    textField.text = "You leave the gate network and return home. You sell the resources you collected, and manage to cover all your expenses with a small amount left over. You invest in some upgrades for your ship and manage to become a competative name in the inner system shipping business. You do well and make a modest living.";
                }
            }
            victoryPanel.SetActive(true);
        }
        else if(resourceAndUpgradeManager.Resources > 5000&& resourceAndUpgradeManager.Resources<10000)
        {
            foreach (TMP_Text textField in victoryTextFields)
            {
                if (textField.name == "VicotryText(TMP)")
                {
                    textField.text = "You leave the gate network and return home. You sell the resources you collected, and make a substantial amount of money. You easily cover your expenses and have enough left over to be a little ambitious. You buy a second ship and hire a crew. Slowly you build a small business running cargo in the inner systems. You do very well and live a good life.";
                }
            }
            victoryPanel.SetActive(true);
        }
        else if(resourceAndUpgradeManager.Resources > 10000&& resourceAndUpgradeManager.Resources<20000)
        {
            foreach (TMP_Text textField in victoryTextFields)
            {
                if (textField.name == "VicotryText(TMP)")
                {
                    textField.text = "You leave the gate network and return home. You smile the whole trip back. These are the riches you had dreamed of. With what you've collected you could buy half a dozen small ships, or you could just retire. Buy a big house on a tropical planet. Your future can be whatever you want it to be. You intend to enjoy it.";
                }
            }
            victoryPanel.SetActive(true);
        }
        else if(resourceAndUpgradeManager.Resources > 20000&& resourceAndUpgradeManager.Resources<50000)
        {
            foreach (TMP_Text textField in victoryTextFields)
            {
                if (textField.name == "VicotryText(TMP)")
                {
                    textField.text = "You are in awe as you leave the gate network and return home. This was more than you had ever imagined you would find. Your mind races as you imagine what you could do with this wealth. Whatever you do next, you know one thing for sure. People are going to know your name. You're the one who conqured the gate network.";
                }
            }
            victoryPanel.SetActive(true);
        }
        else if(resourceAndUpgradeManager.Resources > 50000)
        {
            foreach (TMP_Text textField in victoryTextFields)
            {
                if (textField.name == "VicotryText(TMP)")
                {
                    textField.text = "It doesn't even seem real. It was a struggle. You fought tooth and nail, and at the end you weren't even sure you would make it out, but here it is. You look again at the wealth of resources you have amassed and it leaves you dumbfounded. This changes everything.";
                }
            }
            victoryPanel.SetActive(true);
        }
    }
}
