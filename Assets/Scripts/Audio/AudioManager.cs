using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager INSTANCE;
    public AudioSource[] audioSource;
    public enum eSoundsNames 
    {
        bow_shoot,bow_string_pull,
        arrow_hit_enemy, arrow_hit_ground, arrow_hit_stone, arrow_hit_wood,
        button_pressed, back_button_pressed, 
        enemy_hurt1, enemy_hurt2,
        coin, win,
        explosion      
    };
    //public  static eSoundsNames SoundName;
    public bool isMute = false;
    public float volume = 1f;
    public Slider slider;
    public Toggle toggle;
    void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else /*(INSTANCE == this)*/
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        audioSource = GetComponents<AudioSource>();

        Initialize();
    }
    void Start()
    {       
        
    }
    void Update()
    {
        //Debug.Log(isMute);
    }
    private void Initialize()
    {
        if (PlayerPrefs.HasKey("isMute"))
        {
            isMute = System.Convert.ToBoolean(PlayerPrefs.GetString("isMute"));
            //Debug.Log(PlayerPrefs.GetString("isMute"));
            for (int i = 0; i < audioSource.Length; i++)
            {
                audioSource[i].mute = isMute;
            }

            toggle.SetIsOnWithoutNotify(isMute);
        }
        if (PlayerPrefs.HasKey("volume"))
        {
            volume = PlayerPrefs.GetFloat("volume");

            for (int i = 0; i < audioSource.Length; i++)
            {
                audioSource[i].volume = volume;
            }

            slider.value = volume;
        }
    }
    public void Mute()
    {    
        isMute = !isMute;

        for (int i = 0; i < audioSource.Length; i++)
        {
            audioSource[i].mute = isMute;
        }

        PlayerPrefs.SetString("isMute", isMute.ToString());
        PlayerPrefs.Save();
        Debug.Log(PlayerPrefs.GetString("isMute"));
    }

    public void ChangeVolume(Slider slider)
    {
        volume = slider.value;

        for (int i = 0; i < audioSource.Length; i++)
        {
            audioSource[i].volume = volume;
        }

        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.Save();
        Debug.Log(PlayerPrefs.GetFloat("volume"));
    }

    public void PlaySound(eSoundsNames SoundName)
    {
       // Debug.Log(SoundName);
        for (int i = 0; i < audioSource.Length; i++)
        {
            if (audioSource[i].clip.name == SoundName.ToString())
            {
                audioSource[i].Play();
            }
        }
    }
}
