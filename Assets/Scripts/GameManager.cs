using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float organicStat;
    [SerializeField] private float fuelStat;
    [SerializeField] private float scrapStat;

    [SerializeField] private float maxStat;

    [SerializeField] private float organicDepleteRate;
    [SerializeField] private float fuelDepleteRate;
    [SerializeField] private float scrapDepleteRate;

    [SerializeField] private Bin organicBin;
    [SerializeField] private Bin fuelBin;
    [SerializeField] private Bin scrapBin;

    [SerializeField] private int dayCounter;
    [SerializeField] private float timer;
    [SerializeField] private bool isCountingDown;

    private void Update()
    {
        HandleBin(organicBin, ref organicStat);
        HandleBin(fuelBin, ref fuelStat);
        HandleBin(scrapBin, ref scrapStat);

        PassiveDeplete();
        CountDown();

    }

    private void CountDown()
    {
        if (isCountingDown)
        {
            timer -= Time.deltaTime;

            if(timer <= 0f)
            {
                //timer has hit zero
            }
        }
    }


    private void HandleBin(Bin bin, ref float stat)
    {
        if (bin.GetItems().Count != 0)
        {
            foreach (GameObject itemGameObject in bin.GetItems().ToList())
            {
                Item item = itemGameObject.GetComponent<Item>();
                if (!item.GetHeld())
                {
                    if (item.getItemData().Sorting == bin.getSortingType())
                    {
                        stat += item.getItemData().Value;
                        stat = Mathf.Min(stat, maxStat);
                        bin.DeleteItem(itemGameObject);
                        Destroy(itemGameObject);
                    }
                    else
                    {
                        stat -= item.getItemData().Value;
                        bin.DeleteItem(itemGameObject);
                        Destroy(itemGameObject);
                        CheckLoss();
                    }
                }
            }
        }
    }

    public float[] GetStats()
    {
        float[] stats = { organicStat, fuelStat, scrapStat };
        return stats;
    }

    public float GetTime()
    {
        return timer;
    }

    public void PassiveDeplete()
    {
        organicStat -= organicDepleteRate * Time.deltaTime;
        fuelStat -= fuelDepleteRate * Time.deltaTime;
        scrapStat -= scrapDepleteRate * Time.deltaTime;

        organicStat = Mathf.Max(organicStat, 0);
        fuelStat = Mathf.Max(fuelStat, 0);
        scrapStat = Mathf.Max(scrapStat, 0);

        CheckLoss();
    }

    public void CheckLoss()
    {
        if (organicStat <= 0)
        {
            // Trigger organic loss logic
            Debug.Log("Food loss ending");
        }

        if (fuelStat <= 0)
        {
            // Trigger fuel loss logic
            Debug.Log("Fuel loss ending");
        }

        if (scrapStat <= 0)
        {
            // Trigger scrap loss logic
            Debug.Log("Scrap loss ending");
        }
    }
}
