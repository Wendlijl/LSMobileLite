using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This script is designed to control the behaviour of the players abilities 
public class AbilityController : MonoBehaviour
{
    public bool laserState; //variable to track the activation state of the laser ability 
    public int laserRange = 3; //variable to set the range of the laser ability
    public int maxLaserRange; //the maximum current range of the laser
    public int jumpRange = 3;
    public int maxJumpRange;
    public bool weaponState;
    private bool abilityUsed;
    public bool jumpState;
    public bool shieldState;
    public int shieldBoostRechargeTime = 5;
    public int currentShieldBoostCharge = 5;
    public bool rocketState;
    public int rocketRange;
    public int rocketReloadTime = 5;
    public int currentRocketReloadAmount = 5;
    public bool abilityActive;
    private bool hasFired;
    public GameObject player; //variable to store a reference to the player game object
    private GameObject gameController;
    public Vector3Int target; //variable to store the position vector of the target
    public GameObject laser; //variable to store a reference to the laser game object
    public GameObject rocket;

    private float instX; //varaible to store the x position of the location where projectile abilities will be instantiated 
    private float instY; //varaible to store the y position of the location where projectile abilities will be instantiated
    private float clickDistance; //variable for storing the calculated distance between the point clicked and the player game object
    private ManageMap mapManager; //variable to store a reference to the map manager script
    private GameObject newInstance; //variable to store a reference to the instanciated object (not currently used)
    private Vector3 laserSpwanMod; //variable to store desired modifications to the instantiated position 
    private Vector3Int playerHex; //variable to store the current hex grid coordinates of the player game object
    private GridLayout gridLayout; //variable for storing a reference to the grid layout
    private UIControl uiController;
    private MovementController movementController;
    private List<Vector3Int> jumpCells;
    private ClickManager clickManager;
    private List<Vector3Int> playerFlats;
    private TurnManager turnManager;
    private PlayerHealthControl playerHealthControl;
    private ResourceAndUpgradeManager resourceAndUpgradeManager;
    private TutorialManager tutorialManager;

    private float timer;
    private bool turnOffAb;

    public bool AbilityUsed { get { return abilityUsed; } set { abilityUsed = value; } }
    public bool HasFired { get { return hasFired; } set { hasFired = value; } }

    // Start is called before the first frame update
    void Start()
    {
        weaponState = true;
        gameController = GameObject.Find("GameController");
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>(); //store a reference to the grid layout component
        mapManager = gameController.GetComponent<ManageMap>(); //store a reference to the map manager
        clickManager = gameController.GetComponent<ClickManager>();
        uiController = gameController.GetComponent<UIControl>();
        turnManager = gameController.GetComponent<TurnManager>();
        resourceAndUpgradeManager = gameController.GetComponent<ResourceAndUpgradeManager>();
        tutorialManager = gameController.GetComponent<TutorialManager>();
        player = GameObject.FindGameObjectWithTag("Player"); //store a reference to the player game object
        playerHealthControl = player.GetComponent<PlayerHealthControl>();
        movementController = player.GetComponent<MovementController>();
        maxLaserRange = resourceAndUpgradeManager.CurrentMaxLaserRange; //set the initial state of the maximum laser range parameter
        maxJumpRange = resourceAndUpgradeManager.CurrentMaxJumpRange;
        instX = player.transform.position.x; //set the initial x position for instantiated objects
        instY = player.transform.position.y; //set the initial y position for instantiated objects
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //get the current position of the mouse pointer
        //target = gridLayout.WorldToCell(ray.origin); //set the initial position of the target (i.e. where is the mouse pointing in grid coordinates)
        //target = gridLayout.WorldToCell(clickManager.TouchPosition); //set the initial position of the target (i.e. where is the mouse pointing in grid coordinates)
        target = gridLayout.WorldToCell(clickManager.ClickPosition); //set the initial position of the target (i.e. where is the mouse pointing in grid coordinates)
        laserState = false; //set the initial state of the laser ability activation
        jumpState = false;
        jumpCells = new List<Vector3Int>();
        rocketRange = resourceAndUpgradeManager.CurrentMaxRocketRange;
        rocketReloadTime = resourceAndUpgradeManager.CurrentMaxRocketReload;
        playerFlats = new List<Vector3Int>();
        timer = 0.0f;
        turnOffAb = false;
        abilityActive = false;
        uiController.SetLaserCharge(laserRange, maxLaserRange);
        uiController.SetJumpCharge(jumpRange, maxJumpRange);
        uiController.SetShieldBoostRechargeState(currentShieldBoostCharge, shieldBoostRechargeTime);
        uiController.SetRocketReloadState(currentRocketReloadAmount, rocketReloadTime);
    }

    // Update is called once per frame
    void Update()
    {
        //player = GameObject.FindGameObjectWithTag("Player"); //at one point, the player game object was being set every update. I don't think this is necessary, but it may have had some effect.
        if (clickManager.MouseClicked && !turnOffAb) //check if the laser has been activated
        //if (clickManager.TouchRegistered && !turnOffAb) //check if the laser has been activated
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //get the current position of the mouse pointer
            //target = gridLayout.WorldToCell(ray.origin); //set the position of the target to the position of the mouse pointer in grid coordinates
            //target = gridLayout.WorldToCell(clickManager.TouchPosition); //set the position of the target to the position of the mouse pointer in grid coordinates
            target = gridLayout.WorldToCell(clickManager.ClickPosition); //set the position of the target to the position of the mouse pointer in grid coordinates
            //clickDistance = Vector3.Distance(gridLayout.CellToWorld(target), gridLayout.CellToWorld(gridLayout.WorldToCell(player.transform.position))); //this is find the distance between the player and the point where they click. It converts multiple times bewteen the grid and world coordinates because it wants the world coordinates of the exact center of the relevant hexs. The easiest way I have found to do this is to first take world coordinates, convert them to grid coordinates, then convert those back to world coordinates
            playerHex = gridLayout.WorldToCell(player.transform.position); //get the current position of the player in grid coordinates
            clickDistance = mapManager.HexCellDistance(mapManager.evenq2cube(target),mapManager.evenq2cube(playerHex)); //This calculation determines the distance to the clicked cell using the cube coordiante method 
            
            if (laserState && clickDistance<=laserRange) //if the player clicks the mouse and it is within the set range for the laser then initiate the firing sequence
            {
                //The purpose of this loop is to identify what hexes within the range above are also identified by the highlighted hexes shown to the player. This is a hack and there must certainly be a better way to do this, but it ultimately comes down to figuring out a better method of measuring distance on the hex grid
                for (int x = -laserRange; x <= laserRange; x++) //iterate x's
                {
                    for (int y = -laserRange; y <= laserRange; y++) //iterate y's
                    {
                        //Debug.Log(target);
                        //Debug.Log(playerHex + new Vector3Int(x, y, 0));
                        if (target == (playerHex + new Vector3Int(x, y, 0))&&weaponState) //check if the target hex matches one of the highlighted hexes
                        {
                            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                            foreach (GameObject enemy in enemies) //loop through the list of any enemies currently in the scene and destroy them
                            {
                                if (!enemy.GetComponent<EnemyShipControl>().CheckShotRunning)
                                {
                                    StartCoroutine(enemy.GetComponent<EnemyShipControl>().CheckShot(target));
                                }
                                
                            }

                            instX = player.transform.position.x; //set the x position of the instatiation equal to the player's current x position
                            instY = player.transform.position.y; //set the y position of the instatiation equal to the player's current x position
                            //laserSpwanMod = Vector3.Normalize(player.transform.position - ray.origin); //calculate a vector that points between the target and the player, then normalize it
                            //laserSpwanMod = Vector3.Normalize(player.transform.position - clickManager.TouchPosition); //calculate a vector that points between the target and the player, then normalize it
                            laserSpwanMod = Vector3.Normalize(player.transform.position - clickManager.ClickPosition); //calculate a vector that points between the target and the player, then normalize it
                            newInstance = Instantiate(laser, new Vector3(instX - (laserSpwanMod.x ), instY - (laserSpwanMod.y ), 0), Quaternion.identity); //Instantiate the laser object and apply some modifications that move the laser off of the ceneter of the player game object and to the edge of the ship sprite (this looks better in game then spawning at the center).
                            turnOffAb = true;
                        }
                    }
                }
            }else if (jumpState)
            {
                bool makeJump = false;
                foreach (Vector3Int cell in jumpCells)
                {
                    if(target == cell)
                    {
                        makeJump = true;
                    }
                }
                if (makeJump)
                {
                    
                    JumpActive();
                    //Debug.Log("set true at Ab con 112");
                    abilityUsed = true;
                    clickManager.WaitForQuarterSec();
                    movementController.MovePlayer(target, false);
                    //jumpRange = 0;
                    jumpRange -= (int)clickDistance;
                    uiController.SetJumpCharge(jumpRange,maxJumpRange);
                    if (turnManager.combatActive)
                    {
                        movementController.HasMoved = true;
                        uiController.SetEndTurnButtonState();
                    }
                    turnManager.StartCoroutine(turnManager.UpdateTurn());
                }
            }else if (rocketState)
            {
                bool inRange = false;
                foreach(Vector3Int flat in playerFlats)
                {
                    if (target == flat)
                    {
                        inRange = true;
                    }
                }
                if (inRange && !hasFired)
                {
                    Debug.Log("sending off a rocket");
                    hasFired = true;
                    currentRocketReloadAmount = 0;
                    Instantiate(rocket, player.transform.position, Quaternion.identity);
                    turnManager.StartCoroutine(turnManager.UpdateTurn());
                }
            }
        }
        if (turnOffAb)
        {
            
            timer += Time.deltaTime;
            if (timer > 0.5)
            {
                //Debug.Log("Timer");
                if (laserState)
                {
                    LaserActive();
                }
                if (turnManager.combatActive)
                {
                    
                    //laserRange = 0;
                    laserRange -= (int)clickDistance;
                    uiController.SetLaserCharge(laserRange, maxLaserRange);
                }
                timer = 0;
                turnOffAb = false;
                //Debug.Log("set true at Ab con 156");
                abilityUsed = true;
                turnManager.StartCoroutine(turnManager.UpdateTurn());
                if (turnManager.combatActive && movementController.HasMoved)
                {
                    uiController.SetEndTurnButtonState();
                }
            }
        }
    }

    public void LaserActive()
    {
        //Debug.Log("Laser");
        if (jumpState)
        {
            JumpActive();
        }
        else if (shieldState)
        {
            ShieldActive();
        }
        else if (rocketState)
        {
            RocketsActive();
        }
        //Debug.Log(abilityUsed);
        if (turnManager.playerTurn && weaponState && !abilityUsed && turnManager.combatActive ||laserState)
        {
            //When this function is called, it checks the current state of the laser abilty then switches to the other state and applies the necessary updates
            if (laserState)
            {
                laserState = false; //the laser state was true, so set it false
                abilityActive = laserState;
                //player.GetComponent<MovementController>().abilityActive = laserState; //set the abilityActive variable in the MovementController equal to the laser state
                mapManager.UpdateHighlight(laserRange, player.GetComponent<MovementController>().playerCellPosition, laserState); //call the method that will update the map display to disable the range display
            }
            else
            {
                laserState = true; //the laser state was false, so set it true
                abilityActive = laserState;
                //player.GetComponent<MovementController>().abilityActive = laserState; //set the abilityActive variable in the MovementController equal to the laser state
                mapManager.UpdateHighlight(laserRange, player.GetComponent<MovementController>().playerCellPosition, laserState); //call the method that will update the map display to enable the range display
            }
        }else if (!turnManager.combatActive)
        {
            //mapManager.ClearHighlighting();
            laserState = false; //the laser state was true, so set it false
            abilityActive = laserState;
            //player.GetComponent<MovementController>().abilityActive = laserState; //set the abilityActive variable in the MovementController equal to the laser state
            mapManager.UpdateHighlight(laserRange, player.GetComponent<MovementController>().playerCellPosition, laserState); //call the method that will update the map display to disable the range display
        
        }
        if(mapManager.saveName == "TutorialFile" && weaponState)
        {
            tutorialManager.ExplainLaser();
        }
    }

    public void ShieldActive()
    {
        if (jumpState)
        {
            JumpActive();
        }
        else if (rocketState)
        {
            RocketsActive();
        }
        else if (laserState)
        {
            LaserActive();
        }
        if (turnManager.playerTurn && weaponState && !abilityUsed && turnManager.combatActive &&(currentShieldBoostCharge>=shieldBoostRechargeTime||shieldState))
        {
            //abilityUsed = true; set in PlayerHealthControl based on whether ability triggers
            playerHealthControl.IncreaseShields(resourceAndUpgradeManager.CurrentMaxShieldBoost, resourceAndUpgradeManager.CurrentShieldOverboostActive);
            currentShieldBoostCharge = 0;
            uiController.SetShieldBoostRechargeState(currentShieldBoostCharge, shieldBoostRechargeTime);
        }
    }

    public void RocketsActive()
    {
        //Debug.Log("Rockets");
        if (jumpState)
        {
            JumpActive();
        }
        else if (shieldState)
        {
            ShieldActive();
        }
        else if (laserState)
        {
            LaserActive();
        }
        if (turnManager.playerTurn && weaponState && !abilityUsed && turnManager.combatActive && (currentRocketReloadAmount >= rocketReloadTime||rocketState)) 
        {
            playerFlats.Clear();
            rocketState = !rocketState;
            //player.GetComponent<MovementController>().abilityActive = rocketState;
            abilityActive = rocketState;
            playerFlats = mapManager.GetFlats(rocketRange, gridLayout.WorldToCell(player.transform.position), false);
            mapManager.HighlightSet(playerFlats, rocketState);
        }
        
    }

    public void JumpActive()
    {
        //Debug.Log("Jump");
        if (laserState)
        {
            LaserActive();
        }
        else if (shieldState)
        {
            ShieldActive();
        }
        else if (rocketState)
        {
            RocketsActive();
        }
        //Debug.Log("ability checkpoints: Player turn -"+ turnManager.playerTurn +"; Weapon state - "+ weaponState + "; Ability used - " + abilityUsed + "; Combat active - " + turnManager.combatActive);
        if (turnManager.playerTurn && weaponState && !abilityUsed && turnManager.combatActive)
        {
            jumpState = !jumpState;
            abilityActive = jumpState;
            jumpCells.Clear();
            //player.GetComponent<MovementController>().abilityActive = jumpState;
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); //create a list of any enemies that are currently in the scene
            
            for (int x = -jumpRange; x <= jumpRange; x++) //iterate through the range of the laser to generate the x coordinates
            {
                for (int y = -jumpRange; y <= jumpRange; y++) //iterate through the range of the laser to generate the y coordindates
                {
                    Vector3Int tempCell = movementController.playerCellPosition + new Vector3Int(x, y, 0);
                    float hexCellDistance = mapManager.HexCellDistance(mapManager.evenq2cube(gridLayout.WorldToCell(player.transform.position)), mapManager.evenq2cube(tempCell));
                    bool cellUnavailable = false;
                    //Debug.Log("hex cell distance " + hexCellDistance);
                    if (hexCellDistance <= jumpRange)
                    {
                        foreach (GameObject enemy in enemies)
                        {
                            if (tempCell == gridLayout.WorldToCell(enemy.transform.position))
                            {
                                cellUnavailable = true;
                            }
                        }
                        if ( tempCell.x > mapManager.mapXMax || tempCell.x < mapManager.mapXMin || tempCell.y > mapManager.mapYMax || tempCell.y < mapManager.mapYMin)
                        {
                            cellUnavailable = true;
                        }
                        if (!cellUnavailable)
                        {
                            jumpCells.Add(tempCell);
                        }
                    }

                }
            }
            mapManager.HighlightSet(jumpCells, jumpState);
            //Debug.Log("Jump cell count is " + jumpCells.Count);
            //Debug.Log("jump range is " + jumpRange);
            //foreach (Vector3Int cell in jumpCells)
            //{
            //    Debug.Log(cell);
            //}
            if (!jumpState)
            {
                mapManager.ClearHighlighting();
            }
        }
    }

    public void TurnOffAbilities()
    {
        if (jumpState)
        {
            JumpActive();
        }
        else if (laserState)
        {
            LaserActive();
        }
        else if (shieldState)
        {
            ShieldActive();
        }
        else if (rocketState)
        {
            RocketsActive();
        }
    }

}
