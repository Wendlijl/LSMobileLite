using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketExplosionControl : MonoBehaviour
{
    private float timer; //Variable to track the life of the explosion
    public float timerLimit = 0.45f; //Variable to define the life of the explosion
    private ResourceAndUpgradeManager resourceAndUpgradeManager;
    // Start is called before the first frame update
    void Awake()
    {
        resourceAndUpgradeManager = GameObject.Find("GameController").GetComponent<ResourceAndUpgradeManager>();
        if (resourceAndUpgradeManager.CurrentMaxRocketYield == 2)
        {
            transform.localScale = new Vector3(4,4,1);
        }else if(resourceAndUpgradeManager.CurrentMaxRocketYield == 3)
        {
            transform.localScale = new Vector3(5, 5, 1);
        }
        timer = 0; //Sets the initial state of the timer used to track the explosion life
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime; //increment the timer tracking the explosion life
        if (timer > timerLimit) //When the timer life track exceeds the timer lifespan, destroy the explosion object
        {
            Destroy(gameObject);

        }
    }
}
