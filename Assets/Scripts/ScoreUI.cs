using UnityEngine;
using TMPro;  // très important si tu utilises TextMeshPro

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public float refreshDelay = 0.1f; // entre 0.1 et 0.2 comme demandé

    private void Start()
    {
        StartCoroutine(UpdateScoreRoutine());
    }

    private System.Collections.IEnumerator UpdateScoreRoutine()
    {
        while (true)
        {
            if (GameplayManager.Instance != null && scoreText != null)
            {
                int score = GameplayManager.Instance.GetScore();
                scoreText.text = "Score : " + score;
            }

            yield return new WaitForSeconds(refreshDelay);
        }
    }
}
