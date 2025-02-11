using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;

    private Vector3 respawnPoint;
    public GameObject fallDetector;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        respawnPoint = transform.position; // Initial respawn point
    }

    private void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
    }

    private void Update() {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(3, 3, 3);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-3, 3, 3);

        bool grounded = IsGrounded();
        if (Input.GetKey(KeyCode.W) && grounded)
        {
            Jump();
        }

        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", grounded);

        if (grounded) {
            anim.ResetTrigger("jump");
        }

        fallDetector.transform.position = new Vector2(transform.position.x, fallDetector.transform.position.y);
    }


    private void Jump() {
        body.velocity = new Vector2(body.velocity.x, speed);
        anim.SetTrigger("jump");
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("FallDetector")) {
            transform.position = respawnPoint;
        }
        else if (collision.CompareTag("Checkpoint")) {
            respawnPoint = collision.transform.position;
        }
    }

    private bool IsGroundedAtPosition(Vector3 position) {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            new Vector2(position.x, boxCollider.bounds.center.y),
            boxCollider.bounds.size * 0.9f,
            0f,
            Vector2.down,
            0.2f,
            groundLayer
        );

        return raycastHit.collider != null;
    }

    private bool IsGrounded() {
        bool groundedOnTerrain = IsGroundedAtPosition(transform.position);
        bool groundedOnPlatform = false;

        // Check if player is touching a platform
        Collider2D hit = Physics2D.OverlapBox(
            boxCollider.bounds.center, 
            boxCollider.bounds.size * 0.9f, 
            0f, 
            LayerMask.GetMask("MovingPlatform") // Ensure the platform layer exists
        );

        if (hit != null)
            groundedOnPlatform = hit.CompareTag("MovingPlatform");

        return groundedOnTerrain || groundedOnPlatform;
    }

}
