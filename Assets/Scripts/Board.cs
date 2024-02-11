using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    public GameObject tileObject;

    public float CameraSizeOffset;
    public float CameraVerticalOffset;

    public GameObject[] availablePieces;

    Tile[,] Tiles; //<-- 2D array of tiles
    Piece[,] Pieces; //<-- 2D array of pieces

    Tile startTile;
    Tile endTile;

    // Start is called before the first frame update
    void Start()
    {
        Tiles = new Tile[width, height];
        Pieces = new Piece[width, height];
        SetupBoard();
        PositionCamera();
        SetupPieces();
    }

    private void SetupBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var o = Instantiate(tileObject, new Vector3(x, y, -5), Quaternion.identity);
                o.transform.parent = transform;
                Tiles[x, y] = o.GetComponent<Tile>();
                Tiles[x, y]?.Setup(x, y, this);
            }
        }
    }

    private void SetupPieces()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var selectedPiece = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)];
                var o = Instantiate(selectedPiece, new Vector3(x, y, -5), Quaternion.identity);
                o.transform.parent = transform;
                Pieces[x, y] = o.GetComponent<Piece>();
                Pieces[x, y]?.Setup(x, y, this);
            }
        }
    }

    private void PositionCamera()
    {
        float newPosx = (float)width / 2f;
        float newPosy = (float)height / 2f;

        Camera.main.transform.position = new Vector3(newPosx - 0.5f, newPosy - 0.5f + CameraVerticalOffset, -10f);

        float horizontal = (float)width + 1f;
        float vertical = (float)height / 2f + 1f;

        Camera.main.orthographicSize = (horizontal > vertical) ? horizontal + CameraSizeOffset : vertical + CameraSizeOffset;
    }

    public void TileDown(Tile tile_)
    {
        startTile = tile_;
    }

    public void TileOver(Tile tile_)
    {
        endTile = tile_;
    }

    public void TileUp(Tile tile_)
    {
        if (startTile != null && endTile != null && IsCloseTo(startTile, endTile))
        {
            SwapTiles(startTile, endTile);
        }
        startTile = null;
        endTile = null;
    }

    private void SwapTiles(Tile startTile, Tile endTile)
    {
        var startPiece = Pieces[startTile.x, startTile.y];
        var endPiece = Pieces[endTile.x, endTile.y];

        startPiece.Move(endTile.x, endTile.y);
        endPiece.Move(startTile.x, startTile.y);

        Pieces[startTile.x, startTile.y] = endPiece;
        Pieces[endTile.x, endTile.y] = startPiece;
    }

    public bool IsCloseTo(Tile startTile, Tile endTile)
    {
        // Verificar si las casillas están una al lado de la otra en el eje x
        if (Math.Abs((startTile.x - endTile.x)) == 1 && startTile.y == endTile.y)
        {
            return true;
        }

        // Verificar si las casillas están una al lado de la otra en el eje y
        if (Math.Abs((startTile.y - endTile.y)) == 1 && startTile.x == endTile.x)
        {
            return true;
        }
        return false;
    }
}
