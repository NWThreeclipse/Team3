using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField] private float alarmThreshold;
    public event Action<GameManager> AlarmThreshold;
    private bool alarmTriggered = false;

    [SerializeField] private List<Sprite> trashSprites;
    [SerializeField] private GameObject trashPrefab;
    [SerializeField] private float trashSpawnrate;


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
        StartCoroutine(SpawnTrash());
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
            int randomIndex = UnityEngine.Random.Range(0, UnityEngine.Random.value <= itemSpawnrate ? commonItems.Count : uncommonItems.Count);
            GameObject itemInstance = Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);
            Item item = itemInstance.GetComponent<Item>();
            item.SetItemData(UnityEngine.Random.value <= itemSpawnrate ? commonItems[randomIndex] : uncommonItems[randomIndex]);
            conveyorBelt.AddItem(itemInstance);
            yield return new WaitForSeconds(itemSpawnrate);
        }
    }

    private IEnumerator SpawnTrash()
    {
        yield return new WaitForSeconds(1f);
        while(true)
        {
            int randomIndex = UnityEngine.Random.Range(0, trashSprites.Count);
            GameObject trashInstance = Instantiate(trashPrefab, spawnPoint.position, Quaternion.identity);
            Trash trash = trashInstance.GetComponent<Trash>();
            trash.SetSprite(trashSprites[randomIndex]);
            conveyorBelt.AddItem(trashInstance);
            yield return new WaitForSeconds(trashSpawnrate);
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

        CheckThreshold();
        CheckLoss();
    }


    public void CheckThreshold()
    {
        if (!alarmTriggered &&
            (organicStat <= alarmThreshold || scrapStat <= alarmThreshold || fuelStat <= alarmThreshold))
        {
            alarmTriggered = true;
            AlarmThreshold?.Invoke(this);
        }

        if (organicStat > alarmThreshold && fuelStat > alarmThreshold && scrapStat > alarmThreshold)
        {
            alarmTriggered = false;
        }
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
