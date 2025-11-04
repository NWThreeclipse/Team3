using System.Collections;
using UnityEngine;

public class Bin : DragZone
{
    [SerializeField] private Sorting sorting;
    [SerializeField] private GameManager gameManager;

    protected override void HandleItemRelease(Draggable draggable)
    {
        base.HandleItemRelease(draggable);
        GameObject itemObject = draggable.gameObject;
       
        //check if colliding with multiple bins, if so then snap back to safe area

        Item item = itemObject.GetComponent<Item>();

        StartCoroutine(PlayEffectWithDelay(item));

        enteredItem = null;
        Destroy(itemObject);
    }

    private IEnumerator PlayEffectWithDelay(Item item)
    {
        bool isCorrect = item.GetItemData().Sorting == sorting;

        float soundLength = isCorrect ? AudioController.PlayCorrectSound() : AudioController.PlayIncorrectSound();

        yield return new WaitForSeconds(soundLength-1f);

        if (isCorrect)
        {
            gameManager.AddStat(sorting, item.GetItemData().Value);
        }
        else
        {
            gameManager.SubtractStat(sorting, item.GetItemData().Value);
            gameManager.CheckLoss();

        }
    }

}
