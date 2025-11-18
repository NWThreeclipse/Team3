using TMPro;
using UnityEngine;

public class BioScanner : MonoBehaviour
{
    [SerializeField] private bool isPlaying = false;
    [SerializeField] private ViewingBoard viewingBoard;
    [SerializeField] private TMP_Text textfield;


    void Start()
    {
        textfield.text = "";
    }

    private void Update()
    {
        if(isPlaying && !viewingBoard.IsHoldingItem())
        {
            DisableMiniGame();
        }
    }

    public void EnableMiniGame()
    {
        if (viewingBoard.IsHoldingItem())
        {
            isPlaying = true;
            textfield.text = viewingBoard.GetItem().Organic.ToString() + "%";
        }
    }

    public void DisableMiniGame()

    {
        isPlaying = false;
        textfield.text = "";
    }
}
