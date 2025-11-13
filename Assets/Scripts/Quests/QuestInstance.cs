using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[System.Serializable]
public class QuestInstance
{
    public SkoogeQuestSO questData;
    public bool isActive = true;
    public bool[] progress;

    public QuestInstance(SkoogeQuestSO data)
    {
        questData = data;
        progress = new bool[data.questItems.Length];
    }

    public bool IsComplete()
    {
        for (int i = 0; i < progress.Length; i++)
        {
            if (progress[i] == false)
            {
                return false;
            }
        }
        return true;
    }
    public void UpdateProgress(int itemIndex, bool isCompleted)
    {
        progress[itemIndex] = isCompleted;
        if (IsComplete())
        {
            CompleteQuest();
        }
    }

    public void CompleteQuest()
    {
        isActive = false;
        Debug.Log("Quest " + questData.questName + " was completed");
        
    }
}
