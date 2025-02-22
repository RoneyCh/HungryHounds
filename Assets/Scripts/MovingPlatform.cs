using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 3f;
    public float x1 = 201.7f;
    public float x2 = 220.5f;

    private int direction = 1;
    private Vector3 lastPosition;
    private Vector3 velocity;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        // Move a plataforma
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        if (transform.position.x >= x2)
        {
            direction = -1;
        }
        else if (transform.position.x <= x1)
        {
            direction = 1;
        }

        // Calcula a velocidade manualmente
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }

    public Vector3 GetPlatformVelocity()
    {
        return velocity;
    }
}
