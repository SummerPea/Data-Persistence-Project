using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DPManager : MonoBehaviour
{
    public static DPManager Instance { get; private set; }
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private TextMeshProUGUI bestScore;
    public string playerName;
    public List<KeyValuePair<string, int>> highScores;
    int lastSceneIndex;
    const string PDpath = "/gameData.json";

    void Start()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerName = "";
        lastSceneIndex = 0;
        highScores = new List<KeyValuePair<string, int>>();
        for(int i = 0; i < 5; i++)
           highScores.Add(new KeyValuePair<string, int>("", 0));

        LoadData();

        Init();

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex != lastSceneIndex)
        {
            if(sceneIndex == 0) Init();
            lastSceneIndex = sceneIndex;
        }
    }

    private void Init()
    {
        inputField = GameObject.Find("NameInput").GetComponent<TMP_InputField>();
        startButton = GameObject.Find("Start").GetComponent<Button>();
        quitButton = GameObject.Find("Quit").GetComponent<Button>();
        resetButton = GameObject.Find("Reset").GetComponent<Button>();
        bestScore = GameObject.Find("BestScore").GetComponent<TextMeshProUGUI>();

        inputField.onEndEdit.AddListener(SetName);
        startButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);
        resetButton.onClick.AddListener(ClearData);
        DisplayBestScore();
    }

    public void SetName(string name)
    {
        playerName = name;
    }

    public void ClearData()
    {
        highScores.Clear();
        for (int i = 0; i < 5; i++)
            highScores.Add(new KeyValuePair<string, int>("", 0));
        File.Delete(Application.persistentDataPath + PDpath);

        bestScore.text = "BestScore: None";
    }

    public void SortHighScores()
    {
        KeyValuePair<string, int> temp = highScores[4];
        int i = 3;
        for (; i >= 0; i--)
        {
            if (highScores[i].Value < temp.Value)
                highScores[i + 1] = highScores[i];
            else break;
        }
        highScores[i + 1] = temp;
    }

    private void DisplayBestScore()
    {
        Debug.Log(highScores[0]);
        bestScore.text = highScores[0].Value > 0 ?
            string.Format("Best Score: {0} : {1}", highScores[0].Key, highScores[0].Value)
            : "BestScore: None";
    }

    public void StartGame()
    {
        SceneManager.LoadScene("main");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    [System.Serializable]
    class DataToSave
    {
        public List<KeyValuePair<string, int>> highScores;

        public DataToSave(List<KeyValuePair<string, int>> scores)
        {
            highScores = scores;
        }
    }

    public void SaveData()
    {
        DataToSave data = new DataToSave(highScores);
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + PDpath, json);
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + PDpath;
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            DataToSave data = JsonUtility.FromJson<DataToSave>(json);
            if(data.highScores != null) highScores = data.highScores;
        }
    }
}
