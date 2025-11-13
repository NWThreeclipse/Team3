using NUnit.Framework.Interfaces;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Skooge : DragZone
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private QuestInstance currentQuest; 

    public ItemSO[] GetQuestItems() => currentQuest.questData.questItems;


    protected override void HandleItemRelease(Draggable draggable)
    {
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
        int day = gameManager.GetDay();
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
