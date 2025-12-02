using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThermalScanner : DragZone
{
    [SerializeField] private bool isPlaying = false;
    [SerializeField] private GameObject minigameCanvas;
    //[SerializeField] private ViewingBoard viewingBoard;


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
    [SerializeField] private TMP_Text temperatureText;

    private const float TAU = 2 * Mathf.PI;

    [SerializeField] private Slider amplitudeSlider;
    [SerializeField] private Slider frequencySlider;

    [SerializeField] private AudioSource minigameSFX;
    [SerializeField] private AudioSource minigameWinSFX;



    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        DisableMiniGame();
    }
    void Start()
    {
        minigameCanvas.SetActive(false);
        goalLine.positionCount = points;
        playerLine.positionCount = points;
        ResetMiniGame();
        temperatureText.text = "";

    }

    void Update()
    {

        if (isPlaying)
        {
            Draw(goalLine, goalAmplitude, goalFrequency);
            Draw(playerLine, playerAmplitude, playerFrequency);
            CheckWin();

        }
    }

    public void CheckWin()
    {
        if(IsHoldingItem())
        {
            if (Mathf.Abs(playerAmplitude - goalAmplitude) < amplitudeWinThreshold && Mathf.Abs(playerFrequency - goalFrequency) < frequencyWinThreshold)
            {
                temperatureText.text = "Temperature: " + GetItem().TemperatureC.ToString() + "°C";
                amplitudeSlider.interactable = false;
                frequencySlider.interactable = false;
                minigameSFX.Stop();
                minigameWinSFX.Play();
            }
        }
    }

    public void Draw(LineRenderer line, float amp, float freq)
    {
        for(int i = 0; i < points; i++)
        {
            float progress = (float)i / (points - 1);
            float x = Mathf.Lerp(xLimits.x, xLimits.y, progress);
            float y = amp * Mathf.Sin((TAU * freq * x) + (Time.timeSinceLevelLoad * movementSpeed));
            line.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    public void EnableMiniGame()
    {
        if(IsHoldingItem())
        {
            minigameCanvas.SetActive(true);
            isPlaying = true;
            SetGoalValues();
            temperatureText.text = "";
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

    public void DisableMiniGame()
    {
        minigameCanvas.SetActive(false);
        isPlaying = false;
        ResetMiniGame();
        temperatureText.text = "";
        minigameSFX.Stop();

    }


    public void ResetMiniGame()
    {
        playerAmplitude = 1f;
        playerFrequency = 1f;
        amplitudeSlider.value = 1f;
        frequencySlider.value = 1f;
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
