using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField]
    private TMP_Text scoreText;

    [SerializeField]
    private int targetScore = 10;

    [SerializeField]
    private string nextSceneName;

    private int score;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddPoint()
    {
        score++;

        if (score >= targetScore)
        {
            score = targetScore;

            UpdateUI();

            SceneManager.LoadScene(nextSceneName);
            return;
        }

        UpdateUI();
    }

    public void RemovePoint()
    {
        score--;

        if (score < 0)
            score = 0;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text =
                "Punkte: " +
                score +
                "/" +
                targetScore;
        }
    }
}