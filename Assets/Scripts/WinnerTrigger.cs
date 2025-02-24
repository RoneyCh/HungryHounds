using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class WinnerTrigger : MonoBehaviour
{
    public GameObject playerGameObject;
    public GameObject[] dogs;

    void PlayerWins()
    {
        Debug.Log("Player é o campeão!");
        SceneManager.LoadScene("WinnerScene", LoadSceneMode.Additive);
        DisableMovement();
    }

    void AdversaryWins()
    {
        Debug.Log("Adversário chegou primeiro. Game Over!");
        SceneManager.LoadScene("GameOverScene", LoadSceneMode.Additive);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = collision.GetComponent<PlayerMovement>();
        AdversaryAI adversary = collision.GetComponent<AdversaryAI>();
        if (player != null)
        {
            PlayerWins();
        }
        else if (adversary != null)
        {
            AdversaryWins();
        }

        Destroy(gameObject);
        DisableMovement();
    }

    void DisableMovement()
    {
        if (playerGameObject != null)
        {
            var playerMovement = playerGameObject.GetComponent<PlayerMovement>();
            playerMovement.enabled = false;

            var rb = playerGameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        foreach (GameObject dog in dogs)
        {
            if (dog != null)
            {
                var adversaryAI = dog.GetComponent<AdversaryAI>();
                if (adversaryAI != null)
                    adversaryAI.enabled = false;

                var dogRb = dog.GetComponent<Rigidbody2D>();
                if (dogRb != null)
                {
                    dogRb.velocity = Vector2.zero;
                    dogRb.angularVelocity = 0f;
                }
            }
        }
    }

}