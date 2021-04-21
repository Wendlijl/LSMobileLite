using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthControl : MonoBehaviour
{
    public int maxPlayerHealth=3;
    public int currentPlayerHealth=3;
    public int maxPlayerShields=2;
    public int currentPlayerShields=2;

    private GameObject gameController;
    private UIControl uiControl;
    private ResourceAndUpgradeManager resourceAndUpgradeManager;
    private AbilityController abilityController;
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController");
        uiControl = gameController.GetComponent<UIControl>();
        resourceAndUpgradeManager = gameController.GetComponent<ResourceAndUpgradeManager>();
        abilityController = GameObject.Find("Player").GetComponent<AbilityController>();
        maxPlayerHealth = resourceAndUpgradeManager.CurrentMaxHealth;
        maxPlayerShields = resourceAndUpgradeManager.CurrentMaxShields;
        uiControl.SetHealthState(maxPlayerHealth, currentPlayerHealth, maxPlayerShields, currentPlayerShields);
    }

    public void PlayerHit(int damageAmount)
    {
        for(int i = 0; i <= damageAmount-1; i++)
        {
            if (currentPlayerShields > 0)
            {
                currentPlayerShields--;
            }
            else
            {
                currentPlayerHealth--;
            }
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
