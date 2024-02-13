using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int Points = 0;
    public UnityEvent OnPointsUpdated;
    public UnityEvent<GameState> OnGameStateUpdated;

    public float timeToMatch = 10f;
    public float currentTimeToMatch = 0f;

    public enum GameState
    {
        Idle,
        Playing,
        GameOver
    }

    public GameState gameState;

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

    public void AddPoints(int newPoints)
    {
        Points += newPoints;
        OnPointsUpdated.Invoke();
        currentTimeToMatch = 0;
    }


    void Start()
    {

    }

    private void Update()
    {
        if(gameState == GameState.Playing){
            currentTimeToMatch += Time.deltaTime;
            if(currentTimeToMatch >= timeToMatch){
                gameState = GameState.GameOver;
                OnGameStateUpdated.Invoke(gameState);
            }
        }

    }
}
