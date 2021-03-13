using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private AbilityController abilityController;
    private MovementController movementController;
    private PlanetTrigger planetTrigger;



    // Start is called before the first frame update
    void Start()
    {
        abilityController = GameObject.Find("Player").GetComponent<AbilityController>();
        movementController = GameObject.Find("Player").GetComponent<MovementController>();
        planetTrigger = GameObject.Find("Player").GetComponent<PlanetTrigger>();
    }


    public void SetMovementState()
    {
        movementController.movementState = !movementController.movementState;
    }

    public void SetWeaponsState()
    {
        abilityController.weaponState = !abilityController.weaponState;
    }

    public void SetPlanetState()
    {
        planetTrigger.planetState = !planetTrigger.planetState;
    }
}
