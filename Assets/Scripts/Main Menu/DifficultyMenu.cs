using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DifficultyMenu : MonoBehaviour
{
    public TMP_Text difficultyText;

    public DifficultyOptionSO[] difficultyOptions;

    public Transform difficultyButtonsParent;
    
    public GameObject difficultyButtonPrefab;

    public Sprite buttonUnpressedImage;
    public Sprite buttonPressedImage;

    private Button[] _difficultyButtons;
    
    private void Start()
    {
        _difficultyButtons = new Button[difficultyOptions.Length];
        
        // Create one button for each difficulty
        for (int i = 0; i < difficultyOptions.Length; i++)
        {
            var difficultyButton = Instantiate(difficultyButtonPrefab, difficultyButtonsParent).GetComponent<Button>();
            difficultyButton.gameObject.name = difficultyOptions[i].name + " Button";

            difficultyButton.GetComponentInChildren<TMP_Text>().text = difficultyOptions[i].name;

            var buttonIndex = i;
            difficultyButton.onClick.AddListener(() => SetSelectedButton(buttonIndex));
            
            _difficultyButtons[i] = difficultyButton;
        }
        
        _difficultyButtons[0].onClick.Invoke();
    }

    private void SetSelectedButton(int index)
    {
        print("Clicked " + index);
        for (int i = 0; i < _difficultyButtons.Length; i++)
        {
            _difficultyButtons[i].image.sprite = index == i ? buttonPressedImage : buttonUnpressedImage;
            _difficultyButtons[i].interactable = index != i;
        }

        SetDifficulty(difficultyOptions[index]);
    }
    
    private void SetDifficulty(DifficultyOptionSO difficulty) 
    {
        print("Selected " + difficulty.name + " difficulty");
        GameManager.SetDifficulty(difficulty);

        difficultyText.text = String.Format("Spawn Rate: {0} seconds\n" +
                                            "Upgrade Rate: {1} seconds\n" +
                                            "Point Multiplier: {2}x",
            difficulty.timeBetweenSpawns, difficulty.timeBetweenUpgrades, difficulty.pointMultiplier);
    }
}
