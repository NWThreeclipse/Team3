using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider[] statBars;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Slider suspicionSlider;
    private bool sus = true;

    private void Start()
    {
        UpdateBars();
        UpdateTimer();
        if (StatsController.Instance.GetDays() < 3)
        {
            suspicionSlider.gameObject.SetActive(false);
            sus = false;
        }
    }

    private void FixedUpdate()
    {
        UpdateBars();
        UpdateTimer();
    }

    private void UpdateBars()
    {
        float[] stats = gameManager.GetStats();
        for (int i = 0; i < statBars.Count(); i++)
        {
            statBars[i].value = stats[i];
        }

        if (sus)
        {
            suspicionSlider.value = gameManager.GetSuspicionStat();
        }
    }

    public void UpdateTimer()
    {
        float time = gameManager.GetTime();
        int elapsedTimeMinutes = Mathf.FloorToInt(time / 60F);
        int elapsedTimeSeconds = Mathf.FloorToInt(time - elapsedTimeMinutes * 60);
        string elapsedTime = string.Format("{0:0}:{1:00}", elapsedTimeMinutes, elapsedTimeSeconds);
        timerText.text = elapsedTime;

    }
}
