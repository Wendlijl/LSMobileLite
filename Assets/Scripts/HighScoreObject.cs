using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreObject: IComparable<HighScoreObject>
{
    //This is a custom class created to store information about the map tiles when saving or building the map
    public string scoreString; //variable to hold the keyword for the tile
    public int scoreValue;

    [SerializeField] //I'm not actually sure what this does. Look it up.
    public HighScoreObject(string newScoreString, int newScoreValue) //This method assigns the given inputs to the variable that were created to hold them
    {
        scoreString = newScoreString;
        scoreValue = newScoreValue;
    }

    // Default comparer for Part type.
    public int CompareTo(HighScoreObject compareScore)
    {
        // A null value means that this object is greater.
        if (compareScore == null)
            return 1;

        else
            return this.scoreValue.CompareTo(compareScore.scoreValue);
    }
}
