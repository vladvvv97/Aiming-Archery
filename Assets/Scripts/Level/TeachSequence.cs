using UnityEngine;
using UnityEngine.UI;

// Level 1 teach flow (move -> jump -> aim/shoot -> kill), replacing the legacy TrainingManager/coin
// gated flow for this level. No coins involved (see AGENTS.md soft-launch constraints).
public class TeachSequence : MonoBehaviour
{
    private enum Stage { Move, Jump, Shoot, Kill, Done }

    [Header("Set in Inspector: Teach Sequence")]
    public Text promptText;
    public Player player;
    public LevelController levelController;

    [Header("Move stage")]
    [SerializeField] private float moveStageClearX = -10f;
    [SerializeField] private string moveStageMessage = "Двигайся вправо: используй левый джойстик";

    [Header("Jump stage")]
    [SerializeField] private float hillMinX = 0f;
    [SerializeField] private float hillMaxX = 6f;
    [SerializeField] private float hillStandY = -2f;
    [SerializeField] private string jumpStageMessage = "Запрыгни на холм: потяни левый джойстик вверх";

    [Header("Shoot stage")]
    [SerializeField] private string shootStageMessage = "Прицелься и выстрели: используй правый джойстик";

    [Header("Kill stage")]
    [SerializeField] private string killStageMessageFormat = "Убей всех орков! Осталось: {0}";

    private Stage _stage = Stage.Move;

    private void Start()
    {
        ShowStage(moveStageMessage);
    }

    private void Update()
    {
        if (player == null || levelController == null) return;

        switch (_stage)
        {
            case Stage.Move:
                if (player.transform.position.x > moveStageClearX)
                {
                    _stage = Stage.Jump;
                    ShowStage(jumpStageMessage);
                }
                break;

            case Stage.Jump:
                if (IsStandingOnHill())
                {
                    _stage = Stage.Shoot;
                    ShowStage(shootStageMessage);
                }
                break;

            case Stage.Shoot:
                if (Player.IsShooting)
                {
                    _stage = Stage.Kill;
                }
                break;

            case Stage.Kill:
                if (levelController.EnemiesAlive <= 0)
                {
                    _stage = Stage.Done;
                    HidePrompt();
                }
                else
                {
                    ShowStage(string.Format(killStageMessageFormat, levelController.EnemiesAlive));
                }
                break;
        }
    }

    private bool IsStandingOnHill()
    {
        Vector3 p = player.transform.position;
        if (p.x < hillMinX || p.x > hillMaxX || p.y < hillStandY) return false;

        var body = player.GetComponent<Rigidbody2D>();
        return body == null || Mathf.Abs(body.linearVelocity.y) < 0.4f;
    }

    private void ShowStage(string message)
    {
        if (promptText == null) return;
        promptText.gameObject.SetActive(true);
        promptText.text = message;
    }

    private void HidePrompt()
    {
        if (promptText == null) return;
        promptText.gameObject.SetActive(false);
    }
}
