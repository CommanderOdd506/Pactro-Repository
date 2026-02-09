using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public static int boardWidth = 28;
    public static int boardHeight = 36;
    int count = 0;
    public int score = 0;
    private int pellets;

    public GameObject[,] board = new GameObject[boardWidth, boardHeight];
    // Start is called before the first frame update
    public GameObject[,] tiles = new GameObject[boardWidth, boardHeight];

    void Start()
    {
        // Collect nodes as before
        Node[] nodes = FindObjectsOfType<Node>();
        foreach (Node node in nodes)
        {
            Vector2 pos = node.transform.position;
            int x = Mathf.RoundToInt(pos.x);
            int y = Mathf.RoundToInt(pos.y);
            if (x < 0 || x >= boardWidth || y < 0 || y >= boardHeight)
                continue;
            board[x, y] = node.gameObject;
        }

        // Collect tiles separately
        Tile[] allTiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in allTiles)
        {
            Vector2 pos = tile.transform.position;
            int x = Mathf.RoundToInt(pos.x);
            int y = Mathf.RoundToInt(pos.y);
            if (x < 0 || x >= boardWidth || y < 0 || y >= boardHeight)
                continue;
            tiles[x, y] = tile.gameObject;
            if (tile.isPellet)
            {
                pellets++;
            }
        }
    }

    public void AddOneScore()
    {
        score++;
        Debug.Log("Score: " + score);
        if (score >= pellets)
        {

            Debug.Log("Won Level");
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
