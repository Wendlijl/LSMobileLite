using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CI.QuickSave;

public class StarGateControl : MonoBehaviour
{
    private GameObject gameController;
    private TurnManager turnManager;
    private UIControl uiController;
    private GridLayout gridLayout;
    private ManageMap mapManager;
    private TutorialManager tutorialManager;

    private void Awake()
    {
        gameController = GameObject.Find("GameController");
        turnManager = gameController.GetComponent<TurnManager>();
        uiController = gameController.GetComponent<UIControl>();
        mapManager = gameController.GetComponent<ManageMap>();
        tutorialManager = gameController.GetComponent<TutorialManager>();

        //Awake should run before anything else in the game
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>(); //Get and store reference to the grid object

        Vector3Int cellPosition = gridLayout.WorldToCell(transform.position); //Get the position of this object and convert it to the coordinates of the nearest hex
        transform.position = gridLayout.CellToWorld(cellPosition); //Take the coordinates of the nearest cell, convert them back to world coordinates and assign that position to this object. 
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!turnManager.combatActive && collision.gameObject.tag == "Player")
        {
            if (mapManager.saveName == "TutorialFile")
            {
                if (tutorialManager.ReadyToExplainWarGate)
                {
                    uiController.DisplayStarGateMessage();
                    tutorialManager.SetMovementState();
                }
            }
            else
            {
                uiController.DisplayStarGateMessage();
                tutorialManager.SetMovementState();
            }
            
            
        }
    }
}
