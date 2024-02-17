using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource audioSource;

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
    public AudioClip moveSFX;
    public AudioClip matchSFX;
    public AudioClip missSFX;
    public AudioClip gameOverSFX;


    void Start()
    {
        GameManager.instance.OnPointsUpdated.AddListener(PointsUpdated);
        GameManager.instance.OnGameStateUpdated.AddListener(GameStateUpdated);
    }

    private void OnDestory()
    {
        GameManager.instance.OnPointsUpdated.RemoveListener(PointsUpdated);
        GameManager.instance.OnGameStateUpdated.RemoveListener(GameStateUpdated);
    }

    private void PointsUpdated()
    {
        audioSource.PlayOneShot(moveSFX);
    }

    private void GameStateUpdated(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.GameOver)
        {
            audioSource.PlayOneShot(gameOverSFX);
        }

        if (newState == GameManager.GameState.Playing)
        {
            audioSource.PlayOneShot(matchSFX);
        }
    }

    public void Move()
    {
        audioSource.PlayOneShot(moveSFX);
    }

    public void Miss()
    {
        audioSource.PlayOneShot(missSFX);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
