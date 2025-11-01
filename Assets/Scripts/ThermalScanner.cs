using TMPro;
using UnityEngine;

public class ThermalScanner : MonoBehaviour
{
    [SerializeField] private bool isPlaying = false;
    [SerializeField] private GameObject MinigameCanvas;
    [SerializeField] private ViewingBoard viewingBoard;


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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MinigameCanvas.SetActive(false);
        goalLine.positionCount = points;
        playerLine.positionCount = points;
        ResetMiniGame();
        temperatureText.text = "";

    }

    // Update is called once per frame
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
        if(viewingBoard.IsHoldingItem())
        {
            if (Mathf.Abs(playerAmplitude - goalAmplitude) < amplitudeWinThreshold && Mathf.Abs(playerFrequency - goalFrequency) < frequencyWinThreshold)
            {
                temperatureText.text = "Temperature: " + viewingBoard.GetItem().TemperatureC.ToString() + "°C";
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
        if(viewingBoard.IsHoldingItem())
        {
            MinigameCanvas.SetActive(true);
            isPlaying = true;
            SetGoalValues();
            temperatureText.text = "";
        }
    }
    public void SetGoalValues()
    {
        goalAmplitude = Random.Range(0.2f, 1f);
        goalFrequency = Random.Range(0.5f, 2.5f);
    }

    public void DisableMiniGame()
    {
        MinigameCanvas.SetActive(false);
        isPlaying = false;
        ResetMiniGame();
        temperatureText.text = "";

    }

    public void ResetMiniGame()
    {
        playerAmplitude = 1f;
        playerFrequency = 1f;
    }

    public void IncreaseAmp()
    {
        playerAmplitude = Mathf.Clamp(playerAmplitude + 0.1f, 0.2f, 1f);
    }

    public void DecreaseAmp()
    {
        playerAmplitude = Mathf.Clamp(playerAmplitude - 0.1f, 0.2f, 1f);
    }

    public void IncreaseFreq()
    {
        playerFrequency = Mathf.Clamp(playerFrequency + 0.1f, 0.5f, 2.5f);
    }

    public void DecreaseFreq()
    {
        playerFrequency = Mathf.Clamp(playerFrequency - 0.1f, 0.5f, 2.5f);
    }
}
