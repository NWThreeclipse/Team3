using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XNode;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogueMenu;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text sentenceText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private TMP_Text[] optionButtonsText;

    [SerializeField] private Image portrait;
    [SerializeField] private AudioClip panelOpen, panelClose;
    [SerializeField] private Vector3 showPanelPos;
    [SerializeField] private Vector3 hidePanelPos;
    [SerializeField] private float panelAnimationTime = 1;
    [SerializeField] private float textSpeed = 0.01f;

    Node curNode;
    Queue<string> sentences = new Queue<string>();
    AudioSource source;
    AudioClip talkingClip;

    public event Action<DialogueManager> OnDialogueEnd;


    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void StartDialogue(Node rootNode)
    {
        StopAllCoroutines();
        curNode = rootNode;
        //option node
        if (curNode.GetType() == typeof(OptionDialogueNode))
        {
            //load node for speaker
            OptionDialogueNode options = curNode as OptionDialogueNode;
            Dialogue dialogue = options.speaker;
            
            //set panel
            nameText.text = dialogue.name;
            portrait.sprite = dialogue.portrait;
            AlignIcons(dialogue.name);
            talkingClip = dialogue.talkingClip;
            sentenceText.text = "";

            //set buttons
            nextButton.gameObject.SetActive(false);
            for (int i = 0; i < options.responses.sentences.Length; i++)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtonsText[i].text = options.responses.sentences[i];
            }


            sentences.Clear();
            for (int i = 0; i < dialogue.sentences.Length; i++)
            {
                sentences.Enqueue(dialogue.sentences[i]);
            }

            //source.PlayOneShot(panelOpen); <-- bug
            dialogueMenu.transform.DOLocalMove(showPanelPos, panelAnimationTime).OnComplete(() => DisplaySentence());
        }
        //simple node
        else if (curNode.GetType() == typeof(SimpleDialogueNode))
        {
            //load node for speaker
            SimpleDialogueNode simple = curNode as SimpleDialogueNode;
            Dialogue dialogue = simple.sentence;
            AlignIcons(dialogue.name);

            //set panel
            nameText.text = dialogue.name;
            portrait.sprite = dialogue.portrait;
            talkingClip = dialogue.talkingClip;
            sentenceText.text = "";

            //set buttons
            nextButton.gameObject.SetActive(true);
            for (int i = 0; i < optionButtons.Length; i++)
            {
                optionButtons[i].gameObject.SetActive(false);
            }

            sentences.Clear();
            for (int i = 0; i < dialogue.sentences.Length; i++)
            {
                sentences.Enqueue(dialogue.sentences[i]);
            }

            //source.PlayOneShot(panelOpen); <-- this was giving bug
            dialogueMenu.transform.DOLocalMove(showPanelPos, panelAnimationTime).OnComplete(() => DisplaySentence());
        }
        //control node;
        else
        {
            //load node for speaker
            DialogueControlNode control = curNode as DialogueControlNode;
            
            if (control.dialogueControl == DialogueControlNode.option.endDialogue)
            {
                EndDialogue();
            }
            else if (control.dialogueControl == DialogueControlNode.option.continueDialogue)
            {
                //continue Dialogue
            }
            else
            {
                //restart Dialogue
            }
        }
    }

    public void DisplayNextOption(string option)
    {
        OptionDialogueNode optionNode = curNode as OptionDialogueNode;

        NodePort port = null;

        if (option == "A")
        {
            port = optionNode.GetOutputPort("optionA");
        }
        else if (option == "B")
        {
            port = optionNode.GetOutputPort("optionB");
        }
        else if (option == "C")
        {
            port = optionNode.GetOutputPort("optionC");
        }
        else if (option == "D")
        {
            port = optionNode.GetOutputPort("optionD");
        }
        if (port != null && port.Connection != null)
        {
            curNode = port.Connection.node;
            StartDialogue(curNode);
        }
    }


    public void DisplayNextSimple()
    {
        SimpleDialogueNode simpleNode = curNode as SimpleDialogueNode;
            
        NodePort port = simpleNode.GetOutputPort("nextNode").Connection;
            
        if (port != null)
        {
            curNode = port.node;
        }
        
        StartDialogue(curNode);
    }

    public void DisplaySentence()
    {
        StopAllCoroutines();
        StartCoroutine(RenderSentence(sentences.Dequeue()));
    }

    IEnumerator RenderSentence(string sentence)
    {
        sentenceText.text = "";
        char[] letters = sentence.ToCharArray();
        for (int i = 0; i < letters.Length; i++)
        {
            sentenceText.text += letters[i];
            if(i % 4 == 0)
            {
                source.volume = UnityEngine.Random.Range(0.5f, 1.0f);
                source.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
                source.PlayOneShot(talkingClip);
            }
            
            yield return new WaitForSeconds(textSpeed);
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        source.PlayOneShot(panelClose);
        dialogueMenu.transform.DOLocalMove(hidePanelPos, panelAnimationTime);
        OnDialogueEnd?.Invoke(this);

    }

    private void AlignIcons(string name)
    {
        if (name == "Supervisor")
        {
            Vector3 portraitPosition = new Vector3(0, -74.7f, 0);
            Vector3 portraitScale = new Vector3(2.7f, 2.7f, 2.7f);
            portrait.gameObject.transform.localPosition = portraitPosition;
            portrait.gameObject.transform.localScale = portraitScale;
        }
        else if (name == "Xanthon" || name == "Unit 481202")
        {
            Vector3 portraitPosition = new Vector3(0, -64, 0);
            Vector3 portraitScale = new Vector3(2.5f, 2.5f, 2.5f);
            portrait.gameObject.transform.localPosition = portraitPosition;
            portrait.gameObject.transform.localScale = portraitScale;
        }
        else //skooge
        {

        }
    }
}
