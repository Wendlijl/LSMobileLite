using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    public Vector3 target; //Variable to hold the target position
    public GameObject player; //Variable to hold the player game object
    public GameObject rocketExplosion;

    private float timer; //Variable to hold the timer tracking the life of the laser animation
    private Quaternion targetRotation; //Variable to hold the intended rotation of this game object
    private GridLayout gridLayout; //Variable to hold a reference to the grid layout
    private Rigidbody2D rb2d;
    private ManageMap mapManager;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player"); //Access and store a reference to the player game object
        mapManager = GameObject.Find("GameController").GetComponent<ManageMap>();
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>(); //Access and store a reference to the grid layout
        target = gridLayout.CellToWorld(player.GetComponent<AbilityController>().target);
        timer = 0; //set the initial value of the timer that tracks the life of the laser animation
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        Debug.Log("The target is "+target);

        SetRotation(); //Call the function that will orient this object in the direction of the target. 

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime; //Increment the timer tracking the life of the laser shot
        if (timer > 2) //If the timer exceeds the lifespan of the laser animation, destroy this object
        {
            Instantiate(rocketExplosion, transform.position, Quaternion.identity) ;
            GetNeighbours(gridLayout.WorldToCell(transform.position));
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        Vector3 moveTarget = transform.position - target;
        //rb2d.MovePosition(transform.position - moveTarget*Time.deltaTime*2);
        //rb2d.MovePosition(transform.position - moveTarget);
        //transform.position -= moveTarget * Time.deltaTime * 2;
        transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * 2);
    }

    //The following function sets the rotation of this object based on the intended target and this objects orientation at the time the function is called 
    private void SetRotation()
    {
        targetRotation = Quaternion.LookRotation(Vector3.forward, new Vector3(target.x, target.y, 0) - transform.position); //Uses quaternion math to determine what rotation is necessary to point at the target
        transform.rotation = targetRotation; //instantly rotates laser to correct orientation 
    }

    public void GetNeighbours(Vector3Int origin)
    {
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int modX = origin.x + x;
                int modY = origin.y + y;

                if (modX < mapManager.mapXMax && modX > mapManager.mapXMin && modY < mapManager.mapYMax && modY > mapManager.mapYMin)
                {
                    if (mapManager.HexCellDistance(mapManager.evenq2cube(origin), mapManager.evenq2cube(new Vector3Int(modX, modY, 0))) <= 1)
                    {
                        foreach (GameObject enemy in enemies)
                        {
                            Vector3Int enemyPos = gridLayout.WorldToCell(enemy.transform.position);

                            if (enemyPos.x == modX && enemyPos.y == modY)
                            {
                                enemy.GetComponent<EnemyShipControl>().DestroySelf(true);
                            }
                        }
                    }
                }
            }
        }
    }
}
