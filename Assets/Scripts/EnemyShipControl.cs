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
    private bool highlightEnabled;
    private float timer; //A timer for tracking the life of the laser shot
    private string thisEnemyName;
    private GameObject player; //Variable to hold an instance of the player game object
    private GridLayout gridLayout; //Variable to hold an instance of the grid layout
    private ManageMap mapManager; //Variable to hold an instance of the map manager
    public EnemyObject thisEnemyObject; //This variable gets set initially from ManageMap when this object is created

    // Start is called before the first frame update
    void Awake()
    {
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>(); //Access and store the GridLayout component
        enemyCellPosition = gridLayout.WorldToCell(transform.position); //Get this enemy's cell position and convert it to the nearest hex coordinates. This is the first half of an operation to center this object in it's position in the hex grid 
        transform.position = gridLayout.CellToWorld(enemyCellPosition); //Take the hex grid position from the last operation, convert it back to world coordinates and set this object's position to those coordinates
        player = GameObject.FindGameObjectWithTag("Player"); //Access and store the player game object
        laserState = player.GetComponent<AbilityController>().laserState; //Access and store the initial state of the laser ability
        highlightEnabled = false;
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
            if (enemyCellPosition == player.GetComponent<AbilityController>().target || shotIncoming) //This checks if the cell clicked by the player contains this enemy
            {
                if (!laserState && !shotIncoming)
                {
                    ShowFlats();
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
                        if (timer > 0.5)
                        {
                            //Once the timer has reached the determined length of the laser lifespan, create an instance of the explosion animation, destroy this game object, and set the shotIncoming and inRange values to false.
                            Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
                            //The following foreach loop is designed to remove dead enemies from the list of living enemies that gets written to the save file. This prevents killed enemies from spawning again when the game is loaded.
                            foreach (EnemyObject listEnemy in mapManager.spawnedEnemies) {
                                if (listEnemy.xCoordinate == thisEnemyObject.xCoordinate && listEnemy.yCoordinate == thisEnemyObject.yCoordinate)
                                {
                                    mapManager.spawnedEnemies.Remove(listEnemy);
                                    break;
                                }
                            }
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
        //this method just anticipates needing to destory the enemy from another game script somewhere at some point
        Destroy(gameObject);
    }

    public void TakeTurn()
    {
        if (!shotIncoming)
        {
            //Debug.Log(enemyCellPosition);
            //Debug.Log(mapManager.evenq2cube(enemyCellPosition));
            //Debug.Log(player.gameObject.GetComponent<MovementController>().playerCellPosition);
            //Debug.Log(mapManager.evenq2cube(player.gameObject.GetComponent<MovementController>().playerCellPosition));
            //enemyCellPosition

            int distToPlayer = mapManager.HexCellDistance(mapManager.evenq2cube(enemyCellPosition), mapManager.evenq2cube(player.gameObject.GetComponent<MovementController>().playerCellPosition));


            switch (thisEnemyName)
            {
                case "EnemyA":
                    if (distToPlayer > 3)
                    {
                        //Debug.Log("EnemyA is at " + enemyCellPosition);
                        //Debug.Log("Player is at " + player.gameObject.GetComponent<MovementController>().playerCellPosition);
                        List<Vector3Int> neighbours = GetNeighbours(enemyCellPosition);
                        Vector3Int shortestMove = new Vector3Int(0, 0, 0);
                        int shortestMoveDist = 100;
                        int i = 1;
                        foreach (Vector3Int neighbour in neighbours)
                        {
                            if (i == 1)
                            {
                                shortestMove = neighbour;
                                shortestMoveDist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), player.gameObject.GetComponent<MovementController>().playerCellPositionCubeCoords);
                            }
                            else
                            {
                                if (mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), player.gameObject.GetComponent<MovementController>().playerCellPositionCubeCoords) < shortestMoveDist)
                                {
                                    shortestMove = neighbour;
                                    shortestMoveDist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), player.gameObject.GetComponent<MovementController>().playerCellPositionCubeCoords);
                                }
                            }
                            i++;
                        }
                        //Debug.Log("Shortest move is " + shortestMove + " at a distance of " + shortestMoveDist);

                        foreach (EnemyObject listEnemy in mapManager.spawnedEnemies)
                        {
                            if (listEnemy.xCoordinate == thisEnemyObject.xCoordinate && listEnemy.yCoordinate == thisEnemyObject.yCoordinate)
                            {
                                mapManager.spawnedEnemies.Remove(listEnemy);
                                break;
                            }
                        }

                        SetOrientation(gridLayout.CellToWorld(shortestMove));
                        transform.position += (gridLayout.CellToWorld(shortestMove) - gridLayout.CellToWorld(enemyCellPosition));
                        enemyCellPosition = gridLayout.WorldToCell(transform.position);
                        thisEnemyObject = new EnemyObject(enemyCellPosition.x, enemyCellPosition.y, thisEnemyObject.enemyString);
                        mapManager.spawnedEnemies.Add(thisEnemyObject);
                    }
                    else
                    {
                        //Debug.Log("EnemyA attacked");
                    }


                    break;
                case "EnemyB":
                    if (distToPlayer > 1)
                    {
                        //Debug.Log("EnemyB is at " + enemyCellPosition);
                        //Debug.Log("Player is at " + player.gameObject.GetComponent<MovementController>().playerCellPosition);
                        List<Vector3Int> neighbours = GetNeighbours(enemyCellPosition);
                        Vector3Int shortestMove = new Vector3Int(0, 0, 0);
                        int shortestMoveDist = 100;
                        int i = 1;
                        foreach (Vector3Int neighbour in neighbours)
                        {
                            if (i == 1)
                            {
                                shortestMove = neighbour;
                                shortestMoveDist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), player.gameObject.GetComponent<MovementController>().playerCellPositionCubeCoords);
                            }
                            else
                            {
                                if (mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), player.gameObject.GetComponent<MovementController>().playerCellPositionCubeCoords) < shortestMoveDist)
                                {
                                    shortestMove = neighbour;
                                    shortestMoveDist = mapManager.HexCellDistance(mapManager.evenq2cube(neighbour), player.gameObject.GetComponent<MovementController>().playerCellPositionCubeCoords);
                                }
                            }
                            i++;
                        }
                        //Debug.Log("Shortest move is " + shortestMove + " at a distance of " + shortestMoveDist);

                        foreach (EnemyObject listEnemy in mapManager.spawnedEnemies)
                        {
                            if (listEnemy.xCoordinate == thisEnemyObject.xCoordinate && listEnemy.yCoordinate == thisEnemyObject.yCoordinate)
                            {
                                mapManager.spawnedEnemies.Remove(listEnemy);
                                break;
                            }
                        }

                        SetOrientation(gridLayout.CellToWorld(shortestMove));
                        transform.position += (gridLayout.CellToWorld(shortestMove) - gridLayout.CellToWorld(enemyCellPosition));
                        enemyCellPosition = gridLayout.WorldToCell(transform.position);
                        thisEnemyObject = new EnemyObject(enemyCellPosition.x, enemyCellPosition.y, thisEnemyObject.enemyString);
                        mapManager.spawnedEnemies.Add(thisEnemyObject);
                    }
                    else
                    {
                        //Debug.Log("EnemyB attacked");
                    }

                    break;
            }


        }
    }

    public List<Vector3Int> GetNeighbours(Vector3Int origin)
    {
        List<Vector3Int> neighbours = new List<Vector3Int>();
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
                                setSkip = true;

                        }
                        if (!setSkip)
                        {
                            neighbours.Add(new Vector3Int(modX, modY, 0));
                        }
                        else
                        {
                            setSkip = false;
                        }

                    }


                }
            }
        }

        return neighbours;
    }

    private void SetOrientation(Vector3 target)
    {

        transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(target.x, target.y, 0) - transform.position); ; //Uses quaternion math to determine what rotation is necessary to point at the target then rotates the ship to correct orientation

    }

    public List<Vector3Int> GetFlats(int flatLength)
    {
        List<Vector3Int> flats = new List<Vector3Int>();
        for(int i = 0; i <= flatLength-1; i++)
        {
            int modx = 0;
            Vector3Int tempHexCalc = new Vector3Int(enemyCellPosition.x + i+1, enemyCellPosition.y, enemyCellPosition.z);
            flats.Add(tempHexCalc);
            tempHexCalc = new Vector3Int(enemyCellPosition.x - i-1, enemyCellPosition.y, enemyCellPosition.z);
            flats.Add(tempHexCalc);
            if (enemyCellPosition.y % 2 == 0)
            {
                if (i % 2 == 0)
                {
                    tempHexCalc = new Vector3Int(enemyCellPosition.x+Mathf.FloorToInt(i/2), enemyCellPosition.y-i-1, enemyCellPosition.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(enemyCellPosition.x-Mathf.FloorToInt(i/2)-1, enemyCellPosition.y-i-1, enemyCellPosition.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(enemyCellPosition.x+Mathf.FloorToInt(i/2), enemyCellPosition.y + i + 1, enemyCellPosition.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(enemyCellPosition.x-Mathf.FloorToInt(i/2)-1, enemyCellPosition.y + i + 1, enemyCellPosition.z);
                    flats.Add(tempHexCalc);
                    modx++;
                }
                else
                {
                    tempHexCalc = new Vector3Int(enemyCellPosition.x+Mathf.FloorToInt(i/2)+1, enemyCellPosition.y - i - 1, enemyCellPosition.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(enemyCellPosition.x-Mathf.FloorToInt(i/2)-1, enemyCellPosition.y - i - 1, enemyCellPosition.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(enemyCellPosition.x+Mathf.FloorToInt(i/2)+1, enemyCellPosition.y + i + 1, enemyCellPosition.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(enemyCellPosition.x-Mathf.FloorToInt(i/2)-1, enemyCellPosition.y + i + 1, enemyCellPosition.z);
                    flats.Add(tempHexCalc);
                }
                Debug.Log("Y is even");
            }
            else
            {
                if (i % 2 == 0)
                {
                    tempHexCalc = new Vector3Int(enemyCellPosition.x+Mathf.FloorToInt(i/2)+1, enemyCellPosition.y - i - 1, enemyCellPosition.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(enemyCellPosition.x-Mathf.FloorToInt(i/2), enemyCellPosition.y - i - 1, enemyCellPosition.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(enemyCellPosition.x+Mathf.FloorToInt(i/2)+1, enemyCellPosition.y + i + 1, enemyCellPosition.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(enemyCellPosition.x-Mathf.FloorToInt(i/2), enemyCellPosition.y + i + 1, enemyCellPosition.z);
                    flats.Add(tempHexCalc);
                }
                else
                {
                    tempHexCalc = new Vector3Int(enemyCellPosition.x+Mathf.FloorToInt(i/2)+1, enemyCellPosition.y - i - 1, enemyCellPosition.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(enemyCellPosition.x-Mathf.FloorToInt(i/2)-1, enemyCellPosition.y - i - 1, enemyCellPosition.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(enemyCellPosition.x+Mathf.FloorToInt(i/2)+1, enemyCellPosition.y + i + 1, enemyCellPosition.z);
                    flats.Add(tempHexCalc);
                    tempHexCalc = new Vector3Int(enemyCellPosition.x-Mathf.FloorToInt(i/2)-1, enemyCellPosition.y + i + 1, enemyCellPosition.z);
                    flats.Add(tempHexCalc);
                }
                Debug.Log("Y is odd");
            }
            
        }
        foreach(Vector3Int flat in flats)
        {
            Debug.Log(flat);
        }

        return flats;
    }
    public void ShowFlats()
    {
        List<Vector3Int> flats = new List<Vector3Int>();
        if (thisEnemyName == "EnemyA")
        {
            flats = GetFlats(8);
        }
        else if (thisEnemyName == "EnemyB")
        {
            flats = GetFlats(1);
        }

        highlightEnabled = !highlightEnabled;

        mapManager.HighlightSet(flats, highlightEnabled);
    }
}
