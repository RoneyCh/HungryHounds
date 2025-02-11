using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 3f;
    public float x1 = 201.7f;
    public float x2 = 220.5f;

    private int direction = 1;
    private bool playerOnPlatform = false;
    private Transform player;

    private void Update()
    {
        // Move platform
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        if (transform.position.x >= x2)
        {
            direction = -1;
        }
        else if (transform.position.x <= x1)
        {
            direction = 1;
        }

        // Move player manually without setting it as a child
        if (playerOnPlatform && player != null)
        {
            player.position += Vector3.right * direction * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerOnPlatform = true;
            player = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerOnPlatform = false;
            player = null;
        }
    }
}
