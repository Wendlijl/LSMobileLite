using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    private GridLayout gridLayout; //Create a variable to hold an instance of the grid layout
    private Vector3Int cellPosition; //Create a variable to store the cellPosition of the object
    private ManageMap mapManager;
    private bool resourcesCollected = false;
    public bool ResourcesCollectd { get { return resourcesCollected; }set { resourcesCollected = value; } }
    void Awake()
    {
        //Awake should run before anything else in the game
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>(); //Get and store reference to the grid object
        mapManager = GameObject.Find("GameController").GetComponent<ManageMap>();
        cellPosition = gridLayout.WorldToCell(transform.position); //Get the position of this object and convert it to the coordinates of the nearest hex
        transform.position = gridLayout.CellToWorld(cellPosition); //Take the coordinates of the nearest cell, convert them back to world coordinates and assign that position to this object. 
    }
    private void Start()
    {
        Vector3Int thisPlanetCell = gridLayout.WorldToCell(transform.position);
        foreach(PlanetObject planet in mapManager.spawnedPlanets)
        {
            if (thisPlanetCell.x == planet.xCoordinate && thisPlanetCell.y == planet.yCoordinate)
            {
                resourcesCollected = planet.resourcesCollected;
            }
        }
    }
}
