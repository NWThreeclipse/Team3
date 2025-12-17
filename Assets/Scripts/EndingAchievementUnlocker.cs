using UnityEngine;

public class EndingAchievementUnlocker : MonoBehaviour
{
    [SerializeField] private string achievementName;

    private void Start()
    {
        AchievementManager.Instance.UnlockAchievement(achievementName);
    }
}
