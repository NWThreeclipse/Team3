using UnityEngine;

public class ViewingBoard : DragZone
{
    [SerializeField] private BarkManager barkManager;
    protected override void HandleItemRelease(Draggable draggable)
    {
        base.HandleItemRelease(draggable);
        Item item = draggable.gameObject.GetComponent<Item>();
        if (item.GetItemData().Rarity == Rarity.Anomalous)
        {
            barkManager.HidePlayerBark();
            item.EnableSorting();
            //disable quip
        }
    }

}
