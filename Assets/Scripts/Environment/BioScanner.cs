using UnityEngine;

public class BioScanner : MonoBehaviour
{
    [SerializeField] private bool isPlaying = false;
    [SerializeField] private GameObject MinigameCanvas;
    [SerializeField] private ViewingBoard viewingBoard;

    void Start()
    {
        MinigameCanvas.SetActive(false);

    }

    public void EnableMiniGame()
    {
        if (viewingBoard.IsHoldingItem())
        {
            MinigameCanvas.SetActive(true);
            isPlaying = true;
        }
    }

    public void DisableMiniGame()

    {
        MinigameCanvas.SetActive(false);
        isPlaying = false;
    }
}
