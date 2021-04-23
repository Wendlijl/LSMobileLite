using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//This script is designed to facilitate the behaviour of the enemy ships
public class EnemyShipControl : MonoBehaviour
{

    public Vector3Int enemyCellPosition; //Variable to store the cellPosition of this enemy
    public GameObject explosion; //Variable to hold an instance of the explosion animation
    public EnemyObject thisEnemyObject; //This variable gets set initially from ManageMap when this object is created
    public bool highlightEnabled;
    public GameObject enemyLaser;

    private bool laserState; //Boolean to represent whether the laser ability is active
    private bool shotIncoming; //Boolean to track if the laser animation is running
    private bool inRagne; //Boolean to track if this enemy is currently in range of the player 
    private bool inFlats;
    private bool runAway;

    private float timer; //A timer for tracking the life of the laser shot
    public string thisEnemyName;
    private GameObject player; //Variable to hold an instance of the player game object
    private GameObject gameController;
    private GridLayout gridLayout; //Variable to hold an instance of the grid layout
    private ManageMap mapManager; //Variable to hold an instance of the map manager
    private ClickManager clickManager;
    private PlayerHealthControl playerHealthControl;
    private AbilityController abilityController;
    private ResourceAndUpgradeManager resourceAndUpgradeManager;

    // Start is called before the first frame update
    void Awake()
    {
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>(); //Access and store the GridLayout component
        enemyCellPosition = gridLayout.WorldToCell(transform.position); //Get this enemy's cell position and convert it to the nearest hex coordinates. This is the first half of an operation to center this object in it's position in the hex grid 
        transform.position = gridLayout.CellToWorld(enemyCellPosition); //Take the hex grid position from the last operation, convert it back to world coordinates and set this object's position to those coordinates
        player = GameObject.FindGameObjectWithTag("Player"); //Access and store the player game object
        gameController = GameObject.Find("GameController");
        abilityController = player.GetComponent<AbilityController>(); //Access and store the ability controller
        laserState = player.GetComponent<AbilityController>().laserState; //Access and store the initial state of the laser ability
        highlightEnabled = false;
        mapManager = gameController.GetComponent<ManageMap>(); //Access and store a reference to the map manager script
        clickManager = gameController.GetComponent<ClickManager>(); //Access and store a reference to the click manager script
        resourceAndUpgradeManager = gameController.GetComponent<ResourceAndUpgradeManager>();
        playerHealthControl = player.GetComponent<PlayerHealthControl>();
        inRagne = false; //Set the initial state of the Boolean tracking range to the player
        shotIncoming = false; //Set the initial state of the laser animation
        timer = 0; //Set the initial value for the timer tracking the laser lifespan
        string clone = "(Clone)";
        thisEnemyName = gameObject.name;
        thisEnemyName = thisEnemyName.Replace(clone, "");
        inFlats = false;
        //thisEnemyObject = new EnemyObject(enemyCellPosition.x, enemyCellPosition.y, thisEnemyName);
    }

    // Update is called once per frame
    void Update()
    {
        //The following Operation is to determine if this enemy should be destroyed
        laserState = player.GetComponent<AbilityController>().laserState; //On each frame, set the state of the laser ability
        if (clickManager.mouseClicked || shotIncoming) //This operation is initiated by the player clicking the fire button. If the previous loop determined that a laser shot would hit this enemy, then the loop is held open throughout the entire laser animation using the shotIncoming Boolean
        {
            if (enemyCellPosition == player.GetComponent<AbilityController>().target || shotIncoming) //This checks if the cell clicked by the player contains this enemy
            {
                
                if (!laserState && !shotIncoming && !abilityController.abilityActive)
                {
                    mapManager.ShowFlats(thisEnemyName, enemyCellPosition, gameObject);
                }
                else if (laserState || shotIncoming) //This checks if the laser ability is active when the player clicks the mouse. shotIncoming holds the loop open if an incoming laser is going to hit this enemy
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
                        if (timer > 0.3)
                        {
                            //Once the timer has reached the determined length of the laser lifespan, create an instance of the explosion animation, destroy this game object, and set the shotIncoming and inRange values to false.
                            DestroySelf(true);
                            shotIncoming = false;
                            inRagne = false;
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Rocket")
        {
            collision.gameObject.GetComponent<RocketController>().Detonate(transform.position);
        }
    }

    public void DestroySelf(bool makeExplosion)
    {
        //this method just allows the enemy to be destroyed from another game
        if (makeExplosion)
        {
            Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
        }
        //The following foreach loop is designed to remove dead enemies from the list of living enemies that gets written to the save file. This prevents killed enemies from spawning again when the game is loaded.
        foreach (EnemyObject listEnemy in mapManager.spawnedEnemies)
        {
            if (listEnemy.xCoordinate == thisEnemyObject.xCoordinate && listEnemy.yCoordinate == thisEnemyObject.yCoordinate)
            {
                mapManager.spawnedEnemies.Remove(listEnemy);
                break;
            }
        }
        Destroy(gameObject);
    }

    public void TakeTurn()
    {
        if (!shotIncoming)
        {
            int distToPlayer = mapManager.HexCellDistance(mapManager.evenq2cube(enemyCellPosition), mapManager.evenq2cube(player.gameObject.GetComponent<MovementController>().playerCellPosition));
            GameObject[] rockets = GameObject.FindGameObjectsWithTag("Rocket");
            foreach(GameObject rocket in rockets)
            {
                int distToRocket = mapManager.HexCellDistance(mapManager.evenq2cube(enemyCellPosition), mapManager.evenq2cube(gridLayout.WorldToCell(rocket.transform.position)));
                Debug.Log(thisEnemyName +" is "+ distToRocket+" hexes from a rocket");
                if (distToRocket < resourceAndUpgradeManager.CurrentMaxRocketYield+2)
                {
                    Debug.Log(thisEnemyName + " needs to get out of here!");
                    runAway = true;
                }
            }

            if (rockets.Length <= 0)
            {
                runAway = false;
            }


            switch (thisEnemyName)
            {
                case "EnemyA":
                    List<Vector3Int> playerFlats = mapManager.GetFlats(3, player.gameObject.GetComponent<MovementController>().playerCellPosition, false);
                    List<Vector3Int> playerEndFlats = mapManager.GetFlats(3, player.gameObject.GetComponent<MovementController>().playerCellPosition, true);
                    Vector3Int nearestEndFlat = playerEndFlats[0];
                    int distToNearestEndFlat = 999999;
                    foreach (Vector3Int flat in playerEndFlats)
                    {
                        int distToFlat = mapManager.HexCellDistance(mapManager.evenq2cube(enemyCellPosition), mapManager.evenq2cube(flat));

                        if (distToFlat < distToNearestEndFlat)
                        {
                            distToNearestEndFlat = distToFlat;
                            nearestEndFlat = flat;
                        }
                    }

                    Vector3Int nearestFlat = playerFlats[0];
                    int distToNearestFlat = 999999;
                    foreach (Vector3Int flat in playerFlats)
                    {
                        int distToFlat = mapManager.HexCellDistance(mapManager.evenq2cube(enemyCellPosition), mapManager.evenq2cube(flat));
                        if (distToFlat < distToNearestFlat)
                        {
                            distToNearestFlat = distToFlat;
                            nearestFlat = flat;
                        }
                        if (flat == enemyCellPosition)
                        {
                            inFlats = true;
                            break;
                        }
                        else
                        {
                            inFlats = false;
                        }
                    }
                    if (enemyCellPosition != nearestEndFlat && !inFlats ||runAway)
                    {
                        if (distToPlayer < 3)
                        {
                            nearestEndFlat = nearestFlat;
                        }
                        List<Vector3Int> neighbours = GetNeighbours(enemyCellPosition);
                        Vector3Int shortestMove = new Vector3Int(0, 0, 0);
                        int shortestMoveDist = 100;
                        Vector3Int furthestFromRocket = new Vector3Int(0, 0, 0);
                        int furthestFromRocketdist = 100;
                        int i = 1;
                        foreach (Vector3Int neighbour in neighbours)
                        {
                            if (i == 1)
                            {
                                shortestMove = neighbour;
                                shortestMoveDist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), mapManager.evenq2cube(nearestEndFlat));

                                if (rockets.Length > 0)
                                {
                                    furthestFromRocket = neighbour;
                                    furthestFromRocketdist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), mapManager.evenq2cube(gridLayout.WorldToCell(rockets[0].transform.position)));
                                }
                            }
                            else
                            {
                                if (mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), mapManager.evenq2cube(nearestEndFlat)) < shortestMoveDist)
                                {
                                    shortestMove = neighbour;
                                    shortestMoveDist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), mapManager.evenq2cube(nearestEndFlat));
                                }
                                if (rockets.Length>0 && mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), mapManager.evenq2cube(gridLayout.WorldToCell(rockets[0].transform.position)))>furthestFromRocketdist)
                                {
                                    furthestFromRocket = neighbour;
                                    furthestFromRocketdist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), mapManager.evenq2cube(gridLayout.WorldToCell(rockets[0].transform.position)));
                                }

                            }
                            i++;
                        }
                        foreach (EnemyObject listEnemy in mapManager.spawnedEnemies)
                        {
                            if (listEnemy.xCoordinate == thisEnemyObject.xCoordinate && listEnemy.yCoordinate == thisEnemyObject.yCoordinate)
                            {
                                mapManager.spawnedEnemies.Remove(listEnemy);
                                break;
                            }
                        }

                        if (runAway)
                        {
                            SetOrientation(gridLayout.CellToWorld(furthestFromRocket));
                            transform.position += (gridLayout.CellToWorld(furthestFromRocket) - gridLayout.CellToWorld(enemyCellPosition));
                        }
                        else
                        {
                            SetOrientation(gridLayout.CellToWorld(shortestMove));
                            transform.position += (gridLayout.CellToWorld(shortestMove) - gridLayout.CellToWorld(enemyCellPosition));
                        }
                        enemyCellPosition = gridLayout.WorldToCell(transform.position);
                        thisEnemyObject = new EnemyObject(enemyCellPosition.x, enemyCellPosition.y, thisEnemyObject.enemyString);
                        mapManager.spawnedEnemies.Add(thisEnemyObject);
                    }
                    else
                    {
                        Instantiate(enemyLaser, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                    }


                    break;
                case "EnemyB":
                    if (distToPlayer > 1||runAway)
                    {
                        List<Vector3Int> neighbours = GetNeighbours(enemyCellPosition);
                        Vector3Int shortestMove = new Vector3Int(0, 0, 0);
                        int shortestMoveDist = 100;
                        Vector3Int furthestFromRocket = new Vector3Int(0, 0, 0);
                        int furthestFromRocketdist = 100;
                        int i = 1;
                        foreach (Vector3Int neighbour in neighbours)
                        {
                            if (i == 1)
                            {
                                shortestMove = neighbour;
                                shortestMoveDist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), player.gameObject.GetComponent<MovementController>().playerCellPositionCubeCoords);

                                if (rockets.Length > 0)
                                {
                                    furthestFromRocket = neighbour;
                                    furthestFromRocketdist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), mapManager.evenq2cube(gridLayout.WorldToCell(rockets[0].transform.position)));
                                }
                            }
                            else
                            {
                                if (mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), player.gameObject.GetComponent<MovementController>().playerCellPositionCubeCoords) < shortestMoveDist)
                                {
                                    shortestMove = neighbour;
                                    shortestMoveDist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), player.gameObject.GetComponent<MovementController>().playerCellPositionCubeCoords);
                                }
                                if (rockets.Length > 0 && mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), mapManager.evenq2cube(gridLayout.WorldToCell(rockets[0].transform.position))) > furthestFromRocketdist)
                                {
                                    furthestFromRocket = neighbour;
                                    furthestFromRocketdist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), mapManager.evenq2cube(gridLayout.WorldToCell(rockets[0].transform.position)));
                                }
                            }
                            i++;
                        }

                        foreach (EnemyObject listEnemy in mapManager.spawnedEnemies)
                        {
                            if (listEnemy.xCoordinate == thisEnemyObject.xCoordinate && listEnemy.yCoordinate == thisEnemyObject.yCoordinate)
                            {
                                mapManager.spawnedEnemies.Remove(listEnemy);
                                break;
                            }
                        }

                        if (runAway)
                        {
                            SetOrientation(gridLayout.CellToWorld(furthestFromRocket));
                            transform.position += (gridLayout.CellToWorld(furthestFromRocket) - gridLayout.CellToWorld(enemyCellPosition));
                        }
                        else
                        {
                            SetOrientation(gridLayout.CellToWorld(shortestMove));
                            transform.position += (gridLayout.CellToWorld(shortestMove) - gridLayout.CellToWorld(enemyCellPosition));
                        }
                        enemyCellPosition = gridLayout.WorldToCell(transform.position);
                        thisEnemyObject = new EnemyObject(enemyCellPosition.x, enemyCellPosition.y, thisEnemyObject.enemyString);
                        mapManager.spawnedEnemies.Add(thisEnemyObject);
                    }
                    else
                    {
                        Instantiate(enemyLaser, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                        playerHealthControl.PlayerHit(1);
                    }

                    break;
            }


        }
    }

    public List<Vector3Int> GetNeighbours(Vector3Int origin)
    {
        List<Vector3Int> openNeighbours = new List<Vector3Int>();
        bool setSkip = false;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int modX = origin.x + x;
                int modY = origin.y + y;

                if (modX < mapManager.mapXMax && modX > mapManager.mapXMin && modY < mapManager.mapYMax && modY > mapManager.mapYMin)
                {


                    if (mapManager.HexCellDistance(mapManager.evenq2cube(origin), mapManager.evenq2cube(new Vector3Int(modX, modY, 0))) <= 1)
                    {
                        foreach (EnemyObject fellowEnemy in mapManager.spawnedEnemies)
                        {
                            if (fellowEnemy.xCoordinate == modX && fellowEnemy.yCoordinate == modY)
                            {
                                setSkip = true;
                            }
                                

                        }
                        if (!setSkip)
                        {
                            openNeighbours.Add(new Vector3Int(modX, modY, 0));
                        }
                        else
                        {
                            setSkip = false;
                        }

                    }


                }
            }
        }

        return openNeighbours;
    }

    private void SetOrientation(Vector3 target)
    {

        transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(target.x, target.y, 0) - transform.position); ; //Uses quaternion math to determine what rotation is necessary to point at the target then rotates the ship to correct orientation

    }


}
