using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    public GameObject tileObject;

    public float CameraSizeOffset;
    public float CameraVerticalOffset;

    // Start is called before the first frame update
    void Start()
    {
        SetupBoard();
        PositionCamera();
    }

    private void SetupBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var o = Instantiate(tileObject, new Vector3(x, y, -5), Quaternion.identity);
                o.transform.parent = transform;
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

    // Update is called once per frame
    void Update()
    {

    }
}
