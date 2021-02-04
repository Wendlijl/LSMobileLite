using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//This script is designed to facilitate the behaviour of the enemy ships
public class EnemyShipControl : MonoBehaviour
{
    
    public Vector3Int enemyCellPosition; //Variable to store the cellPosition of this enemy
    public GameObject explosion; //Variable to hold an instance of the explosion animation

    private bool laserState; //Boolean to represent whether the laser ability is active
    private bool shotIncoming; //Boolean to track if the laser animation is running
    private bool inRagne; //Boolean to track if this enemy is currently in range of the player 
    private float timer; //A timer for tracking the life of the laser shot
    private string thisEnemyName;
    private GameObject player; //Variable to hold an instance of the player game object
    private GridLayout gridLayout; //Variable to hold an instance of the grid layout
    private ManageMap mapManager; //Variable to hold an instance of the map manager
    public EnemyObject thisEnemyObject;

    // Start is called before the first frame update
    void Awake()
    {
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>(); //Access and store the GridLayout component
        enemyCellPosition = gridLayout.WorldToCell(transform.position); //Get this enemy's cell position and convert it to the nearest hex coordinates. This is the first half of an operation to center this object in it's position in the hex grid 
        transform.position = gridLayout.CellToWorld(enemyCellPosition); //Take the hex grid position from the last operation, convert it back to world coordinates and set this object's position to those coordinates
        player = GameObject.FindGameObjectWithTag("Player"); //Access and store the player game object
        laserState = player.GetComponent<AbilityController>().laserState; //Access and store the initial state of the laser ability
        mapManager = GameObject.Find("GameController").GetComponent<ManageMap>(); //Access and store a reference to the map manager script
        inRagne = false; //Set the initial state of the Boolean tracking range to the player
        shotIncoming = false; //Set the initial state of the laser animation
        timer = 0; //Set the initial value for the timer tracking the laser lifespan
        string clone = "(Clone)";
        thisEnemyName = gameObject.name;
        thisEnemyName = thisEnemyName.Replace(clone, "");
        //thisEnemyObject = new EnemyObject(enemyCellPosition.x, enemyCellPosition.y, thisEnemyName);
    }

    // Update is called once per frame
    void Update()
    {
        //The following Operation is to determine if this enemy should be destroyed
        laserState = player.GetComponent<AbilityController>().laserState; //On each frame, set the state of the laser ability
        if (Input.GetMouseButtonDown(0) || shotIncoming) //This operation is initiated by the player clicking the fire button. If the previous loop determined that a laser shot would hit this enemy, then the loop is held open throughout the entire laser animation using the shotIncoming Boolean
        {
            if (laserState || shotIncoming) //This checks if the laser ability is active when the player clicks the mouse. shotIncoming holds the loop open if an incoming laser is going to hit this enemy
            {
                if (enemyCellPosition == player.GetComponent<AbilityController>().target || shotIncoming) //This checks if the cell clicked by the player contains this enemy
                {
                    foreach (Vector3Int highLightedCell in mapManager.currentHighlightedTiles) //This loops through the list of highlighted cells that the player sees to indicate their laser range. 
                    {
                        if (highLightedCell == player.GetComponent<AbilityController>().target) //Checks that this enemy is within the range defined by the highlighted cells shown to the player
                        {
                            inRagne = true; //Sets the inRange Boolean true 
                        }
                    }

                    if (inRagne) //If the inRange Boolean is true then set shot incoming to true and start a timer to determine when to destroy this enemy
                    {
                        shotIncoming = true;
                        timer += Time.deltaTime;
                        if (timer > 0.5)
                        {
                            //Once the timer has reached the determined length of the laser lifespan, create an instance of the explosion animation, destroy this game object, and set the shotIncoming and inRange values to false.
                            Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
                            
                            Debug.Log("Enemy Destroyed"+thisEnemyObject.xCoordinate + " " + thisEnemyObject.yCoordinate + " " + thisEnemyObject.enemyString);
                            int i= 0;
                            foreach (EnemyObject listEnemy in mapManager.spawnedEnemies) {
                            Debug.Log("List Enemy " + i + " " + listEnemy.xCoordinate + " " + listEnemy.yCoordinate + " " + listEnemy.enemyString);
                                i++;
                                if (listEnemy.xCoordinate == thisEnemyObject.xCoordinate && listEnemy.yCoordinate == thisEnemyObject.yCoordinate)
                                {
                                    mapManager.spawnedEnemies.Remove(listEnemy);
                                    break;
                                }
                            }
                            //mapManager.spawnedEnemies.Remove(thisEnemyObject);
                            Destroy(gameObject);
                            shotIncoming = false;
                            inRagne = false;
                        }
                    }
                }
            }
        }
    }

    public void DestroySelf()
    {
        //this method just anticipates needing to destory the enempy from another game script somewhere at some point
        Destroy(gameObject);
    }
}
