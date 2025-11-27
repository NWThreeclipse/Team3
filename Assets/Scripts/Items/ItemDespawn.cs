using System.Collections.Generic;
using UnityEngine;

public class ItemDespawn : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();
            if(item.GetItemData().Rarity == Rarity.Anomalous)
            {
                gameManager.ResetAnomalous();
            }
            Destroy(collision.gameObject);

        }

        if (collision.CompareTag("Trash"))
        {
            Destroy(collision.gameObject);
        }
    }

    
}
