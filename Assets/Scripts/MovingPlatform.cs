using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 3f;
    public float x1 = 201.7f;
    public float x2 = 220.5f;

    private int direction = 1;
    private bool playerOnPlatform = false;
    private bool adversaryOnPlatform = false;
    private Transform player;
    private Transform dog2;
    private Vector3 velocity;

    private void Update()
    {
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        if (transform.position.x >= x2)
        {
            direction = -1;
        }
        else if (transform.position.x <= x1)
        {
            direction = 1;
        }

        if (playerOnPlatform && player != null)
        {
            player.position += Vector3.right * direction * speed * Time.deltaTime;
        }

        if (adversaryOnPlatform && dog2 != null)
        {
            dog2.position += Vector3.right * direction * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerOnPlatform = true;
            player = collision.transform;
        }
        else if (collision.CompareTag("Dog2"))
        {
            dog2 = collision.transform;
            adversaryOnPlatform = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerOnPlatform = false;
            player = null;
        }
        else if (collision.CompareTag("Dog2"))
        {
            adversaryOnPlatform = false;
            dog2 = null;
        }
    }

    public Vector3 GetPlatformVelocity()
    {
        return velocity;
    }

    public Vector3 GetEndPoint()
    {
        return new Vector3(x2, transform.position.y, transform.position.z);
    }
    public bool HasReachedEnd()
    {
        return transform.position.x >= x2 || transform.position.x <= x1;
    }
}
