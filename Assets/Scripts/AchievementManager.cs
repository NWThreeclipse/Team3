using UnityEngine;
using Steamworks;
using JetBrains.Annotations;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam not initialized!");
            return;
        }
    }

    public void UnlockAchievement(string achievementID)
    {
        SteamUserStats.SetAchievement(achievementID);

        SteamUserStats.StoreStats();
    }
}
