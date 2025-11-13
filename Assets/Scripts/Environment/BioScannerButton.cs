using UnityEngine;

public class BioScannerButton : MonoBehaviour
{
    [SerializeField] private BioScanner bioScanner;
    [SerializeField] private GameManager gameManager;
    private void Start()
    {
        if (gameManager.GetDay() < 3)
        {
            bioScanner.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
    private void OnMouseDown()
    {
        bioScanner.EnableMiniGame();
    }
}
