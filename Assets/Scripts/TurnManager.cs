using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    //The purpose of this script is to controll all actions that transition between turns
    public bool combatActive;
    private bool firstTurn;
    public bool playerTurn;
    public bool enemyTurn;
    private GameObject gameController;
    private GameObject player;
    private ManageMap mapManager;
    private AbilityController abilityController;
    private MovementController movementController;
    private UIControl uiController;
    private PlayerHealthControl playerHealthControl;
    private ResourceAndUpgradeManager resourceAndUpgradeManager;
    private TutorialManager tutorialManager;

    void Start()
    {
        gameController = GameObject.Find("GameController");
        player = GameObject.Find("Player");
        tutorialManager = gameController.GetComponent<TutorialManager>();
        mapManager = gameController.GetComponent<ManageMap>();
        uiController = gameController.GetComponent<UIControl>();
        resourceAndUpgradeManager = gameController.GetComponent<ResourceAndUpgradeManager>();
        abilityController = player.GetComponent<AbilityController>();
        movementController = player.GetComponent<MovementController>();
        playerHealthControl = player.GetComponent<PlayerHealthControl>();

        firstTurn = false;
        combatActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (mapManager.spawnedEnemies.Count > 0 && !combatActive && !playerHealthControl.BeenDestroyed)
        {
            //Debug.Log("Combat is active");
            firstTurn = true;
            combatActive = true;
            uiController.DeactivateLandOnPlanet();
            StartCoroutine(UpdateTurn());
        }
        else if(mapManager.spawnedEnemies.Count <= 0 && combatActive)
        {
            //Debug.Log("Combat is inactive");
            combatActive = false;
            StartCoroutine(UpdateTurn());
            //Debug.Log("TM 45");
        }
    }

    public IEnumerator UpdateTurn()
    {
        uiController.SetPlayerTurnIndicators();
        yield return new WaitForSeconds(0.5f);
        if (firstTurn)
        {
            firstTurn = false;
            playerTurn = true;
            //uiController.SetEndTurnButtonState();
        }

        if (combatActive)
        {
            if (playerTurn && abilityController.abilityUsed && movementController.hasMoved)
            {
                if (abilityController.laserRange < resourceAndUpgradeManager.CurrentMaxLaserRange)
                {
                    abilityController.laserRange+= resourceAndUpgradeManager.CurrentMaxLaserRecharge;
                    if (abilityController.laserRange > resourceAndUpgradeManager.CurrentMaxLaserRange)
                    {
                        abilityController.laserRange = resourceAndUpgradeManager.CurrentMaxLaserRange;
                    }
                }
                if (abilityController.jumpRange < resourceAndUpgradeManager.CurrentMaxJumpRange)
                {
                    abilityController.jumpRange+=resourceAndUpgradeManager.CurrentMaxJumpRecharge;
                    if (abilityController.jumpRange > resourceAndUpgradeManager.CurrentMaxJumpRange)
                    {
                        abilityController.jumpRange = resourceAndUpgradeManager.CurrentMaxJumpRange;
                    }
                }
                if (abilityController.currentRocketReloadAmount < abilityController.rocketReloadTime)
                {
                    abilityController.currentRocketReloadAmount++;

                }
                if (abilityController.currentShieldBoostCharge < abilityController.shieldBoostRechargeTime)
                {
                    abilityController.currentShieldBoostCharge++;
                }

                GameObject[] rockets = GameObject.FindGameObjectsWithTag("Rocket");
                foreach (GameObject rocket in rockets)
                {
                    rocket.GetComponent<RocketController>().turnsAlive++;

                }
                playerTurn = false;
                enemyTurn = true;
                //Debug.Log("Update UI. Reset player controls");
                //Debug.Log("TM 74");
                //
                movementController.hasMoved = false;
                abilityController.abilityUsed = false;
                StartCoroutine("OrderEnemyTurns");
            }
            else if (enemyTurn)
            {
                //Debug.Log("Order enemy turns. Update UI");
                enemyTurn = false;
                playerTurn = true;
                //uiController.SetEndTurnButtonState();
            }
        }
        else
        {
            //Debug.Log("Reset all combat stats");
            abilityController.laserRange = abilityController.maxLaserRange;
            abilityController.jumpRange = resourceAndUpgradeManager.CurrentMaxJumpRange;
            abilityController.currentRocketReloadAmount = abilityController.rocketReloadTime;
            abilityController.currentShieldBoostCharge = abilityController.shieldBoostRechargeTime;
            uiController.SetLaserCharge(abilityController.laserRange, abilityController.maxLaserRange);
            uiController.SetJumpCharge(abilityController.jumpRange, resourceAndUpgradeManager.CurrentMaxJumpRange);
            uiController.SetRocketReloadState(abilityController.currentRocketReloadAmount, abilityController.rocketReloadTime);
            uiController.SetShieldBoostRechargeState(abilityController.currentShieldBoostCharge, abilityController.shieldBoostRechargeTime);
            movementController.hasMoved = false;
            abilityController.abilityUsed = false;
            playerHealthControl.RestoreShields();
            mapManager.ClearHighlighting();
            if (mapManager.saveName == "TutorialFile")
            {
                tutorialManager.PlayerWon();
            }
        }

        uiController.SetEndTurnButtonState();
    }

    public void EndPlayerTurnButton()
    {
        abilityController.abilityUsed = true;
        movementController.hasMoved = true;
        StartCoroutine(UpdateTurn());
        //Debug.Log("TM 102");
    }

    public IEnumerator OrderEnemyTurns()
    {
        //yield return new WaitForSeconds(.5f);
        GameObject[] enemyGameObjects;
        enemyGameObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemyGameObjects)
        {
            enemy.GetComponent<EnemyShipControl>().TakeTurn();
            yield return new WaitForSeconds(.005f);
        }
        uiController.SetEndTurnButtonState();
        StartCoroutine(UpdateTurn());
    }
}
