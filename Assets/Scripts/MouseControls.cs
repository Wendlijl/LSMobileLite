using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControls : MonoBehaviour
{
    //This script was designed to control all of the mouse controls for the game. However, it is currently depricated and it's functionality has been moved to the MovementController script
    public Transform playerTransform; //variable to hold the player game object transform

    private float clickDistance;//variable to hold the calculated distance between the player character and the clicked cell
    private GridLayout gridLayout; //variable to hold an instance of the grid layout
    private Vector3Int clickCellPosition; //variable to hold the cell position of the clicked hex
    private MovementController movementController;//variable to hold an instance of the movementController
    private ManageMap mapManager; //variable to hold an instance of the map manager
   


    void Start()
    {
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>(); //On game start, center the player inside the grid point they are within
        movementController = GameObject.Find("Player").GetComponent<MovementController>(); //On game start, center the player inside the grid point they are within
        mapManager = GameObject.Find("GameController").GetComponent<ManageMap>();
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            clickCellPosition = gridLayout.WorldToCell(ray.origin);
            clickDistance = Vector3.Distance(gridLayout.CellToWorld(clickCellPosition), gridLayout.CellToWorld(movementController.playerCellPosition));
            //Debug.Log("Click distance: " + clickDistance);
            Debug.Log("Click position: " + clickCellPosition);

            //Debug.Log(Vector3.Distance(gridLayout.CellToWorld(clickCellPosition), gridLayout.CellToWorld(movementController.playerCellPosition)));
            if (clickDistance < 0.35f)
            {
                //Debug.Log("Pre Player Transform: "+playerTransform.position);
                playerTransform.position += (gridLayout.CellToWorld(clickCellPosition)- gridLayout.CellToWorld(movementController.playerCellPosition));
                //Debug.Log("Post Player Transform: " + playerTransform.position);
                //Debug.Log("Delta Click Cell Position: " + (gridLayout.CellToWorld(clickCellPosition) - gridLayout.CellToWorld(movementController.playerCellPosition)));
                movementController.playerCellPosition = gridLayout.WorldToCell(playerTransform.position);
                playerTransform.position = gridLayout.CellToWorld(movementController.playerCellPosition);
                //Debug.Log("Adj Player Transform: " + playerTransform.position);
                mapManager.UpdateFogOfWar(movementController.Vision,movementController.playerCellPosition);
                //Debug.Log("Player Cell Position: " + gridLayout.CellToWorld(movementController.playerCellPosition));
            }
        }
    }
}
