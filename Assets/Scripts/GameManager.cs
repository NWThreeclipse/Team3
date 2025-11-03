using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float organicStat;
    [SerializeField] private float fuelStat;
    [SerializeField] private float scrapStat;

    [SerializeField] private float maxStat;

    [SerializeField] private float organicDepleteRate;
    [SerializeField] private float fuelDepleteRate;
    [SerializeField] private float scrapDepleteRate;

    [SerializeField] private int dayCounter;
    [SerializeField] private float timer;
    [SerializeField] private bool isCountingDown;
    [SerializeField] private float itemSpawnrate;
    private List<ItemSO> gameItems;
    private List<ItemSO> commonItems;
    private List<ItemSO> uncommonItems;

    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private ConveyorBelt conveyorBelt;

    public float GetTime() => timer;


    private void Start()
    {
        gameItems = Resources.LoadAll<ItemSO>("").ToList();
        if (gameItems.Count > 0)
        {
            commonItems = gameItems.Where(item => item.Rarity == Rarity.Common).ToList();
            uncommonItems = gameItems.Where(item => item.Rarity == Rarity.Uncommon).ToList();
        }
        StartCoroutine(SpawnItem());
    }
    private void Update()
    {

        PassiveDeplete();
        CountDown();

    }

    private IEnumerator SpawnItem()
    {
        while(true)
        {
            int randomIndex = Random.Range(0, Random.value <= itemSpawnrate ? commonItems.Count : uncommonItems.Count);
            GameObject itemInstance = Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);
            Item item = itemInstance.GetComponent<Item>();
            item.SetItemData(Random.value <= itemSpawnrate ? commonItems[randomIndex] : uncommonItems[randomIndex]);
            conveyorBelt.AddItem(itemInstance);
            yield return new WaitForSeconds(itemSpawnrate);
        }
    }

    private void CountDown()
    {
        if (isCountingDown)
        {
            timer -= Time.deltaTime;

            if(timer <= 0f)
            {
                LoadScene("WinScene");

            }
        }
    }


    public void AddStat(Sorting type, float value)
    {
        switch (type)
        {
            case Sorting.Organic: organicStat = Mathf.Min(organicStat + value, maxStat); break;
            case Sorting.Fuel: fuelStat = Mathf.Min(fuelStat + value, maxStat); break;
            case Sorting.Scrap: scrapStat = Mathf.Min(scrapStat + value, maxStat); break;
        }
    }

    public void SubtractStat(Sorting type, float value)
    {
        switch (type)
        {
            case Sorting.Organic: organicStat = Mathf.Max(organicStat - value, 0); break;
            case Sorting.Fuel: fuelStat = Mathf.Max(fuelStat - value, 0); break;
            case Sorting.Scrap: scrapStat = Mathf.Max(scrapStat - value, 0); break;
        }
    }


    public float[] GetStats()
    {
        float[] stats = { organicStat, fuelStat, scrapStat };
        return stats;
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
            LoadScene("LossScene");
        }

        if (fuelStat <= 0)
        {
            // Trigger fuel loss logic
            Debug.Log("Fuel loss ending");
            LoadScene("LossScene");

        }

        if (scrapStat <= 0)
        {
            // Trigger scrap loss logic
            Debug.Log("Scrap loss ending");
            LoadScene("LossScene");

        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
