using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThermalScanner : DragZone
{
    [SerializeField] private bool isPlaying = false;
    [SerializeField] private GameObject minigameCanvas;


    [SerializeField] private LineRenderer goalLine;
    [Range(.2f, 1f)]
    [SerializeField] private float goalAmplitude;
    [Range(.5f, 2.5f)]
    [SerializeField] private float goalFrequency;


    [SerializeField] private LineRenderer playerLine;
    [Range(.2f, 1f)]
    [SerializeField] private float playerAmplitude;
    [Range(.5f, 2.5f)]
    [SerializeField] private float playerFrequency;

    [SerializeField] private Vector2 xLimits;
    [SerializeField] private float movementSpeed;
    [SerializeField] private int points;

    [SerializeField] private float amplitudeWinThreshold;
    [SerializeField] private float frequencyWinThreshold;
    [SerializeField] private Slider temperatureSlider;

    private const float TAU = 2 * Mathf.PI;

    [SerializeField] private Slider amplitudeSlider;
    [SerializeField] private Slider frequencySlider;

    [SerializeField] private AudioSource minigameSFX;
    [SerializeField] private AudioSource minigameWinSFX;

    [SerializeField] private GameObject helpCanvas;

    [SerializeField] private AudioClip panelOpen, panelClose;
    [SerializeField] private Vector3 showPanelPos;
    [SerializeField] private Vector3 hidePanelPos;
    [SerializeField] private float panelAnimationTime = 1;

    [SerializeField] private Vector3 showPanelPos2;
    [SerializeField] private Vector3 hidePanelPos2;
    private bool helpCanvasEnabled = false;
    [SerializeField] private Magnometer magnometer;

    private bool hasWon = false;

    public bool GetPlaying() => isPlaying;
    protected override void OnTriggerExit2D(Collider2D collision)
    {
        DisableMiniGame();
        base.OnTriggerExit2D(collision);
    }
    void Start()
    {
        //minigameCanvas.SetActive(false);
        goalLine.positionCount = points;
        playerLine.positionCount = points;
        ResetMiniGame();
        temperatureSlider.value = -273;
        //helpCanvas.SetActive(false);

    }

    private void Update()
    {
        if (GetEnteredItem() != null)
        {
            Item item = GetEnteredItem().GetComponent<Item>();
            if (isPlaying)
            {
                item.SetInteractive(false);

            }
            else
            {
                item.SetInteractive(true);

            }

        }
        if (isPlaying)
        {
            Draw(goalLine, goalAmplitude, goalFrequency);
            Draw(playerLine, playerAmplitude, playerFrequency);
            CheckWin();

        }

    }

    public void CheckWin()
    {
        if (IsHoldingItem() && !hasWon)
        {
            if (Mathf.Abs(playerAmplitude - goalAmplitude) < amplitudeWinThreshold && Mathf.Abs(playerFrequency - goalFrequency) < frequencyWinThreshold)
            {
                hasWon = true;
                temperatureSlider.value = Mathf.Lerp(temperatureSlider.value, GetItem().TemperatureC >= 190 ? GetItem().TemperatureC : GetItem().TemperatureC + 10, .1f);
                amplitudeSlider.interactable = false;
                frequencySlider.interactable = false;
                minigameSFX.Stop();
                minigameWinSFX.Play();
            }
        }
    }

    public void Draw(LineRenderer line, float amp, float freq)
    {
        for (int i = 0; i < points; i++)
        {
            float progress = (float)i / (points - 1);
            float x = Mathf.Lerp(xLimits.x, xLimits.y, progress);
            float y = amp * Mathf.Sin((TAU * freq * x) + (Time.timeSinceLevelLoad * movementSpeed));
            line.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    public void EnableMiniGame()
    {
        if (magnometer.GetPlaying())
        {
            return;
        }
        if (IsHoldingItem())
        {
            minigameCanvas.transform.DOLocalMove(showPanelPos, panelAnimationTime);

            //minigameCanvas.SetActive(true);
            isPlaying = true;
            SetGoalValues();
            temperatureSlider.value = -273;
            amplitudeSlider.interactable = true;
            frequencySlider.interactable = true;
            minigameSFX.Play();
        }
    }
    public void SetGoalValues()
    {
        goalAmplitude = Random.Range(0.2f, 1f);
        goalFrequency = Random.Range(0.5f, 2.5f);
    }
    public void ToggleHelpCanvas()
    {
        helpCanvasEnabled = !helpCanvasEnabled;
        Debug.Log(helpCanvasEnabled);
        helpCanvas.transform.DOLocalMove(helpCanvasEnabled ? showPanelPos2 : hidePanelPos2, panelAnimationTime).SetEase(helpCanvasEnabled ? Ease.OutQuad : Ease.InQuad);
    }
    public void DisableMiniGame()
    {
        try
        {
            minigameCanvas.transform.DOLocalMove(hidePanelPos, panelAnimationTime).SetEase(Ease.InQuad);
        }
        catch
        {
            Debug.Log("Minigame canvas being weird");
        }
        isPlaying = false;
        ResetMiniGame();
        temperatureSlider.value = -273;
        minigameSFX.Stop();
        if (helpCanvasEnabled)
        {
            ToggleHelpCanvas();
        }
        //helpCanvas.SetActive(false);


    }


    public void ResetMiniGame()
    {
        hasWon = false;
        playerAmplitude = 1f;
        playerFrequency = 1f;
        amplitudeSlider.value = 1f;
        frequencySlider.value = 1f;
        temperatureSlider.value = -273f;
        amplitudeSlider.interactable = true;
        frequencySlider.interactable = true;
        SetGoalValues();

    }

    public void ChangeAmplitude(float amp)
    {
        playerAmplitude = amp;
    }

    public void ChangeFrequency(float freq)
    {
        playerFrequency = freq;
    }

}
