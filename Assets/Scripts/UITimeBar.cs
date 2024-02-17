using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UITimeBar : MonoBehaviour
{
    public RectTransform fillRect;
    public UnityEngine.UI.Image fillColor;
    public Gradient gradient;

    void Update()
    {
        float factor = GameManager.instance.currentTimeToMatch / GameManager.instance.timeToMatch;
        factor = Mathf.Clamp(factor, 0f, 1f); //Clamp01 returns a value between 0 and 1
        factor = 1 - factor; //Invert the value / time remaining
        fillRect.localScale = new Vector3(factor, 1, 1);
        fillColor.color = gradient.Evaluate(factor);
    }

    

}
