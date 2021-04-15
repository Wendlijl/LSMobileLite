using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlanetTrigger : MonoBehaviour
{
    //This script is designed to control the contextual prompt associated with interacting with planets on the hex grid 
    public bool planetState;
    public int totalResourceCount;
    private int planetResourceAmount;
    private GameObject gameController;
    private UIControl uiController; //variable to store a reference to the UIControl script 
    private TurnManager turnManager; //variable to store a reference to the TurnManager script 
    private ManageMap mapManager; //variable to store a reference to the ManageMap script 
    private string planetName;
    private ResourceAndUpgradeManager resourceAndUpgradeManager;
    
    
    //private int loadingIndex; //variable to set what scense should be loaded when landing on a planet
    private void Start()
    {
        totalResourceCount = 0;
        planetResourceAmount = 0;
        planetState = true;
        gameController = GameObject.Find("GameController");
        uiController = gameController.GetComponent<UIControl>(); //get a reference to the UIControl script
        turnManager = gameController.GetComponent<TurnManager>(); //get a reference to the UIControl script
        mapManager = gameController.GetComponent<ManageMap>();
        resourceAndUpgradeManager = gameController.GetComponent<ResourceAndUpgradeManager>();
        //uiController.SetResourceCount(totalResourceCount, false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!turnManager.combatActive && planetState && collision.gameObject.tag=="Planet")
        {
            uiController.ActivateLandOnPlanet(); //When over a planet, display the "Land" contextutal prompt
            switch (collision.name) //Determine what planet scene to load based on the name of the game object returned from the collision
            {
                case "Planet1(Clone)":
                    Debug.Log("Planet1");
                    planetName = "Planet1";
                    planetResourceAmount = 100;
                    //loadingIndex = 2;
                    break;
                case "Planet2(Clone)":
                    Debug.Log("Planet2");
                    planetName = "Planet2";
                    planetResourceAmount = 200;
                    //loadingIndex = 3;
                    break;
                case "Planet3(Clone)":
                    Debug.Log("Planet3");
                    planetName = "Planet3";
                    planetResourceAmount = 300;
                    //loadingIndex = 4;
                    break;
                case "Planet4(Clone)":
                    Debug.Log("Planet4");
                    planetName = "Planet4";
                    planetResourceAmount = 400;
                    //loadingIndex = 4;
                    break;
                case "Planet5(Clone)":
                    Debug.Log("Planet5");
                    planetName = "Planet5";
                    planetResourceAmount = 500;
                    //loadingIndex = 4;
                    break;
                case "Planet6(Clone)":
                    Debug.Log("Planet6");
                    planetName = "Planet6";
                    planetResourceAmount = 600;
                    //loadingIndex = 4;
                    break;
                case "Planet7(Clone)":
                    Debug.Log("Planet7");
                    planetName = "Planet7";
                    planetResourceAmount = 700;
                    //loadingIndex = 4;
                    break;
                case "Planet8(Clone)":
                    Debug.Log("Planet8");
                    planetName = "Planet8";
                    planetResourceAmount = 800;
                    //loadingIndex = 4;
                    break;
                case "Planet9(Clone)":
                    Debug.Log("Planet9");
                    planetName = "Planet9";
                    planetResourceAmount = 900;
                    //loadingIndex = 4;
                    break;
                case "Planet10(Clone)":
                    Debug.Log("Planet10");
                    planetName = "Planet10";
                    planetResourceAmount = 1000;
                    //loadingIndex = 4;
                    break;
                case "Planet0(Clone)":
                    Debug.Log("Planet0");
                    planetName = "Planet0";
                    planetResourceAmount = 1100;
                    //loadingIndex = 4;
                    break;
            }
        }
        //gameController.levelIndex = loadingIndex; //load the correct scene based on the index of the given planet
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        uiController.DeactivateLandOnPlanet(); //when no longer over a planet object, turn off the contextual prompt
    }

    public void landButton()
    {
        Debug.Log(planetName);
        mapManager.GenericSpawnEnemies();
        resourceAndUpgradeManager.ModifyResources(planetResourceAmount, true);
    }
}
