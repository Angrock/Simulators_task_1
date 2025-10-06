using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Счёт")]
    public int score = 0;
    public int targetsHit = 0;
    public int totalShots = 0;
    
    [Header("UI")]
    public Text scoreText;
    public Text statsText;
    
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        UpdateUI();
    }

    public void AddScore(int points, float mass, float radius)
    {
        score += points;
        targetsHit++;
        
        Debug.Log($"+{points} очков! Мишень: масса={mass:F2}, радиус={radius:F2}");
    }

    public void RegisterShot()
    {
        totalShots++;
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Очки: {score}";
            
        if (statsText != null)
            statsText.text = $"Попадания: {targetsHit}";
    }
}