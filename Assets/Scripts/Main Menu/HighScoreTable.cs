using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HighScoreTable : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    private List <Transform> highScoreEntryTransformList;
    private void Awake()
    {
        
        entryContainer = transform.Find("HighscoreContainer");
        entryTemplate = entryContainer.Find("Template");

        entryTemplate.gameObject.SetActive(false);
        
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        for(int i = 0; i < highscores.highScoreEntryList.Count; i++)
        {
            for(int j = i+1; j < highscores.highScoreEntryList.Count; j++)
            {
                if(highscores.highScoreEntryList[j].score > highscores.highScoreEntryList[i].score)
                {
                    HighScoreEntry temp = highscores.highScoreEntryList[i];
                    highscores.highScoreEntryList[i] = highscores.highScoreEntryList[j];
                    highscores.highScoreEntryList[j] = temp;
                }
            }
        }

        highScoreEntryTransformList = new List<Transform>();
        foreach (HighScoreEntry highScoreEntry in highscores.highScoreEntryList)
        {
            CreateHighscoreEntry(highScoreEntry,entryContainer,highScoreEntryTransformList);
        }
        
    }

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

    private void AddHighScoreEntry(int score, string name)
    {
        HighScoreEntry highScoreEntry = new HighScoreEntry{score = score, name = name};
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
        highscores.highScoreEntryList.Add(highScoreEntry);
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable",json);
        PlayerPrefs.Save();

    }
    private class Highscores
    {
        public List<HighScoreEntry> highScoreEntryList;
        
    }
    [System.Serializable]
    private class HighScoreEntry
    {
        public int score;
        public string name;
    }
}
