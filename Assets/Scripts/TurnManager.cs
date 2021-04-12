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

    void Start()
    {
        gameController = GameObject.Find("GameController");
        player = GameObject.Find("Player");
        mapManager = gameController.GetComponent<ManageMap>();
        uiController = gameController.GetComponent<UIControl>();
        abilityController = player.GetComponent<AbilityController>();
        movementController = player.GetComponent<MovementController>();
        playerHealthControl = player.GetComponent<PlayerHealthControl>();

        firstTurn = false;
        combatActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (mapManager.spawnedEnemies.Count > 0 && !combatActive)
        {
            //Debug.Log("Combat is active");
            firstTurn = true;
            combatActive = true;
            uiController.DeactivateLandOnPlanet();
            UpdateTurn();
        }
        else if(mapManager.spawnedEnemies.Count <= 0 && combatActive)
        {
            //Debug.Log("Combat is inactive");
            combatActive = false;
            UpdateTurn();
            Debug.Log("TM 45");
        }
    }

    public void UpdateTurn()
    {
        
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
                if (abilityController.laserRange < abilityController.maxLaserRange)
                {
                    abilityController.laserRange++;
                }
                if (abilityController.jumpRange < abilityController.maxJumpRange)
                {
                    abilityController.jumpRange+=2;
                    if (abilityController.jumpRange > abilityController.maxJumpRange)
                    {
                        abilityController.jumpRange = abilityController.maxJumpRange;
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
                Debug.Log("TM 74");
                //
                movementController.hasMoved = false;
                abilityController.abilityUsed = false;
                OrderEnemyTurns();
                UpdateTurn();

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
            abilityController.jumpRange = abilityController.maxJumpRange;
            abilityController.currentRocketReloadAmount = abilityController.rocketReloadTime;
            abilityController.currentShieldBoostCharge = abilityController.shieldBoostRechargeTime;
            uiController.SetLaserCharge(abilityController.laserRange, abilityController.maxLaserRange);
            uiController.SetJumpCharge(abilityController.jumpRange, abilityController.maxJumpRange);
            uiController.SetRocketReloadState(abilityController.currentRocketReloadAmount, abilityController.rocketReloadTime);
            uiController.SetShieldBoostRechargeState(abilityController.currentShieldBoostCharge, abilityController.shieldBoostRechargeTime);
            movementController.hasMoved = false;
            abilityController.abilityUsed = false;
            playerHealthControl.RestoreShields();
        }

        uiController.SetEndTurnButtonState();
    }

    public void EndPlayerTurnButton()
    {
        abilityController.abilityUsed = true;
        movementController.hasMoved = true;
        UpdateTurn();
        Debug.Log("TM 102");
    }

    public void OrderEnemyTurns()
    {
        GameObject[] enemyGameObjects;
        enemyGameObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemyGameObjects)
        {
            enemy.GetComponent<EnemyShipControl>().TakeTurn();
        }
        uiController.SetEndTurnButtonState();
    }
}
