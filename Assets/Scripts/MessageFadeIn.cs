using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageFadeIn : MonoBehaviour
{
    private float currentFade= 0;
    private float fadeTarget = 1;
    private bool setFadeIn = false;
    public bool SetFadeIn { get { return setFadeIn; } set { setFadeIn = value; } }

    private void Start()
    {
        setFadeIn = false;
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
    }
    private void Update()
    {
        if (setFadeIn && currentFade < fadeTarget)
        {
            gameObject.GetComponent<CanvasGroup>().alpha += 1*Time.deltaTime;
        }
    }
}
