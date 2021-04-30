using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    public Vector3 target; //Variable to hold the target position
    public GameObject player; //Variable to hold the player game object
    public GameObject rocketExplosion;
    public int turnsAlive;
    public int turnDelay;
    private int rocketYield;

    private Quaternion targetRotation; //Variable to hold the intended rotation of this game object
    private GridLayout gridLayout; //Variable to hold a reference to the grid layout
    private ManageMap mapManager;
    private AbilityController abilityController;
    private MovementController movementController;
    private UIControl uiController;
    private TurnManager turnManager;
    private GameObject gameController;
    private ResourceAndUpgradeManager resourceAndUpgradeManager;
    private PlayerHealthControl playerHealthControl;
    private bool alreadyUsed;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player"); //Access and store a reference to the player game object
        gameController = GameObject.Find("GameController"); //Access and store a reference to the player game object
        abilityController = player.GetComponent<AbilityController>();
        movementController = player.GetComponent<MovementController>();
        playerHealthControl = player.GetComponent<PlayerHealthControl>();
        resourceAndUpgradeManager = gameController.GetComponent<ResourceAndUpgradeManager>();
        mapManager = gameController.GetComponent<ManageMap>();
        uiController = gameController.GetComponent<UIControl>();
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>(); //Access and store a reference to the grid layout
        turnManager = gameController.GetComponent<TurnManager>();
        target = gridLayout.CellToWorld(player.GetComponent<AbilityController>().target);
        turnsAlive = 0;
        turnDelay = 1;
        alreadyUsed = false;
        rocketYield = resourceAndUpgradeManager.CurrentMaxRocketYield;
        SetRotation(); //Call the function that will orient this object in the direction of the target. 

    }

    // Update is called once per frame
    void Update()
    {
        if (turnsAlive > turnDelay) //If the timer exceeds the lifespan of the laser animation, destroy this object
        {
            Detonate(transform.position);
        }
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * 2);
        if(transform.position == target && abilityController.rocketState &&!alreadyUsed)
        {
            alreadyUsed = true;
            abilityController.RocketsActive();
            //Debug.Log("set true at rocket 55");
            abilityController.abilityUsed = true;
            turnManager.StartCoroutine(turnManager.UpdateTurn());
            if(turnManager.combatActive && movementController.hasMoved)
            {
                uiController.SetEndTurnButtonState();
            }
        }
    }

    public void Detonate(Vector3 origin)
    {
        if (abilityController.rocketState)
        {
            abilityController.RocketsActive();
            //Debug.Log("set true at rocket 69");
            abilityController.abilityUsed = true;
            turnManager.StartCoroutine(turnManager.UpdateTurn());
            if (turnManager.combatActive && movementController.hasMoved)
            {
                uiController.SetEndTurnButtonState();
            }
        }
        Instantiate(rocketExplosion, origin, Quaternion.identity);
        rocketYield = resourceAndUpgradeManager.CurrentMaxRocketYield;
        GetExplosionNeighbours(gridLayout.WorldToCell(origin),rocketYield);
        Destroy(gameObject);
    }

    //The following function sets the rotation of this object based on the intended target and this objects orientation at the time the function is called 
    private void SetRotation()
    {
        targetRotation = Quaternion.LookRotation(Vector3.forward, new Vector3(target.x, target.y, 0) - transform.position); //Uses quaternion math to determine what rotation is necessary to point at the target
        transform.rotation = targetRotation; //instantly rotates laser to correct orientation 
    }

    private void GetExplosionNeighbours(Vector3Int origin, int explosionYield)
    {
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Vector3Int playerPos = player.GetComponent<MovementController>().playerCellPosition;
        for (int x = -1 * explosionYield; x <= 1 * explosionYield; x++)
        {
            for (int y = -1 * explosionYield; y <= 1 * explosionYield; y++)
            {
                int modX = origin.x + x;
                int modY = origin.y + y;

                if (modX < mapManager.mapXMax && modX > mapManager.mapXMin && modY < mapManager.mapYMax && modY > mapManager.mapYMin)
                {
                    if (mapManager.HexCellDistance(mapManager.evenq2cube(origin), mapManager.evenq2cube(new Vector3Int(modX, modY, 0))) <= 1 * explosionYield)
                    {
                        foreach (GameObject enemy in enemies)
                        {
                            Vector3Int enemyPos = gridLayout.WorldToCell(enemy.transform.position);

                            if (enemyPos.x == modX && enemyPos.y == modY)
                            {
                                enemy.GetComponent<EnemyShipControl>().DestroySelf(true);
                            }
                        }
                        if(playerPos.x ==modX && playerPos.y == modY)
                        {
                            playerHealthControl.PlayerHit(1*explosionYield);
                            if (playerHealthControl.currentPlayerHealth <= 0)
                            {
                                //playerHealthControl.DestroyPlayer();
                                playerHealthControl.StartCoroutine("DestroyPlayer");
                            }
                        }
                    }
                }
            }
        }
    }
}
