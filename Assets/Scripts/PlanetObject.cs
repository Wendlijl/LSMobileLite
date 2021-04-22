using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetObject
{
    //This is a custom class created to store information about the map tiles when saving or building the map
    public int xCoordinate; //variable to hold the x coordinate of the tile
    public int yCoordinate; //variable to hold the y coordinate of the tile
    public string planetString; //variable to hold the keyword for the tile
    public bool resourcesCollected;

    [SerializeField] //I'm not actually sure what this does. Look it up.
    public PlanetObject(int newX, int newY, string newPlanetString, bool newResourcesCollected) //This method assigns the given inputs to the variable that were created to hold them
    {
        xCoordinate = newX;
        yCoordinate = newY;
        planetString = newPlanetString;
        resourcesCollected = newResourcesCollected;
    }

}
