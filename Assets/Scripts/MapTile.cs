using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapTile
{
    //This is a custom class created to store information about the map tiles when saving or building the map
    public int xCoordinate; //variable to hold the x coordinate of the tile
    public int yCoordinate; //variable to hold the y coordinate of the tile
    public string tileString; //variable to hold the keyword for the tile

    [SerializeField] //I'm not actually sure what this does. Look it up.
    public MapTile(int newX, int newY, string newTileString) //This method assigns the given inputs to the variable that were created to hold them
    {
        xCoordinate = newX;
        yCoordinate = newY;
        tileString = newTileString;
    }


    //Below is an earlier iteration of this script that did not work


    //private int _xCoordinate;
    //private int _yCoordinate;

    //public int XCoordinate
    //{
    //    get { return _xCoordinate; }
    //    set
    //    {
    //        _xCoordinate = value;
    //    }
    //}
    //public int YCoordinate
    //{
    //    get { return _yCoordinate; }
    //    set
    //    {
    //        _yCoordinate = value;
    //    }
    //}

}
