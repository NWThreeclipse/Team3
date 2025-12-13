using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SceneFader scenefader;
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
    [Range(0, 5)][SerializeField] private int dayCounter;
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
    private bool gameStarted = false;

    [SerializeField] private Skooge skooge;
    private ItemSO[] mustSpawnItems;

    private float organicGraceTimer = 0f;
    private float fuelGraceTimer = 0f;
    private float scrapGraceTimer = 0f;

    private bool organicInGracePeriod = false;
    private bool fuelInGracePeriod = false;
    private bool scrapInGracePeriod = false;

    private const float gracePeriodDuration = 10f;

    [SerializeField] private float organicWeight;
    [SerializeField] private float scrapWeight;
    [SerializeField] private float fuelWeight;

    private const float weightShift = 0.05f;

    [SerializeField] private ItemSO[] itemVariants;
    [SerializeField] private StartLever startLever;
    [SerializeField] private BarkManager barkManager;

    private float pointFadeTime = 0.5f;
    private float multi = 1f;

    private bool anom = false;

    [SerializeField] private MusicController musicController;

    [SerializeField] private Transform organicText;
    [SerializeField] private Transform fuelText;
    [SerializeField] private Transform scrapText;

    [SerializeField] private GameObject textPrefab;

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

        DialogueTree tree;
        if (dayCounter <= 2)
        {
            tree = dailyDialogues[dayCounter];
        }
        else
        {
            if (dayCounter == 3)
            {
                tree = dailyDialogues[skooge.GetQuestName() == "Repairs" ? 3 : 4];
            }
            else if (dayCounter == 4)
            {
                tree = dailyDialogues[skooge.GetQuestName() == "Medicine" ? 5 : 6];
            }
            else
            {
                tree = dailyDialogues[skooge.GetQuestName() == "Weapon" ? 7 : 8];
            }
        }
        Debug.Log(tree.name);
        dialogueManager.StartDialogue(tree.nodes[0]);
        dialogueManager.OnDialogueEnd += HandleDialogueEnd;

        //if (dayCounter == 0 || dayCounter >= 3)
        //{

        //    if (tree == null || tree.nodes == null || tree.nodes.Count == 0 || tree.nodes[0] == null)
         //   {
          //      Debug.LogError("DialogueTree for day " + dayCounter + " is missing a root node!");
         //       return;
         //   }


        //}
        //else //if day 1-2
        //{
        //    HandleDialogueEnd(dialogueManager);
        //}

        ResetItemTypeWeights();
        conveyorBelt.ToggleBelt();
    }




    private void Update()
    {
        if (gameStarted)
        {
            CountDown();
            PassiveDeplete();

        }

    }

    private void HandleGameStart(StartLever lever)
    {
        gameStarted = true;
        if (dayCounter >= 3)
        {
            mustSpawnItems = skooge.GetQuestItems();
        }
        StartCoroutine(SpawnItem());
        StartCoroutine(SpawnTrash());
        conveyorBelt.ToggleBelt();
       

    }
    private void HandleDialogueEnd(DialogueManager dialogueManager)
    {
        startLever.OnGameStart += HandleGameStart;
        startLever.SetInteractible(true);

    }

    public void SetPointMultiplier(string multiplier)
    {
        if (float.TryParse(multiplier, out float result))
        {
            multi = result;
        }
        else
        {
            Debug.LogWarning("Invalid value entered.");
        }
    }

    public void SetItemSpawnMin(string min)
    {
        if (float.TryParse(min, out float result))
        {
            itemSpawnrate.x = result;
        }
        else
        {
            Debug.LogWarning("Invalid value entered.");
        }
    }
    public void SetItemSpawnMax(string max)
    {
        if (float.TryParse(max, out float result))
        {
            itemSpawnrate.y = result;
        }
        else
        {
            Debug.LogWarning("Invalid value entered.");
        }
    }



    private IEnumerator SpawnItem()
    {
        while (true)
        {
            ItemSO itemData;

            if (dayCounter > 1)
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
                        anom = true;
                    }
                    else
                    {
                        // choose rarity first
                        bool isCommon = UnityEngine.Random.value <= 0.8f;
                        List<ItemSO> pool = isCommon ? commonItems : uncommonItems;

                        Sorting chosenType = GetWeightedItemType();

                        List<ItemSO> filtered = pool.Where(i => i.Sorting == chosenType).ToList();

                        itemData = filtered[UnityEngine.Random.Range(0, filtered.Count)];

                        AdjustWeights(chosenType);

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
            if (anom)
            {
                anom = false;
                StartCoroutine(AnomalousBark());
            }
            //check if anom, play sound

            // Instantiate the item 
            GameObject itemInstance = Instantiate(itemPrefab, spawnPoint.position + new Vector3(0, UnityEngine.Random.Range(-0.1f, 0.3f)), Quaternion.identity);
            Item item = itemInstance.GetComponent<Item>();
            item.SetItemData(itemData);
            conveyorBelt.AddItem(itemInstance);
            AudioController.PlayItemSpawn();


            yield return new WaitForSeconds(UnityEngine.Random.Range(itemSpawnrate.x, itemSpawnrate.y));
        }
    }

    private IEnumerator AnomalousBark()
    {
        yield return new WaitForSeconds(1f);
        barkManager.StartPlayerBark();

    }
    private IEnumerator SpawnTrash()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            int randomIndex = UnityEngine.Random.Range(0, trashSprites.Count);
            GameObject trashInstance = Instantiate(trashPrefab, spawnPoint.position + new Vector3(0, UnityEngine.Random.Range(-0.4f, 0.3f)), Quaternion.identity);
            Trash trash = trashInstance.GetComponent<Trash>();
            trash.SetSprite(trashSprites[randomIndex]);
            conveyorBelt.AddItem(trashInstance);
            AudioController.PlayItemSpawn();
            yield return new WaitForSeconds(UnityEngine.Random.Range(trashSpawnrate.x, trashSpawnrate.y));
        }
    }

    private void CountDown()
    {
        if (isCountingDown)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f && dayCounter == 5)
            {
                if (StatsController.Instance.GetRebellionScore() >= 3)
                {
                    isCountingDown = false;
                    //rebellion ending
                    AchievementManager.Instance.UnlockAchievement("RECYCLAMATION_NARRATIVE_WIN");
                    scenefader.FadeToScene("NarrativeWinScene");
                    return;
                } 
                else
                {
                    isCountingDown = false;
                    AchievementManager.Instance.UnlockAchievement("RECYCLAMATION_NARRATIVE_LOSS");
                    musicController.FadeOutMusic();
                    scenefader.FadeToScene("NarrativeLossScene");
                }
                    
            }
            else if (timer <= 0f)
            {
                isCountingDown = false;
                StatsController.Instance.IncrementDay();
                musicController.FadeOutMusic();
                scenefader.FadeToScene("BarracksScene");

            }
        }
        else
        {
            timer = 0f;
        }
    }


    public void AddStat(Sorting type, float value)
    {
        switch (type)
        {
            case Sorting.Organic:
                organicStat = Mathf.Min(organicStat + (value * multi), maxStat);
                break;
            case Sorting.Fuel:
                fuelStat = Mathf.Min(fuelStat + (value * multi), maxStat);
                break;
            case Sorting.Scrap:
                scrapStat = Mathf.Min(scrapStat + (value * multi), maxStat);
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


    //unused quota code
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
            AchievementManager.Instance.UnlockAchievement("RECYCLAMATION_GAME_LOSS");
            musicController.FadeOutMusic();

            scenefader.FadeToScene("LossScene");
        }

        if (fuelStat <= 0 && fuelInGracePeriod && fuelGraceTimer <= 0)
        {
            // Trigger fuel loss logic
            Debug.Log("Fuel loss ending");
            AchievementManager.Instance.UnlockAchievement("RECYCLAMATION_GAME_LOSS");
            musicController.FadeOutMusic();

            scenefader.FadeToScene("LossScene");

        }

        if (scrapStat <= 0 && scrapInGracePeriod && scrapGraceTimer <= 0)
        {
            // Trigger scrap loss logic
            Debug.Log("Scrap loss ending");
            AchievementManager.Instance.UnlockAchievement("RECYCLAMATION_GAME_LOSS");
            musicController.FadeOutMusic();

            scenefader.FadeToScene("LossScene");
        }
        if (suspicionStat >= 99.5)
        {
            AchievementManager.Instance.UnlockAchievement("RECYCLAMATION_GAME_LOSS");
            musicController.FadeOutMusic();

            scenefader.FadeToScene("LossScene");
        }
    }

    public void SuspicionLoss()
    {
        AchievementManager.Instance.UnlockAchievement("RECYCLAMATION_GAME_LOSS");
        scenefader.FadeToScene("LossScene");
    }

    private void ResetItemTypeWeights()
    {
        organicWeight = 0.33f;
        scrapWeight = 0.33f;
        fuelWeight = 0.33f;
    }

    private Sorting GetWeightedItemType()
    {
        float total = organicWeight + scrapWeight + fuelWeight;
        float roll = UnityEngine.Random.value * total;

        if (roll < organicWeight)
            return Sorting.Organic;

        roll -= organicWeight;
        if (roll < scrapWeight)
            return Sorting.Scrap;

        return Sorting.Fuel;
    }

    private void AdjustWeights(Sorting chosenType)
    {


        switch (chosenType)
        {
            case Sorting.Organic:
                organicWeight -= weightShift * 2;
                scrapWeight += weightShift;
                fuelWeight += weightShift;
                break;

            case Sorting.Scrap:
                scrapWeight -= weightShift * 2;
                organicWeight += weightShift;
                fuelWeight += weightShift;
                break;

            case Sorting.Fuel:
                fuelWeight -= weightShift * 2;
                organicWeight += weightShift;
                scrapWeight += weightShift;
                break;
        }

        // Clamp to prevent negatives
        organicWeight = Mathf.Max(0.01f, organicWeight);
        scrapWeight = Mathf.Max(0.01f, scrapWeight);
        fuelWeight = Mathf.Max(0.01f, fuelWeight);
    }



    public IEnumerator CreatePointsText(bool correct, float score, Sorting bin)
    {
        GameObject pointsGO;
        if (bin == Sorting.Organic) //organic
        {
          pointsGO = Instantiate(textPrefab, organicText);
        }
        else if (bin == Sorting.Fuel) //fuel
        {
            pointsGO = Instantiate(textPrefab, fuelText);

        }
        else //scrap
        {
            pointsGO = Instantiate(textPrefab, scrapText);
        }

        TMP_Text pointsText = pointsGO.GetComponent<TMP_Text>();

        if (correct)
        {
            pointsText.text = "+" + score.ToString();
            pointsText.color = new Color32(13, 164, 97, 255);

        }
        else
        {
            pointsText.text = "-" + score.ToString();
            pointsText.color = new Color32(227, 22, 2, 255);
        }

        yield return new WaitForSeconds(1f);

        Color startColor = pointsText.color;
        Color endColor = startColor;
        endColor.a = 0;

        float time = 0;
        while (time < pointFadeTime)
        {
            float t = time / pointFadeTime;
            pointsText.color = Color.Lerp(startColor, endColor, t);
            time += Time.deltaTime;
            yield return null;
        }

        pointsText.color = endColor;
        Destroy(pointsGO);
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
