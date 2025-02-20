using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdversaryAI : MonoBehaviour
{
    public Node currentNode;
    public List<Node> path = new List<Node>();

    public float speed = 3f;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;
    public GameObject fallDetector;
    private Animator anim;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isStuck;
    private float stuckTimer;
    private Vector2 lastPosition;
    private Vector3 respawnPoint;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (currentNode == null)
        {
            currentNode = FindClosestNode();
        }

        anim = GetComponent<Animator>();
        lastPosition = transform.position;
    }
    private void FixedUpdate()
    {
        if (path.Count > 0)
        {
            Node targetNode = path[0];

            if (ShouldJump(targetNode) || ShouldJumpGap(targetNode))
            {
                Jump();
            }

            MoveTowards(targetNode);


            if (transform.position.y > targetNode.transform.position.y + 0.1f &&
                Mathf.Abs(transform.position.x - targetNode.transform.position.x) < 0.3f)
            {
                currentNode = targetNode;
                path.RemoveAt(0);
            }

            else if (Vector2.Distance(transform.position, targetNode.transform.position) < 0.1f)
            {
                currentNode = targetNode;
                path.RemoveAt(0);
            }
        }
    }


    private void Update()
    {

        bool isMoving = Mathf.Abs(transform.position.x - lastPosition.x) > 0.1f;


        if (isMoving)
        {
            transform.localScale = new Vector3(Mathf.Sign(transform.position.x - lastPosition.x) * 3, 3, 3);
        }


        anim.SetBool("IsRunning", isMoving);


        lastPosition = transform.position;

        CheckGrounded();
        CheckIfStuck();
        CreatePath();

        fallDetector.transform.position = new Vector2(transform.position.x, fallDetector.transform.position.y);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FallDetector"))
        {
            transform.position = respawnPoint;
        }
        else if (collision.CompareTag("Checkpoint"))
        {
            respawnPoint = collision.transform.position;
        }
    }

    private void CreatePath()
    {
        if (path.Count > 0)
        {
            int x = 0;
            Node targetNode = path[x];

            if (ShouldJump(targetNode) || ShouldJumpGap(targetNode))
            {
                Jump();
            }

            if (Vector2.Distance(transform.position, targetNode.transform.position) < 0.1f)
            {
                currentNode = targetNode;
                path.RemoveAt(x);
            }
        }
        else
        {
            Node[] nodes = FindObjectsOfType<Node>();
            if (nodes.Length > 0)
            {
                path = AStarManager.instance.GeneratePath(currentNode, nodes[Random.Range(0, nodes.Length)]);
            }
        }
    }

    private void MoveTowards(Node target)
    {
        if (!isStuck)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector3(target.transform.position.x, transform.position.y, -2), speed * Time.deltaTime);
        }
    }

    private bool ShouldJump(Node target)
    {
        if (target.transform.position.y > currentNode.transform.position.y + 0.5f)
        {
            return true;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * Mathf.Sign(target.transform.position.x - transform.position.x), 1f, obstacleLayer);
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

    private bool ShouldJumpGap(Node target)
    {

        Vector2 groundCheckPos = new Vector2(transform.position.x + Mathf.Sign(target.transform.position.x - transform.position.x) * 0.6f, transform.position.y - 0.5f);
        RaycastHit2D groundCheck = Physics2D.Raycast(groundCheckPos, Vector2.down, 1f, groundLayer);


        if (!groundCheck.collider)
        {
            return true;
        }

        return false;
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, speed);
        }
    }

    private void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundLayer);
        isGrounded = hit.collider != null;
    }

    private void CheckIfStuck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 0.6f, groundLayer);
        if (hit)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > 1.5f)
            {
                isStuck = true;
                RecalculatePath();
            }
        }
        else
        {
            stuckTimer = 0;
            isStuck = false;
        }

        lastPosition = transform.position;
    }

    private IEnumerator RecalculatePath()
    {
        Debug.Log("O adversário ficou preso! Tentando se mover para trás...");

        Vector2 moveBackDirection = -transform.right * 0.5f;
        transform.position = new Vector2(transform.position.x + moveBackDirection.x, transform.position.y);

        yield return new WaitForSeconds(0.3f);

        Node[] nodes = FindObjectsOfType<Node>();
        if (nodes.Length > 0)
        {
            path = AStarManager.instance.GeneratePath(currentNode, nodes[Random.Range(0, nodes.Length)]);
        }
    }

    private Node FindClosestNode()
    {
        Node[] nodes = FindObjectsOfType<Node>();
        Node closest = null;
        float minDist = Mathf.Infinity;

        foreach (Node node in nodes)
        {
            float dist = Vector2.Distance(transform.position, node.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }

        return closest;
    }
}
