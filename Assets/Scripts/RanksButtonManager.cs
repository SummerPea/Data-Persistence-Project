using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RanksButtonManager : MonoBehaviour
{
    Button button;
    TextMeshProUGUI ranks;

    void Start()
    {
        button = GameObject.Find("Back").GetComponent<Button>();
        ranks = GameObject.Find("Ranks").GetComponent<TextMeshProUGUI>();
        button.onClick.AddListener(GoBack);
        DisplayRanks();
    }

    void GoBack()
    {
        SceneManager.LoadScene("main");
    }

    void DisplayRanks()
    {
        var highScores = DPManager.Instance.highScores;
        string text = "";
        for(int i = 0; i < 5; i++)
        {
            text = text + string.Format("No.{0}\t\t{1}\t\t{2}\n",
                i + 1, highScores[i].Key, highScores[i].Value);
        }
        ranks.text = text;
    }
}
