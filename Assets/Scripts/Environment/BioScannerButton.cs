using UnityEngine;

public class BioScannerButton : MonoBehaviour
{
    [SerializeField] private BioScanner bioScanner;
    private void OnMouseDown()
    {
        bioScanner.EnableMiniGame();
    }
}
