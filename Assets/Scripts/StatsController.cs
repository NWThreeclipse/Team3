using UnityEngine;

public class StatsController : MonoBehaviour
{
    public static StatsController Instance;

    [SerializeField] private int dayNumber;
    [SerializeField] private int itemsSorted;
    [SerializeField] private int correctSorted;
    [SerializeField] private int incorrectSorted;
    [SerializeField] private int anomalousItemsRecieved;


    public int GetDays() => dayNumber;
    public int GetItems() => itemsSorted;
    public int GetCorrectItems() => correctSorted;
    public int GetIncorrectItems() => incorrectSorted;
    public int GetAnomalousItems() => anomalousItemsRecieved;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IncrementItem(bool correct)
    {
        itemsSorted++;
        if (correct)
        {
            correctSorted++;
        }
        else
        {
            incorrectSorted++;
        }
    }

    public void SaveStats()
    {
        PlayerPrefs.SetInt("DayNumber", dayNumber);
        PlayerPrefs.SetInt("ItemsSorted", itemsSorted);
        PlayerPrefs.SetInt("CorrectlySorted", correctSorted);
        PlayerPrefs.SetInt("IncorrectlySorted", incorrectSorted);
        PlayerPrefs.SetInt("AnomalousItems", anomalousItemsRecieved);
    }

    public void LoadStats()
    {
        dayNumber = PlayerPrefs.GetInt("DayNumber", 0);
        itemsSorted = PlayerPrefs.GetInt("ItemsSorted", 0);
        correctSorted = PlayerPrefs.GetInt("CorrectlySorted", 0);
        incorrectSorted = PlayerPrefs.GetInt("IncorrectlySorted", 0);
        anomalousItemsRecieved = PlayerPrefs.GetInt("AnomalousItems", 0);
    }

    public void IncrementDay()
    {
        dayNumber++;
    }
    public void ResetStats()
    {
        itemsSorted = 0;
        correctSorted = 0;
        incorrectSorted = 0;
        anomalousItemsRecieved = 0;
    }

}
