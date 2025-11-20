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

    private bool isShrinking = false;
    private bool isGrowing = false;
    [SerializeField] private Volume postprocessingVolume;
    private Vignette vignette;
    private bool spawnedAnom = false;


    private void Start()
    {
        base.Start();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        
        postprocessingVolume = FindAnyObjectByType<Volume>();
        if (postprocessingVolume.profile.TryGet<Vignette>(out vignette))
        {
            vignette.intensity.value = 0f;
        }
        if (itemData != null && itemData.Sprite.Count() != 0)
        {
            SetSprite();
            //Vector2 spriteSize = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;

            //BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            //boxCollider.size = spriteSize;
        }
    }

    public ItemSO GetItemData() => itemData;

    public void Shrink()
    {
        offset = new Vector3(0, 0, offset.z);

        if (!isShrinking && !isGrowing)
            isShrinking = true;
    }

    public void Grow()
    {
        if (!isShrinking && !isGrowing)
            isGrowing = true;
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
        }
        else
        {
            spriteRenderer.sprite = itemData.Sprite[UnityEngine.Random.Range(0, itemData.Sprite.Count())];
        }
    }

    void Update()
    {
        if (isShrinking && transform.localScale.x > .2f)
        {
            transform.localScale -= new Vector3(5f, 5f, 0) * Time.deltaTime;
        }
        else if (isShrinking)
        {
            isShrinking = false;
        }

        if (isGrowing && transform.localScale.x < 1f)
        {
            transform.localScale += new Vector3(5f, 5f, 0) * Time.deltaTime;
        }
        else if (isGrowing)
        {
            isGrowing = false;
        }
    }

    private void OnMouseDown()
    {
        base.OnMouseDown();
        spriteRenderer.sortingLayerID = SortingLayer.NameToID("Held");
        if (itemData.Rarity == Rarity.Anomalous && !spawnedAnom)
        {
            spawnedAnom = true;
            //play quip
            StartCoroutine(FadeInVignette());
        }
        

    }
    private IEnumerator FadeInVignette()
    {
        float targetIntensity = 0.6f;
        float fadeDuration = 1f;
        float startIntensity = vignette.intensity.value;
        float timeElapsed = 0f;

        float targetTimeScale = 0.7f;
        float startTimeScale = Time.timeScale;
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, timeElapsed / fadeDuration);
            Time.timeScale = Mathf.Lerp(startTimeScale, targetTimeScale, timeElapsed / fadeDuration);
            yield return null;
        }

        vignette.intensity.value = targetIntensity;
        Time.timeScale = targetTimeScale;
        
        yield return new WaitForSeconds(2f);

        timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(targetIntensity, 0f, timeElapsed / fadeDuration);
            Time.timeScale = Mathf.Lerp(targetTimeScale, 1.0f, timeElapsed / fadeDuration);

            yield return null;
        }
        Time.timeScale = 1f;

    }

    private void OnMouseUp()
    {
        base.OnMouseUp();
        spriteRenderer.sortingLayerID = SortingLayer.NameToID("Item");
    }

    
}
