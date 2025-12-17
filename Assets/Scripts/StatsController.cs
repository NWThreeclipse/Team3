using System.Collections.Generic;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    public static StatsController Instance;

    [SerializeField] private int dayNumber;
    [SerializeField] private int itemsSorted;
    [SerializeField] private int correctSorted;
    [SerializeField] private int incorrectSorted;
    [SerializeField] private int anomalousItemsRecieved;
    [SerializeField] private int rebellionScore;

    [SerializeField] private List<string> anomalousItems = new List<string>();


    public int GetDays() => dayNumber;
    public int GetItems() => itemsSorted;
    public int GetCorrectItems() => correctSorted;
    public int GetIncorrectItems() => incorrectSorted;
    public int GetAnomalousItems() => anomalousItemsRecieved;

    public int GetRebellionScore() => rebellionScore;

    public List<string> GetAnomalousItemNames() => anomalousItems;

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

    public void IncrementRebellionScore()
    {
        rebellionScore++;
    }

    public void IncrementAnomalousItem(string anomName)
    {
        anomalousItemsRecieved++;
        anomalousItems.Add(anomName);
    }

    public void SaveStats()
    {
        PlayerPrefs.SetInt("DayNumber", dayNumber);
        PlayerPrefs.SetInt("ItemsSorted", itemsSorted);
        PlayerPrefs.SetInt("CorrectlySorted", correctSorted);
        PlayerPrefs.SetInt("IncorrectlySorted", incorrectSorted);
        PlayerPrefs.SetInt("AnomalousItems", anomalousItemsRecieved);
        PlayerPrefs.SetInt("RebellionScore", rebellionScore);

        for (int i = 0; i < anomalousItems.Count; i++)
        {
            PlayerPrefs.SetString("AnomalousItem_" + i, anomalousItems[i]);
        }
    }

    public void LoadStats()
    {
        dayNumber = PlayerPrefs.GetInt("DayNumber", 0);
        itemsSorted = PlayerPrefs.GetInt("ItemsSorted", 0);
        correctSorted = PlayerPrefs.GetInt("CorrectlySorted", 0);
        incorrectSorted = PlayerPrefs.GetInt("IncorrectlySorted", 0);
        anomalousItemsRecieved = PlayerPrefs.GetInt("AnomalousItems", 0);
        rebellionScore = PlayerPrefs.GetInt("RebellionScore", 0);

        anomalousItems.Clear();
        for (int i = 0; i < anomalousItemsRecieved; i++)
        {
            anomalousItems.Add(PlayerPrefs.GetString("AnomalousItem_" + i));
        }
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
        rebellionScore = 0;
        anomalousItems.Clear();
    }

    public void ResetDays()
    {
        dayNumber = 0;
    }

    public void ResetDailyStats()
    {
        itemsSorted = 0;
        correctSorted = 0;
        incorrectSorted = 0;
        anomalousItemsRecieved = 0;
    }

    public void SetDay(float day)
    {
        dayNumber = (int)day;
    }

}
