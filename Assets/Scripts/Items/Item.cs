using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Item : Draggable
{
    [SerializeField] private ItemSO itemData;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Volume postprocessingVolume;
    private Vignette vignette;
    private bool spawnedAnom = false;
    private bool animationOver = false;

    private Material itemMaterial;
    private bool isSortable = true;
    private Coroutine vignetteRoutine;


    public bool GetAnimOver() => animationOver;
    public ItemSO GetItemData() => itemData;
    public bool GetSortable() => isSortable;
    public Vignette GetVignette() => vignette;
    public Coroutine GetVignetteCoroutine() => vignetteRoutine;


    public void SetInteractive(bool toggle)
    {
        isInteractible = toggle;
    }
    private void Start()
    {
        base.Start();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();


        
        
        itemMaterial = spriteRenderer.material;


        if (itemData != null && itemData.Sprite.Count() != 0)
        {
            SetSprite();
            //Vector2 spriteSize = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;

            //BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            //boxCollider.size = spriteSize;
        }

        if (itemData.Rarity == Rarity.Anomalous)
        {
            isSortable = false;
            postprocessingVolume = FindAnyObjectByType<Volume>();
            postprocessingVolume.profile.TryGet<Vignette>(out vignette);

        }
    }

    public void SetItemData(ItemSO newItemData)
    {
        itemData = newItemData;
        if (spriteRenderer != null && itemData != null && itemData.Sprite != null)
        {
            SetSprite();
        }
    }

    private void SetSprite()
    {
        if (itemData.Sprite.Count() == 1)
        {
            spriteRenderer.sprite = itemData.Sprite[0];
            itemMaterial.SetTexture("_Texture", itemData.Sprite[0].texture);
        }
        else
        {
            int randInt = UnityEngine.Random.Range(0, itemData.Sprite.Count());
            spriteRenderer.sprite = itemData.Sprite[randInt];
            itemMaterial.SetTexture("_Texture", itemData.Sprite[randInt].texture);
        }

        itemMaterial.SetFloat("_IsEnabled", itemData.Rarity == Rarity.Common ? 0f : 1f);
        itemMaterial.SetFloat("_IsAnomalous", itemData.Rarity == Rarity.Anomalous ? 1f : 0f);




    }

    private void OnDestroy()
    {
        if (itemData.Rarity == Rarity.Anomalous)
        {
            StopAllCoroutines();
        }

    }


    private void OnMouseDown()
    {
        if (!isInteractible)
        {
            return;
        }
        base.OnMouseDown();
        spriteRenderer.sortingLayerID = SortingLayer.NameToID("Held");
        if (itemData.Rarity == Rarity.Anomalous && !spawnedAnom)
        {
            spawnedAnom = true;
            vignetteRoutine = StartCoroutine(FadeInVignette());

        }


    }
    private IEnumerator FadeInVignette()
    {
        animationOver = false;
        float targetIntensity = 0.6f;
        float fadeDuration = 1f;
        float startIntensity = vignette.intensity.value;
        float timeElapsed = 0f;

        //float targetTimeScale = 0.7f;
        //float startTimeScale = Time.timeScale;
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, timeElapsed / fadeDuration);
            //Time.timeScale = Mathf.Lerp(startTimeScale, targetTimeScale, timeElapsed / fadeDuration);
            yield return null;
        }

        vignette.intensity.value = targetIntensity;
        //Time.timeScale = targetTimeScale;
        
        yield return new WaitForSeconds(2f);

        timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(targetIntensity, 0f, timeElapsed / fadeDuration);
            //Time.timeScale = Mathf.Lerp(targetTimeScale, 1.0f, timeElapsed / fadeDuration);

            yield return null;
        }
        vignette.intensity.value = 0f;

        //Time.timeScale = 1f;
        animationOver = true;

    }

    private void OnMouseUp()
    {
        base.OnMouseUp();
        spriteRenderer.sortingLayerID = SortingLayer.NameToID("Item");
    }

    public void EnableSorting()
    {
        isSortable = true;
    }
    
}
