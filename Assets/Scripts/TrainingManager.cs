using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrainingManager : MonoBehaviour
{
    static public TrainingManager S;
    public int TrainingCoinsCount = 0;
    [SerializeField] private Player player;
    [SerializeField] private Image leftFade;
    [SerializeField] private Image rightFade1;
    [SerializeField] private Image rightFade2;
    [SerializeField] private Image leftFade1;
    [SerializeField] private Image rightFade3;
    [SerializeField] private Text questText;
    [SerializeField] private Transform jumpTraining;
    [SerializeField] private Transform shootTraining1;
    [SerializeField] private Transform shootTraining2;
    [SerializeField] private Transform shootTraining3;
    [SerializeField] private Transform shootTraining4;

    private bool _onMoveTraining = true;
    private bool _onJumpTraining = true;
    private bool _onShootTraining1 = true;
    private bool _onShootTraining2 = true;
    private bool _onShootTraining3 = true;
    private bool _onShootTraining4 = true;


    void Awake()
    {
        if (S == null)
        {
            S = this;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (TrainingCoinsCount < 2 && _onMoveTraining)
        {
            StartCoroutine(MoveTraining());
        }

        if (TrainingCoinsCount >= 2 && TrainingCoinsCount < 4 && _onJumpTraining)
        {
            StartCoroutine(JumpTraining());
        }

        if (TrainingCoinsCount >= 4 && TrainingCoinsCount < 5 && _onShootTraining1)
        {
            StartCoroutine(ShootTraining1());
        }
        if (TrainingCoinsCount >= 5 && TrainingCoinsCount < 6 && _onShootTraining2)
        {
            StartCoroutine(ShootTraining2());
        }
        if (TrainingCoinsCount >= 6 && TrainingCoinsCount < 7 && _onShootTraining3)
        {
            StartCoroutine(ShootTraining3());
        }
        if (TrainingCoinsCount >= 7 && TrainingCoinsCount < 10 && _onShootTraining4)
        {
            StartCoroutine(ShootTraining4());
        }
        if (TrainingCoinsCount >= 10)
        {
            PlayerPrefs.Save();
            SceneManager.LoadScene("Scene_Game_Start");
            AudioManager.INSTANCE.PlaySound(AudioManager.eSoundsNames.win);
        }
    }

    IEnumerator MoveTraining()
    {
        _onMoveTraining = false;
        questText.gameObject.SetActive(true);
        questText.text = "To complete this level, collect 2 coins by using the movement joystick";
        yield return new WaitForSeconds(3);
        rightFade1.gameObject.SetActive(true);
        rightFade2.gameObject.SetActive(true);
        yield return new WaitForSeconds(5);
        rightFade1.gameObject.SetActive(false);
        rightFade2.gameObject.SetActive(false);
        questText.gameObject.SetActive(false);
        
        yield return null;
    }

    IEnumerator JumpTraining()
    {
        yield return new WaitForSeconds(1);
        _onJumpTraining = false;
        player.transform.position = jumpTraining.transform.position;
        yield return new WaitForSeconds(1);
        questText.gameObject.SetActive(true);
        questText.text = "To complete this level, collect 2 coins by pulling the movement joystick up";
        yield return new WaitForSeconds(5);
        questText.gameObject.SetActive(false);
        yield return null;
    }

    IEnumerator ShootTraining1()
    {
        yield return new WaitForSeconds(1);
        _onShootTraining1 = false;
        player.transform.position = shootTraining1.transform.position;
        yield return new WaitForSeconds(1);
        questText.gameObject.SetActive(true);
        questText.text = "To complete this level, kill 1 orc by using the shooting joystick"
        + "\n" + "Hint: You can hit 3 zones" + "\n" +
        "1) Head (Highest damage)" + "\n" +
        "2) Body(medium damage)" + "\n" +
        "3) Legs(least damage)";
        yield return new WaitForSeconds(3);
        leftFade.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        leftFade.gameObject.SetActive(false);
        yield return new WaitForSeconds(3);
        questText.gameObject.SetActive(false);
        yield return null;

    }

    IEnumerator ShootTraining2()
    {
        yield return new WaitForSeconds(1);
        _onShootTraining2 = false;
        player.transform.position = shootTraining2.transform.position;
        yield return new WaitForSeconds(1);
        questText.gameObject.SetActive(true);
        questText.text = "To complete this level, kill 1 orc by using the force of gravity acting on the arrows"+ "\n" + 
        "Hint: You can ZOOM IN/OUT by using touch screen in the highlighted zone";
        yield return new WaitForSeconds(5);
        leftFade1.gameObject.SetActive(true);
        rightFade3.gameObject.SetActive(true);
        yield return new WaitForSeconds(5);
        leftFade1.gameObject.SetActive(false);
        rightFade3.gameObject.SetActive(false);
        questText.gameObject.SetActive(false);
        yield return null;
    }

    IEnumerator ShootTraining3()
    {
        yield return new WaitForSeconds(1);
        _onShootTraining3 = false;
        player.transform.position = shootTraining3.transform.position;
        yield return new WaitForSeconds(1);
        questText.gameObject.SetActive(true);
        questText.text = "To complete this level, kill 1 orc by using the JUMP while shooting";
        yield return new WaitForSeconds(7);
        questText.gameObject.SetActive(false);
        yield return null;
    }

    IEnumerator ShootTraining4()
    {
        yield return new WaitForSeconds(1);
        _onShootTraining4 = false;
        player.transform.position = shootTraining4.transform.position;
        yield return new WaitForSeconds(1);
        questText.gameObject.SetActive(true);
        questText.text = "To complete this level, kill 3 orcs"
        + "\n" + "Hints: " + "\n" +
        "1) The flag means that you can't leave the starting area" + "\n" +
        "2) You can hit explosive barrel to kill orcs and destroy buildings";
        yield return new WaitForSeconds(10);
        questText.gameObject.SetActive(false);
        yield return null;
    }
}
