using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioToggle : MonoBehaviour
{
    Toggle toggle;
    AudioManager audioManager;
    void Awake()
    {
        toggle = GetComponent<Toggle>();
        audioManager = FindObjectOfType<AudioManager>();
    }
    void Start()
    {
        if (toggle != null && audioManager != null)
            toggle.onValueChanged.AddListener(value => audioManager.Mute());
    }
}
