using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed = 4.0f;

    private Vector2 direction = Vector2.zero;
    private Vector2 nextDirection;

    private float startingScale;

    private Node currentNode, previousNode, targetNode;

    // Start is called before the first frame update
    void Start()
    {
        Node node = GetNodeAtPosition(transform.localPosition);

        if (node != null)
        {
            currentNode = node;
            Debug.Log("Player node found: " + node.name);
        }
        else
        {
            Debug.LogError("NO NODE FOUND FOR PLAYER");
        }
        startingScale = transform.localScale.x;
        direction = Vector2.left;
        ChangePosition(direction);
    }

    Node CanMove(Vector2 d)
    {
        Node moveToNode = null;
        for (int i = 0; i < currentNode.neighbors.Length; i++)
        {
            if (currentNode.validDirections[i] == d)
            {
                moveToNode = currentNode.neighbors[i];
                break;
            }
        }

        return moveToNode;
    }

    void ConsumePellet()
    {
        GameObject o = GetTileAtPosition(transform.position);
        if (o != null)
        {
            Tile tile = o.GetComponent<Tile>();

            if (tile != null)
            {
                if (!tile.didConsume && (tile.isPellet || tile.isSuperPellet))
                {
                    o.GetComponent<SpriteRenderer>().enabled = false;
                    tile.didConsume = true;
                    if (tile.isPellet)
                    {
                        GameObject.Find("Game").GetComponent<GameBoard>().AddOneScore();
                    }
                }
            }
        }
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            ChangePosition(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            ChangePosition(Vector2.right);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            ChangePosition(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            ChangePosition(Vector2.down);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();

        Move();

        ConsumePellet();
        //Rotate();
    }

    void ChangePosition(Vector2 d)
    {
        if (d != direction)
        {
            nextDirection = d;
        }

        if (currentNode != null)
        {
            Node moveToNode = CanMove(d);

            if (moveToNode != null)
            {
                direction = d;
                targetNode = moveToNode;
                previousNode = currentNode;
                currentNode = null;
            }
        }
    }

    GameObject GetTileAtPosition(Vector2 pos)
    {
        int x = Mathf.RoundToInt(pos.x);
        int y = Mathf.RoundToInt(pos.y);

        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().tiles[x, y];

        return tile;
    }

    private void Move()
    {
        if (targetNode != currentNode && targetNode != null)
        {
            if (OverShotTarget())
            {
                currentNode = targetNode;
                transform.localPosition = currentNode.transform.position;

                Node moveToNode = CanMove(nextDirection);

                if (moveToNode != null)
                {
                    direction = nextDirection;
                }

                if (moveToNode == null)
                {
                    moveToNode = CanMove(direction);
                }

                if (moveToNode != null)
                {
                    targetNode = moveToNode;
                    previousNode = currentNode;
                    currentNode = null;
                }
                else
                {
                    direction = Vector2.zero;
                }
            }
            else
            {
                transform.localPosition += (Vector3)(direction * speed) * Time.deltaTime;
            }
        }

    }

    void MoveToNode(Vector2 d)
    {
        Node moveToNode = CanMove(d);

        if (moveToNode != null)
        {
            transform.localPosition = moveToNode.transform.position;
            currentNode = moveToNode;
        }
    }
    /*
    private void Rotate()
    {
        if(direction == Vector2.left)
        {
            transform.localScale = new Vector3(-startingScale, startingScale, startingScale);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction == Vector2.right)
        {
            transform.localScale = new Vector3(startingScale, startingScale, startingScale);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if(direction == Vector2.up)
        {
            transform.localScale = new Vector3(startingScale, startingScale, startingScale);
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if(direction == Vector2.down)
        {
            transform.localScale = new Vector3(startingScale, startingScale, startingScale);
            transform.localRotation = Quaternion.Euler(0, 0, 270);
        }
    }
    */
    Node GetNodeAtPosition(Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];

        if (tile != null)
        {
            return tile.GetComponent<Node>();
        }

        return null;
    }

    bool OverShotTarget()
    {
        float nodeToTarget = LengthFromPreviousNode(targetNode.transform.position);
        float nodeToSelf = LengthFromPreviousNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }

    float LengthFromPreviousNode(Vector2 targetPosition)
    {
        Vector2 length = targetPosition - (Vector2)previousNode.transform.position;
        return length.sqrMagnitude;
    }
}
