using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    Slider slider;
    AudioManager audioManager;
    void Awake()
    {
        slider = GetComponent<Slider>();
        audioManager = FindObjectOfType<AudioManager>();
    }
    void Start()
    {
        if (slider != null && audioManager != null)
            slider.onValueChanged.AddListener(value => audioManager.ChangeVolume(slider));
    }

}
