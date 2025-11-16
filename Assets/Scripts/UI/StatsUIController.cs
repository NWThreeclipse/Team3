using TMPro;
using UnityEngine;

public class StatsUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text totalItems;
    [SerializeField] private TMP_Text correctItems;
    [SerializeField] private TMP_Text incorrectItems;
    [SerializeField] private TMP_Text sortSpeed;
    [SerializeField] private TMP_Text anomalousItems;

    private void Start()
    {
        totalItems.text = StatsController.Instance.GetItems().ToString();
        correctItems.text = StatsController.Instance.GetCorrectItems().ToString();
        incorrectItems.text = StatsController.Instance.GetIncorrectItems().ToString();
        if (StatsController.Instance.GetItems() == 0)
        {
            sortSpeed.text = "0";
        } else
        {
            sortSpeed.text = (150 / StatsController.Instance.GetItems()).ToString();
        }
        anomalousItems.text = StatsController.Instance.GetAnomalousItems().ToString();
    }
}
