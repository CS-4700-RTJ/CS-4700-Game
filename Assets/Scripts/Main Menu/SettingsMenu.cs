using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
   public AudioMixer mixer;
   public TMP_Dropdown resolutionDropdown;
   Resolution[] resolutions;
   void Start()
   {
      
      resolutions = Screen.resolutions;

      resolutionDropdown.ClearOptions();

      List<string> options = new List<string>();
      
      int currentResolutionIndex = 0;
      for(int i = 0; i < resolutions.Length; i++)
      {
         string option = resolutions[i].width + "x" + resolutions[i].height;
         options.Add(option);

         if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
         {
            currentResolutionIndex = i;
         }
      }

      resolutionDropdown.AddOptions(options);
      resolutionDropdown.value = currentResolutionIndex;
      resolutionDropdown.RefreshShownValue();
   }
   public void SetVolume(float volume)
   {
      mixer.SetFloat("MusicVol", Mathf.Log10(volume) * 20);
   }
   public void SetQuality(int qualityIndex)
   {
      QualitySettings.SetQualityLevel(qualityIndex);
   }

   public void SetResolution(int resolutionIndex)
   {
      Resolution resolution = resolutions[resolutionIndex];
      Screen.SetResolution(resolution.width,resolution.height,Screen.fullScreen); 
   }

   public void SetFullscreen(bool isFullScreen)
   {
      Screen.fullScreen = isFullScreen;
   }
}
