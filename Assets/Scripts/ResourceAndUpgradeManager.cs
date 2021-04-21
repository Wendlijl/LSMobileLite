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

    private string resourceAndUpgradeDataSaveFileName = "resourceAndUpgradeDataSaveFile";

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

    private int resources = 10000;

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
    private int laserRechargeUpgradeCost=100;
    private int rocketRangeUpgradeCost=1000;
    private int rocketReloadUpgradeCost=100;
    private int rocketYieldUpgradeCost=100;
    private int jumpRangeUpgradeCost=1000;
    private int jumpRechargeUpgradeCost=100;
    private int shieldBoostUpgradeCost=1000;
    private int shieldOverboostUpgradeCost=100;
    private int shieldBoostRechargeUpgradeCost = 100;
    private int shieldMaxUpgradeCost=100;
    private int healthMaxUpgradeCost=100;
    private int healthRepairCost = 500;
    private int sensorRangeUpgradeCost = 100;

    private bool rocketsInstalled=false;
    private bool jumpDriveInstalled=false;
    private bool shieldBoostInstalled=false;


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

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        abilityController = player.GetComponent<AbilityController>();
        playerHealthControl = player.GetComponent<PlayerHealthControl>();
        movementController = player.GetComponent<MovementController>();

        gameController = GameObject.Find("GameController");
        uiController = gameController.GetComponent<UIControl>();
        uiController.SetResourceCount(resources);

        if (QuickSaveRoot.Exists(resourceAndUpgradeDataSaveFileName)) //use the quicksave feature to check if a save file exists 
        {
            LoadResourceAndUpgradeData(); //if a save file exists, call the load function
        }

    }

    private void LoadResourceAndUpgradeData()
    {
        if (QuickSaveRoot.Exists(resourceAndUpgradeDataSaveFileName)) //if a save file exists, load data from that file
        {
            QuickSaveReader instReader = QuickSaveReader.Create(resourceAndUpgradeDataSaveFileName); //create an instance of the quick save reader to pull in the save file

            resources = instReader.Read<int>("resources");
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
            movementController.vision = CurrentMaxSensorRange;

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

            uiController.SetHealthState(CurrentMaxHealth, playerHealthControl.currentPlayerHealth, CurrentMaxShields, playerHealthControl.currentPlayerShields);
            uiController.SetLaserCharge(abilityController.laserRange, currentMaxLaserRange);
            uiController.SetJumpCharge(abilityController.jumpRange, CurrentMaxJumpRange);
            uiController.SetShieldBoostRechargeState(abilityController.currentShieldBoostCharge, CurrentMaxShieldBoostRecharge);
            uiController.SetRocketReloadState(abilityController.currentRocketReloadAmount, CurrentMaxRocketReload);
            uiController.SetResourceCount(resources);
            uiController.SetUpgradeButtons();

            Debug.Log("Tried to load resources and upgrades");
        }
    }

    public void SaveResourceAndUpgradeData()
    {
        QuickSaveWriter instWriter = QuickSaveWriter.Create(resourceAndUpgradeDataSaveFileName); //create an instance of the QuickSaveWriter

        instWriter.Write<int>("resources", Resources); 
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
        uiController.SetResourceCount(resources);
        SaveResourceAndUpgradeData();
    }

    public void UpgradeHealth()
    {
        if (resources >= HealthMaxUpgradeCost && currentMaxHealth < 6)
        {
            resources -= HealthMaxUpgradeCost;
            currentMaxHealth += 1;
            playerHealthControl.maxPlayerHealth = currentMaxHealth;
            playerHealthControl.RestoreHealth();
            uiController.SetResourceCount(resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
        }
    }

    public void UpgradeShields()
    {
        if (resources >= ShieldMaxUpgradeCost && currentMaxShields < 6)
        {
            resources -= ShieldMaxUpgradeCost;
            currentMaxShields += 1;
            playerHealthControl.maxPlayerShields = currentMaxShields;
            playerHealthControl.RestoreShields();
            uiController.SetResourceCount(resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
        }
    }

    public void RepairHealth()
    {
        if (resources >= healthRepairCost)
        {
            resources -= healthRepairCost;
            playerHealthControl.IncreaseHealth(1);
            uiController.SetResourceCount(resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
        }
    }

    public void UpgradeLaserRange()
    {
        if (resources >= laserRangeUpgradeCost && currentMaxLaserRange < 6)
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
        }
    }

    public void UpgradeLaserRecharge()
    {
        if(resources>=laserRechargeUpgradeCost && currentMaxLaserRecharge < 3)
        {
            resources -= laserRechargeUpgradeCost;
            currentMaxLaserRecharge += 1;
            laserRechargeUpgradeCost *= 3;
            uiController.SetResourceCount(resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
        }
    }

    public void UpgradeRocketRange()
    {
        if (rocketsInstalled)
        {
            if (resources >= RocketRangeUpgradeCost && currentMaxRocketRange < 6)
            {
                resources -= rocketRangeUpgradeCost;
                currentMaxRocketRange += 1;
                rocketRangeUpgradeCost *= 3;
                abilityController.rocketRange = currentMaxRocketRange;
                uiController.SetResourceCount(Resources);
                uiController.SetUpgradeButtons();
                SaveResourceAndUpgradeData();
            }
        }
        else
        {
            if (resources >= RocketRangeUpgradeCost)
            {
                resources -= rocketRangeUpgradeCost;
                rocketsInstalled = true;
                rocketRangeUpgradeCost = 100;
                uiController.SetResourceCount(Resources);
                uiController.SetUpgradeButtons();
                SaveResourceAndUpgradeData();
            }
        }

    }

    public void UpgradeRocketReload()
    {
        if (resources >= rocketReloadUpgradeCost && currentMaxRocketReload > 2)
        {
            resources -= rocketReloadUpgradeCost;
            rocketReloadUpgradeCost *= 3;
            currentMaxRocketReload -= 1;
            abilityController.rocketReloadTime = currentMaxRocketReload;
            uiController.SetResourceCount(Resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
        }
    }

    public void UpgradeRocketYield()
    {
        if (resources >= RocketYieldUpgradeCost && CurrentMaxRocketYield < 3)
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
            if (resources >= shieldBoostUpgradeCost && currentMaxShieldBoost < 3)
            {
                resources -= shieldBoostUpgradeCost;
                currentMaxShieldBoost += 1;
                shieldBoostUpgradeCost *= 3;
                uiController.SetResourceCount(Resources);
                uiController.SetUpgradeButtons();
                SaveResourceAndUpgradeData();
            }
        }
        else
        {
            resources -= shieldBoostUpgradeCost;
            shieldBoostInstalled = true;
            shieldBoostUpgradeCost = 100;
            uiController.SetResourceCount(Resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
        }
    }

    public void UpgradeShieldBoostRecharge()
    {
        if (resources >= shieldBoostRechargeUpgradeCost && currentMaxShieldBoostRecharge>2)
        {
            resources -= shieldBoostRechargeUpgradeCost;
            currentMaxShieldBoostRecharge -= 1;
            shieldBoostRechargeUpgradeCost *= 3;
            uiController.SetResourceCount(Resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
        }
    }

    public void UpgradeShieldOverboost()
    {
        if (resources >= ShieldOverboostUpgradeCost&&!currentShieldOverboostActive)
        {
            resources -= ShieldOverboostUpgradeCost;
            currentShieldOverboostActive = true;
            uiController.SetResourceCount(Resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
        }
    }

    public void UpgradeJumpRange()
    {
        if (jumpDriveInstalled)
        {
            if (resources >= jumpRangeUpgradeCost && currentMaxJumpRange < 6)
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
            }
        }
        else
        {
            resources -= jumpRangeUpgradeCost;
            jumpDriveInstalled = true;
            jumpRangeUpgradeCost = 100;
            uiController.SetResourceCount(Resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
        }
    }

    public void UpgradeJumpRecharge()
    {
        if (resources >= jumpRechargeUpgradeCost)
        {
            resources -= jumpRechargeUpgradeCost;
            currentMaxJumpRecharge += 1;
            jumpRechargeUpgradeCost *= 3;
            uiController.SetResourceCount(Resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
        }
    }

    public void UpgradeSensorRange()
    {
        if (resources >= sensorRangeUpgradeCost)
        {
            resources -= sensorRangeUpgradeCost;
            currentMaxSensorRange += 1;
            sensorRangeUpgradeCost *= 3;
            movementController.vision = CurrentMaxSensorRange;
            uiController.SetResourceCount(Resources);
            uiController.SetUpgradeButtons();
            SaveResourceAndUpgradeData();
        }
    }
}
