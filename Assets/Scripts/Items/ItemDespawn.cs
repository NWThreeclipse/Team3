using System.Collections.Generic;
using UnityEngine;

public class ItemDespawn : MonoBehaviour
{
    [SerializeField] private BarkManager barkManager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.gameObject.GetComponent<Item>();
            if (item.GetItemData().Rarity == Rarity.Anomalous)
            {
                barkManager.HidePlayerBark();
            }
            Destroy(collision.gameObject);


        }

        if (collision.CompareTag("Trash"))
        {
            Destroy(collision.gameObject);
        }
    }

    
}
