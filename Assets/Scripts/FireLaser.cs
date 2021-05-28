using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This script defines the behaviour of the laser animation
public class FireLaser : MonoBehaviour
{
    public Vector3 target; //Variable to hold the target position
    public GameObject player; //Variable to hold the player game object

    private float timer; //Variable to hold the timer tracking the life of the laser animation
    //private float strength = 500; //Variable to determine how quickly to rotate the laser object to the correct orientation. We want this to happen instantaneously so this is set to a very high value. There is likely a better solution to this.
    //private float str; //Variable to hold the frame rate adjusted rotation strength
    private float laserLength; //Variable to hold the length of the laser shot
    private Quaternion targetRotation; //Variable to hold the intended rotation of this game object
    private SpriteRenderer laserSprite; //Variable to hold a reference to the Sprite Renderer component
    private GridLayout gridLayout; //Variable to hold a reference to the grid layout
    private ClickManager clickManager;
    //The below value is used in another method to set the rotation of this object
    //private float rotAngle;


    // Start is called before the first frame update
    void Awake()
    {
        clickManager = GameObject.Find("GameController").GetComponent<ClickManager>();
        gridLayout = GameObject.Find("Grid").GetComponent<GridLayout>(); //Access and store a reference to the grid layout
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Create a Ray defining the loction of the mouse pointer within the screen
        //target = gridLayout.CellToWorld(gridLayout.WorldToCell(ray.origin)); //set the target position equal to the position of the mouse pointer
        target = gridLayout.CellToWorld(gridLayout.WorldToCell(clickManager.TouchPosition)); //set the target position equal to the position of the mouse pointer
        timer = 0; //set the initial value of the timer that tracks the life of the laser animation

        player = GameObject.FindGameObjectWithTag("Player"); //Access and store a reference to the player game object
        SetRotation(); //Call the function that will orient this object in the direction of the target. 

        laserSprite = gameObject.GetComponent<SpriteRenderer>(); //Access and store a reference to the sprite renderer so it can be used to set the length and width of the laser
        laserLength = Vector3.Distance(target, transform.position); //Set the laser length equal to the distance between the player and the target
        laserSprite.size = new Vector2(0.1f, laserLength-0.1f); //Set the size of the x and y components of the laser sprite. The width of the laser sprite is set as a fixed value while the length is determined based on the distance between the player and the target. A small amount is subtracted from the legth so the laser appears to hit the outside of the target instead of the center.
        //The below operations were an attempt to set the angle in a different way from what is done. It was not successful, but may be a better way to do it if you can figure out how to get it to work
        //rotAngle = Vector2.SignedAngle(Vector2.right,target.transform.position- transform.position);
        //transform.eulerAngles=new Vector3(0, 0, rotAngle*Mathf.Rad2Deg);
    }

    // Update is called once per frame
    void Update()
    {
        SetRotation(); //Call the set rotation function each frame to ensure that the rotation setting operation always runs to completion
        timer += Time.deltaTime; //Increment the timer tracking the life of the laser shot
        if (timer > 0.6) //If the timer exceeds the lifespan of the laser animation, destroy this object
        {
            Destroy(gameObject);

        }
    }

    //The following function sets the rotation of this object based on the intended target and this objects orientation at the time the function is called 
    private void SetRotation()
    {
        targetRotation = Quaternion.LookRotation(Vector3.forward, new Vector3(target.x , target.y, 0) - transform.position); //Uses quaternion math to determine what rotation is necessary to point at the target
        //str = Mathf.Min(strength * Time.deltaTime, 1); //Sets the frame rate adjusted speed of the rotation
        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, str); //smoothly rotates this object towards it's target. Perhaps we can just change this logic so it turns instantly? Do some testing.
        transform.rotation = targetRotation; //instantly rotates laser to correct orientation 
    }
}
