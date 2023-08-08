using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    [SerializeField] Text highScoreText;
    public Button ranksButton;
    public GameObject GameOverText;
    
    bool m_Started = false;
    int m_Points;
    
    bool m_GameOver = false;

    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        highScoreText.text = string.Format("Best Score : {0} : {1}",
            DPManager.Instance.highScores[0].Key, DPManager.Instance.highScores[0].Value);
    }

    void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void UpdateHighScoreUI()
    {
        if(m_Points > DPManager.Instance.highScores[0].Value)
        {
            highScoreText.text = string.Format("Best Score: {0} : {1}",
                DPManager.Instance.playerName, m_Points);
        }
        
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
        UpdateHighScoreUI();
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        UpdateHighScore();
    }

    void UpdateHighScore()
    {
        if (m_Points > DPManager.Instance.highScores[4].Value)
        {
            DPManager.Instance.highScores[4] =
                new KeyValuePair<string, int>(DPManager.Instance.playerName, m_Points);
            DPManager.Instance.SortHighScores();
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("menu");
    }

    public void GoRanks()
    {
        SceneManager.LoadScene("ranks");
    }
}
