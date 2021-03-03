using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
//using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


public class MovementController : MonoBehaviour
{
    // private PlayerControls playerControls; //Create a variable to hold an instance of the player controls
    public int vision = 1; //variable to determine player view range
    public int moveRange = 1; //variable to control how far the player can move
    public float moveScale = 1; //variable to allow scaling of player movement
    public bool abilityActive; //boolean to indicate when abilities are active
    public Tilemap fogOfWar; //Get a reference to the overlay tiles
    public Tilemap starField; //Get a reference to the overlay tiles
    public Vector3Int playerCellPosition; //variable to store the cell position of the player
    public Vector3Int playerCellPositionCubeCoords; //variable to store the cellPosition of the player
    public Vector3 setAngleVector; //variable to hold the player ship directional angle


    private bool hasMoved; //variable to check for whether movement has happened yet
    private bool cantMove;
    private float sidewaysMovement; //varaible to define sideways movement of player
    private float upDownMovement; //variable to define vertical movement of player
    //private float rotTrack; //variable to track where the ship is in it's rotation (Depricated)
    private float clickDistance; //variable to determine how far away a clicked cell is 
    private GridLayout gridLayout; //variable to hold an instance of the grid layout
    private ManageMap mapManager; //variable to hold an instance of the map manager
    private Vector3 direction; //variable to define direction of player movement
    private Vector3Int clickCellPosition; //variable to hold the cell position of the clicked hex
    private Vector3Int clickCellPositionCubeCoords; //variable to hold the cell position of the clicked hex converted to cube coordinates
    private UIControl uiController;

    void Start()
    {
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>(); //search for and save a reference to the grid layout component
        playerCellPosition = gridLayout.WorldToCell(transform.position); //get a reference to the player cell position in cell coordinates
        transform.position = gridLayout.CellToWorld(playerCellPosition); //center the player game object within it's nearest cell by converting the cell coordinates to world coordinates
        mapManager = GameObject.Find("GameController").GetComponent<ManageMap>(); //get a reference to the map manager 
        mapManager.UpdateFogOfWar(vision, playerCellPosition); //run an update fog of war command from the map manager to clear the fog around the player character on scene start
        //rotTrack = 0; //set rotation tracking to 0 (this will likely be depreicated with a new rotation system
        abilityActive = false; //set the ability active flag to false
        cantMove = false;
        uiController = GameObject.Find("GameController").GetComponent<UIControl>();

    }

    void LateUpdate()
    {
        //This update loop is looking for player input and determining if movement is available to the hex indicated
        if (!abilityActive && (!mapManager.combatActive || mapManager.playerTurn)) //if the ability active flag is true then disable movement
        {
            //The next two parameters and the following if statement are for keyboard movement. Primary movement is mouse based, but keyboard is kept for an alternate control scheme. Will need to make sure that all functionality is duplicated on the keyboard
            sidewaysMovement = Input.GetAxis("Horizontal");
            upDownMovement = Input.GetAxis("Vertical");

            //Determine if the player has moved yet for a given keypress. Since up/down movement does not require any addition definition, that is used as the initiation of all motion
            if (upDownMovement == 0)
            {
                hasMoved = false;
            }
            else if (upDownMovement != 0 && !hasMoved) //If up or down input is detected and the player has not moved, then call the movement function and set hasMoved to true
            {
                hasMoved = true;
                GetMovementDirection();
            }

            //the following if statement controls the mouse movement 
            if (Input.GetMouseButtonDown(0)) //listen for mouse input from the user
            {
                cantMove = false;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //when the mouse is clicked, create a ray whose origin is at the mouse click position
                clickCellPosition = gridLayout.WorldToCell(ray.origin); //extract the mouse click position from the ray and convert it to grid space
                clickCellPositionCubeCoords = mapManager.evenq2cube(clickCellPosition); //the clicked cell coordinates converted to cube coordinates
                playerCellPositionCubeCoords = mapManager.evenq2cube(playerCellPosition);//the player cell coordinates converted to cube coordinates
                //Debug.Log(mapManager.HexCellDistance(playerCellPositionCubeCoords, clickCellPositionCubeCoords));
                //Debug.Log(clickCellPosition);
                //Calculate the distance between the player game object and the clicked cell
                clickDistance = mapManager.HexCellDistance(playerCellPositionCubeCoords, clickCellPositionCubeCoords);
                int i = 1;
                foreach(EnemyObject enemyPos in mapManager.spawnedEnemies)
                {
                    Debug.Log("Enemy " + i + " " + enemyPos.xCoordinate + " " + enemyPos.yCoordinate);
                    i++;

                    if(clickCellPosition.x == enemyPos.xCoordinate && clickCellPosition.y == enemyPos.yCoordinate)
                    {
                        cantMove = true;
                    }
                }

                if (clickCellPosition.x > mapManager.mapXMax - 2 || clickCellPosition.x < mapManager.mapXMin + 2 || clickCellPosition.y > mapManager.mapYMax - 1 || clickCellPosition.y < mapManager.mapYMin + 1)
                {
                    cantMove = true;
                }


                //if (clickDistance < 0.33f) //Each cell is 32 pixels wide, so if the click distance is 32 or less then allow the player to move
                if (clickDistance <= moveRange && !cantMove) //distance calculations in cube coordinates return distance in integer units so this can be compared directly to the value defining the movement range
                {
                    SetOrientation(gridLayout.CellToWorld(clickCellPosition)); //first, orient the ship correctly for the hex it will be moving to
                    transform.position += (gridLayout.CellToWorld(clickCellPosition) - gridLayout.CellToWorld(playerCellPosition)); //update the player game object transform position to the new coordinates from the clicked cell (look at how to do this smoothly)
                    playerCellPosition = gridLayout.WorldToCell(transform.position); //update the player cell position 
                    transform.position = gridLayout.CellToWorld(playerCellPosition); //use the updated player cell position to ensure the player game object is centered in the cell
                    //transform.position = Vector3.Lerp(transform.position,gridLayout.CellToWorld(playerCellPosition),Time.deltaTime); //use the updated player cell position to ensure the player game object is centered in the cell
                    mapManager.UpdateFogOfWar(vision, playerCellPosition); //clear the fog of was from the new position
                    if (mapManager.combatActive)
                    {
                        uiController.SetEndTurnButtonState();
                    }
                }
            }
        }





    }
    public void GetMovementDirection()
    {
        //The GetMovementDirection function determines the players intended movement based on updown and left right keys
        if (upDownMovement < 0)
        {
            if (sidewaysMovement > 0)
            {
                direction = new Vector3(0.5f, -0.5f); //Move player down right
            }
            else if (sidewaysMovement < 0)
            {
                direction = new Vector3(-0.5f, -0.5f); //Move player down left
            }
            else
            {
                direction = new Vector3(0, -1, 0); //Move player down
            }

        }
        else if (upDownMovement > 0)
        {
            if (sidewaysMovement > 0)
            {
                direction = new Vector3(0.5f, 0.5f); //Move player up right
            }
            else if (sidewaysMovement < 0)
            {
                direction = new Vector3(-0.5f, 0.5f); //Move player up left
            }
            else
            {
                direction = new Vector3(0, 1, 0); //Move player up
            }
        }
        //Once direction is determined, the player transform is moved to that location and centered in the destination cell

        transform.position += direction * moveScale;
        playerCellPosition = gridLayout.WorldToCell(transform.position);
        transform.position = gridLayout.CellToWorld(playerCellPosition);
        mapManager.UpdateFogOfWar(vision, playerCellPosition); //A call to the fog of war function is made to update what the player can see
    }

    public void SetOrientation(Vector3 target)
    {
 
        transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector3(target.x, target.y, 0) - transform.position); ; //Uses quaternion math to determine what rotation is necessary to point at the target then rotates the ship to correct orientation



        /*
         All of the below code was the original attempt to handle ship rotation. It has since been replaced by the above line.  
         
        ////Debug.Log(gridLayout.CellToWorld(clickCellPosition) - gridLayout.CellToWorld(playerCellPosition));
        setAngleVector = gridLayout.CellToWorld(clickCellPosition) - gridLayout.CellToWorld(playerCellPosition);
        ////Debug.Log(setAngleVector.x);
        ////Debug.Log(transform.rotation.z);
        ////Debug.Log(transform.rotation);
        if (setAngleVector.x < 0.01f && setAngleVector.x > -0.01f)
        {
            if (setAngleVector.y < 0.33f && setAngleVector.y > 0.31f)
            {
                //Debug.Log("Up");
                //Debug.Log(rotTrack);
                if (rotTrack < 0.01f && rotTrack > -0.01f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 0f));
                    //Debug.Log("Do Nothing");
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < 180.1f && rotTrack > 179.9f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 180f));
                    //Debug.Log("Do 180");
                    rotTrack -= 180;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < -44.9f && rotTrack > -45.1f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 45f));
                    //Debug.Log("Do +45");
                    rotTrack += 45;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < 45.1f && rotTrack > 44.9f)
                {
                    transform.Rotate(new Vector3(0f, 0f, -45f));
                    //Debug.Log("Do -45");
                    rotTrack -= 45;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < 135.1f && rotTrack > 134.9f)
                {
                    transform.Rotate(new Vector3(0f, 0f, -135f));
                    //Debug.Log("Do -135");
                    rotTrack -= 135;
                    //Debug.Log(rotTrack);
                }                
                else if (rotTrack < -134.9f && rotTrack > -135.1f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 135f));
                    //Debug.Log("Do +135");
                    rotTrack += 135;
                    //Debug.Log(rotTrack);
                }
            }
            else if (setAngleVector.y < -0.31f && setAngleVector.y > -0.33f)
            {
                //Debug.Log("Down");
                //Debug.Log(rotTrack);
                if (rotTrack < 0.01f && rotTrack > -0.01f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 180f));
                    //Debug.Log("Do 180");
                    rotTrack += 180;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < 180.1f && rotTrack > 179.9f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 0f));
                    //Debug.Log("Do Nothing");
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < -44.9f && rotTrack > -45.1f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 225f));
                    //Debug.Log("Do +225");
                    rotTrack += 225;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < 45.1f && rotTrack > 44.9f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 135f));
                    //Debug.Log("Do +135");
                    rotTrack += 135;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < 135.1f && rotTrack > 134.9f)
                {
                    transform.Rotate(new Vector3(0f, 0f, +45f));
                    //Debug.Log("Do +45");
                    rotTrack += 45;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < -134.9f && rotTrack > -135.1f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 315f));
                    //Debug.Log("Do +315");
                    rotTrack += 315;
                    //Debug.Log(rotTrack);
                }
            }
        }
        else if (setAngleVector.x < 0.25f && setAngleVector.x > 0.23f)
        {
            if (setAngleVector.y < 0.17f && setAngleVector.y > 0.15f)
            {
                //Debug.Log("Up Right");
                //Debug.Log(rotTrack);
                if (rotTrack < 0.01f && rotTrack > -0.01f)
                {
                    transform.Rotate(new Vector3(0f, 0f, -45f));
                    //Debug.Log("Do -45");
                    rotTrack -= 45;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < 180.1f && rotTrack > 179.9f)
                {
                    transform.Rotate(new Vector3(0f, 0f, -225f));
                    //Debug.Log("Do -225");
                    rotTrack -= 225;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < -44.9f && rotTrack > -45.1f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 0f));
                    //Debug.Log("Do Nothing");
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < 45.1f && rotTrack > 44.9f)
                {
                    transform.Rotate(new Vector3(0f, 0f, -90f));
                    //Debug.Log("Do -90");
                    rotTrack -= 90;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < 135.1f && rotTrack > 134.9f)
                {
                    transform.Rotate(new Vector3(0f, 0f, -180f));
                    //Debug.Log("Do -180");
                    rotTrack -= 180;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < -134.9f && rotTrack > -135.1f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 90f));
                    //Debug.Log("Do +90");
                    rotTrack += 90;
                    //Debug.Log(rotTrack);
                }
            }
            else if (setAngleVector.y < -0.15f && setAngleVector.y > -0.17f)
            {
                //Debug.Log("Down Right");
                //Debug.Log(rotTrack);
                if (rotTrack < 0.01f && rotTrack > -0.01f)
                {
                    transform.Rotate(new Vector3(0f, 0f, -135f));
                    //Debug.Log("Do -135");
                    rotTrack -= 135;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < 180.1f && rotTrack > 179.9f)
                {
                    transform.Rotate(new Vector3(0f, 0f, -315f));
                    //Debug.Log("Do -315");
                    rotTrack -= 315;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < -44.9f && rotTrack > -45.1f)
                {
                    transform.Rotate(new Vector3(0f, 0f, -90f));
                    //Debug.Log("Do -90");
                    rotTrack -= 90;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < 45.1f && rotTrack > 44.9f)
                {
                    transform.Rotate(new Vector3(0f, 0f, -180f));
                    //Debug.Log("Do -180");
                    rotTrack -= 180;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < 135.1f && rotTrack > 134.9f)
                {
                    transform.Rotate(new Vector3(0f, 0f, -270f));
                    //Debug.Log("Do -270");
                    rotTrack -= 270;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < -134.9f && rotTrack > -135.1f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 0f));
                    //Debug.Log("Do Nothing");
                    //Debug.Log(rotTrack);
                }
            }
        }
        else if (setAngleVector.x < -0.23f && setAngleVector.x > -0.25f)
        {
            if (setAngleVector.y < 0.17f && setAngleVector.y > 0.15f)
            {
                //Debug.Log("Up Left");
                //Debug.Log(rotTrack);
                if (rotTrack < 0.01f && rotTrack > -0.01f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 45f));
                    //Debug.Log("Do +45");
                    rotTrack += 45;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < 180.1f && rotTrack > 179.9f)
                {
                    transform.Rotate(new Vector3(0f, 0f, -135f));
                    //Debug.Log("Do -135");
                    rotTrack -= 135;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < -44.9f && rotTrack > -45.1f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 90f));
                    //Debug.Log("Do +90");
                    rotTrack += 90;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < 45.1f && rotTrack > 44.9f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 0f));
                    //Debug.Log("Do Nothing");
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < 135.1f && rotTrack > 134.9f)
                {
                    transform.Rotate(new Vector3(0f, 0f, -90f));
                    //Debug.Log("Do -90");
                    rotTrack -= 90;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < -134.9f && rotTrack > -135.1f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 180f));
                    //Debug.Log("Do +180");
                    rotTrack += 180;
                    //Debug.Log(rotTrack);
                }
            }
            else if (setAngleVector.y < -0.15f && setAngleVector.y > -0.17f)
            {
                //Debug.Log("Down Left");
                //Debug.Log(rotTrack);
                if (rotTrack < 0.01f && rotTrack > -0.01f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 135f));
                    //Debug.Log("Do 135");
                    rotTrack += 135;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < 180.1f && rotTrack > 179.9f)
                {
                    transform.Rotate(new Vector3(0f, 0f, -45f));
                    //Debug.Log("Do -45");
                    rotTrack -= 45;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < -44.9f && rotTrack > -45.1f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 180f));
                    //Debug.Log("Do +180");
                    rotTrack += 180;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < 45.1f && rotTrack > 44.9f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 90f));
                    //Debug.Log("Do +90");
                    rotTrack += 90;
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < 135.1f && rotTrack > 134.9f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 0f));
                    //Debug.Log("Do Nothing");
                    //Debug.Log(rotTrack);
                }
                else if (rotTrack < -134.9f && rotTrack > -135.1f)
                {
                    transform.Rotate(new Vector3(0f, 0f, 270f));
                    //Debug.Log("Do +270");
                    rotTrack += 270;
                    //Debug.Log(rotTrack);
                }
            }
        }
        ////Debug.Log(transform.rotation.z);
        ////Debug.Log(transform.rotation);
        */
    }

}
