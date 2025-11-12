using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bin : DragZone
{
    [SerializeField] private Sorting sorting;
    [SerializeField] private GameManager gameManager;

    protected override void HandleItemRelease(Draggable draggable)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(draggable.transform.position);
        bool stillInBin = false;

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
            {
                stillInBin = true;
                break;
            }
        }
        if (!stillInBin)
        {
            return;
        }

        base.HandleItemRelease(draggable);

        Item item = draggable.GetComponent<Item>();
        if (item == null)
        {
            return;
        }

        StartCoroutine(PlayEffectWithDelay(item));

        enteredItem = null;
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
