using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bin : DragZone
{
    [SerializeField] private Sorting sorting;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private List<Bin> otherBins;

    protected override void HandleItemRelease(Draggable draggable)
    {
        base.HandleItemRelease(draggable);
        GameObject itemObject = draggable.gameObject;

        bool multipleCollision = false;
        foreach (Bin b in otherBins)
        {
            if(b.IsHoldingItem() && IsHoldingItem())
            {
                multipleCollision = true;
            }
        }
        //check if colliding with multiple bins, if so then snap back to safe area

        if (multipleCollision)
        {
            draggable.ResetPosition();
            //isHoldingItem = false;
            //enteredItem = null;
            Debug.Log("multi bin collision");
        }
        else
        {
            Item item = itemObject.GetComponent<Item>();

            StartCoroutine(PlayEffectWithDelay(item));

            enteredItem = null;
            Destroy(itemObject);
        }
            
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
