using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIScript : MonoBehaviour
{
    public RectTransform containerRect;
    public CanvasGroup containerCanvas;
    public UnityEngine.UI.Image background;

    public GameManager.GameState visibleState;

    public float fadeDuration = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnGameStateUpdated.AddListener(GameStateUpdated);

        bool initialState = GameManager.instance.gameState == visibleState;
        background.enabled = initialState;
        containerRect.gameObject.SetActive(initialState);
    }

    private void GameStateUpdated(GameManager.GameState newState)
    {
        if (newState == visibleState)
        {
            ShowScreen();
        }
        else
        {
            HideScreen();
        }
    }

    private void HideScreen()
    {
        // background fade
        var bgColor = background.color;
        bgColor.a = 0;
        background.DOColor(bgColor, fadeDuration * 0.5f);

        // container fade
        containerCanvas.alpha = 1;
        containerRect.anchoredPosition = Vector2.zero;
        containerCanvas.DOFade(0, 0.5f);
        containerRect.DOAnchorPos(new Vector2(0, 100), fadeDuration*0.5f).OnComplete(() =>
        {
            // disable elements
            background.enabled = false;
            containerRect.gameObject.SetActive(false);
        });

    }

    private void ShowScreen()
    {
        // enable elements
        background.enabled = true;
        containerRect.gameObject.SetActive(true);

        // background fade
        var bgColor = background.color;
        bgColor.a = 0;
        background.color = bgColor;
        bgColor.a = 1;
        background.DOColor(bgColor, fadeDuration);

        // container fade
        containerCanvas.alpha = 0;
        containerRect.anchoredPosition = new Vector2(0, 100);
        containerCanvas.DOFade(1, fadeDuration);
        containerRect.DOAnchorPos(Vector2.zero, fadeDuration);
    }
}
