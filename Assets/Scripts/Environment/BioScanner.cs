using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class BioScanner : DragZone
{
    [SerializeField] private bool isPlaying = false;
    [SerializeField] private TMP_Text textfield;
    [SerializeField] private float shakeStrength = 0.05f;
    [SerializeField] private GameObject scannerLid;
    private Vector3 originalPosition;

    private int CountFPS = 30;
    private float Duration = 1f;
    private string NumberFormat = "F0";
    private int value = 0;

    private Coroutine countingCoroutine = null;
    private bool hasCompleted = false;

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        DisableMiniGame();
    }

    void Start()
    {
        textfield.text = "";
        originalPosition = scannerLid.transform.position;
    }

    private void Update()
    { 
        if (isPlaying && countingCoroutine == null && !hasCompleted && IsHoldingItem())
        {
            int percentage = (int)GetItem().Organic;
            value = 0;

            countingCoroutine = StartCoroutine(CountToPercentage(percentage));
        }

        if (isPlaying && !IsHoldingItem())
        {
            DisableMiniGame();
        }
    }

    protected override void HandleItemRelease(Draggable draggable)
    {
        if (!DOTween.IsTweening(scannerLid))
        {
            scannerLid.transform.DOShakePosition(shakeStrength, 0.1f).OnComplete(() => scannerLid.transform.DOMove(originalPosition, 0.1f));
        }
        base.HandleItemRelease(draggable);
    }

    public void EnableMiniGame()
    {
        if (IsHoldingItem() && !hasCompleted)  
        {
            isPlaying = true;
            int percentage = (int)GetItem().Organic;
            textfield.text = "0%";
            value = 0;

            if (countingCoroutine != null)
            {
                StopCoroutine(countingCoroutine);
            }

            countingCoroutine = StartCoroutine(CountToPercentage(percentage));
        }
    }

    public void DisableMiniGame()
    {
        isPlaying = false;
        textfield.text = "";

        if (countingCoroutine != null)
        {
            StopCoroutine(countingCoroutine);
            countingCoroutine = null;
        }
        hasCompleted = false;
    }

    private IEnumerator CountToPercentage(int targetPercentage)
    {
        float stepAmount = Mathf.CeilToInt((targetPercentage - value) / (CountFPS * Duration));
        WaitForSeconds wait = new WaitForSeconds(1f / CountFPS);
        while (value < targetPercentage)
        {
            value += (int)stepAmount;

            if (value > targetPercentage)
            {
                value = targetPercentage;
            }

            textfield.text = value.ToString(NumberFormat) + "%";

            yield return wait;
        }

        textfield.text = targetPercentage.ToString(NumberFormat) + "%";

        hasCompleted = true;
        countingCoroutine = null;
    }
}
