using UnityEngine;
using UnityEngine.UI;

public class ThoughtBubble : MonoBehaviour
{

    [SerializeField] private Image backgroundBubble;
    [SerializeField] private Image anomalousSprite;
    [SerializeField] private Button button;

    public void InitializeThoughtBubble(int i, Sprite itemSprite, StatsUIController statsUIController, int index)
    {
        Vector3 currentScale = backgroundBubble.transform.localScale;
        // flip speech bubble
        if (i % 2 == 0)
        {
            currentScale.x = -currentScale.x;
            backgroundBubble.transform.localScale = currentScale;
        }
        if (i >= 2)
        {
            currentScale.y = -currentScale.y;
            backgroundBubble.transform.localScale = currentScale;
        }

        anomalousSprite.sprite = itemSprite;
        button.onClick.AddListener(() => statsUIController.StartPlayerReflection(index));
    }

    public void InitializeEmptyBubble()
    {
        Image[] backgrounds = GetComponentsInChildren<Image>();
        foreach (Image img in backgrounds)
        {
            img.enabled = false;
        }
    }
}
