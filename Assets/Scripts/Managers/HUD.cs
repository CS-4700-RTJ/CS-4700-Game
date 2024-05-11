using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider staminaSlider;

    public Image currentSpellImage;
    public TMP_Text spellNameText;
    public Color disabledColor;
    public Image progressReticle;
    
    private const float SpellTextVisibleDuration = 2f;
    private const float SpellTextFadeTime = 1f;
    private Coroutine _spellNameFadeRoutine;

    private void Start()
    {
        _spellNameFadeRoutine = StartCoroutine(FadeSpellText());
    }

    public void SetHealthSliderPercent(float percent)
    {
        healthSlider.value = percent;
    }

    public void SetManaSliderPercent(float percent)
    {
        manaSlider.value = percent;
    }

    public void SetStaminaSliderPercent(float percent)
    {
        staminaSlider.value = percent;
        staminaSlider.value = percent;
    }
    
    public void SetReticleFillPercent(float percent)
    {
        progressReticle.fillAmount = percent;
    }

    public void SetCurrentSpellImage(Sprite image)
    {
        currentSpellImage.sprite = image;
    }

    public void SetSpellColor(bool canCastSpell)
    {
        currentSpellImage.color = canCastSpell ? Color.white : disabledColor;
    }

    public void SetSpellNameText(string spellName)
    {
        if (_spellNameFadeRoutine != null) StopCoroutine(_spellNameFadeRoutine);

        spellNameText.text = spellName;
        
        _spellNameFadeRoutine = StartCoroutine(FadeSpellText());
    }
    
    private IEnumerator FadeSpellText() 
    {        
        // Make spell text fully visible
        Color textColor = Color.white;

        spellNameText.color = textColor;

        yield return new WaitForSeconds(SpellTextVisibleDuration);
        
        float timer = SpellTextFadeTime;
        
        while (timer > 0f)
        {
            textColor.a = Mathf.Lerp(0, 1, timer / SpellTextFadeTime);
            spellNameText.color = textColor;
            
            timer -= Time.deltaTime;
            yield return null;
        }

        textColor.a = 0;
        spellNameText.color = textColor;
    }
}
