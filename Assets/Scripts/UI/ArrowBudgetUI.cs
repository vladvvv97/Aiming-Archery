using UnityEngine;
using UnityEngine.UI;

// One arrow button: type icon plus the remaining count. The icon is the only type label.
public class ArrowBudgetUI : MonoBehaviour
{
    [SerializeField] private Text countText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private ArrowTypeId type = ArrowTypeId.Normal;
    [SerializeField] private Button selectButton;
    [SerializeField] private Color activeColor = new Color(1f, 0.92f, 0.45f, 1f);
    [SerializeField] private Color inactiveColor = new Color(0.45f, 0.36f, 0.24f, 0.92f);

    private void Awake()
    {
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
        if (selectButton == null)
            selectButton = GetComponent<Button>();
        if (selectButton != null)
            selectButton.onClick.AddListener(OnSelect);
    }

    private void OnSelect()
    {
        LevelController.Instance?.Select(type);
    }

    private void Update()
    {
        if (LevelController.Instance == null) return;

        int remaining = LevelController.Instance.Remaining(type);
        if (countText != null)
            countText.text = remaining.ToString();

        ArrowTypeConfig typeConfig = LevelController.Instance.ConfigFor(type);
        if (iconImage != null && typeConfig != null && typeConfig.icon != null)
        {
            iconImage.sprite = typeConfig.icon;
            iconImage.preserveAspect = true;
            iconImage.color = remaining > 0 ? Color.white : new Color(1f, 1f, 1f, 0.35f);
        }

        bool active = LevelController.Instance.ActiveType == type && remaining > 0;
        if (backgroundImage != null)
            backgroundImage.color = active ? activeColor : inactiveColor;
        if (selectButton != null)
            selectButton.interactable = remaining > 0;
    }
}
