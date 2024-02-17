using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Board : MonoBehaviour
{
    public float timeBetweenPieces = 0.001f;

    public int width;
    public int height;

    public GameObject tileObject;

    public float CameraSizeOffset;
    public float CameraVerticalOffset;

    public int PointsPerMatch;
    public GameObject[] availablePieces;

    Tile[,] Tiles; //<-- 2D array of tiles
    Piece[,] Pieces; //<-- 2D array of pieces

    Tile startTile;
    Tile endTile;

    bool swapping;

    // Start is called before the first frame update
    void Start()
    {
        Tiles = new Tile[width, height];
        Pieces = new Piece[width, height];
        SetupBoard();
        PositionCamera();
        if(GameManager.instance.gameState==GameManager.GameState.Playing){
            StartCoroutine(SetupPieces());
        }
        GameManager.instance.OnGameStateUpdated.AddListener(OnGameStateUpdated);
    }

    private void OnDestroy()
    {
        GameManager.instance.OnGameStateUpdated.RemoveListener(OnGameStateUpdated);
    }

    private void OnGameStateUpdated(GameManager.GameState newState)
    {
        if(newState==GameManager.GameState.Playing){
            StartCoroutine(SetupPieces());
        }
        if(newState==GameManager.GameState.GameOver){
            ClearBoard();
        }
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
                Tiles[x, y].Setup(x, y, this);
            }
        }
    }

    IEnumerator SetupPieces()
    {
        var maxIterations = 50;
        var currentIterations = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                yield return new WaitForSeconds(timeBetweenPieces);
                if (Pieces[x, y] != null) continue;
                currentIterations = 0;
                var newPiece = CreatePieceAt(x, y);
                while (HasPreviousMatches(x, y))
                {
                    ClearPieceAt(x, y);
                    newPiece = CreatePieceAt(x, y);
                    currentIterations++;
                    if (currentIterations > maxIterations)
                    {
                        break;
                    }
                }
            }
        }
        yield return null;
    }

    private void ClearPieceAt(int x, int y)
    {
        var pieceToClear = Pieces[x, y];
        pieceToClear.Remove(true);
        Pieces[x, y] = null;
    }

    private void ClearBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                ClearPieceAt(x, y);
            }
        }
    }
    private Piece CreatePieceAt(int x, int y)
    {
        var selectedPiece = availablePieces[UnityEngine.Random.Range(0, availablePieces.Length)];
        var o = Instantiate(selectedPiece, new Vector3(x, y + 1, -5), Quaternion.identity);
        o.transform.parent = transform;
        Pieces[x, y] = o.GetComponent<Piece>();
        Pieces[x, y].Setup(x, y, this);
        Pieces[x, y].Move(x, y);
        return Pieces[x, y];
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
        if (!swapping && GameManager.instance.gameState==GameManager.GameState.Playing) startTile = tile_;
    }

    public void TileOver(Tile tile_)
    {
        if (!swapping && GameManager.instance.gameState==GameManager.GameState.Playing) endTile = tile_;
    }

    public void TileUp(Tile tile_)
    {
        if (!swapping && GameManager.instance.gameState==GameManager.GameState.Playing)
        {
            if (startTile != null && endTile != null && IsCloseTo(startTile, endTile) && !swapping)
            {
                StartCoroutine(SwapTiles(startTile, endTile));
            }
            startTile = null;
            endTile = null;
        }
    }

    IEnumerator SwapTiles(Tile startTile, Tile endTile)
    {
        swapping = true;
        var startPiece = Pieces[startTile.x, startTile.y];
        var endPiece = Pieces[endTile.x, endTile.y];

        startPiece.Move(endTile.x, endTile.y);
        endPiece.Move(startTile.x, startTile.y);

        Pieces[startTile.x, startTile.y] = endPiece;
        Pieces[endTile.x, endTile.y] = startPiece;

        yield return new WaitForSeconds(0.6f);

        var startMatches = GetMatchByPiece(startTile.x, startTile.y, 3);
        var endMatches = GetMatchByPiece(endTile.x, endTile.y, 3);

        var allMatches = startMatches.Union(endMatches).ToList();
        //log all matches
        allMatches.ForEach(p =>
        {
            Debug.Log(p.x + " " + p.y);
        });
        if (allMatches.Count == 0)
        {
            startPiece.Move(startTile.x, startTile.y);
            endPiece.Move(endTile.x, endTile.y);
            Pieces[startTile.x, startTile.y] = startPiece;
            Pieces[endTile.x, endTile.y] = endPiece;
        }
        else
        {
            ClearPieces(allMatches);
            AwardPoints(allMatches);
        }

        startTile = null;
        endTile = null;
        swapping = false;

        yield return null;
    }

    private void ClearPieces(List<Piece> piecesToClear)
    {
        piecesToClear.ForEach(p =>
        {
            ClearPieceAt(p.x, p.y);
        });
        List<int> columns = GetColumns(piecesToClear);
        List<Piece> collapsedPieces = CollapseColumns(columns, 0.3f);
        FindMatchRecursive(collapsedPieces);
    }

    private void FindMatchRecursive(List<Piece> collapsedPieces)
    {
        StartCoroutine(FindMatchRecursiveCoroutine(collapsedPieces));
    }

    IEnumerator FindMatchRecursiveCoroutine(List<Piece> collapsedPieces)
    {
        yield return new WaitForSeconds(1f);
        List<Piece> newMatches = new List<Piece>();
        collapsedPieces.ForEach(piece =>
        {
            var matches = GetMatchByPiece(piece.x, piece.y, 3);
            if (matches != null)
            {
                newMatches = newMatches.Union(matches).ToList();
                ClearPieces(matches);
                AwardPoints(matches);
            }
        });
        if (newMatches.Count > 0)
        {
            var newCollapsedPieces = CollapseColumns(GetColumns(newMatches), 0.001f);
            FindMatchRecursive(newCollapsedPieces);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(SetupPieces());
            swapping = false;
        }
        yield return null;
    }

    private List<int> GetColumns(List<Piece> piecesToClear)
    {
        var result = new List<int>();
        piecesToClear.ForEach(p =>
        {
            if (!result.Contains(p.x))
            {
                result.Add(p.x);
            }
        });
        return result;
    }

    private List<Piece> CollapseColumns(List<int> columns, float timeToCollapse)
    {
        List<Piece> movingPieces = new List<Piece>();

        for (int i = 0; i < columns.Count; i++)
        {
            var column = columns[i];
            for (int y = 0; y < height; y++)
            {
                if (Pieces[column, y] == null)
                {
                    for (int y2 = y + 1; y2 < height; y2++)
                    {
                        if (Pieces[column, y2] != null)
                        {
                            Pieces[column, y2].Move(column, y);
                            Pieces[column, y] = Pieces[column, y2];
                            if (!movingPieces.Contains(Pieces[column, y]))
                            {
                                movingPieces.Add(Pieces[column, y]);
                            }
                            Pieces[column, y2] = null;
                            break;
                        }
                    }
                }
            }
        }
        return movingPieces;
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

    public List<Piece> GetMatchByDirection(int xPos, int yPos, Vector2 direction, int minPieces = 3)
    {
        List<Piece> matches = new List<Piece>();
        Piece startPiece = Pieces[xPos, yPos];
        matches.Add(startPiece);

        int nextX = xPos + (int)direction.x;
        int nextY = yPos + (int)direction.y;
        int maxVal = width > height ? width : height;

        for (int i = 1; i < maxVal; i++)
        {
            nextX = xPos + (int)direction.x * i;
            nextY = yPos + (int)direction.y * i;

            if (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height)
            {
                var nextPiece = Pieces[nextX, nextY];
                if (nextPiece != null && nextPiece.pieceType == startPiece.pieceType)
                {
                    matches.Add(nextPiece);
                }
                else
                {
                    break;
                }
            }
        }
        if (matches.Count >= minPieces)
        {
            return matches;
        }
        return null;
    }

    public List<Piece> GetMatchByPiece(int xPos, int yPos, int minPieces = 3)
    {
        var upMatchs = GetMatchByDirection(xPos, yPos, new Vector2(0, 1), 2);
        var downMatchs = GetMatchByDirection(xPos, yPos, new Vector2(0, -1), 2);
        var leftMatchs = GetMatchByDirection(xPos, yPos, new Vector2(-1, 0), 2);
        var rightMatchs = GetMatchByDirection(xPos, yPos, new Vector2(1, 0), 2);

        if (upMatchs == null) upMatchs = new List<Piece>();
        if (downMatchs == null) downMatchs = new List<Piece>();
        if (leftMatchs == null) leftMatchs = new List<Piece>();
        if (rightMatchs == null) rightMatchs = new List<Piece>();

        var verticalMatches = upMatchs.Union(downMatchs).ToList();
        var horizontalMatches = leftMatchs.Union(rightMatchs).ToList();

        var foundMatches = new List<Piece>();

        if (verticalMatches.Count >= minPieces)
        {
            foundMatches = foundMatches.Union(verticalMatches).ToList();
        }

        if (horizontalMatches.Count >= minPieces)
        {
            foundMatches = foundMatches.Union(horizontalMatches).ToList();
        }

        return foundMatches;
    }

    private bool HasPreviousMatches(int xPos, int yPos)
    {
        var downMatchs = GetMatchByDirection(xPos, yPos, new Vector2(0, -1), 2);
        var leftMatchs = GetMatchByDirection(xPos, yPos, new Vector2(-1, 0), 2);

        if (downMatchs == null) downMatchs = new List<Piece>();
        if (leftMatchs == null) leftMatchs = new List<Piece>();

        return downMatchs.Count > 0 || leftMatchs.Count > 0;
    }

    public void AwardPoints(List<Piece> allMatches)
    {
        GameManager.instance.AddPoints(allMatches.Count * PointsPerMatch);
    }
}
