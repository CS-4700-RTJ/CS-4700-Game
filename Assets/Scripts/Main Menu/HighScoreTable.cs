using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HighScoreTable : MonoBehaviour
{
    public const string HIGHSCORE_TABLE_PREF = "highscoreTable";
    
    public Transform highscoreContainer;
    private Transform entryTemplate;
    private List <Transform> highScoreEntryTransformList;

    private void Awake()
    {
        // entryTemplate = highscoreContainer.Find("Template");
        entryTemplate = highscoreContainer.GetChild(0);

        entryTemplate.gameObject.SetActive(false);
        
        highScoreEntryTransformList = new List<Transform>();
        
        InitDB();
    }

    private void InitDB()
    {
        if (PlayerPrefs.HasKey(HIGHSCORE_TABLE_PREF))
        {
            string jsonString = PlayerPrefs.GetString(HIGHSCORE_TABLE_PREF);
            Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

            // swap them into order
            highscores.highScoreEntryList.Sort(((a, b) => b.score - a.score));

            foreach (HighScoreEntry highScoreEntry in highscores.highScoreEntryList)
            {
                CreateHighscoreEntry(highScoreEntry, highscoreContainer, highScoreEntryTransformList);
            }
        }
    }

    // Creates an entry in the UI
    private void CreateHighscoreEntry(HighScoreEntry highScoreEntry, Transform container, List<Transform> transformList)
    {
            float templateHeight = 30f;
            Transform entryTransform = Instantiate(entryTemplate,container);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            
            entryRectTransform.anchoredPosition = new Vector2(0,- templateHeight * transformList.Count);
            entryTransform.gameObject.SetActive(true);

            int rank = transformList.Count + 1;
            string rankString;
            switch(rank){
                default: rankString = rank + "TH"; break;
                case 1: rankString =  "1ST"; break;
                case 2: rankString = "2ND"; break;
                case 3: rankString = "3RD"; break;
            }
            entryTransform.Find("Position").GetComponent<TextMeshProUGUI>().text = rankString;  

            int score = highScoreEntry.score; 
            entryTransform.Find("Score").GetComponent<TextMeshProUGUI>().text = score.ToString();

            String name = highScoreEntry.name;  
            entryTransform.Find("Name").GetComponent<TextMeshProUGUI>().text = name;

            entryTransform.Find("Entry Background").gameObject.SetActive(rank % 2 == 1);
            if(rank == 1)
            {
                entryTransform.Find("Name").GetComponent<TextMeshProUGUI>().text = name;
                entryTransform.Find("Score").GetComponent<TextMeshProUGUI>().text = score.ToString();
                 entryTransform.Find("Position").GetComponent<TextMeshProUGUI>().text = rankString;  
            }
            transformList.Add (entryTransform);
    }

    public class Highscores
    {
        public List<HighScoreEntry> highScoreEntryList;

        public Highscores()
        {
            highScoreEntryList = new List<HighScoreEntry>();
        }
    }
   
    [System.Serializable]
    public class HighScoreEntry
    {
        public int score;
        public string name;
    }
}
