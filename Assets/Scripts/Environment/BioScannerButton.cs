using UnityEngine;

public class BioScannerButton : MonoBehaviour
{
    [SerializeField] private BioScanner bioScanner;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private HelpMenuButton helpButton;
    private void Start()
    {
        if (gameManager.GetDay() < 3)
        {
            helpButton.gameObject.SetActive(false);
            bioScanner.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
    private void OnMouseDown()
    {
        bioScanner.EnableMiniGame();
    }
}
