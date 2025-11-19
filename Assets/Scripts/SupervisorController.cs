using DG.Tweening;
using System.Collections;
using UnityEngine;

public class SupervisorController : MonoBehaviour
{
    [SerializeField] private GameObject supervisor;
    [SerializeField] private Transform[] startEndPoints; // 0 = left, 1 = right
    [SerializeField] private Vector2 moveSpeeds;
    [SerializeField] private Vector2 moveCooldown;
    [SerializeField] private GameManager gameManager;

    private bool isWalking = false;
    private bool isOnCooldown = false;
    private bool isSus = false;

    private Vector3 currentTarget;

    private void Start()
    {
        if (StatsController.Instance.GetDays() <= 1)
        {
            supervisor.SetActive(false);
            gameObject.SetActive(false);
            if (StatsController.Instance.GetDays() >= 3)
            {
                isSus = true;
            }
        }
        currentTarget = startEndPoints[1].position;

    }

    private void Update()
    {
        if (!isWalking && (gameManager.GetSuspicionStat() == 50 || gameManager.GetSuspicionStat() == 80))
        {
            Move();
        }
        if (!isWalking && !isOnCooldown && !IsAtDestination())
        {
            Move();
        }
        if (isWalking)
        {
            //scan for evil activity
        }

        if (isWalking && IsAtDestination())
        {
            isWalking = false;
            StartCoroutine(MovementDelay());
        }
    }

    private void Move()
    {
        if (Mathf.Abs(supervisor.transform.position.x - startEndPoints[0].position.x) < 0.1f)
        {
            currentTarget = startEndPoints[1].position;  //at left, go right
        }
        else
        {
            currentTarget = startEndPoints[0].position;  //at right, go left
        }

        supervisor.transform.DOMove(currentTarget, Random.Range(moveSpeeds.x, moveSpeeds.y)).SetEase(Ease.InQuad);
        isWalking = true;
    }
    private IEnumerator MovementDelay()
    {
        isOnCooldown = true;
        float delayTime = Random.Range(moveCooldown.x, moveCooldown.y);
        yield return new WaitForSeconds(delayTime);
        isOnCooldown = false;
    }

    private bool IsAtDestination()
    {
        float tolerance = 0.1f;
        return Mathf.Abs(supervisor.transform.position.x - currentTarget.x) < tolerance;
    }
}
