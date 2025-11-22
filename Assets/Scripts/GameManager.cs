using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float organicStat;
    [SerializeField] private float fuelStat;
    [SerializeField] private float scrapStat;
    [SerializeField] private float suspicionStat;

    [SerializeField] private float maxStat;

    [SerializeField] private float organicDepleteRate;
    [SerializeField] private float fuelDepleteRate;
    [SerializeField] private float scrapDepleteRate;
    [SerializeField] private float suspicionDepleteRate;

    //DAYS ARE FROM 0-5
    [Range(0,5)][SerializeField] private int dayCounter;
    [SerializeField] private float timer;
    [SerializeField] private bool isCountingDown;
    [SerializeField] private Vector2 itemSpawnrate;
    private List<ItemSO> gameItems;
    private List<ItemSO> commonItems;
    private List<ItemSO> uncommonItems;
    [SerializeField] private List<ItemSO> anomalousItems;

    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private ConveyorBelt conveyorBelt;

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
    
    [SerializeField] private Skooge skooge;
    private ItemSO[] mustSpawnItems;

    private float organicGraceTimer = 0f;
    private float fuelGraceTimer = 0f;
    private float scrapGraceTimer = 0f;

    private bool organicInGracePeriod = false;
    private bool fuelInGracePeriod = false;
    private bool scrapInGracePeriod = false;

    private const float gracePeriodDuration = 10f;

    [SerializeField] private ItemSO[] itemVariants;
    public float GetTime() => timer;
    public int GetDay() => dayCounter;

    public float GetSuspicionStat() => suspicionStat;


    private void Start()
    {
        dayCounter = StatsController.Instance.GetDays();
        gameItems = Resources.LoadAll<ItemSO>("").ToList();
        if (gameItems.Count > 0)
        {
            commonItems = gameItems.Where(item => item.Rarity == Rarity.Common).ToList();
            uncommonItems = gameItems.Where(item => item.Rarity == Rarity.Uncommon).ToList();
        }
        if (dayCounter >= 2)
        {
            if (dayCounter != 2)
            {
                mustSpawnItems = skooge.GetQuestItems();
            }
        }
        dialogueManager.StartDialogue(dailyDialogues[dayCounter].nodes[0]);
        dialogueManager.OnDialogueEnd += HandleDialogueEnd;
    }

    

   
    private void Update()
    {
        if(isDialogueEnded)
        {
            CountDown();
            PassiveDeplete();
          
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
        while (true)
        {
            ItemSO itemData;

            if ( dayCounter > 1)
            {
                // check to spawn a quest item (must spawn item)
                bool spawnQuestItem = dayCounter >= 3 && mustSpawnItems.Length > 0 && UnityEngine.Random.value <= 0.2f;
                if (spawnQuestItem)
                {
                    itemData = mustSpawnItems[UnityEngine.Random.Range(0, mustSpawnItems.Length)];
                }
                else
                {
                    // check to spawn an anomalous item
                    bool spawnAnomalousItem = UnityEngine.Random.value <= 0.2f && !anomalousItemSpawnedToday && dayCounter >= 2;

                    if (spawnAnomalousItem && itemsSpawnedToday > 0)
                    {
                        anomalousItemSpawnedToday = true;
                        itemData = anomalousItems[dayCounter - 2];
                        //anom = true
                    }
                    else
                    {
                        // randomly pick common or uncommon 
                        bool itemType = UnityEngine.Random.value <= 0.8f;
                        int randomIndex = UnityEngine.Random.Range(0, itemType ? commonItems.Count : uncommonItems.Count);
                        itemData = itemType ? commonItems[randomIndex] : uncommonItems[randomIndex];
                    }
                }
            }
            else 
            {
                //day 1 only spawns commons
                int randomIndex = UnityEngine.Random.Range(0, commonItems.Count);
                itemData = commonItems[randomIndex];
            }
            itemsSpawnedToday++;

            //check if anom, play sound

            // Instantiate the item 
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

            if (timer <= 0f && dayCounter == 5 && CheckWinThreshold(dayCounter))
            {
                if (StatsController.Instance.GetRebellionScore() >= 3)
                {
                    //rebellion ending
                }
                LoadScene("WinScene");
            }
            if(timer <= 0f && CheckWinThreshold(dayCounter))
            {
                StatsController.Instance.IncrementDay();
                LoadScene("BarracksScene");

            }
        }
    }


    public void AddStat(Sorting type, float value)
    {
        switch (type)
        {
            case Sorting.Organic:
                organicStat = Mathf.Min(organicStat + (value*50), maxStat); 
                break;
            case Sorting.Fuel:
                fuelStat = Mathf.Min(fuelStat + (value*50), maxStat); 
                break;
            case Sorting.Scrap:
                scrapStat = Mathf.Min(scrapStat + (value*50), maxStat); 
                break;
        }
    }

    public void AddSuspicion(float value)
    {
        suspicionStat = MathF.Min(suspicionStat + value, maxStat);
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
        if (!organicInGracePeriod)
        {
            organicStat -= organicDepleteRate * Time.deltaTime;
        }
        if (!fuelInGracePeriod)
        {
            fuelStat -= fuelDepleteRate * Time.deltaTime;
        }
        if (!scrapInGracePeriod)
        {
            scrapStat -= scrapDepleteRate * Time.deltaTime;
        }

        organicStat = Mathf.Max(organicStat, 0);
        fuelStat = Mathf.Max(fuelStat, 0);
        scrapStat = Mathf.Max(scrapStat, 0);

        suspicionStat -= suspicionDepleteRate * Time.deltaTime;
        suspicionStat = MathF.Max(suspicionStat, 0);

        CheckLoss();

    }



    public bool CheckWinThreshold(int day)
    {
        if (day <= 1)
        {
            return true;
        }
        else if (day == 2)
        {
            return scrapStat >= 40 || fuelStat >= 40 || organicStat >= 40;
        }
        else if (day == 3)
        {
            return scrapStat >= 50 || fuelStat >= 50 || organicStat >= 50;
        }
        else if (day == 4)
        {
            return scrapStat >= 60 || fuelStat >= 60 || organicStat >= 60;
        }
        else
        {
            return scrapStat >= 40 && fuelStat >= 40 && organicStat >= 40;
        }

    }
    private void HandleGracePeriod(ref float resourceStat, ref bool inGracePeriod, ref float graceTimer)
    {
        if (resourceStat <= 0 && !inGracePeriod)
        {
            inGracePeriod = true;
            graceTimer = gracePeriodDuration;
            if (!alarmTriggered && resourceStat <= 0)
            {
                alarmTriggered = true;
                AlarmThreshold?.Invoke(this);
            }
            Debug.Log("Grace period started for resource");
        }

        if (inGracePeriod)
        {
            graceTimer -= Time.deltaTime;
        }
    }
    public void CheckLoss()
    {
        HandleGracePeriod(ref organicStat, ref organicInGracePeriod, ref organicGraceTimer);
        HandleGracePeriod(ref fuelStat, ref fuelInGracePeriod, ref fuelGraceTimer);
        HandleGracePeriod(ref scrapStat, ref scrapInGracePeriod, ref scrapGraceTimer);

        if (organicStat <= 0 && organicInGracePeriod && organicGraceTimer <= 0)
        {
            // Trigger organic loss logic
            Debug.Log("Food loss ending");
            LoadScene("LossScene");
        }

        if (fuelStat <= 0 && fuelInGracePeriod && fuelGraceTimer <= 0)
        {
            // Trigger fuel loss logic
            Debug.Log("Fuel loss ending");
            LoadScene("LossScene");

        }

        if (scrapStat <= 0 && scrapInGracePeriod && scrapGraceTimer <= 0)
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
