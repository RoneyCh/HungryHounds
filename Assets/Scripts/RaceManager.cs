using UnityEngine;
using UnityEngine.SceneManagement;
public class RaceManager : MonoBehaviour
{
    public PlayerMovement player;
    public AdversaryAI adversary;

    public float distanceThreshold = 5f;
    public float buffMultiplier = 1.2f;
    public float debuffMultiplier = 1f;
    public float effectDuration = 3f;

    [Header("Cooldown Settings")]
    public float buffCooldownTime = 15f;
    private float buffCooldownTimer = 0f;

    private float playerBaseSpeed;
    private float adversaryBaseSpeed;

    [Header("Corrida")]
    public float finishLineX = 285f;
    private bool raceOver = false;
    [SerializeField] private string gameSceneName = "Scene";
    private void Start()
    {
        playerBaseSpeed = player.speed;
        adversaryBaseSpeed = adversary.speed;
    }

    private void Update()
    {
        if (!raceOver)
        {
            // Verifica se o Player chegou na linha de chegada
            if (player.transform.position.x >= finishLineX)
            {
                raceOver = true;
                //PlayerWins();
            }
            // Verifica se o Adversário chegou na linha de chegada
            else if (adversary.transform.position.x >= finishLineX)
            {
                raceOver = true;
                //AdversaryWins();
            }
        }

        if (buffCooldownTimer > 0f)
        {
            buffCooldownTimer -= Time.deltaTime;
            return;
        }

        float playerDistance = player.distanceTraveled;
        float adversaryDistance = adversary.distanceTraveled;

        float diff = Mathf.Abs(playerDistance - adversaryDistance);

        if (diff > distanceThreshold)
        {
            buffCooldownTimer = buffCooldownTime;

            if (playerDistance > adversaryDistance)
            {
                Debug.Log("Aplicando Buff na IA!");
                StartCoroutine(ApplyTemporaryDebuff(player, debuffMultiplier, effectDuration));
                StartCoroutine(ApplyTemporaryBuff(adversary, buffMultiplier, effectDuration));
            }
            else
            {
                Debug.Log("Aplicando Buff no Player!");
                StartCoroutine(ApplyTemporaryBuff(player, buffMultiplier, effectDuration));
            }
        }
    }

    private System.Collections.IEnumerator ApplyTemporaryBuff(MonoBehaviour competitor, float multiplier, float duration)
    {
        if (competitor is PlayerMovement)
        {
            var p = competitor as PlayerMovement;
            float originalSpeed = p.speed;
            p.speed *= multiplier;
            yield return new WaitForSeconds(duration);
            p.speed = originalSpeed;
        }
        else if (competitor is AdversaryAI)
        {
            var a = competitor as AdversaryAI;
            float originalSpeed = a.speed;
            a.speed *= multiplier;
            yield return new WaitForSeconds(duration);
            a.speed = originalSpeed;
        }
    }

    private System.Collections.IEnumerator ApplyTemporaryDebuff(MonoBehaviour competitor, float multiplier, float duration)
    {
        if (competitor is PlayerMovement)
        {
            var p = competitor as PlayerMovement;
            float originalSpeed = p.speed;
            p.speed *= multiplier;
            yield return new WaitForSeconds(duration);
            p.speed = originalSpeed;
        }
        else if (competitor is AdversaryAI)
        {
            var a = competitor as AdversaryAI;
            float originalSpeed = a.speed;
            a.speed *= multiplier;
            yield return new WaitForSeconds(duration);
            a.speed = originalSpeed;
        }
    }

    void PlayerWins()
    {
        Debug.Log("Player é o campeão!");
        SceneManager.LoadScene("WinnerScene", LoadSceneMode.Additive);
    }

    void AdversaryWins()
    {
        Debug.Log("Adversário chegou primeiro. Game Over!");
        SceneManager.LoadScene("GameOverScene", LoadSceneMode.Additive);

    }
}
