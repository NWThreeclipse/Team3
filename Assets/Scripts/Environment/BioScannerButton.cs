using DG.Tweening;
using UnityEngine;

public class BioScannerButton : MonoBehaviour
{
    [SerializeField] private BioScanner bioScanner;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float shakeStrength = 0.05f;
    private Vector3 originalPosition;

    private void Start()
    {
        if (gameManager.GetDay() < 3)
        {
            bioScanner.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        originalPosition = transform.position;

    }
    private void OnMouseDown()
    {
        bioScanner.EnableMiniGame();
        if (!DOTween.IsTweening(gameObject))
        {
            transform.DOShakePosition(shakeStrength, 0.1f).OnComplete(() => transform.DOMove(originalPosition, 0.1f));
        }
        AudioController.PlayButton();

    }
}
