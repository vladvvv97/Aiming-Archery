using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour
{
    public Canvas StartScreen;
    public string LoadableSceneName;
    public Text LoadingText;
    public Image LoadingProgress;


    public void LoadScene()
    {
        this.gameObject.SetActive(true);
        StartScreen.gameObject.SetActive(false);

        StartCoroutine(LoadSceneAsync());

        AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.button_pressed);
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(LoadableSceneName);
        asyncLoadScene.allowSceneActivation = false;

        while (!asyncLoadScene.isDone)
        {
            

            

            if (!asyncLoadScene.allowSceneActivation)
            {
                LoadingProgress.fillAmount = asyncLoadScene.progress + 0.1f;

                if (!Input.anyKeyDown && asyncLoadScene.progress < 0.9f)
                {                    
                    LoadingText.text = "Loading.   " + asyncLoadScene.progress * 100 + "%";
                    yield return new WaitForSeconds(0.33f);
                    LoadingText.text = "Loading..  " + asyncLoadScene.progress * 100 + "%";
                    yield return new WaitForSeconds(0.33f);
                    LoadingText.text = "Loading... " + asyncLoadScene.progress * 100 + "%";
                    yield return new WaitForSeconds(0.33f);
                }
                else if (!Input.anyKeyDown && asyncLoadScene.progress >= 0.9f)
                {
                    LoadingText.text = "\n" + "Loading    " + (asyncLoadScene.progress + 0.1f) * 100 + "%" + "\n" + "Touch anywhere";                    
                }
                else if (Input.anyKeyDown && asyncLoadScene.progress >= 0.9f)
                {
                    asyncLoadScene.allowSceneActivation = true;
                }
            }
            

            yield return null;
        }
    }

}
