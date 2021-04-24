using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CI.QuickSave;

public class PlayerHealthControl : MonoBehaviour
{
    private int maxPlayerHealth=3;
    public int currentPlayerHealth=3;
    public int maxPlayerShields=2;
    public int currentPlayerShields=2;
    public GameObject explosion;

    public int MaxPlayerHealth { get {  return maxPlayerHealth; } set { Debug.Log("modfied player health"); maxPlayerHealth = value; } }

    private GameObject gameController;
    private UIControl uiControl;
    private ResourceAndUpgradeManager resourceAndUpgradeManager;
    private AbilityController abilityController;
    private MovementController movementController;
    private TurnManager turnManager;
    private bool beenDestroyed = false;

    public bool BeenDestroyed { get { return beenDestroyed; } set { beenDestroyed = value; } }
    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController");
        uiControl = gameController.GetComponent<UIControl>();
        turnManager = gameController.GetComponent<TurnManager>();
        resourceAndUpgradeManager = gameController.GetComponent<ResourceAndUpgradeManager>();
        abilityController = GameObject.Find("Player").GetComponent<AbilityController>();

        if (QuickSaveRoot.Exists(resourceAndUpgradeManager.ResourceAndUpgradeDataSaveFileName)) //use the quicksave feature to check if a save file exists 
        {
            QuickSaveReader instReader = QuickSaveReader.Create(resourceAndUpgradeManager.ResourceAndUpgradeDataSaveFileName); //create an instance of the quick save reader to pull in the save file
            MaxPlayerHealth = instReader.Read<int>("currentMaxHealth");
            maxPlayerShields = instReader.Read<int>("currentMaxShields");
            uiControl.SetHealthState(maxPlayerHealth, currentPlayerHealth, maxPlayerShields, currentPlayerShields);
        }

        movementController = gameObject.GetComponent<MovementController>();
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

        uiControl.SetHealthState(MaxPlayerHealth, currentPlayerHealth, maxPlayerShields, currentPlayerShields);
    }

    public IEnumerator DestroyPlayer()
    {
        if (!beenDestroyed)
        {
            beenDestroyed = true;
            Color color = new Color();
            color.a = 0f;
            gameObject.GetComponent<SpriteRenderer>().color = color;
            Instantiate(explosion, transform.position, Quaternion.identity);
            turnManager.combatActive = false;
            abilityController.weaponState = false;
            movementController.movementState = false;
            uiControl.SetEndTurnButtonState();
            yield return new WaitForSeconds(.5f);
            uiControl.SetGameOverPanelState();
        }
    }

    public void RestoreShields()
    {
        currentPlayerShields = maxPlayerShields;
        uiControl.SetHealthState(MaxPlayerHealth, currentPlayerHealth, maxPlayerShields, currentPlayerShields);
    }
    public void RestoreHealth()
    {
        currentPlayerHealth = MaxPlayerHealth;
        uiControl.SetHealthState(MaxPlayerHealth, currentPlayerHealth, maxPlayerShields, currentPlayerShields);
    }

    public void IncreaseHealth(int healthIncrease)
    {
        if (currentPlayerHealth < MaxPlayerHealth)
        {
            currentPlayerHealth = currentPlayerHealth + healthIncrease;
            uiControl.SetHealthState(MaxPlayerHealth, currentPlayerHealth, maxPlayerShields, currentPlayerShields);
        }        
    }

    public void IncreaseShields(int shieldIncrease, bool sheildOverboost)
    {
        if (sheildOverboost)
        {
            if (currentPlayerShields < 6)
            {
                currentPlayerShields = currentPlayerShields + shieldIncrease;
                uiControl.SetHealthState(MaxPlayerHealth, currentPlayerHealth, maxPlayerShields, currentPlayerShields);
                abilityController.abilityUsed = true;
            }
        }
        else
        {
            if (currentPlayerShields < maxPlayerShields)
            {
                abilityController.abilityUsed = true;
                currentPlayerShields = currentPlayerShields + shieldIncrease;
                uiControl.SetHealthState(MaxPlayerHealth, currentPlayerHealth, maxPlayerShields, currentPlayerShields);
            }
        }
       
    }
}
