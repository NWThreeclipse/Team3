using DG.Tweening;
using System.Collections;
using UnityEngine;

public class SupervisorController : MonoBehaviour
{
    [SerializeField] private GameObject supervisor;
    [SerializeField] private Transform[] startEndPoints; // 0 = left, 1 = middle, 2 = right
    [SerializeField] private Vector2 moveSpeeds;
    [SerializeField] private Vector2 moveCooldown;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Skooge skooge;
    [SerializeField] private float suspicionDamage;

    [SerializeField] private bool isWalking;
    [SerializeField] private bool isOnCooldown;
    [SerializeField] private bool isSus;
    [SerializeField] private bool gotCaught;
    private Vector3 currentTarget;
    private float previousSuspicionValue;

    private void Start()
    {
        if (StatsController.Instance.GetDays() <= 1)
        {
            supervisor.SetActive(false);
            gameObject.SetActive(false);
            
        }
        else if (StatsController.Instance.GetDays() >= 3)
        {
            isSus = true;
        }

        currentTarget = startEndPoints[1].position;
        supervisor.transform.position = startEndPoints[0].position;
        previousSuspicionValue = gameManager.GetSuspicionStat();
    }

    private void Update()
    {
        float currentSuspicion = gameManager.GetSuspicionStat();

        if (currentSuspicion >= 50 && previousSuspicionValue < 50)
        {
            StartCoroutine(ImmediateInspection());
        }
        else if (currentSuspicion >= 80 && previousSuspicionValue < 80)
        {
            StartCoroutine(ImmediateInspection());
        }

        previousSuspicionValue = currentSuspicion;
        if (!isWalking && !isOnCooldown)
        {
            Move();
        }

        if (isWalking && isSus && !gotCaught)
        {
            if (skooge.GetIsItemStaying())
            {
                gameManager.AddSuspicion(suspicionDamage);
                gotCaught = true;
            }
        }

        if (isWalking && IsAtDestination())
        {
            isWalking = false;
            StartCoroutine(InspectionCooldown());
        }

    }

    private void Move()
    {
        if (Mathf.Abs(supervisor.transform.position.x - startEndPoints[0].position.x) < 0.1f)
        {
            currentTarget = startEndPoints[1].position;  // Move right
        }
        else
        {
            currentTarget = startEndPoints[0].position;  // Move left
        }

        supervisor.transform.DOMove(currentTarget, Random.Range(moveSpeeds.x, moveSpeeds.y)).SetEase(Ease.InQuad);
        isWalking = true;
    }

    private IEnumerator InspectionCooldown()
    {
        isOnCooldown = true; 
        float cooldownTime = Random.Range(moveCooldown.x, moveCooldown.y); 
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
        gotCaught = false;
    }

    private IEnumerator ImmediateInspection()
    {
        isOnCooldown = false;
        gotCaught = false;

        Move();
        yield return null;
    }

    private bool IsAtDestination()
    {
        float tolerance = 0.1f;
        return Mathf.Abs(supervisor.transform.position.x - currentTarget.x) < tolerance;
    }
}
