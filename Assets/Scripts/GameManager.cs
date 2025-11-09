using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    [SerializeField] private Vector2 itemSpawnrate;
    private List<ItemSO> gameItems;
    private List<ItemSO> commonItems;
    private List<ItemSO> uncommonItems;
    private List<ItemSO> anomalousItems;

    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private ConveyorBelt conveyorBelt;

    [SerializeField] private float alarmThreshold;
    public event Action<GameManager> AlarmThreshold;
    private bool alarmTriggered = false;

    [SerializeField] private List<Sprite> trashSprites;
    [SerializeField] private GameObject trashPrefab;
    [SerializeField] private Vector2 trashSpawnrate;

    private bool anomalousItemSpawnedToday = false;
    private int itemsSpawnedToday = 0;

    [SerializeField] private List<DialogueTree> dailyDialogues;
    [SerializeField] private DialogueManager dialogueManager;
    private bool isDialogueEnded = false;

    public float GetTime() => timer;


    private void Start()
    {
        gameItems = Resources.LoadAll<ItemSO>("").ToList();
        if (gameItems.Count > 0)
        {
            commonItems = gameItems.Where(item => item.Rarity == Rarity.Common).ToList();
            uncommonItems = gameItems.Where(item => item.Rarity == Rarity.Uncommon).ToList();
            anomalousItems = gameItems.Where(item => item.Rarity == Rarity.Anomalous).ToList();
        }
        dialogueManager.StartDialogue(dailyDialogues[dayCounter].nodes[0]);
        dialogueManager.OnDialogueEnd += HandleDialogueEnd;

    }
    private void Update()
    {
        if(isDialogueEnded)
        {
            PassiveDeplete();
            CountDown();
        }

    }

    private void HandleDialogueEnd(DialogueManager dialogueManager)
    {
        isDialogueEnded = true;
        StartCoroutine(SpawnItem());
        StartCoroutine(SpawnTrash());
    }
    



    private IEnumerator SpawnItem()
    {
        while(true)
        {
            bool itemType = UnityEngine.Random.value <= .8f;
            bool spawnAnomalousItem = UnityEngine.Random.value <= 0.1f && !anomalousItemSpawnedToday;
            ItemSO itemData;

            if (spawnAnomalousItem && itemsSpawnedToday > 0)
            {
                anomalousItemSpawnedToday = true;
                itemsSpawnedToday++;
                int randomIndex = UnityEngine.Random.Range(0, anomalousItems.Count);
                itemData = anomalousItems[randomIndex];
                //time slow when item is picked up
                //music weird when spawn

            }
            else
            {
                int randomIndex = UnityEngine.Random.Range(0, itemType ? commonItems.Count : uncommonItems.Count);
                itemData = itemType ? commonItems[randomIndex] : uncommonItems[randomIndex];
            }
                
            GameObject itemInstance = Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);
            Item item = itemInstance.GetComponent<Item>();
            item.SetItemData(itemData);
            conveyorBelt.AddItem(itemInstance);
            yield return new WaitForSeconds(UnityEngine.Random.Range(itemSpawnrate.x, itemSpawnrate.y));
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
            yield return new WaitForSeconds(UnityEngine.Random.Range(trashSpawnrate.x, trashSpawnrate.y));
        }
    }

    private void CountDown()
    {
        if (isCountingDown)
        {
            timer -= Time.deltaTime;

            if(timer <= 0f)
            {
                StatsController.Instance.IncrementDay();
                LoadScene("WinScene");

            }
        }
    }


    public void AddStat(Sorting type, float value)
    {
        switch (type)
        {
            case Sorting.Organic: organicStat = Mathf.Min(organicStat + (value * 50), maxStat); break;
            case Sorting.Fuel: fuelStat = Mathf.Min(fuelStat + (value * 50), maxStat); break;
            case Sorting.Scrap: scrapStat = Mathf.Min(scrapStat + (value * 50), maxStat); break;
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
