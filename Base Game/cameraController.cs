using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    private Board board;
    public float cameraOffset; // the z axis offset
    public float aspectRatio = 0.625f; // the aspect of the game res " 10/16"
    public float padding = 2; // space from the sides
    public float yOffset = 1; // to make enough space for the top UI
    
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        if (board != null)
        {
            rePositionCamera(board.width-1, board.height-1);
        }
    }

    void rePositionCamera(float x, float y)
    {
        Vector3 temp = new Vector3(x / 2, y / 2 + yOffset,cameraOffset);
        transform.position = temp;
        if (board.width > board.height)
            Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRatio;
        else
            Camera.main.orthographicSize = (board.height / 2 + padding) / aspectRatio;

    }
}
