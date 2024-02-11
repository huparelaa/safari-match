using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditorInternal;
public class Piece : MonoBehaviour
{
    public int x;
    public int y;

    public Board board;

    public enum type
    {
        elephant,
        giraffe,
        hippo,
        monkey,
        panda,
        parrot,
        penguin,
        pig,
        rabbit,
        snake
    }
    public type pieceType;

    public void Setup(int x_, int y_, Board board_)
    {
        x = x_;
        y = y_;
        board = board_;

        transform.localScale = Vector3.one * 0.4f;
        transform.DOScale(Vector3.one, 0.4f);
    }

    public void Move(int destinationX, int destinationY)
    {
        transform.DOMove(new Vector3(destinationX, destinationY, -5f), 0.25f).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            x = destinationX;
            y = destinationY;
        });
    }

    [ContextMenu("Move testing")]
    public void MoveTest()
    {
        Move(0, 0);
    }

    public void Remove(bool animated)
    {
        if (animated)
        {
            transform.DORotate(new Vector3(0, 0,-120), 0.125f);
            transform.DOScale(Vector3.one * 1.25f, 0.005f).OnComplete(() =>
            {
                transform.DOScale(Vector3.zero, 0.1f).OnComplete(() =>
                {
                    Destroy(gameObject);
                });
            }
            );
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
