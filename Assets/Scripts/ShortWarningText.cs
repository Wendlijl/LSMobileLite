using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShortWarningText : MonoBehaviour
{
    public IEnumerator FadeOut()
    {
        gameObject.GetComponent<TMP_Text>().canvasRenderer.SetAlpha(1.0f);
        yield return new WaitForSeconds(3f);
        gameObject.GetComponent<TMP_Text>().CrossFadeAlpha(0.0f, 1, false);
        yield return new WaitForSeconds(1f);
        
    }
}
