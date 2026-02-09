using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public static int boardWidth = 28;
    public static int boardHeight = 36;
    int count = 0;

    public GameObject[,] board = new GameObject[boardWidth, boardHeight]; 
    // Start is called before the first frame update
    void Start()
    {
        Node[] nodes = FindObjectsOfType<Node>();

        foreach (Node node in nodes)
        {
            Vector2 pos = node.transform.position;

            int x = Mathf.RoundToInt(pos.x);
            int y = Mathf.RoundToInt(pos.y);

            if (x < 0 || x >= boardWidth || y < 0 || y >= boardHeight)
                continue;

            board[x, y] = node.gameObject;
            count++;
        }

        Debug.Log("Tiles added to board: " + count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
