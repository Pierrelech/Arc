using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [Header("Score")]
    [SerializeField] private int currentScore = 0;

    [Header("Cibles")]
    public int maxTargets = 5;          // configurable dans l’inspecteur
    public int currentTargets = 0;      // nombre de cibles actives

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // -------- SCORE ----------
    public void AddScore(int amount)
    {
        currentScore += amount;
        if (currentScore < 0) currentScore = 0;
    }

    public int GetScore()
    {
        return currentScore;
    }

    // -------- CIBLES ----------
    public bool CanSpawnTarget()
    {
        return currentTargets < maxTargets;
    }

    public void RegisterTargetSpawn()
    {
        currentTargets = Mathf.Min(currentTargets + 1, maxTargets);
        // Debug.Log("Target spawn -> " + currentTargets);
    }

    public void RegisterTargetDespawn()
    {
        currentTargets = Mathf.Max(0, currentTargets - 1);
        // Debug.Log("Target despawn -> " + currentTargets);
    }
}
