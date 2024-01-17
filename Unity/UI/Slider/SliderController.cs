using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public TMP_Text Text;
    public Slider Slider;
    
    private int progress = 0;
    
    public void OnSliderChanged(float value)
    {
        Text.text = value.ToString();
    }

    public void UpdateProgress()
    {
        progress++;
        Slider.value = progress;
    }
}
