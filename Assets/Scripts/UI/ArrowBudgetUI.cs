using UnityEngine;
using UnityEngine.UI;

// Minimal HUD slot for the Normal arrow budget (Phase 1; multi-type slot bar is Phase 2, see ARCHITECTURE.md).
public class ArrowBudgetUI : MonoBehaviour
{
    [SerializeField] private Text countText;
    [SerializeField] private ArrowTypeId type = ArrowTypeId.Normal;
    [SerializeField] private string labelFormat = "Стрелы: {0}";

    private void Update()
    {
        if (countText == null || LevelController.Instance == null) return;
        countText.text = string.Format(labelFormat, LevelController.Instance.Remaining(type));
    }
}
