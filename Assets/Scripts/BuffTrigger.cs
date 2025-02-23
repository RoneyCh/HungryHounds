using UnityEngine;

public class BuffTrigger : MonoBehaviour
{
    [Header("Buff/ Debuff Settings")]
    public float buffMultiplier = 1.5f;
    public float effectDuration = 3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = collision.GetComponent<PlayerMovement>();
        AdversaryAI adversary = collision.GetComponent<AdversaryAI>();

        if (player != null)
        {
            player.StartCoroutine(player.ApplyTemporaryBuff(buffMultiplier, effectDuration));
        }
        else if (adversary != null)
        {
            adversary.StartCoroutine(adversary.ApplyTemporaryBuff(buffMultiplier, effectDuration));
        }

        Destroy(gameObject);
    }


}
