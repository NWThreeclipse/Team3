using TMPro;
using UnityEngine;

public class ViewingBoard : DragZone
{
    [SerializeField] private BarkManager barkManager;

    [SerializeField] private TMP_Text textfield;
    [SerializeField] private AudioSource minigameSFX;


    protected override void OnTriggerEnter2D(Collider2D collision)
    {

        base.OnTriggerEnter2D(collision);
    }
    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        textfield.text = "";

    }
    
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
        textfield.text = item.GetItemData().Weight.ToString();
        minigameSFX.Play();
    }

}
