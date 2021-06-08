using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void Exit()
    {
        Application.Quit(); //
        AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.button_pressed);
    }
}
