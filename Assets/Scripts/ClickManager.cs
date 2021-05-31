using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    //public bool mouseClicked;
    public bool waitForQuarterSec;
    private float timer;
    private bool touchRegistered;
    private bool touchEnded;
    private Touch touch;
    private Vector3 touchPosition;
    private GameObject gameController;
    private ManageMap mapManager;
    private UIControl uiControler;
    private GameObject player;
    private MovementController movementController;
    private AbilityController abilityController;

    public bool TouchRegistered { get { return touchRegistered; } set { touchRegistered = value; } }
    public Vector3 TouchPosition { get { return touchPosition; } set { touchPosition = value; } }

    // Start is called before the first frame update
    void Start()
    {
        touchRegistered = false;
        touchEnded = true;
        gameController = GameObject.Find("GameController");
        mapManager = gameController.GetComponent<ManageMap>();
        uiControler = gameController.GetComponent<UIControl>();
        player = GameObject.Find("Player");
        movementController = player.GetComponent<MovementController>();
        abilityController = player.GetComponent<AbilityController>();
        waitForQuarterSec = false;
        //mouseClicked = false;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0)&&!uiControler.IsPaused) //This operation is initiated by the player clicking the fire button.
        if (Input.touchCount>0&&!uiControler.IsPaused) //This operation is initiated by the player clicking the fire button.
        {
            touch = Input.GetTouch(0);
            touchPosition = Camera.main.ScreenToWorldPoint(touch.position);


            //Debug.Log(Input.mousePosition);
            Debug.Log(touch.position);
            Debug.Log("Screen height " + Screen.height + " Screen width " + Screen.width);
            Debug.Log("% Screen height " + touch.position.y/Screen.height + " % Screen width " + touch.position.x / Screen.width);
            //float percentageScreenHeight = Input.mousePosition.y / Screen.height;
            float percentageScreenHeight = touch.position.y / Screen.height;
            //Debug.Log(percentageScreenHeight);
            if (percentageScreenHeight > 0.1 && percentageScreenHeight < 0.9)
            {
                if (!touchEnded)
                {
                    touchRegistered = false;
                    movementController.TouchRegistered = touchRegistered;
                }
                //mouseClicked = true;
                if (touch.phase == TouchPhase.Began)
                {
                    Debug.Log("Click Manager hears the touch");
                    touchRegistered = true;
                    touchEnded = false;
                    movementController.TouchRegistered = touchRegistered;

                }
                if(touch.phase == TouchPhase.Moved)
                {
                    touchRegistered = false;
                    movementController.TouchRegistered = touchRegistered;
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    touchEnded = true;
                    touchRegistered = false;
                    movementController.TouchRegistered = touchRegistered;
                }
            }
            else
            {
                //mouseClicked = false;
                touchRegistered = false;
                movementController.TouchRegistered = touchRegistered;
            }

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies) //loop through the list of any enemies currently in the scene and destroy them
            {
                if (enemy.GetComponent<EnemyShipControl>().highlightEnabled)
                {
                    mapManager.ShowFlats(enemy.GetComponent<EnemyShipControl>().thisEnemyName, enemy.GetComponent<EnemyShipControl>().enemyCellPosition, enemy, false);
                }
            }

        }
        else
        {
            //mouseClicked = false;
            touchRegistered = false;
            movementController.TouchRegistered = touchRegistered;
        }

        if (timer>0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            waitForQuarterSec = false;
        }

    }

    public void WaitForQuarterSec()
    {
        waitForQuarterSec = true;
        timer = 0.25f;
    }
}
