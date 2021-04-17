using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceAndUpgradeManager : MonoBehaviour
{
    //This script is designed to control all of the resources the player collects and manage the upgrades the player has purchased
    private AbilityController abilityController;
    private GameObject player;

    private UIControl uiController;
    private GameObject gameController;

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

    private int resources = 0;

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

    private int laserRangeUpgradeCost=200;
    private int laserRechargeUpgradeCost=100;
    private int rocketRangeUpgradeCost=100;
    private int rocketReloadUpgradeCost=100;
    private int rocketYieldUpgradeCost=100;
    private int jumpRangeUpgradeCost=100;
    private int jumpRechargeUpgradeCost=100;
    private int shieldBoostUpgradeCost=100;
    private int shieldOverboostUpgradeCost=100;
    private int shieldRechargeUpgradeCost=100;
    private int shieldMaxUpgradeCost=100;
    private int healthMaxUpgradeCost=100;
    private int healthRepairCost = 100;

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

    public int LaserRangeUpgradeCost { get { return laserRangeUpgradeCost; }}
    public int LaserRechargeUpgradeCost { get { return laserRechargeUpgradeCost; }}
    public int RocketRangeUpgradeCost { get { return rocketRangeUpgradeCost; }}
    public int RocketReloadUpgradeCost { get { return rocketReloadUpgradeCost; }}
    public int RocketYieldUpgradeCost { get { return rocketYieldUpgradeCost; }}
    public int JumpRangeUpgradeCost { get { return jumpRangeUpgradeCost; }}
    public int JumpRechargeUpgradeCost { get { return jumpRechargeUpgradeCost; }}
    public int ShieldBoostUpgradeCost { get { return shieldBoostUpgradeCost; }}
    public int ShieldOverboostUpgradeCost { get { return shieldOverboostUpgradeCost; }}
    public int ShieldRechargeUpgradeCost { get { return shieldRechargeUpgradeCost; }}
    public int ShieldMaxUpgradeCost { get { return shieldMaxUpgradeCost; }}
    public int HealthMaxUpgradeCost { get { return healthMaxUpgradeCost; }}
    public int HealthRepairCost { get { return healthRepairCost; }}

    public bool RocketsInstalled { get { return rocketsInstalled; } }
    public bool JumpDriveInstalled { get { return jumpDriveInstalled; } }
    public bool ShieldBoostInstalled { get { return shieldBoostInstalled; } }


    public int Resources { get { return resources; } set { resources = value; } }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        abilityController = player.GetComponent<AbilityController>();

        gameController = GameObject.Find("GameController");
        uiController = gameController.GetComponent<UIControl>();
        uiController.SetResourceCount(resources);
        
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
    }

    public void UpgradeHealth()
    {

    }

    public void UpgradeShields()
    {

    }

    public void RepairHealth()
    {

    }

    public void UpgradeLaserRange()
    {
        if (resources >= laserRangeUpgradeCost && currentMaxLaserRange < 6)
        {
            resources -= laserRangeUpgradeCost;
            currentMaxLaserRange += 1;
            laserRangeUpgradeCost *= 3;
            uiController.SetResourceCount(resources);
            uiController.SetUpgradeButtons();
            abilityController.maxLaserRange = currentMaxLaserRange;
        }
    }

    public void UpgradeLaserRecharge()
    {

    }

    public void UpgradeRocketRange()
    {

    }

    public void UpgradeRocketReload()
    {
        
    }

    public void UpgradeRocketYield()
    {

    }

    public void UpgradeShieldBoost()
    {

    }

    public void UpgradeShieldBoostRecharge()
    {

    }

    public void UpgradeShieldOverboost()
    {

    }

    public void UpgradeJumpRange()
    {

    }

    public void UpgradeJumpRecharge()
    {

    }
}
