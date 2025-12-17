using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class temp : MonoBehaviour
{
    private List<ItemSO> gameItems;
    private List<ItemSO> commonItems;
    private List<ItemSO> uncommonItems;

    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private ConveyorBelt conveyorBelt;
    [SerializeField] private Vector2 itemSpawnrate;

    [SerializeField] private List<Sprite> trashSprites;
    [SerializeField] private GameObject trashPrefab;
    [SerializeField] private Vector2 trashSpawnrate;

    private void Start()
    {
        gameItems = Resources.LoadAll<ItemSO>("").ToList();
        if (gameItems.Count > 0)
        {
            commonItems = gameItems.Where(item => item.Rarity == Rarity.Common).ToList();
            uncommonItems = gameItems.Where(item => item.Rarity == Rarity.Uncommon).ToList();
        }
        StartCoroutine(SpawnItem());
        StartCoroutine(SpawnTrash());
    }
    private IEnumerator SpawnItem()
    {
        while (true)
        {
            ItemSO itemData;

            bool isCommon = UnityEngine.Random.value <= 0.8f;
            List<ItemSO> pool = isCommon ? commonItems : uncommonItems;
            itemData = pool[UnityEngine.Random.Range(0, pool.Count)];



            // Instantiate the item 
            GameObject itemInstance = Instantiate(itemPrefab, spawnPoint.position + new Vector3(0, UnityEngine.Random.Range(-0.1f, 0.3f)), Quaternion.identity);
            Item item = itemInstance.GetComponent<Item>();
            item.SetItemData(itemData);
            conveyorBelt.AddItem(itemInstance);
            AudioController.PlayItemSpawn();


            yield return new WaitForSeconds(UnityEngine.Random.Range(itemSpawnrate.x, itemSpawnrate.y));
        }
    }

    private IEnumerator SpawnTrash()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            int randomIndex = UnityEngine.Random.Range(0, trashSprites.Count);
            GameObject trashInstance = Instantiate(trashPrefab, spawnPoint.position + new Vector3(0, UnityEngine.Random.Range(-0.4f, 0.3f)), Quaternion.identity);
            Trash trash = trashInstance.GetComponent<Trash>();
            trash.SetSprite(trashSprites[randomIndex]);
            conveyorBelt.AddItem(trashInstance);
            yield return new WaitForSeconds(UnityEngine.Random.Range(trashSpawnrate.x, trashSpawnrate.y));
        }
    }
}
