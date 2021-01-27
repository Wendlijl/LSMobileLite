using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This script controls the behaviour of the explosion animation which will run once when a ship is destroyed. 
public class ExplosionControl : MonoBehaviour
{
    private float timer; //Variable to track the life of the explosion
    public float timerLimit = 0.45f; //Variable to define the life of the explosion
    // Start is called before the first frame update
    void Start()
    {
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
