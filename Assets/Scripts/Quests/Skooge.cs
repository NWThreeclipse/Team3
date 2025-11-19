using NUnit.Framework.Interfaces;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Skooge : DragZone
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private QuestInstance currentQuest;
    [SerializeField] private float holdTime;
    [SerializeField] private float holdTimeRequired;
    [SerializeField] private bool itemIsStaying;
    

    public ItemSO[] GetQuestItems() => currentQuest.questData.questItems;
    public bool GetIsItemStaying() => itemIsStaying;
    
    public float[] GetHoldTimeInfo()
    {
        float[] stats = { holdTime, holdTimeRequired };
        return stats;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Item"))
        {
            var draggable = collision.GetComponent<Draggable>();

            if (draggable != null)
            {
                draggable.OnReleased += HandleItemRelease;
            }
        }

        isHoveringItem = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        holdTime += Time.deltaTime;
        itemIsStaying = true;

    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.CompareTag("Item"))
        {
            var draggable = collision.GetComponent<Draggable>();
            if (draggable != null)
            {
                draggable.OnReleased -= HandleItemRelease;
            }

            enteredItem = null;
            isHoldingItem = false;
            isHoveringItem = false;
            holdTime = 0;
            itemIsStaying = false;
        }
    }
    protected override void HandleItemRelease(Draggable draggable)
    {
        itemIsStaying = false;
        if (holdTime < 5f)
        {
            Item i = draggable.GetComponent<Item>();
            i.ResetPosition();
            return;
        }
        Collider2D[] hits = Physics2D.OverlapPointAll(draggable.transform.position);
        bool stillInBin = false;

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
            {
                stillInBin = true;
                break;
            }
        }
        if (!stillInBin)
        {
            return;
        }

        base.HandleItemRelease(draggable);

        Item item = draggable.GetComponent<Item>();
        if (item == null)
        {
            return;
        }
        if (currentQuest.isActive && currentQuest.questData.questItems.Contains(item.GetItemData()) && !currentQuest.progress[Array.IndexOf(currentQuest.questData.questItems, item.GetItemData())])
        {
            //will autocheck for completion in the QuestInstance
            currentQuest.UpdateProgress(Array.IndexOf(currentQuest.questData.questItems, item.GetItemData()), true);
            enteredItem = null;
            Destroy(draggable.gameObject);
        } else
        {
            item.ResetPosition();
        }
        
    }
    private void Start()
    {
        int day = StatsController.Instance.GetDays();
        if (day < 3)
        {
            gameObject.SetActive(false);
        }
        StartDailyQuest(day);
    }

    public void StartDailyQuest(int day)
    {
        if (day < 3)
        {
            return;
        }
        SkoogeQuestSO[] quests = Resources.LoadAll<SkoogeQuestSO>("");
        SkoogeQuestSO questData = quests[UnityEngine.Random.Range(0, quests.Length)];

        if (questData != null)
        {
            currentQuest = new QuestInstance(questData);
            Debug.Log("Started quest for Day " + day);
        }
        else
        {
            Debug.Log("No quest found for Day " + day);
        }

    }

    public void UpdateQuestProgress(int itemIndex, bool isCompleted)
    {
        if (currentQuest != null)
        {
            currentQuest.UpdateProgress(itemIndex, isCompleted);
        }
    }
}
