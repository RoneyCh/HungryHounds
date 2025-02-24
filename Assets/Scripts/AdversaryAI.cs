using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdversaryAI : CompetitorBase
{
    public Node currentNode;
    public List<Node> path = new List<Node>();

    [SerializeField] private float jumpForce = 10f;

    public LayerMask groundLayer;
    public LayerMask obstacleLayer;
    public LayerMask playerLayer;
    public GameObject fallDetector;
    public float gapThresholdPlatform = 3f;
    public LayerMask movingPlatformLayer;
    public Transform startPointPlatform;
    public Transform endPointPlatform;
    public float minSpeed = 4f;
    public float maxSpeed = 7f;
    private Animator anim;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isStuck;
    private float stuckTimer;
    private Vector2 lastPosition;
    private Vector3 respawnPoint;
    private MovingPlatform currentPlatform = null;
    private BoxCollider2D boxCollider;
    private float stuckTime = 0f;
    private float lastXPosition = 0f;


    private bool waitingOnPlatform = false;
    private bool waitingOnStartPoint = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (currentNode == null)
        {
            currentNode = AStarManager.instance.FindNearestNode(transform.position);
        }

        anim = GetComponent<Animator>();
        lastPosition = transform.position;
        boxCollider = GetComponent<BoxCollider2D>();

    }
    private void FixedUpdate()
    {
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);

        if (path?.Count > 0)
        {
            Node targetNode = path[0];

            if (ShouldJumpGap(targetNode) || ShouldJump(targetNode))
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

            if (IsStuckAndTargetAbove(targetNode))
            {
                currentNode = AStarManager.instance.FindNearestNode(transform.position);
                StartCoroutine(RecalculatePath());
            }
        }
    }

    private void Update()
    {
        distanceTraveled = transform.position.x;
        bool isMoving = Mathf.Abs(transform.position.x - lastPosition.x) > 0.01f;

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

        if (transform.position.y < -10f)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        transform.position = respawnPoint;
        rb.velocity = Vector2.zero;
        currentNode = AStarManager.instance.FindNearestNode(transform.position);
        StartCoroutine(RecalculatePath());
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
            waitingOnPlatform = false;
            waitingOnStartPoint = false;
        }
        else if (collision.CompareTag("StartPointPlatform"))
        {
            waitingOnStartPoint = true;
        }
        else if (collision.CompareTag("EndPointPlatform"))
        {
            waitingOnPlatform = false;
            MoveTowards(path[0]);
            Jump();
        }
    }

    private void CreatePath()
    {
        if (path?.Count > 0)
        {
            int x = 0;
            Node targetNode = path[x];

            if (ShouldJumpGap(targetNode) || ShouldJump(targetNode))
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
        if (waitingOnPlatform) return;

        if (!isStuck)
        {
            if (!ShouldMoveTowardsPlatform())
            {
                anim.SetBool("IsRunning", false);
                return;
            }

            transform.position = Vector2.MoveTowards(transform.position,
                new Vector3(target.transform.position.x, transform.position.y, -2),
                speed * Time.deltaTime);
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
        RaycastHit2D groundPlatformCheck = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, movingPlatformLayer);

        if (!groundCheck.collider && groundPlatformCheck.collider == null)
        {
            return true;
        }

        return false;
    }

    private void Jump()
    {
        if (isGrounded && !waitingOnPlatform)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }


    private void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundLayer);
        RaycastHit2D hitPlatform = Physics2D.BoxCast(
            new Vector2(transform.position.x, boxCollider.bounds.center.y),
            boxCollider.bounds.size * 0.9f,
            0f,
            Vector2.down,
            0.1f,
            movingPlatformLayer
        );

        RaycastHit2D hitEndPointPlatform = Physics2D.BoxCast(
            new Vector2(transform.position.x, boxCollider.bounds.center.y),
            boxCollider.bounds.size * 0.9f,
            0f,
            Vector2.down,
            0.1f,
            LayerMask.GetMask("EndPointPlatform")
        );

        isGrounded = hit.collider != null || hitPlatform.collider != null;

        if (hitPlatform.collider != null && !waitingOnPlatform && hitEndPointPlatform.collider == null)
        {
            waitingOnPlatform = true;
            rb.velocity = Vector2.zero;
        }
        else if (hitPlatform.collider == null)
        {
            currentPlatform = null;
            transform.parent = null;
        }

        if (waitingOnPlatform)
        {
            anim.SetBool("IsRunning", false);
        }
        anim.SetBool("IsJumping", !isGrounded);
    }



    private void CheckIfStuck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 0.6f, groundLayer);
        if (hit.collider != null)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > 1.5f)
            {
                Debug.Log("Stuck! Recalculating path...");
                isStuck = true;
                StartCoroutine(RecalculatePath());
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
        Vector2 moveBackDirection = -transform.right * 0.7f;
        transform.position = new Vector2(transform.position.x + moveBackDirection.x, transform.position.y);

        yield return new WaitForSeconds(0.3f);

        Node[] nodes = FindObjectsOfType<Node>();
        if (nodes.Length > 0)
        {
            path = AStarManager.instance.GeneratePath(currentNode, nodes[Random.Range(0, nodes.Length)]);
        }
    }

    private bool ShouldMoveTowardsPlatform()
    {
        if (waitingOnStartPoint)
        {
            Vector2 detectionOrigin = transform.position + Vector3.right * 6f;
            float detectionRadius = 3f;

            Collider2D platformNearby = Physics2D.OverlapCircle(detectionOrigin, detectionRadius, movingPlatformLayer);

            if (platformNearby != null && (platformNearby.transform.position.x - transform.position.x) > gapThresholdPlatform)
            {
                waitingOnStartPoint = false;
                return true;
            }
            return false;
        }
        return true;
    }

    public System.Collections.IEnumerator ApplyTemporaryBuff(float multiplier, float duration)
    {
        float originalSpeed = speed;
        speed *= multiplier;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        yield return new WaitForSeconds(duration);
        speed = originalSpeed;
    }

    private bool IsStuckAndTargetAbove(Node targetNode)
    {
        if (targetNode == null) return false;

        if (Mathf.Abs(transform.position.x - lastXPosition) < 0.01f)
        {
            stuckTime += Time.deltaTime;
        }
        else
        {
            stuckTime = 0f;
            lastXPosition = transform.position.x;
        }

        return stuckTime >= 1f && targetNode.transform.position.y > transform.position.y;
    }

    public void TakeDamage()
    {
        anim.SetTrigger("IsHurt");

    }
    public void stopAdversary()
    {
        StopAllCoroutines();
        rb.velocity = Vector2.zero;
        anim.SetBool("IsRunning", false);
        anim.SetBool("IsJumping", false);
    }
}