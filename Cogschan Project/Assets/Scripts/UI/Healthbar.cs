using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Simple wrapper class for the slider UI element that turns it into a healthbar
// Make sure to disable user interaction on the healthbar, and remove unnecessary UI elements (like the handle)
public class Healthbar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void UpdateValue(float value)
    {
        slider.value = value;
        fill.color = gradient.Evaluate(value);
    }
}
