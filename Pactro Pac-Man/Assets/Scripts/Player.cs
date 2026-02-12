using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed = 4.0f;

    private Vector2 direction = Vector2.zero;
    private Vector2 nextDirection;
    private Animator animator;
    public Animator goldAnimator;
    public GameObject goldKnight;
    public GameObject deadKnight;
    public GameObject knight;

    [Header("Power Mode")]
    public float powerDuration = 10f;
    public float flickerStartTime = 3f;
    public float flickerInterval = 0.15f;

    private float powerTimer = 0f;
    private float flickerTimer = 0f;
    private bool isPowered = false;
    private bool isFlickering = false;

    private SpriteRenderer knightRenderer;
    private SpriteRenderer goldRenderer;
    private float startingScale;

    private Node currentNode, previousNode, targetNode;

    // Start is called before the first frame update
    void Start()
    {
        knightRenderer = knight.GetComponent<SpriteRenderer>();
        goldRenderer = goldKnight.GetComponent<SpriteRenderer>();

        goldRenderer.enabled = false; // start white
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
        animator = GetComponent<Animator>();
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

    public void ActivateGoldKnightro()
    {
        powerTimer = powerDuration;
        isPowered = true;
        isFlickering = false;
        flickerTimer = 0f;

        knightRenderer.enabled = false;
        goldRenderer.enabled = true;
    }
    public void DeactivateGoldKnightro()
    {
        isPowered = false;
        isFlickering = false;

        knightRenderer.enabled = true;
        goldRenderer.enabled = false;
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
                    else if (tile.isSuperPellet)
                    {
                        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
                        foreach (GameObject go in ghosts)
                        {
                            go.GetComponent<Ghost>().StartFrightenedMode();
                            ActivateGoldKnightro();
                        }
                    }

                    
                }
            }
        }
    }
    void UpdatePowerMode()
    {
        if (!isPowered)
            return;

        powerTimer -= Time.deltaTime;

        if (powerTimer <= flickerStartTime && !isFlickering)
        {
            isFlickering = true;
        }

        if (isFlickering)
        {
            flickerTimer += Time.deltaTime;

            if (flickerTimer >= flickerInterval)
            {
                flickerTimer = 0f;

                bool goldVisible = goldRenderer.enabled;

                goldRenderer.enabled = !goldVisible;
                knightRenderer.enabled = goldVisible;
            }
        }

        if (powerTimer <= 0f)
        {
            DeactivateGoldKnightro();
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

        UpdatePowerMode();

        Animate();
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

    public void Died()
    {
        knight.SetActive(false);
        deadKnight.SetActive(true);
        if (direction == Vector2.left)
        {
            deadKnight.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
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

    private void Animate()
    {
        animator.SetInteger("State", 0);
        goldAnimator.SetInteger("State", 0);
        if (direction == Vector2.left)
        {
            animator.SetInteger("State", 4);
            goldAnimator.SetInteger("State", 4);
        }
        else if (direction == Vector2.right)
        {
            animator.SetInteger("State", 2);
            goldAnimator.SetInteger("State", 2);
        }
        else if(direction == Vector2.up)
        {
            animator.SetInteger("State", 1);
            goldAnimator.SetInteger("State", 1);
        }
        else if(direction == Vector2.down)
        {
            animator.SetInteger("State", 3);
            goldAnimator.SetInteger("State", 3);
        }
    }

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
