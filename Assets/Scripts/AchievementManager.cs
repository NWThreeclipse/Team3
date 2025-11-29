using Steamworks;
using System;
using UnityEngine;

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
        Debug.Log(SteamFriends.GetPersonaName());
    }

    public void UnlockAchievement(string achievementID)
    {

        bool isAchieved = false;

        bool success = SteamUserStats.GetAchievement(achievementID, out isAchieved);
        if (success && !isAchieved)
        {
            SteamUserStats.SetAchievement(achievementID);
            SteamUserStats.StoreStats();
        }
        else
        {
            if (isAchieved)
            {
                Debug.Log("Achievement has already been unlocked.");
            }
            else
            {
                Debug.Log("Failed to check achievement status.");
            }
        }
    }
}
