using System.Collections.Generic;
using UnityEngine;

public class ItemDespawn : MonoBehaviour
{
    [SerializeField] private GameObject spawnPoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            collision.gameObject.transform.position = spawnPoint.transform.position;
        }
    }

    
}
