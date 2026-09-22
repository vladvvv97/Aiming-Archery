using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public void Restart()
    {
        AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.button_pressed);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
