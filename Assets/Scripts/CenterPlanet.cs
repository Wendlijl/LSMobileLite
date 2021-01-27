using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is designed to take the planet object and move it to the exact center of the nearest hex. This ensures that planets always fit within the grid
public class CenterPlanet : MonoBehaviour
{
    GridLayout gridLayout; //Create a variable to hold an instance of the grid layout
    Vector3Int cellPosition; //Create a variable to store the cellPosition of the object
    void Awake()
    {
        //Awake should run before anything else in the game
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>(); //Get and store reference to the grid object
        cellPosition = gridLayout.WorldToCell(transform.position); //Get the position of this object and convert it to the coordinates of the nearest hex
        transform.position = gridLayout.CellToWorld(cellPosition); //Take the coordinates of the nearest cell, convert them back to world coordinates and assign that position to this object. 
    }
}
