using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class TutorialManager : MonoBehaviour
{
    private AbilityController abilityController;
    private MovementController movementController;
    private PlanetTrigger planetTrigger;
    private GameObject player;
    private GameObject starGate;
    private GameObject gameController;
    private Flowchart flowchart;
    private ManageMap mapManager;
    private UIControl uiControl;
    private GridLayout gridLayout;

    private bool explainedMining;
    private bool explainedCombat;
    private bool explainedLaser;
    private bool explainedCombatMovement;
    private bool finishedFight;
    private bool explainedUpgrades;
    private bool revealedMap;
    private bool explainedWarpGate;
    private bool readyToExplainWarpGate;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        starGate = GameObject.FindWithTag("StarGate");
        flowchart = GameObject.Find("Flowchart").GetComponent<Flowchart>();
        Debug.Log("Number of variables are: "+flowchart.Variables.Count);
        Debug.Log("The value of exploration is : "+flowchart.GetBooleanVariable("Exploration"));

        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>(); //get a reference to the grid layout
        abilityController = player.GetComponent<AbilityController>();
        movementController = player.GetComponent<MovementController>();
        planetTrigger = player.GetComponent<PlanetTrigger>();

        gameController = GameObject.Find("GameController");
        mapManager = gameController.GetComponent<ManageMap>();
        uiControl = gameController.GetComponent<UIControl>();
        explainedMining=false;
        explainedCombat=false;
    }

    private void Update()
    {
        if (mapManager.revealedTilesUnique.Count >= 20 &&!flowchart.GetBooleanVariable("Exploration"))
        {
            flowchart.SetBooleanVariable("Exploration", true);
            flowchart.ExecuteBlock("Tutorial5");
            Debug.Log("Exploration changed");
        }

        if (readyToExplainWarpGate && !explainedWarpGate) 
        {
            if (mapManager.HexCellDistance(mapManager.evenq2cube(gridLayout.WorldToCell(player.transform.position)), mapManager.evenq2cube(gridLayout.WorldToCell(starGate.transform.position))) < 2)
            {
                explainedWarpGate = true;
                flowchart.ExecuteBlock("Tutorial15");
            }
        }
    }

    public void SetMovementState()
    {
        movementController.movementState = !movementController.movementState;
    }

    public void SetWeaponsState()
    {
        abilityController.weaponState = !abilityController.weaponState;
    }

    public void SetPlanetState()
    {
        planetTrigger.planetState = !planetTrigger.planetState;
    }

    public void SetUpgradeState()
    {
        uiControl.UpgradeState = !uiControl.UpgradeState;   
    }

    public void ExplainMining()
    {
        if (!explainedMining)
        {
            explainedMining = true;
            flowchart.ExecuteBlock("Tutorial6");
        }
    }

    public void ExplainCombat()
    {
        if (!explainedCombat)
        {
            explainedCombat = true;
            flowchart.ExecuteBlock("Tutorial7");
        }
    }

    public void ExplainLaser()
    {
        if (!explainedLaser)
        {
            explainedLaser = true;
            flowchart.ExecuteBlock("Tutorial8");
        }
    }

    public void ExplainCombatMovement()
    {
        if (!explainedCombatMovement)
        {
            explainedCombatMovement = true;
            flowchart.ExecuteBlock("Tutorial9");
        }
    }

    public void PlayerLost()
    {
        if (!explainedCombatMovement&&!finishedFight)
        {
            explainedCombatMovement = true;
            finishedFight = true;
            flowchart.ExecuteBlock("Tutorial11_LostNoKills");
        }
        else if(explainedCombatMovement&&!finishedFight)
        {
            finishedFight = true;
            flowchart.ExecuteBlock("Tutorial11_LostWithKills");
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyShipControl>().DestroySelf(true);
        }
    }

    public void PlayerWon()
    {
        if (!finishedFight)
        {
            finishedFight = true;
            flowchart.ExecuteBlock("Tutorial11_Won");
        }
    }

    public void ExplainUpgrades()
    {
        if (!explainedUpgrades)
        {
            explainedUpgrades = true;
            flowchart.ExecuteBlock("Tutorial13");
        }
    }

    public void RevealMap()
    {
        if (!revealedMap)
        {
            revealedMap = true;
            readyToExplainWarpGate = true;
            flowchart.ExecuteBlock("Tutorial14");
        }
    }

    public void ExplainWarpGate()
    {
        if (!explainedWarpGate)
        {
            explainedWarpGate = true;
            flowchart.ExecuteBlock("Tutorial15");
        }
    }

}
