using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class EndScreenReader : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float textSpeed;

    [SerializeField] private Image img;
    [SerializeField] private Vector3 targetScale = new Vector3(0f, 0f, 1f);
    [SerializeField] private float shrinkDuration = 2f;
    [SerializeField] private Color targetGreyscaleColor = Color.grey;
    [SerializeField] private MusicController musicController;
    [SerializeField] private int endingType = 0;

    private Color initialColor;
    private Vector3 initialScale;
    private string dialogue;

    private void Awake()
    {
        musicController.FadeInMusic();
    }
    private void Start()
    {
        dialogue = text.text;
        initialScale = img.gameObject.transform.localScale;
        initialColor = img.color;

        if (endingType == 0)
        {
            StartCoroutine(ShrinkImage(img));
        }

        StartCoroutine(RenderSentence(dialogue, text));


    }

    IEnumerator RenderSentence(string sentence, TMP_Text textbox)
    {
        textbox.text = sentence;
        textbox.ForceMeshUpdate();
        textbox.maxVisibleCharacters = 0;
        int totalChars = textbox.textInfo.characterCount;

        for (int i = 0; i < totalChars; i++)
        {
            textbox.maxVisibleCharacters++;

            yield return new WaitForSeconds(textSpeed);
        }
    }

    IEnumerator ShrinkImage(Image img)
    {
        float timeElapsed = 0f;

        while (timeElapsed < shrinkDuration)
        {
            float t = timeElapsed / shrinkDuration;

            img.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);

            img.color = Color.Lerp(initialColor, targetGreyscaleColor, t);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        img.transform.localScale = targetScale;
        img.color = targetGreyscaleColor;
    }
}
