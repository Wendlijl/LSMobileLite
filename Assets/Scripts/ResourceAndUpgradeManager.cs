using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CI.QuickSave;
using System.IO;

public class ResourceAndUpgradeManager : MonoBehaviour
{
    //This script is designed to control all of the resources the player collects and manage the upgrades the player has purchased
    private GameObject player;
    private AbilityController abilityController;
    private PlayerHealthControl playerHealthControl;
    private MovementController movementController;

    private GameObject gameController;
    private UIControl uiController;
    private ManageMap mapManager;
    private TutorialManager tutorialManager;
    private GridLayout gridlayout;

    private string resourceAndUpgradeDataSaveFileName = "resourceAndUpgradeDataSaveFile";
    private int solarSystemNumber=1;

    private int baseLaserRange = 3;
    private int baseLaserRecharge = 1;
    private int baseRocketRange = 3;
    private int baseRocketReload = 5;
    private int baseRocketYield = 1;
    private int baseJumpRange = 3;
    private int baseJumpRecharge = 2;
    private int baseShieldBoost = 1;
    private bool baseShieldOverboost = false;
    private int baseShieldBoostRecharge = 5;
    private int baseHealth = 3;
    private int baseShields = 2;
    private int baseSensorRange = 1;

    private int resources = 0;
    private int totalResources = 0;

    private int  currentMaxLaserRange = 3;
    private int  currentMaxLaserRecharge = 1;
    private int  currentMaxRocketRange = 3;
    private int  currentMaxRocketReload = 5;
    private int  currentMaxRocketYield = 1;
    private int  currentMaxJumpRange = 3;
    private int  currentMaxJumpRecharge = 2;
    private int  currentMaxShieldBoost = 1;
    private bool currentShieldOverboostActive = false;
    private int  currentMaxShieldBoostRecharge = 5;
    private int  currentMaxHealth = 3;
    private int  currentMaxShields = 2;
    private int  currentMaxSensorRange = 1;

    private int laserRangeUpgradeCost=200;
    private int laserRechargeUpgradeCost=500;
    private int rocketRangeUpgradeCost=1000;
    private int rocketReloadUpgradeCost=500;
    private int rocketYieldUpgradeCost=1000;
    private int jumpRangeUpgradeCost=1000;
    private int jumpRechargeUpgradeCost=1000;
    private int shieldBoostUpgradeCost=1000;
    private int shieldOverboostUpgradeCost=250;
    private int shieldBoostRechargeUpgradeCost = 500;
    private int shieldMaxUpgradeCost=500;
    private int healthMaxUpgradeCost=500;
    private int healthRepairCost = 250;
    private int sensorRangeUpgradeCost = 100;

    private bool rocketsInstalled=false;
    private bool jumpDriveInstalled=false;
    private bool shieldBoostInstalled=false;

    private float threatLevel = 0;
    private int maxThreatLevelCounter=0;

    public string ResourceAndUpgradeDataSaveFileName { get { return resourceAndUpgradeDataSaveFileName; } }
    public int SolarSystemNumber { get { return solarSystemNumber; } set { solarSystemNumber = value; } }
    public int BaseShields{get{return baseShields;}}
    public bool BaseShieldOverboost { get{return baseShieldOverboost; }}
    public int BaseShieldBoostRecharge { get{return baseShieldBoostRecharge; }}
    public int BaseHealth{get{return baseHealth; }}
    public int BaseShieldBoost { get{return baseShieldBoost; }}
    public int BaseJumpRange { get{return baseJumpRange; }}
    public int BaseJumpRecharge { get{return baseJumpRecharge; }}
    public int BaseRocketRange { get{return baseRocketRange; }}
    public int BaseRocketReload { get{return baseRocketReload; }}
    public int BaseRocketYield { get{return baseRocketYield; }}
    public int BaseLaserRange { get{return baseLaserRange; }}
    public int BaseLaserRecharge { get{return baseLaserRecharge; }}
    public int BaseSensorRange { get { return baseSensorRange; } }

    public int CurrentMaxShields { get { return currentMaxShields; } }
    public bool CurrentShieldOverboostActive { get { return currentShieldOverboostActive; } }
    public int CurrentMaxShieldBoostRecharge { get { return currentMaxShieldBoostRecharge; } }
    public int CurrentMaxHealth { get { return currentMaxHealth; } }
    public int CurrentMaxShieldBoost { get { return currentMaxShieldBoost; } }
    public int CurrentMaxJumpRange { get { return currentMaxJumpRange; } }
    public int CurrentMaxJumpRecharge { get { return currentMaxJumpRecharge; } }
    public int CurrentMaxRocketRange { get { return currentMaxRocketRange; } }
    public int CurrentMaxRocketReload { get { return currentMaxRocketReload; } }
    public int CurrentMaxRocketYield { get { return currentMaxRocketYield; } }
    public int CurrentMaxLaserRange { get { return currentMaxLaserRange; } }
    public int CurrentMaxLaserRecharge { get { return currentMaxLaserRecharge; } }
    public int CurrentMaxSensorRange { get { return currentMaxSensorRange; } }

    public int LaserRangeUpgradeCost { get { return laserRangeUpgradeCost; }}
    public int LaserRechargeUpgradeCost { get { return laserRechargeUpgradeCost; }}
    public int RocketRangeUpgradeCost { get { return rocketRangeUpgradeCost; }}
    public int RocketReloadUpgradeCost { get { return rocketReloadUpgradeCost; }}
    public int RocketYieldUpgradeCost { get { return rocketYieldUpgradeCost; }}
    public int JumpRangeUpgradeCost { get { return jumpRangeUpgradeCost; }}
    public int JumpRechargeUpgradeCost { get { return jumpRechargeUpgradeCost; }}
    public int ShieldBoostUpgradeCost { get { return shieldBoostUpgradeCost; }}
    public int ShieldOverboostUpgradeCost { get { return shieldOverboostUpgradeCost; }}
    public int ShieldBoostRechargeUpgradeCost { get { return shieldBoostRechargeUpgradeCost; }}
    public int ShieldMaxUpgradeCost { get { return shieldMaxUpgradeCost; }}
    public int HealthMaxUpgradeCost { get { return healthMaxUpgradeCost; }}
    public int HealthRepairCost { get { return healthRepairCost; }}
    public int SensorRangeUpgradeCost { get { return sensorRangeUpgradeCost; } }

    public bool RocketsInstalled { get { return rocketsInstalled; } }
    public bool JumpDriveInstalled { get { return jumpDriveInstalled; } }
    public bool ShieldBoostInstalled { get { return shieldBoostInstalled; } }


    public int Resources { get { return resources; } set { resources = value; } }
    public int TotalResources { get { return totalResources; } set { totalResources = value; } }
    public float ThreatLevel { get { return threatLevel; } set { threatLevel = value; } }
    public int MaxThreatLevelCounter { get { return maxThreatLevelCounter; } set { maxThreatLevelCounter = value; } }

    // Start is called before the first frame update
    void Start()
    {
        ThreatLevel = 0;
        player = GameObject.Find("Player");
        abilityController = player.GetComponent<AbilityController>();
        playerHealthControl = player.GetComponent<PlayerHealthControl>();
        movementController = player.GetComponent<MovementController>();

        gameController = GameObject.Find("GameController");
        uiController = gameController.GetComponent<UIControl>();
        mapManager = gameController.GetComponent<ManageMap>();
        tutorialManager = gameController.GetComponent<TutorialManager>();
        uiController.SetResourceCount(resources);

        gridlayout = GameObject.Find("Grid").GetComponent<GridLayout>();


        if(mapManager.saveName == "TutorialFile")
        {
            resourceAndUpgradeDataSaveFileName = "tutorialResourceAndUpgradeDataSaveFile";
        }

        if (QuickSaveRoot.Exists(resourceAndUpgradeDataSaveFileName)) //use the quicksave feature to check if a save file exists 
        {
            LoadResourceAndUpgradeData(); //if a save file exists, call the load function            
        }
        else
        {
            playerHealthControl.RestoreHealth();
            playerHealthControl.RestoreShields();
            abilityController.laserRange = currentMaxLaserRange;
            uiController.SetLaserCharge(abilityController.laserRange, currentMaxLaserRange);
        }
    }

    private void LoadResourceAndUpgradeData()
    {
        if (QuickSaveRoot.Exists(resourceAndUpgradeDataSaveFileName)) //if a save file exists, load data from that file
        {
            QuickSaveReader instReader = QuickSaveReader.Create(resourceAndUpgradeDataSaveFileName); //create an instance of the quick save reader to pull in the save file

            resources = instReader.Read<int>("resources");
            TotalResources = instReader.Read<int>("totalResources");
            SolarSystemNumber = instReader.Read<int>("solarSystemNumber");
            currentMaxLaserRange = instReader.Read<int>("currentMaxLaserRange");
            currentMaxLaserRecharge = instReader.Read<int>("currentMaxLaserRecharge");
            currentMaxRocketRange = instReader.Read<int>("currentMaxRocketRange");
            currentMaxRocketReload = instReader.Read<int>("currentMaxRocketReload");
            currentMaxRocketYield = instReader.Read<int>("currentMaxRocketYield");
            currentMaxJumpRange = instReader.Read<int>("currentMaxJumpRange");
            currentMaxJumpRecharge = instReader.Read<int>("currentMaxJumpRecharge");
            currentMaxShieldBoost = instReader.Read<int>("currentMaxShieldBoost");
            currentShieldOverboostActive = instReader.Read<bool>("currentShieldOverboostActive");
            currentMaxShieldBoostRecharge = instReader.Read<int>("currentMaxShieldBoostRecharge");
            currentMaxHealth = instReader.Read<int>("currentMaxHealth");
            currentMaxShields = instReader.Read<int>("currentMaxShields");
            currentMaxSensorRange = instReader.Read<int>("currentMaxSensorRange");

            rocketsInstalled = instReader.Read<bool>("rocketsInstalled");
            jumpDriveInstalled = instReader.Read<bool>("jumpDriveInstalled");
            shieldBoostInstalled = instReader.Read<bool>("shieldBoostInstalled");

            abilityController.maxLaserRange = currentMaxLaserRange;
            abilityController.rocketRange = currentMaxRocketRange;
            abilityController.rocketReloadTime = currentMaxRocketReload;
            abilityController.maxJumpRange = currentMaxJumpRange;
            abilityController.shieldBoostRechargeTime = CurrentMaxShieldBoostRecharge;
            movementController.Vision = CurrentMaxSensorRange;
            mapManager.UpdateFogOfWar(CurrentMaxSensorRange, gridlayout.WorldToCell(player.transform.position));

            playerHealthControl.currentPlayerHealth = instReader.Read<int>("currentHealth");
            playerHealthControl.currentPlayerShields = instReader.Read<int>("currentShields");
            abilityController.jumpRange = instReader.Read<int>("currentJumpCharge");
            abilityController.laserRange = instReader.Read<int>("currentLaserCharge");
            //Debug.Log("Laser range loaded as " + abilityController.laserRange);
            abilityController.currentShieldBoostCharge = instReader.Read<int>("currentShieldBoostCharge");
            abilityController.currentRocketReloadAmount = instReader.Read<int>("currentRocketReload");

            laserRangeUpgradeCost = instReader.Read<int>("laserRangeUpgradeCost");
            laserRechargeUpgradeCost = instReader.Read<int>("laserRechargeUpgradeCost");
            rocketRangeUpgradeCost = instReader.Read<int>("rocketRangeUpgradeCost");
            rocketReloadUpgradeCost = instReader.Read<int>("rocketReloadUpgradeCost");
            rocketYieldUpgradeCost = instReader.Read<int>("rocketYieldUpgradeCost");
            jumpRangeUpgradeCost = instReader.Read<int>("jumpRangeUpgradeCost");
            jumpRechargeUpgradeCost = instReader.Read<int>("jumpRechargeUpgradeCost");
            shieldBoostUpgradeCost = instReader.Read<int>("shieldBoostUpgradeCost");
            shieldOverboostUpgradeCost = instReader.Read<int>("shieldOverboostUpgradeCost");
            shieldBoostRechargeUpgradeCost = instReader.Read<int>("shieldBoostRechargeUpgradeCost");
            shieldMaxUpgradeCost = instReader.Read<int>("shieldMaxUpgradeCost");
            healthMaxUpgradeCost = instReader.Read<int>("healthMaxUpgradeCost");
            sensorRangeUpgradeCost = instReader.Read<int>("sensorRangeUpgradeCost");

            threatLevel = instReader.Read<float>("threatLevel");
            MaxThreatLevelCounter = instReader.Read<int>("maxThreatLevelCounter");

            uiController.SetHealthState(CurrentMaxHealth, playerHealthControl.currentPlayerHealth, CurrentMaxShields, playerHealthControl.currentPlayerShields);
            uiController.SetLaserCharge(abilityController.laserRange, currentMaxLaserRange);
            uiController.SetJumpCharge(abilityController.jumpRange, CurrentMaxJumpRange);
            uiController.SetShieldBoostRechargeState(abilityController.currentShieldBoostCharge, CurrentMaxShieldBoostRecharge);
            uiController.SetRocketReloadState(abilityController.currentRocketReloadAmount, CurrentMaxRocketReload);
            uiController.SetResourceCount(resources);
            uiController.SetUpgradeButtons();
            uiController.SetThreatLevelSlider(ThreatLevel);

            Debug.Log("Tried to load resources and upgrades");
        }
    }

    public void SaveResourceAndUpgradeData()
    {
        QuickSaveWriter instWriter = QuickSaveWriter.Create(resourceAndUpgradeDataSaveFileName); //create an instance of the QuickSaveWriter

        instWriter.Write<int>("resources", Resources); 
        instWriter.Write<int>("totalResources", TotalResources); 
        instWriter.Write<int>("solarSystemNumber", SolarSystemNumber); 
        instWriter.Write<int>("currentMaxLaserRange", currentMaxLaserRange); 
        instWriter.Write<int>("currentMaxLaserRecharge", currentMaxLaserRecharge); 
        instWriter.Write<int>("currentMaxRocketRange", currentMaxRocketRange); 
        instWriter.Write<int>("currentMaxRocketReload", currentMaxRocketReload); 
        instWriter.Write<int>("currentMaxRocketYield", currentMaxRocketYield); 
        instWriter.Write<int>("currentMaxJumpRange", currentMaxJumpRange); 
        instWriter.Write<int>("currentMaxJumpRecharge", currentMaxJumpRecharge); 
        instWriter.Write<int>("currentMaxShieldBoost", currentMaxShieldBoost); 
        instWriter.Write<bool>("currentShieldOverboostActive", currentShieldOverboostActive); 
        instWriter.Write<int>("currentMaxShieldBoostRecharge", currentMaxShieldBoostRecharge); 
        instWriter.Write<int>("currentMaxHealth", currentMaxHealth); 
        instWriter.Write<int>("currentMaxShields", currentMaxShields); 
        instWriter.Write<int>("currentMaxSensorRange", currentMaxSensorRange); 
               
        instWriter.Write<bool>("rocketsInstalled", rocketsInstalled); 
        instWriter.Write<bool>("jumpDriveInstalled", jumpDriveInstalled); 
        instWriter.Write<bool>("shieldBoostInstalled", shieldBoostInstalled);

        instWriter.Write<int>("currentHealth", playerHealthControl.currentPlayerHealth);
        instWriter.Write<int>("currentShields", playerHealthControl.currentPlayerShields);
        instWriter.Write<int>("currentJumpCharge", abilityController.jumpRange);
        instWriter.Write<int>("currentLaserCharge", abilityController.laserRange);
        //Debug.Log("Laser range saved as " + abilityController.laserRange);
        instWriter.Write<int>("currentShieldBoostCharge", abilityController.currentShieldBoostCharge);
        instWriter.Write<int>("currentRocketReload", abilityController.currentRocketReloadAmount);
        
        instWriter.Write<int>("healthMaxUpgradeCost", HealthMaxUpgradeCost);
        instWriter.Write<int>("shieldMaxUpgradeCost", ShieldMaxUpgradeCost);
        instWriter.Write<int>("sensorRangeUpgradeCost", SensorRangeUpgradeCost);
        instWriter.Write<int>("rocketRangeUpgradeCost", RocketRangeUpgradeCost);
        instWriter.Write<int>("rocketReloadUpgradeCost", RocketReloadUpgradeCost);
        instWriter.Write<int>("rocketYieldUpgradeCost", RocketYieldUpgradeCost);
        instWriter.Write<int>("laserRangeUpgradeCost", LaserRangeUpgradeCost);
        instWriter.Write<int>("laserRechargeUpgradeCost", LaserRechargeUpgradeCost);
        instWriter.Write<int>("jumpRangeUpgradeCost", JumpRangeUpgradeCost);
        instWriter.Write<int>("jumpRechargeUpgradeCost", JumpRechargeUpgradeCost);
        instWriter.Write<int>("shieldBoostUpgradeCost", ShieldBoostUpgradeCost);
        instWriter.Write<int>("shieldBoostRechargeUpgradeCost", ShieldBoostRechargeUpgradeCost);
        instWriter.Write<int>("shieldOverboostUpgradeCost", ShieldOverboostUpgradeCost);

        instWriter.Write<float>("threatLevel", ThreatLevel);
        instWriter.Write<int>("maxThreatLevelCounter", MaxThreatLevelCounter);


        instWriter.Commit();//write the save file
    }

    public void DeleteResourceAndUpgradeSaveFile() 
    {
        if (QuickSaveRoot.Exists(resourceAndUpgradeDataSaveFileName)) //check if the file exists
        {
            QuickSaveRoot.Delete(resourceAndUpgradeDataSaveFileName); //if the file exists, then delete it
            print("Deleted data file " + resourceAndUpgradeDataSaveFileName); //send a message that the file was deleted
        }
        else
        {
            print("Nothing to delete"); //if no save file exists, send a message that nothing was done
        }
    }

    public void ModifyResources(int newResources, bool addToCurrent)
    {
        if (addToCurrent)
        {
            resources += newResources;
        }
        else
        {
            resources = newResources;
        }
        TotalResources += newResources;
        uiController.SetResourceCount(resources);
        SaveResourceAndUpgradeData();
    }

    public void UpgradeHealth()
    {
        if (resources >= HealthMaxUpgradeCost && currentMaxHealth < 6 && uiController.UpgradeState)
        {
            resources -= HealthMaxUpgradeCost;
            currentMaxHealth += 1;
            playerHealthControl.MaxPlayerHealth = currentMaxHealth;
            playerHealthControl.IncreaseHealth(1);
            uiController.SetResourceCount(resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
            if (mapManager.saveName == "TutorialFile")
            {
                tutorialManager.RevealMap();
            }
        }
    }

    public void UpgradeShields()
    {
        if (resources >= ShieldMaxUpgradeCost && currentMaxShields < 6 && uiController.UpgradeState)
        {
            resources -= ShieldMaxUpgradeCost;
            currentMaxShields += 1;
            playerHealthControl.maxPlayerShields = currentMaxShields;
            playerHealthControl.RestoreShields();
            uiController.SetResourceCount(resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
            if (mapManager.saveName == "TutorialFile")
            {
                tutorialManager.RevealMap();
            }
        }
    }

    public void RepairHealth()
    {
        if (resources >= healthRepairCost && uiController.UpgradeState)
        {
            resources -= healthRepairCost;
            playerHealthControl.IncreaseHealth(1);
            uiController.SetResourceCount(resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
            if (mapManager.saveName == "TutorialFile")
            {
                tutorialManager.RevealMap();
            }
        }
    }

    public void UpgradeLaserRange()
    {
        if (resources >= laserRangeUpgradeCost && currentMaxLaserRange < 6 && uiController.UpgradeState)
        {
            resources -= laserRangeUpgradeCost;
            currentMaxLaserRange += 1;
            laserRangeUpgradeCost *= 3;
            abilityController.maxLaserRange = currentMaxLaserRange;
            abilityController.laserRange = CurrentMaxLaserRange;
            uiController.SetLaserCharge(CurrentMaxLaserRange, CurrentMaxLaserRange);
            uiController.SetResourceCount(resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
            if (mapManager.saveName == "TutorialFile")
            {
                tutorialManager.RevealMap();
            }
        }
    }

    public void UpgradeLaserRecharge()
    {
        if(resources>=laserRechargeUpgradeCost && currentMaxLaserRecharge < 3 && uiController.UpgradeState)
        {
            resources -= laserRechargeUpgradeCost;
            currentMaxLaserRecharge += 1;
            laserRechargeUpgradeCost *= 3;
            uiController.SetResourceCount(resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
            if (mapManager.saveName == "TutorialFile")
            {
                tutorialManager.RevealMap();
            }
        }
    }

    public void UpgradeRocketRange()
    {
        if (rocketsInstalled)
        {
            if (resources >= RocketRangeUpgradeCost && currentMaxRocketRange < 6 && uiController.UpgradeState)
            {
                resources -= rocketRangeUpgradeCost;
                currentMaxRocketRange += 1;
                rocketRangeUpgradeCost *= 3;
                abilityController.rocketRange = currentMaxRocketRange;
                uiController.SetResourceCount(Resources);
                uiController.SetUpgradeButtons();
                SaveResourceAndUpgradeData();
                if (mapManager.saveName == "TutorialFile")
                {
                    tutorialManager.RevealMap();
                }
            }
        }
        else
        {
            if (resources >= RocketRangeUpgradeCost && uiController.UpgradeState)
            {
                resources -= rocketRangeUpgradeCost;
                rocketsInstalled = true;
                rocketRangeUpgradeCost = 300;
                abilityController.currentRocketReloadAmount = CurrentMaxRocketReload;
                uiController.SetRocketReloadState(abilityController.currentRocketReloadAmount, CurrentMaxRocketReload);
                uiController.SetResourceCount(Resources);
                uiController.SetUpgradeButtons();
                SaveResourceAndUpgradeData();
                if (mapManager.saveName == "TutorialFile")
                {
                    tutorialManager.RevealMap();
                }
            }
        }

    }

    public void UpgradeRocketReload()
    {
        if (resources >= rocketReloadUpgradeCost && currentMaxRocketReload > 2 && uiController.UpgradeState)
        {
            resources -= rocketReloadUpgradeCost;
            rocketReloadUpgradeCost *= 3;
            currentMaxRocketReload -= 1;
            abilityController.rocketReloadTime = currentMaxRocketReload;
            uiController.SetResourceCount(Resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
            if (mapManager.saveName == "TutorialFile")
            {
                tutorialManager.RevealMap();
            }
        }
    }

    public void UpgradeRocketYield()
    {
        if (resources >= RocketYieldUpgradeCost && CurrentMaxRocketYield < 3 && uiController.UpgradeState)
        {
            resources -= RocketYieldUpgradeCost;
            rocketYieldUpgradeCost *= 3;
            currentMaxRocketYield += 1;
            uiController.SetResourceCount(Resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
        }
    }

    public void UpgradeShieldBoost()
    {
        if (shieldBoostInstalled)
        {
            if (resources >= shieldBoostUpgradeCost && currentMaxShieldBoost < 3 && uiController.UpgradeState)
            {
                resources -= shieldBoostUpgradeCost;
                currentMaxShieldBoost += 1;
                shieldBoostUpgradeCost *= 3;
                uiController.SetResourceCount(Resources);
                uiController.SetUpgradeButtons();
                SaveResourceAndUpgradeData();
                if (mapManager.saveName == "TutorialFile")
                {
                    tutorialManager.RevealMap();
                }
            }
        }
        else
        {
            if (Resources >= shieldBoostUpgradeCost && uiController.UpgradeState)
            {
                resources -= shieldBoostUpgradeCost;
                shieldBoostInstalled = true;
                shieldBoostUpgradeCost = 100;
                abilityController.currentShieldBoostCharge = CurrentMaxShieldBoostRecharge;
                uiController.SetShieldBoostRechargeState(abilityController.currentShieldBoostCharge, CurrentMaxShieldBoostRecharge);
                uiController.SetResourceCount(Resources);
                uiController.SetUpgradeButtons();
                SaveResourceAndUpgradeData();
                if (mapManager.saveName == "TutorialFile")
                {
                    tutorialManager.RevealMap();
                }
            }
        }
    }

    public void UpgradeShieldBoostRecharge()
    {
        if (resources >= shieldBoostRechargeUpgradeCost && currentMaxShieldBoostRecharge>2 && uiController.UpgradeState)
        {
            resources -= shieldBoostRechargeUpgradeCost;
            currentMaxShieldBoostRecharge -= 1;
            shieldBoostRechargeUpgradeCost *= 3;
            uiController.SetResourceCount(Resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
            if (mapManager.saveName == "TutorialFile")
            {
                tutorialManager.RevealMap();
            }
        }
    }

    public void UpgradeShieldOverboost()
    {
        if (resources >= ShieldOverboostUpgradeCost&&!currentShieldOverboostActive && uiController.UpgradeState)
        {
            resources -= ShieldOverboostUpgradeCost;
            currentShieldOverboostActive = true;
            uiController.SetResourceCount(Resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
            if (mapManager.saveName == "TutorialFile")
            {
                tutorialManager.RevealMap();
            }
        }
    }

    public void UpgradeJumpRange()
    {
        if (jumpDriveInstalled)
        {
            if (resources >= jumpRangeUpgradeCost && currentMaxJumpRange < 6 && uiController.UpgradeState)
            {
                resources -= jumpRangeUpgradeCost;
                currentMaxJumpRange += 1;
                jumpRangeUpgradeCost *= 3;
                abilityController.maxJumpRange = currentMaxJumpRange;
                abilityController.jumpRange = currentMaxJumpRange;
                uiController.SetJumpCharge(currentMaxJumpRange, currentMaxJumpRange);
                uiController.SetResourceCount(Resources);
                uiController.SetUpgradeButtons();
                SaveResourceAndUpgradeData();
                if (mapManager.saveName == "TutorialFile")
                {
                    tutorialManager.RevealMap();
                }
            }
        }
        else
        {
            if (Resources >= jumpRangeUpgradeCost && uiController.UpgradeState)
            {
                resources -= jumpRangeUpgradeCost;
                jumpDriveInstalled = true;
                jumpRangeUpgradeCost = 100;
                abilityController.jumpRange = CurrentMaxJumpRange;
                uiController.SetJumpCharge(abilityController.jumpRange, CurrentMaxJumpRange);
                uiController.SetResourceCount(Resources);
                uiController.SetUpgradeButtons();
                SaveResourceAndUpgradeData();
                if (mapManager.saveName == "TutorialFile")
                {
                    tutorialManager.RevealMap();
                }
            }
        }
    }

    public void UpgradeJumpRecharge()
    {
        if (resources >= jumpRechargeUpgradeCost && uiController.UpgradeState)
        {
            resources -= jumpRechargeUpgradeCost;
            currentMaxJumpRecharge += 1;
            jumpRechargeUpgradeCost *= 3;
            uiController.SetResourceCount(Resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
            if (mapManager.saveName == "TutorialFile")
            {
                tutorialManager.RevealMap();
            }
        }
    }

    public void UpgradeSensorRange()
    {
        if (resources >= sensorRangeUpgradeCost && uiController.UpgradeState)
        {
            resources -= sensorRangeUpgradeCost;
            currentMaxSensorRange += 1;
            if (sensorRangeUpgradeCost < 900)
            {
                sensorRangeUpgradeCost *= 3;
            }
            else
            {
                sensorRangeUpgradeCost += 300;
            }
            
            movementController.Vision = CurrentMaxSensorRange;
            mapManager.UpdateFogOfWar(CurrentMaxSensorRange, gridlayout.WorldToCell(player.transform.position));
            uiController.SetResourceCount(Resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
            if (mapManager.saveName == "TutorialFile")
            {
                tutorialManager.RevealMap();
            }
        }
    }

    public void AdjustThreatLevel(int threat)
    {
        if (ThreatLevel < 1)
        {
            ThreatLevel += 0.00005f * threat;
        }
    }

    public void MaxThreatLevelAssault()
    {
        if (MaxThreatLevelCounter < 5)
        {
            MaxThreatLevelCounter++;
        }
        else
        {
            mapManager.Save();
            SaveResourceAndUpgradeData();
            movementController.HasMoved = false;
            abilityController.AbilityUsed = false;
            mapManager.ContextualSpawnEnemies();
            MaxThreatLevelCounter = 0;
        }
    }
}
