using UnityEngine;

public class SpikeBehavior : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = collision.GetComponent<PlayerMovement>();
        AdversaryAI adversary = collision.GetComponent<AdversaryAI>();
        if (player != null)
        {
            Debug.Log("Player hit by spike");
            player.TakeDamage();
        } else if(adversary != null)
        {
            Debug.Log("Adversary hit by spike");
            adversary.TakeDamage();
        }
    }
}
