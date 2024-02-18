using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class t_slider : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sliderText = null;
    [SerializeField] private float maxSliderAmount = 300f;
    public float w_localvalue;

    public void SliderChange(float value)
    {
       
        float localValue = value * maxSliderAmount;
        sliderText.text = localValue.ToString("150.0");
        w_localvalue = localValue;
        Debug.Log("Valor" + value);
        Debug.Log("W localValue" + w_localvalue);


    }



}
