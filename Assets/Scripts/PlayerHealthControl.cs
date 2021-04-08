using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthControl : MonoBehaviour
{
    public int maxPlayerHealth;
    public int currentPlayerHealth;
    public int maxPlayerShields;
    public int currentPlayerShields;

    private UIControl uiControl;
    private AbilityController abilityController;
    // Start is called before the first frame update
    void Start()
    {
        uiControl = GameObject.Find("GameController").GetComponent<UIControl>();
        abilityController = GameObject.Find("Player").GetComponent<AbilityController>();
        maxPlayerHealth = 3;
        currentPlayerHealth = 3;
        maxPlayerShields = 2;
        currentPlayerShields = 2;
        uiControl.SetHealthState(maxPlayerHealth, currentPlayerHealth, maxPlayerShields, currentPlayerShields);
    }

    public void PlayerHit()
    {
        if (currentPlayerShields > 0)
        {
            currentPlayerShields--;
        }
        else
        {
            currentPlayerHealth--;
        }
        uiControl.SetHealthState(maxPlayerHealth, currentPlayerHealth, maxPlayerShields, currentPlayerShields);
    }

    public void RestoreShields()
    {
        currentPlayerShields = maxPlayerShields;
        uiControl.SetHealthState(maxPlayerHealth, currentPlayerHealth, maxPlayerShields, currentPlayerShields);
    }
    public void RestoreHealth()
    {
        currentPlayerHealth = maxPlayerHealth;
        uiControl.SetHealthState(maxPlayerHealth, currentPlayerHealth, maxPlayerShields, currentPlayerShields);
    }

    public void IncreaseHealth(int healthIncrease)
    {
        if (currentPlayerHealth < maxPlayerHealth)
        {
            currentPlayerHealth = currentPlayerHealth + healthIncrease;
            uiControl.SetHealthState(maxPlayerHealth, currentPlayerHealth, maxPlayerShields, currentPlayerShields);
        }        
    }

    public void IncreaseShields(int shieldIncrease, bool sheildOverboost)
    {
        if (sheildOverboost)
        {
            if (currentPlayerShields < 6)
            {
                currentPlayerShields = currentPlayerShields + shieldIncrease;
                uiControl.SetHealthState(maxPlayerHealth, currentPlayerHealth, maxPlayerShields, currentPlayerShields);
                abilityController.abilityUsed = true;
            }
        }
        else
        {
            if (currentPlayerShields < maxPlayerShields)
            {
                abilityController.abilityUsed = true;
                currentPlayerShields = currentPlayerShields + shieldIncrease;
                uiControl.SetHealthState(maxPlayerHealth, currentPlayerHealth, maxPlayerShields, currentPlayerShields);
            }
        }
       
    }
}
