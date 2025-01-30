using UnityEngine;
using UnityEngine.UI; // Omogoča delo z UI komponentami

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager _instance;
    public static ScoreManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<ScoreManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("ScoreManager");
                    _instance = go.AddComponent<ScoreManager>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] private Text killCountText; // Referenca na UI tekst
    private int _currentScore = 0;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        // DontDestroyOnLoad(gameObject);

        // Poskrbi, da tekst prikaže začetno vrednost
        UpdateKillCountText();
    }

    public void AddScore(int points)
    {
        _currentScore += points;
        Debug.Log($"Score increased! Current score: {_currentScore}");
        UpdateKillCountText();
    }

    public void AddKillScore()
    {
        AddScore(1);
    }

    public int GetCurrentScore()
    {
        return _currentScore;
    }

    public void ResetScore()
    {
        _currentScore = 0;
        Debug.Log("Score reset to 0");
        UpdateKillCountText();
    }

    private void UpdateKillCountText()
    {
        if (killCountText != null)
        {
            Debug.Log("Score is now: " + _currentScore);
            killCountText.text = "Kills: " + _currentScore;
        }
        else
        {
            Debug.LogWarning("KillCountText UI element ni nastavljen v ScoreManager!");
        }
    }
}
