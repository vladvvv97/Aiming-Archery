using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionScreen : MonoBehaviour
{
    public Canvas StartScreen;
    public void Back()
    {
        StartScreen.gameObject.SetActive(true);
        this.gameObject.SetActive(false);

        AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.back_button_pressed);
    }

    public void LoadOptionScreen()
    {
        StartScreen.gameObject.SetActive(false);
        this.gameObject.SetActive(true);

        AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.button_pressed);
    }
}
