using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlanetTrigger : MonoBehaviour
{
    //This script is designed to control the contextual prompt associated with interacting with planets on the hex grid 
    private UIControl gameControllerUI; //variable to store a reference to the UIControl script 
    private ManageMap gameControllerMap; //variable to store a reference to the UIControl script 
    private string planetName;
    //private int loadingIndex; //variable to set what scense should be loaded when landing on a planet
    private void Start()
    {
        gameControllerUI = GameObject.Find("GameController").GetComponent<UIControl>(); //get a reference to the UIControl script
        gameControllerMap = GameObject.Find("GameController").GetComponent<ManageMap>(); //get a reference to the UIControl script
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameControllerMap.combatActive)
        {
            gameControllerUI.ActivateLandOnPlanet(); //When over a planet, display the "Land" contextutal prompt
            switch (collision.name) //Determine what planet scene to load based on the name of the game object returned from the collision
            {
                case "Planet1(Clone)":
                    Debug.Log("Planet1");
                    planetName = "Planet1";
                    //loadingIndex = 2;
                    break;
                case "Planet2(Clone)":
                    Debug.Log("Planet2");
                    planetName = "Planet2";
                    //loadingIndex = 3;
                    break;
                case "Planet3(Clone)":
                    Debug.Log("Planet3");
                    planetName = "Planet3";
                    //loadingIndex = 4;
                    break;
                case "Planet4(Clone)":
                    Debug.Log("Planet4");
                    planetName = "Planet4";
                    //loadingIndex = 4;
                    break;
                case "Planet5(Clone)":
                    Debug.Log("Planet5");
                    planetName = "Planet5";
                    //loadingIndex = 4;
                    break;
                case "Planet6(Clone)":
                    Debug.Log("Planet6");
                    planetName = "Planet6";
                    //loadingIndex = 4;
                    break;
                case "Planet7(Clone)":
                    Debug.Log("Planet7");
                    planetName = "Planet7";
                    //loadingIndex = 4;
                    break;
                case "Planet8(Clone)":
                    Debug.Log("Planet8");
                    planetName = "Planet8";
                    //loadingIndex = 4;
                    break;
                case "Planet9(Clone)":
                    Debug.Log("Planet9");
                    planetName = "Planet9";
                    //loadingIndex = 4;
                    break;
                case "Planet10(Clone)":
                    Debug.Log("Planet10");
                    planetName = "Planet10";
                    //loadingIndex = 4;
                    break;
                case "Planet0(Clone)":
                    Debug.Log("Planet0");
                    planetName = "Planet0";
                    //loadingIndex = 4;
                    break;
            }
        }
        //gameController.levelIndex = loadingIndex; //load the correct scene based on the index of the given planet
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        gameControllerUI.DeactivateLandOnPlanet(); //when no longer over a planet object, turn off the contextual prompt
    }

    public void landButton()
    {
        Debug.Log(planetName);
        gameControllerMap.GenericSpawnEnemies();
    }
}
