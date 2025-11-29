using JetBrains.Annotations;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class BarkManager : MonoBehaviour
{
    [SerializeField] private GameObject skoogeBarkCanvas;
    [SerializeField] private TMP_Text skoogeText;
    [SerializeField] private float textSpeed = 0.01f;
    [SerializeField] private Dialogue skoogeDialogue;

    [SerializeField] private GameObject playerBarkCanvas;
    [SerializeField] private TMP_Text playerText;
    [SerializeField] private Dialogue playerDialogue;


    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        skoogeBarkCanvas.SetActive(false);
        playerBarkCanvas.SetActive(false);
    }

    public void StartSkoogeBark()
    {
        string chosenText = skoogeDialogue.sentences[Random.Range(0, skoogeDialogue.sentences.Length)];
        skoogeText.text = "";
        StopAllCoroutines();
        StartCoroutine(RenderSentence(chosenText, skoogeText, skoogeDialogue.talkingClip, true, skoogeBarkCanvas));
        skoogeBarkCanvas.SetActive(true);
    }

    public void StartPlayerBark()
    {
        string chosenText = playerDialogue.sentences[StatsController.Instance.GetDays() - 2];
        Debug.Log(chosenText);
        playerText.text = "";
        StopAllCoroutines();
        StartCoroutine(RenderSentence(chosenText, playerText, playerDialogue.talkingClip, false, playerBarkCanvas));
        playerBarkCanvas.SetActive(true);
    }

    public void HidePlayerBark()
    {
        playerBarkCanvas.SetActive(false);
    }



    IEnumerator RenderSentence(string sentence, TMP_Text textbox, AudioClip audioClip, bool autoFade, GameObject canvas)
    {
        textbox.text = "";
        char[] letters = sentence.ToCharArray();
        for (int i = 0; i < letters.Length; i++)
        {
            textbox.text += letters[i];
            if (i % 4 == 0)
            {
                source.volume = UnityEngine.Random.Range(0.5f, 1.0f);
                source.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
                source.PlayOneShot(audioClip);
            }

            yield return new WaitForSeconds(textSpeed);
        }

        if (autoFade)
        {
            yield return new WaitForSeconds(2f);
            canvas.SetActive(false);

        }
    }
}
