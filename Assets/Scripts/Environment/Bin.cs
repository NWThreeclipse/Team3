using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bin : DragZone
{
    [SerializeField] private Sorting sorting;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Bin[] otherBins;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        enteredItem = collision.gameObject;
        isHoldingItem = true;
    }
    protected override void HandleItemRelease(Draggable draggable)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, GetComponent<Collider2D>().bounds.size, 0f, LayerMask.GetMask("Item"));

        bool isInsideBin = false;
        Item item = draggable.GetComponent<Item>();

        if (item == null)
        {
            return;
        }

        foreach (var hit in hits)
        {
            if (hit.gameObject == draggable.gameObject)
            {
                isInsideBin = true;
                break;
            }
        }

        if (!isInsideBin)
        {
            draggable.ResetPosition();
            return;
        }

        foreach (Bin otherBin in otherBins)
        {
            if (otherBin.IsHoveringItem() && otherBin.GetItem() == item.GetItemData())
            {
                draggable.ResetPosition();
                return;
            }
        }

        StartCoroutine(PlayEffectWithDelay(item));

        enteredItem = draggable.gameObject;
        isHoldingItem = true;

        Destroy(draggable.gameObject);
    }



    private IEnumerator PlayEffectWithDelay(Item item)
    {
        bool isCorrect = item.GetItemData().Sorting == sorting;

        float soundLength = isCorrect ? AudioController.PlayCorrectSound() : AudioController.PlayIncorrectSound();

        yield return new WaitForSeconds(soundLength);

        if (isCorrect)
        {
            gameManager.AddStat(sorting, item.GetItemData().Value);
        }
        else
        {
            gameManager.SubtractStat(sorting, item.GetItemData().Value);
            gameManager.CheckLoss();
        }

        StatsController.Instance.IncrementItem(isCorrect);
    }

}
