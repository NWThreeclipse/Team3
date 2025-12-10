using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ThoughtBubble : MonoBehaviour
{

    [SerializeField] private Image backgroundBubble;
    [SerializeField] private Image anomalousSprite;
    [SerializeField] private Button button;
    private MusicController musicController;

    public void InitializeThoughtBubble(int i, Sprite itemSprite, StatsUIController statsUIController, int index)
    {
       
        musicController = FindAnyObjectByType<MusicController>();

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
        button.onClick.AddListener(() => musicController.PlayButtonSound());
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
