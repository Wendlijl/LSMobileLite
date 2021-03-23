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
    // Start is called before the first frame update
    void Start()
    {
        uiControl = GameObject.Find("GameController").GetComponent<UIControl>();
        maxPlayerHealth = 3;
        currentPlayerHealth = 3;
        maxPlayerShields = 1;
        currentPlayerShields = 1;
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
}
