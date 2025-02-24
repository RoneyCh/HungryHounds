using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public List<CompetitorBase> competitors;

    private void Start()
    {
        playerBaseSpeed = player.speed;
        adversaryBaseSpeed = adversary.speed;
    }

    private void Update()
    {
        if (buffCooldownTimer > 0f)
        {
            buffCooldownTimer -= Time.deltaTime;
            return;
        }

        float playerDistance = player.distanceTraveled;
        float adversaryDistance = adversary.distanceTraveled;

        var sorted = competitors.OrderByDescending(c => c.distanceTraveled).ToList();

        CompetitorBase first = sorted[0];
        CompetitorBase last  = sorted[sorted.Count - 1];

        float diff = first.distanceTraveled - last.distanceTraveled;

        if (diff > distanceThreshold)
        {
            buffCooldownTimer = buffCooldownTime;
            
            StartCoroutine(ApplyTemporaryDebuff(first, debuffMultiplier, effectDuration));
            StartCoroutine(ApplyTemporaryBuff(last, buffMultiplier, effectDuration));
        }
    }

   private IEnumerator ApplyTemporaryBuff(CompetitorBase competitor, float multiplier, float duration)
    {
        float originalSpeed = competitor.speed;
        competitor.speed = originalSpeed * multiplier;

        yield return new WaitForSeconds(duration);

        competitor.speed = originalSpeed;
    }

    private IEnumerator ApplyTemporaryDebuff(CompetitorBase competitor, float multiplier, float duration)
    {
        float originalSpeed = competitor.speed;
        competitor.speed = originalSpeed * multiplier;

        yield return new WaitForSeconds(duration);

        competitor.speed = originalSpeed;
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
