using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShortWarning : MonoBehaviour
{
    private UIControl uicontroller;

    private void Awake()
    {
        uicontroller = GameObject.Find("GameController").GetComponent<UIControl>();
    }
    public IEnumerator FadeOut()
    {
        gameObject.GetComponent<Image>().canvasRenderer.SetAlpha(1.0f);
        yield return new WaitForSeconds(3f);
        gameObject.GetComponent<Image>().CrossFadeAlpha(0.0f, 1, false);
        yield return new WaitForSeconds(1f);
        uicontroller.DisableResourceCollectedWarning();
    }
}
