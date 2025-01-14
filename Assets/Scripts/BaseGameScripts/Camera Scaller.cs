using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CameraScaller : MonoBehaviour
{
    Board board;
    public float cameraOffset;
    public float padding = 2;
    public float aspectRatio = 0.625f;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        if(board != null)
        {
            RepositionCamera(board.width - 1, board.height - 1);
        }
    }

    void RepositionCamera(float x, float y)
    {
        Vector3 tempPosition = new Vector3(x/2 ,y/2,cameraOffset);

        transform.position = tempPosition;

        if(board.width >= board.height)
        {

             Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRatio;

        }
        else
        {
            Camera.main.orthographicSize = board.width / 2 + padding;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
