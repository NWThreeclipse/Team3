using UnityEngine;

[CreateAssetMenu(fileName = "SkoogeQuestSO", menuName = "Scriptable Objects/SkoogeQuestSO")]
public class SkoogeQuestSO : ScriptableObject
{
    public string questName;
    public ItemSO[] questItems;
    public float statReward;
    public int day;
}
