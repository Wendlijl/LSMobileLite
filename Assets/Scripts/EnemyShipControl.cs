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
    private bool highlightEnabled;
    public GameObject enemyLaser;

    private bool shotIncoming; //Boolean to track if the laser animation is running
    private bool inFlats;
    private bool runAway;
    private bool checkShotRunning;

    public string thisEnemyName;
    private GameObject player; //Variable to hold an instance of the player game object

    private GameObject gameController;
    private GridLayout gridLayout; //Variable to hold an instance of the grid layout
    private ManageMap mapManager; //Variable to hold an instance of the map manager
    private ClickManager clickManager;
    private PlayerHealthControl playerHealthControl;
    private AbilityController abilityController;
    private ResourceAndUpgradeManager resourceAndUpgradeManager;
    private TutorialManager tutorialManager;
    private MovementController movementController;

    private int jumpChargeTracker;

    public bool CheckShotRunning { get { return checkShotRunning; } set { checkShotRunning = value; } }
    public bool HighlightEnabled { get { return highlightEnabled; } set { highlightEnabled = value; } }

    // Start is called before the first frame update
    void Awake()
    {
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>(); //Access and store the GridLayout component
        enemyCellPosition = gridLayout.WorldToCell(transform.position); //Get this enemy's cell position and convert it to the nearest hex coordinates. This is the first half of an operation to center this object in it's position in the hex grid 
        transform.position = gridLayout.CellToWorld(enemyCellPosition); //Take the hex grid position from the last operation, convert it back to world coordinates and set this object's position to those coordinates
        player = GameObject.FindGameObjectWithTag("Player"); //Access and store the player game object
        movementController = player.GetComponent<MovementController>();
        gameController = GameObject.Find("GameController");
        abilityController = player.GetComponent<AbilityController>(); //Access and store the ability controller
        highlightEnabled = false;
        mapManager = gameController.GetComponent<ManageMap>(); //Access and store a reference to the map manager script
        clickManager = gameController.GetComponent<ClickManager>(); //Access and store a reference to the click manager script
        resourceAndUpgradeManager = gameController.GetComponent<ResourceAndUpgradeManager>();
        tutorialManager = gameController.GetComponent<TutorialManager>();
        playerHealthControl = player.GetComponent<PlayerHealthControl>();
        shotIncoming = false; //Set the initial state of the laser animation
        string clone = "(Clone)";
        thisEnemyName = gameObject.name;
        thisEnemyName = thisEnemyName.Replace(clone, "");
        inFlats = false;
        //thisEnemyObject = new EnemyObject(enemyCellPosition.x, enemyCellPosition.y, thisEnemyName);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Rocket")
        {
            Debug.Log("This enemy told him to blow up!");
            collision.gameObject.GetComponent<RocketController>().Detonate(transform.position);
        }
    }

    public void CheckDisplayRange(Vector3Int playerTarget)
    {
        if (enemyCellPosition == playerTarget && !player.GetComponent<AbilityController>().laserState && !shotIncoming && !abilityController.abilityActive)
        {
            mapManager.ShowFlats(thisEnemyName, enemyCellPosition, gameObject);
        }
    }

    public IEnumerator CheckShot(Vector3Int playerTarget)
    {
        CheckShotRunning = true;
        if (enemyCellPosition == playerTarget)
        {
            //Debug.Log("Enemy destroyed");
            shotIncoming = true; 
            yield return new WaitForSeconds(0.3f);
            DestroySelf(true);
            shotIncoming = false;
        }
        CheckShotRunning = false;
        yield return null;
    }

    public void DestroySelf(bool makeExplosion)
    {
        if (mapManager.saveName == "TutorialFile")
        {
            tutorialManager.ExplainCombatMovement();
        }
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
                    List<Vector3Int> playerFlats = mapManager.GetFlats(3, movementController.playerCellPosition, false);
                    List<Vector3Int> playerEndFlats = mapManager.GetFlats(3, movementController.playerCellPosition, true);
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
                        List<Vector3Int> neighbours = mapManager.GetNeighbours(enemyCellPosition,1);
                        Vector3Int shortestMove = new Vector3Int(0, 0, 0);
                        int shortestMoveDist = 100;
                        Vector3Int furthestFromRockets = new Vector3Int(0, 0, 0);
                        int sumFurthestFromRocketsDist = 0;
                        int i = 1;
                        foreach (Vector3Int neighbour in neighbours)
                        {
                            if (i == 1)
                            {
                                shortestMove = neighbour;
                                shortestMoveDist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), mapManager.evenq2cube(nearestEndFlat));

                                if (rockets.Length > 0)
                                {
                                    furthestFromRockets = neighbour;
                                    sumFurthestFromRocketsDist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), mapManager.evenq2cube(gridLayout.WorldToCell(rockets[0].transform.position)));
                                }
                            }
                            else
                            {
                                if (mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), mapManager.evenq2cube(nearestEndFlat)) < shortestMoveDist)
                                {
                                    shortestMove = neighbour;
                                    shortestMoveDist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), mapManager.evenq2cube(nearestEndFlat));
                                }

                                int thisSumFurthestFromRocketsDist = 0;
                                foreach (GameObject rocket in rockets)
                                {
                                    thisSumFurthestFromRocketsDist += mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), mapManager.evenq2cube(gridLayout.WorldToCell(rocket.transform.position)));
                                }
                                if (thisSumFurthestFromRocketsDist> sumFurthestFromRocketsDist)
                                {
                                    furthestFromRockets = neighbour;
                                    sumFurthestFromRocketsDist = thisSumFurthestFromRocketsDist;
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
                            SetOrientation(gridLayout.CellToWorld(furthestFromRockets));
                            transform.position += (gridLayout.CellToWorld(furthestFromRockets) - gridLayout.CellToWorld(enemyCellPosition));
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
                        List<Vector3Int> neighbours = mapManager.GetNeighbours(enemyCellPosition,1);
                        Vector3Int shortestMove = new Vector3Int(0, 0, 0);
                        int shortestMoveDist = 100;
                        Vector3Int furthestFromRockets = new Vector3Int(0, 0, 0);
                        int sumFurthestFromRocketsDist = 0;
                        int i = 1;
                        foreach (Vector3Int neighbour in neighbours)
                        {
                            if (i == 1)
                            {
                                shortestMove = neighbour;
                                shortestMoveDist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), movementController.playerCellPositionCubeCoords);

                                if (rockets.Length > 0)
                                {
                                    furthestFromRockets = neighbour;
                                    sumFurthestFromRocketsDist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), mapManager.evenq2cube(gridLayout.WorldToCell(rockets[0].transform.position)));
                                }
                            }
                            else
                            {
                                if (mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), movementController.playerCellPositionCubeCoords) < shortestMoveDist)
                                {
                                    shortestMove = neighbour;
                                    shortestMoveDist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), movementController.playerCellPositionCubeCoords);
                                }

                                int thisSumFurthestFromRocketsDist = 0;
                                foreach (GameObject rocket in rockets)
                                {
                                    thisSumFurthestFromRocketsDist += mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), mapManager.evenq2cube(gridLayout.WorldToCell(rocket.transform.position)));
                                }
                                if (thisSumFurthestFromRocketsDist > sumFurthestFromRocketsDist)
                                {
                                    furthestFromRockets = neighbour;
                                    sumFurthestFromRocketsDist = thisSumFurthestFromRocketsDist;
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
                            SetOrientation(gridLayout.CellToWorld(furthestFromRockets));
                            transform.position += (gridLayout.CellToWorld(furthestFromRockets) - gridLayout.CellToWorld(enemyCellPosition));
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
                case "EnemyC":
                    if (distToPlayer > 2||runAway)
                    {
                        List<Vector3Int> neighbours = mapManager.GetNeighbours(enemyCellPosition,1);
                        Vector3Int shortestMove = new Vector3Int(0, 0, 0);
                        int shortestMoveDist = 100;
                        Vector3Int longestJump = new Vector3Int(0, 0, 0);
                        int longestJumpDist = 0;
                        Vector3Int furthestFromRockets = new Vector3Int(0, 0, 0);
                        int sumFurthestFromRocketsDist = 0;
                        int i = 1;
                        int randJump = Random.Range(0, 2);
                        if (jumpChargeTracker < 1 && randJump>0.1)
                        {
                            jumpChargeTracker = 1;
                            List<Vector3Int> playerNeighbours = mapManager.GetNeighbours(movementController.playerCellPosition, 2);

                            foreach(Vector3Int playerNeighbour in playerNeighbours)
                            {
                                if(mapManager.HexCellDistance(mapManager.evenq2cube(playerNeighbour), mapManager.evenq2cube(enemyCellPosition)) > longestJumpDist)
                                {
                                    longestJump = playerNeighbour;
                                    longestJumpDist = mapManager.HexCellDistance(mapManager.evenq2cube(playerNeighbour), mapManager.evenq2cube(enemyCellPosition));
                                }
                            }
                            foreach (EnemyObject listEnemy in mapManager.spawnedEnemies)
                            {
                                if (listEnemy.xCoordinate == thisEnemyObject.xCoordinate && listEnemy.yCoordinate == thisEnemyObject.yCoordinate)
                                {
                                    mapManager.spawnedEnemies.Remove(listEnemy);
                                    break;
                                }
                            }
                            SetOrientation(gridLayout.CellToWorld(longestJump));
                            transform.position += (gridLayout.CellToWorld(longestJump) - gridLayout.CellToWorld(enemyCellPosition));
                            enemyCellPosition = gridLayout.WorldToCell(transform.position);
                            thisEnemyObject = new EnemyObject(enemyCellPosition.x, enemyCellPosition.y, thisEnemyObject.enemyString);
                            mapManager.spawnedEnemies.Add(thisEnemyObject);

                        }
                        else
                        {
                            foreach (Vector3Int neighbour in neighbours)
                            {
                                if (i == 1)
                                {
                                    shortestMove = neighbour;
                                    shortestMoveDist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), movementController.playerCellPositionCubeCoords);

                                    if (rockets.Length > 0)
                                    {
                                        furthestFromRockets = neighbour;
                                        sumFurthestFromRocketsDist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), mapManager.evenq2cube(gridLayout.WorldToCell(rockets[0].transform.position)));
                                    }
                                }
                                else
                                {
                                    if (mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), movementController.playerCellPositionCubeCoords) < shortestMoveDist)
                                    {
                                        shortestMove = neighbour;
                                        shortestMoveDist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), movementController.playerCellPositionCubeCoords);
                                    }

                                    int thisSumFurthestFromRocketsDist = 0;
                                    foreach (GameObject rocket in rockets)
                                    {
                                        thisSumFurthestFromRocketsDist += mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), mapManager.evenq2cube(gridLayout.WorldToCell(rocket.transform.position)));
                                    }
                                    if (thisSumFurthestFromRocketsDist > sumFurthestFromRocketsDist)
                                    {
                                        furthestFromRockets = neighbour;
                                        sumFurthestFromRocketsDist = thisSumFurthestFromRocketsDist;
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
                                SetOrientation(gridLayout.CellToWorld(furthestFromRockets));
                                transform.position += (gridLayout.CellToWorld(furthestFromRockets) - gridLayout.CellToWorld(enemyCellPosition));
                            }
                            else
                            {
                                SetOrientation(gridLayout.CellToWorld(shortestMove));
                                transform.position += (gridLayout.CellToWorld(shortestMove) - gridLayout.CellToWorld(enemyCellPosition));
                            }
                            enemyCellPosition = gridLayout.WorldToCell(transform.position);
                            thisEnemyObject = new EnemyObject(enemyCellPosition.x, enemyCellPosition.y, thisEnemyObject.enemyString);
                            mapManager.spawnedEnemies.Add(thisEnemyObject);

                            if (jumpChargeTracker >= 1)
                            {
                                jumpChargeTracker--;
                            }
                        }

                    }
                    else
                    {
                        Instantiate(enemyLaser, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                        if (jumpChargeTracker >= 1)
                        {
                            jumpChargeTracker--;
                        }
                    }

                    break;

                default:
                    break;
            }


        }
    }

    private void SetOrientation(Vector3 target)
    {

        transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(target.x, target.y, 0) - transform.position); ; //Uses quaternion math to determine what rotation is necessary to point at the target then rotates the ship to correct orientation

    }


}
