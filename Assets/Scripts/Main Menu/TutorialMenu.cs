using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMenu : MonoBehaviour
{
    [Serializable]
    public class Tutorial
    {
        public Sprite tutorialImage;
        public string tutorialText;
    }
    
    public Image tutorialImage;
    public TMP_Text tutorialText;
    public Button nextButton;
    public Button previousButton;
    
    public Tutorial[] tutorials;

    private int _currentTutorial;

    private void Start()
    {
        RestartTutorials();
    }

    public void RestartTutorials()
    {
        _currentTutorial = 0;
        tutorialImage.sprite = tutorials[0].tutorialImage;
        tutorialText.text = tutorials[0].tutorialText;
        
        // previousButton.gameObject.SetActive(false);
        // nextButton.gameObject.SetActive(true);

        previousButton.interactable = true;
        nextButton.interactable = true;
    }
    
    public void NextTutorial()
    {
        _currentTutorial++;
        tutorialImage.sprite = tutorials[_currentTutorial].tutorialImage;
        tutorialText.text = tutorials[_currentTutorial].tutorialText;

        // At the last tutorial
        if (_currentTutorial == tutorials.Length - 1)
        {
            // nextButton.gameObject.SetActive(false);
            nextButton.interactable = false;
        }
        
        // previousButton.gameObject.SetActive(true);
        previousButton.interactable = true;
    }

    public void PreviousTutorial()
    {
        _currentTutorial--;
        tutorialImage.sprite = tutorials[_currentTutorial].tutorialImage;
        tutorialText.text = tutorials[_currentTutorial].tutorialText;

        // At the last tutorial
        if (_currentTutorial == 0)
        {
            // previousButton.gameObject.SetActive(false);
            previousButton.interactable = false;
        }
        
        // nextButton.gameObject.SetActive(true);
        nextButton.interactable = true;
    }
}
