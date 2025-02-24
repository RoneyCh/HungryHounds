using UnityEngine;

public class SpikeBehavior : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = collision.GetComponent<PlayerMovement>();
        if (player != null)
        {
            Debug.Log("Player hit by spike");
            player.TakeDamage();
        }
    }
}
