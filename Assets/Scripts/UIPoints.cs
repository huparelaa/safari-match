using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class UIPoints : MonoBehaviour
{

    int displayedPoints = 0;
    public TextMeshProUGUI pointsLabel;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnPointsUpdated.AddListener(UpdatePoints);
    }

    // Update is called once per frame
    void UpdatePoints()
    {
        StartCoroutine(UpdatePointsRoutine());
    }

    IEnumerator UpdatePointsRoutine()
    {
        while(displayedPoints < GameManager.instance.Points){
            displayedPoints++;
            pointsLabel.text = displayedPoints.ToString();
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }
}
