using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;

    private Vector3 respawnPoint;
    private float lastGroundedX; // Track last grounded X position
    private float respawnOffset = 5f; // Distance before respawning
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

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(3, 3, 3);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-3, 3, 3);

        if (Input.GetKey(KeyCode.W) && IsGrounded())
        {
            Jump();
        }

        // If grounded, update last grounded position
        if (IsGrounded())
        {
            lastGroundedX = transform.position.x;
            respawnPoint = GetSafeRespawnPoint(lastGroundedX - respawnOffset);
        }

        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", IsGrounded());

        fallDetector.transform.position = new Vector2(transform.position.x, fallDetector.transform.position.y);
    }

    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, speed);
        anim.SetTrigger("jump");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "FallDetector")
        {
            transform.position = respawnPoint;
        }
    }

    private bool IsGroundedAtPosition(Vector3 position)
    {
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

    private Vector3 GetSafeRespawnPoint(float startX)
    {
        float searchDistance = 3f; // Distance to check backward
        float step = 0.5f; // Step size for checking

        for (float x = startX; x > startX - searchDistance; x -= step)
        {
            Vector3 testPosition = new Vector3(x, transform.position.y, transform.position.z);
            if (IsGroundedAtPosition(testPosition))
            {
                return testPosition;
            }
        }

        // If no ground is found, keep the last grounded position
        return new Vector3(lastGroundedX, transform.position.y, transform.position.z);
    }

    private bool IsGrounded()
    {
        return IsGroundedAtPosition(transform.position);
    }
}
