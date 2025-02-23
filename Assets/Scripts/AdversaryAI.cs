using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdversaryAI : MonoBehaviour
{
    public Node currentNode;
    public List<Node> path = new List<Node>();

    public float speed = 5f;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;
    public GameObject fallDetector;
    public float gapThreshold = 20f; // Define o tamanho do buraco que ele não consegue pular sozinho

    public float gapPlatform = 6.5f;
    public LayerMask movingPlatformLayer; // Layer para detectar plataformas móveis

    private Animator anim;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isStuck;
    private float stuckTimer;
    private Vector2 lastPosition;
    private Vector3 respawnPoint;
    private MovingPlatform currentPlatform = null;
    private BoxCollider2D boxCollider;
    public float distanceTraveled = 0f;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (currentNode == null)
        {
            currentNode = FindClosestNode();
        }

        anim = GetComponent<Animator>();
        lastPosition = transform.position;
        boxCollider = GetComponent<BoxCollider2D>();

    }
    private void FixedUpdate()
    {
        if (path?.Count > 0)
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

        if (currentPlatform != null)
        {
            transform.position += currentPlatform.GetPlatformVelocity() * Time.deltaTime;
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

        // Verifica se o adversário caiu do mapa
        if (transform.position.y < -10f) // Ajuste esse valor conforme necessário
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        transform.position = respawnPoint;
        rb.velocity = Vector2.zero; // Reseta a velocidade para evitar deslizes indesejados
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
        if (path?.Count > 0)
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
            if (!ShouldMoveTowardsPlatform(target))
            {
                anim.SetBool("IsRunning", false);
                return; // Aguarda até a plataforma chegar perto
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundLayer | movingPlatformLayer);
        isGrounded = hit.collider != null;

        bool groundedOnPlatform = false;

        // Check if player is touching a platform
        Collider2D hitPlatform = Physics2D.OverlapBox(
            boxCollider.bounds.center,
            boxCollider.bounds.size * 0.9f,
            0f,
            LayerMask.GetMask("MovingPlatform") // Ensure the platform layer exists
        );

        if (isGrounded)
        {
            isGrounded = hitPlatform == null;
        }

        if (hitPlatform != null)
        {
            groundedOnPlatform = hitPlatform.CompareTag("MovingPlatform");

            if (groundedOnPlatform)
            {
                rb.velocity = Vector2.zero; // Para o movimento
                anim.SetBool("IsRunning", false); // Para a animação de corrida
            }
        }
        else
        {
            currentPlatform = null;
            transform.parent = null; // Remove o vínculo ao sair da plataforma
        }

        anim.SetBool("IsJumping", !isGrounded);
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

    private bool ShouldMoveTowardsPlatform(Node target)
    {
        float gapSize = Mathf.Abs(target.transform.position.x - transform.position.x);

        //Debug.Log("gapSize: " + gapSize);
        if (gapSize > gapThreshold)
        {
            Vector2 detectionOrigin = transform.position + Vector3.right * 10f;
            float detectionRadius = 13f;

            Collider2D platformNearby = Physics2D.OverlapCircle(detectionOrigin, detectionRadius, movingPlatformLayer);

            if (platformNearby != null)
            {
                MovingPlatform platform;
                if (platformNearby.TryGetComponent<MovingPlatform>(out platform))
                {
                    float distanceToPlatform = Mathf.Abs(platform.transform.position.x - transform.position.x);

                    if (distanceToPlatform < gapPlatform)
                    {
                        return true; // Continua andando para subir na plataforma
                    }
                }

                return false; // Para de andar e espera a plataforma chegar
            }
        }

        return true; // Caso não haja um buraco, continua andando normalmente
    }

    public System.Collections.IEnumerator ApplyTemporaryBuff(float multiplier, float duration)
    {
        float originalSpeed = speed;
        speed *= multiplier;
        yield return new WaitForSeconds(duration);
        speed = originalSpeed;
    }


}
