using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SupervisorController : MonoBehaviour
{
    [SerializeField] private int[] dailyAppearances;
    [SerializeField] private float dayDuration = 150f;

    [SerializeField] private GameObject supervisor;
    [SerializeField] private AudioSource footstepAudio;
    [SerializeField] private Transform[] startEndPoints;
    [SerializeField] private Transform[] idlePoints;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Skooge skooge;
    [SerializeField] private ViewingBoard viewingBoard;

    [SerializeField] private Vector2 walkSpeeds;
    [SerializeField] private Vector2 inspectionTimes;
    [SerializeField] private float suspicionDamage; 
    [SerializeField] private float viewingBoardDamage;



    private List<float> spawnTimes = new List<float>();
    private bool isRunningRoutine = false;
    private int currentDay;
    private bool isDangerousDay;

    private bool inspecting = false;
    void Start()
    {
        currentDay = StatsController.Instance.GetDays();

        if (currentDay <= 1)
        {
            supervisor.SetActive(false);
            gameObject.SetActive(false);
            return;
        }

        GenerateSpawnTimes(currentDay);
        StartCoroutine(DailySpawnLoop());
        isDangerousDay = currentDay >= 3;

    }
    private void Update()
    {
        if (isDangerousDay && skooge.GetIsItemStaying())
        {
            if (!skooge.GetCurrentQuest().IsComplete())
            {
                gameManager.AddSuspicion(suspicionDamage * Time.deltaTime);
            }
        }
        if (isDangerousDay && viewingBoard.IsHoldingItem())
        {
            if (viewingBoard.GetItem().Rarity == Rarity.Anomalous)
            {
                gameManager.AddSuspicion(viewingBoardDamage * Time.deltaTime);

            }
        }
    }

    private void GenerateSpawnTimes(int day)
    {
        spawnTimes.Clear();

        int appearances = dailyAppearances[Mathf.Clamp(day, 0, dailyAppearances.Length - 1)];
        float interval = dayDuration / appearances;

        for (int i = 0; i < appearances; i++)
        {
            float randomTime = (i * interval) + Random.Range(0f, interval);
            spawnTimes.Add(randomTime);
        }

        spawnTimes.Sort();
        Debug.Log("Spawn Times: " + string.Join(", ", spawnTimes));
    }

    private IEnumerator DailySpawnLoop()
    {
        float timer = 0f;
        int index = 0;

        while (index < spawnTimes.Count)
        {
            timer += Time.deltaTime;

            if (timer >= spawnTimes[index] && !isRunningRoutine)
            {
                StartCoroutine(SupervisorRoutine());
                index++;
            }

            yield return null;
        }
    }

    private IEnumerator SupervisorRoutine()
    {
        isRunningRoutine = true;

        // Play footsteps
        footstepAudio.Play();
        yield return new WaitForSeconds(2.5f);
        footstepAudio.Stop();

        // Walk to idle point
        Transform idle = idlePoints[Random.Range(0, idlePoints.Length)];
        float walkTime = Random.Range(walkSpeeds.x, walkSpeeds.y);

        yield return supervisor.transform
            .DOMove(idle.position, walkTime)
            .SetEase(Ease.InQuad)
            .WaitForCompletion();


        // Inspection phase
        float inspectDuration = Random.Range(inspectionTimes.x, inspectionTimes.y);
        float t = 0f;

        //inspecting
        inspecting = true;
        while (t < inspectDuration)
        {
            t += Time.deltaTime;

            if (isDangerousDay && skooge.GetIsItemStaying())
            {
                gameManager.SuspicionLoss();
            }
            yield return null;
        }
        inspecting = false;



        // Exit screen 
        Transform exit = startEndPoints[Random.Range(0, startEndPoints.Length)];
        walkTime = Random.Range(walkSpeeds.x, walkSpeeds.y);

        yield return supervisor.transform
            .DOMove(exit.position, walkTime)
            .SetEase(Ease.InQuad)
            .WaitForCompletion();


        isRunningRoutine = false;
    }

    public bool[] GetInspecting()
    {
        bool isLeft = Vector3.Distance(supervisor.transform.position, idlePoints[0].position) < 0.1f;
        bool[] stats = { isLeft, inspecting };
        return stats;
    }
}
