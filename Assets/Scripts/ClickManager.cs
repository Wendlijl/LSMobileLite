using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public bool mouseClicked;
    // Start is called before the first frame update
    void Start()
    {
        mouseClicked = false;
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
                    enemy.GetComponent<EnemyShipControl>().ShowFlats(false);
                }
            }
            mouseClicked = true;
        }
        else
        {
            mouseClicked = false;
        }

    }
}
