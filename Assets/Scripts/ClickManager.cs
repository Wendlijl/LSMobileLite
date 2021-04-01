using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public bool mouseClicked;
    public bool waitForQuarterSec;
    private float timer;
    private ManageMap mapManager;
    // Start is called before the first frame update
    void Start()
    {
        mapManager = GameObject.Find("GameController").GetComponent<ManageMap>();
        waitForQuarterSec = false;
        mouseClicked = false;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //This operation is initiated by the player clicking the fire button.
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies) //loop through the list of any enemies currently in the scene and destroy them
            {
                if (enemy.GetComponent<EnemyShipControl>().highlightEnabled)
                {
                    mapManager.ShowFlats(enemy.GetComponent<EnemyShipControl>().thisEnemyName, enemy.GetComponent<EnemyShipControl>().enemyCellPosition, enemy, false);
                }
            }
            mouseClicked = true;
        }
        else
        {
            mouseClicked = false;
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
