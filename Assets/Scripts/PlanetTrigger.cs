using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlanetTrigger : MonoBehaviour
{
    //This script is designed to control the contextual prompt associated with interacting with planets on the hex grid 
    private UIControl gameController; //variable to store a reference to the UIControl script 
    //private int loadingIndex; //variable to set what scense should be loaded when landing on a planet
    private void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<UIControl>(); //get a reference to the UIControl script
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameController.ActivateLandOnPlanet(); //When over a planet, display the "Land" contextutal prompt
        switch (collision.name) //Determine what planet scene to load based on the name of the game object returned from the collision
        {
            case "Planet1":
                Debug.Log("Planet1");
                //loadingIndex = 2;
                break;
            case "Planet2":
                Debug.Log("Planet2");
                //loadingIndex = 3;
                break;
            case "Planet3":
                Debug.Log("Planet3");
                //loadingIndex = 4;
                break;
            case "Planet4":
                Debug.Log("Planet4");
                //loadingIndex = 4;
                break;
            case "Planet5":
                Debug.Log("Planet5");
                //loadingIndex = 4;
                break;
            case "Planet6":
                Debug.Log("Planet6");
                //loadingIndex = 4;
                break;
            case "Planet7":
                Debug.Log("Planet7");
                //loadingIndex = 4;
                break;
            case "Planet8":
                Debug.Log("Planet8");
                //loadingIndex = 4;
                break;
            case "Planet9":
                Debug.Log("Planet9");
                //loadingIndex = 4;
                break;
            case "Planet10":
                Debug.Log("Planet10");
                //loadingIndex = 4;
                break;
            case "Planet0":
                Debug.Log("Planet0");
                //loadingIndex = 4;
                break;
        }
        //gameController.levelIndex = loadingIndex; //load the correct scene based on the index of the given planet
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        gameController.DeactivateLandOnPlanet(); //when no longer over a planet object, turn off the contextual prompt
    }
}
