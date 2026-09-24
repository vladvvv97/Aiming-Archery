using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Scene-scoped controller: arrow budgets, active type, enemy registry, win/lose/settle, stars (see SPEC.md 3.1).
// Unlock filtering against campaign progress is Phase 5; types listed on the config are available.
public class LevelController : MonoBehaviour
{
    public static LevelController Instance { get; private set; }

    [Header("Set in Inspector: Level Controller")]
    public LevelConfig config;
    [SerializeField] private float settleSeconds = 1.2f;

    [Header("Win UI")]
    public GameObject winPanel;
    public Text winStarsText;
    public Button winRestartButton;
    public Button winMenuButton;

    [Header("Lose UI")]
    public GameObject losePanel;
    public Button loseRestartButton;
    public Button loseMenuButton;

    [Header("Scenes")]
    [SerializeField] private string menuSceneName = "Scene_Game_Start";

    private readonly Dictionary<ArrowTypeId, int> _remaining = new Dictionary<ArrowTypeId, int>();
    private readonly List<Enemy> _enemies = new List<Enemy>();
    private readonly List<Arrow> _arrowsInFlight = new List<Arrow>();

    private bool _levelEnded;
    private Coroutine _settleRoutine;

    public ArrowTypeId ActiveType { get; private set; } = ArrowTypeId.Normal;

    public int EnemiesAlive
    {
        get
        {
            int alive = 0;
            for (int i = 0; i < _enemies.Count; i++)
            {
                if (_enemies[i] != null && _enemies[i].Health > 0f) alive++;
            }
            return alive;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (config != null)
        {
            bool picked = false;
            foreach (var budget in config.arrowBudgets)
            {
                if (budget.count <= 0) continue;
                _remaining[budget.type] = budget.count;
                if (!picked || budget.type == ArrowTypeId.Normal)
                {
                    ActiveType = budget.type;
                    picked = true;
                }
            }
        }

        _enemies.AddRange(FindObjectsByType<Enemy>());

        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        if (winRestartButton != null) winRestartButton.onClick.AddListener(RestartLevel);
        if (winMenuButton != null) winMenuButton.onClick.AddListener(GoToMenu);
        if (loseRestartButton != null) loseRestartButton.onClick.AddListener(RestartLevel);
        if (loseMenuButton != null) loseMenuButton.onClick.AddListener(GoToMenu);
    }

    private void Update()
    {
        if (_levelEnded) return;
        if (_enemies.Count > 0 && EnemiesAlive == 0)
        {
            Win();
        }
    }

    public ArrowTypeConfig ConfigFor(ArrowTypeId type)
    {
        return config != null ? config.GetTypeConfig(type) : null;
    }

    public void Select(ArrowTypeId type)
    {
        if (Remaining(type) <= 0) return;
        ActiveType = type;
    }

    public bool CanShoot(ArrowTypeId type)
    {
        if (_levelEnded) return false;
        return _remaining.TryGetValue(type, out var count) && count > 0;
    }

    public int Remaining(ArrowTypeId type)
    {
        return _remaining.TryGetValue(type, out var count) ? count : 0;
    }

    public void Spend(ArrowTypeId type)
    {
        if (!_remaining.ContainsKey(type)) return;
        _remaining[type] = Mathf.Max(0, _remaining[type] - 1);
        if (Remaining(type) <= 0)
            SelectNextAvailable(type);

        if (AllBudgetsEmpty() && _settleRoutine == null && !_levelEnded)
        {
            _settleRoutine = StartCoroutine(WaitSettleThenLoseUnlessWin());
        }
    }

    private void SelectNextAvailable(ArrowTypeId spent)
    {
        if (config == null || config.arrowBudgets == null) return;

        int count = config.arrowBudgets.Length;
        int start = 0;
        for (int i = 0; i < count; i++)
        {
            if (config.arrowBudgets[i].type == spent)
            {
                start = i;
                break;
            }
        }

        for (int step = 1; step <= count; step++)
        {
            var budget = config.arrowBudgets[(start + step) % count];
            if (Remaining(budget.type) > 0)
            {
                ActiveType = budget.type;
                return;
            }
        }
    }

    public void RegisterArrow(Arrow arrow)
    {
        if (!_arrowsInFlight.Contains(arrow)) _arrowsInFlight.Add(arrow);
    }

    public void UnregisterArrow(Arrow arrow)
    {
        _arrowsInFlight.Remove(arrow);
    }

    private bool AllBudgetsEmpty()
    {
        foreach (var kv in _remaining)
        {
            if (kv.Value > 0) return false;
        }
        return true;
    }

    private IEnumerator WaitSettleThenLoseUnlessWin()
    {
        // Wait until no arrows are in flight, then hold for settleSeconds to let physics calm down.
        while (_arrowsInFlight.Count > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(settleSeconds);

        if (_levelEnded) yield break;

        if (EnemiesAlive == 0)
        {
            Win();
        }
        else
        {
            Lose();
        }

        _settleRoutine = null;
    }

    private int RemainingScore()
    {
        int score = 0;
        foreach (var kv in _remaining) score += kv.Value;
        return score;
    }

    private void Win()
    {
        if (_levelEnded) return;
        _levelEnded = true;

        int stars = config != null ? config.ComputeStars(RemainingScore()) : 1;

        if (winStarsText != null)
        {
            winStarsText.text = new string('\u2605', stars) + new string('\u2606', 3 - stars);
        }

        if (winPanel != null) winPanel.SetActive(true);
    }

    private void Lose()
    {
        if (_levelEnded) return;
        _levelEnded = true;

        if (losePanel != null) losePanel.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}
