using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RaceCountdown : MonoBehaviour
{
    public Text countdownText;
    public GameObject player;
    public GameObject[] dogs;

    void Start()
    {
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        int countdown = 3;
        countdownText.gameObject.SetActive(true);

        while (countdown > 0)
        {
            countdownText.text = countdown.ToString();
            DisableMovement();
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        countdownText.text = "GO!";
        EnableMovement();
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);
    }


    void DisableMovement()
    {
        if (player != null)
        {
            player.GetComponent<PlayerMovement>().enabled = false;
        }

        foreach (GameObject dog in dogs)
        {
            if (dog != null)
                dog.GetComponent<AdversaryAI>().enabled = false;
        }
    }

    void EnableMovement()
    {
        if (player != null)
        {
            player.GetComponent<PlayerMovement>().enabled = true;
        }

        foreach (GameObject dog in dogs)
        {
            if (dog != null)
                dog.GetComponent<AdversaryAI>().enabled = true;
        }
    }
}
