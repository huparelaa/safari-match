using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int Points = 0;
    public UnityEvent OnPointsUpdated;

    public void AddPoints(int newPoints)
    {
        Points += newPoints;
        OnPointsUpdated.Invoke();
    }


    void Start()
    {

    }

    void Update()
    {

    }
}
