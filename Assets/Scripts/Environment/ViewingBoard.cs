using UnityEngine;

public class ViewingBoard : DragZone
{
    //check if held item is anomalous, play quip

    protected override void HandleItemRelease(Draggable draggable)
    {
        base.HandleItemRelease(draggable);
        Item item = draggable.gameObject.GetComponent<Item>();
        if (item.GetItemData().Rarity == Rarity.Anomalous)
        {
            item.EnableSorting();
            //disable quip
        }
    }

}
