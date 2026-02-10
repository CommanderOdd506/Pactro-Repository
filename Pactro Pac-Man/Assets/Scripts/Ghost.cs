using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float moveSpeed = 3.9f;
    public float frightenedModeMoveSpeed = 1.9f;

    public Node startingPosition;
    public Node homeNode;

    public int frightenedModeDuration = 10;

    public int scatterModeTimer1 = 7;
    public int chaseModeTimer1 = 20;
    public int scatterModeTimer2 = 7;
    public int chaseModeTimer2 = 20;
    public int scatterModeTimer3 = 5;
    public int chaseModeTimer3 = 20;
    public int scatterModeTimer4 = 5;

    private int modeChangeIteration = 1;
    private float modeChangeTimer = 0;

    private float frightenedModeTimer = 0;

    private float previousMoveSpeed;

    public enum Mode
    {
        Chase,
        Scatter,
        Frightened
    }

    Mode currentMode = Mode.Scatter;
    Mode previousMode;

    private GameObject player;

    private Node currentNode, targetNode, previousNode;
    private Vector2 direction, nextDirection;

    void OnTriggerEnter2D(Collider2D colldier)
    {
        
        GameObject hit = colldier.gameObject;
        Debug.Log("GameObject1 collided with " + hit.name);
        if (hit.CompareTag("Player"))
        {
            if (currentMode != Mode.Frightened)
            {
                Debug.Log("Player Died");
                player.GetComponent<Player>().Died();
                PauseMenu.instance.LoseGame();
            }
           
            
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        Node node = GetNodeAtPosition(transform.localPosition);

        if (node != null)
        {
            currentNode = node;
            previousNode = node;
        }

        // Instead of jumping straight to Pac-Man's node, pick the next node toward him
        targetNode = ChooseNextNode();

        direction = targetNode.transform.position - currentNode.transform.position;
        direction.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        ModeUpdate();
        Move();
    }
    void Move()
    {
        if(targetNode != currentNode && targetNode != null)
        {
            if (OverShotTarget())
            {
                previousNode = currentNode;
                currentNode = targetNode;

                transform.localPosition = currentNode.transform.position;

                targetNode = ChooseNextNode();

                if (targetNode == null)
                {
                    // fallback: just keep moving in same direction
                    targetNode = previousNode;
                }
            }
            else
            {
                transform.localPosition += (Vector3)direction * moveSpeed * Time.deltaTime;
            }
        }
    }

    void ModeUpdate()
    {
        if (currentMode != Mode.Frightened) {
            
                modeChangeTimer += Time.deltaTime;

                if (modeChangeIteration == 1)
                {
                    if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer1)
                    {
                        ChangeMode(Mode.Chase);
                        modeChangeTimer = 0;
                    }

                    if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer1)
                    {
                        modeChangeIteration = 2;
                        ChangeMode(Mode.Scatter);
                        modeChangeTimer = 0;
                    }
                }
                else if (modeChangeIteration == 2)
                {
                    if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer2)
                    {
                        ChangeMode(Mode.Chase);
                        modeChangeTimer = 0;
                    }

                    if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer2)
                    {
                        modeChangeIteration = 3;
                        ChangeMode(Mode.Scatter);
                        modeChangeTimer = 0;
                    }
                }
                else if (modeChangeIteration == 3)
                {
                    if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer3)
                    {
                        ChangeMode(Mode.Chase);
                        modeChangeTimer = 0;
                    }

                    if (currentMode == Mode.Chase && modeChangeTimer > chaseModeTimer3)
                    {
                        modeChangeIteration = 4;
                        ChangeMode(Mode.Scatter);
                        modeChangeTimer = 0;
                    }
                }
                else if (modeChangeIteration == 4)
                {
                    if (currentMode == Mode.Scatter && modeChangeTimer > scatterModeTimer4)
                    {
                        ChangeMode(Mode.Chase);
                        modeChangeTimer = 0;
                    }

                }
            

        }
        else if (currentMode == Mode.Frightened)
        {
            frightenedModeTimer += Time.deltaTime;
            if (frightenedModeTimer >= frightenedModeDuration)
            {
                frightenedModeTimer = 0;
                ChangeMode(previousMode);
                player.GetComponent<Player>().DeactivateGoldKnightro();
            }
        }
    }

    void ChangeMode(Mode m)
    {
        if (currentMode == m)
        {
            if (currentMode == Mode.Frightened)
            {
                frightenedModeTimer = 0;
            }
            return;
        }

        if (currentMode == Mode.Frightened)
        {
            moveSpeed = previousMoveSpeed;
            
        }

        if(m  == Mode.Frightened)
        {
            previousMoveSpeed = moveSpeed;
            moveSpeed = frightenedModeMoveSpeed;
        }
        previousMode = currentMode;
        currentMode = m;


    }

    public void StartFrightenedMode()
    {
        Debug.Log("Ghost frightened");
        ChangeMode(Mode.Frightened);
    }

    Node ChooseNextNode()
    {
        Vector2 targetTile = Vector2.zero;
        if (currentMode == Mode.Chase)
        {
            targetTile = new Vector2(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y));
        }
        else if(currentMode == Mode.Scatter || currentMode == Mode.Frightened)
        {
            targetTile = homeNode.transform.position;
        }

        Node moveToNode = null;
        Vector2[] directions = currentNode.validDirections;
        Node[] neighbors = currentNode.neighbors;

        float leastDistance = float.MaxValue;

        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] != null && neighbors[i] != previousNode)
            {
                float distance = GetDistance(neighbors[i].transform.position, targetTile);
                if (distance < leastDistance)
                {
                    leastDistance = distance;
                    moveToNode = neighbors[i];
                    direction = directions[i];
                }
            }
        }

        if (moveToNode == null)
        {
            moveToNode = previousNode; // fallback
        }

        return moveToNode;
    }


    Node GetNodeAtPosition(Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];

        if (tile != null)
        {
            if (tile.GetComponent<Node>() != null)
            {
                return tile.GetComponent<Node>();
            }
        }
        return null;
    }

    bool OverShotTarget()
    {
        float nodeToTarget = LengthFromPreviousNode(targetNode.transform.position);
        float nodeToSelf = LengthFromPreviousNode(transform.position);

        return nodeToSelf > nodeToTarget;
    }

    float LengthFromPreviousNode(Vector2 targetPosition)
    {
        Vector2 length = targetPosition - (Vector2)previousNode.transform.position;
        return length.sqrMagnitude;
    }

    float GetDistance(Vector2 posA, Vector2 posB)
    {
        float dx = posA.x - posB.x;
        float dy = posA.y - posB.y; 
        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        return distance;
    }
}
