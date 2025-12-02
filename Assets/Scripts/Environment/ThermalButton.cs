using DG.Tweening;
using UnityEngine;

public class ThermalButton : MonoBehaviour
{
    [SerializeField] private ThermalScanner thermalScanner;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private HelpMenuButton helpButton;
    [SerializeField] private float shakeStrength = 0.05f;
    private Vector3 originalPosition;

    private void Start()
    {
        if (gameManager.GetDay() < 1)
        {
            thermalScanner.gameObject.SetActive(false);
            helpButton.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        originalPosition = transform.position;

    }
    private void OnMouseDown()
    {
        thermalScanner.EnableMiniGame();
        if (!DOTween.IsTweening(gameObject))
        {
            transform.DOShakePosition(shakeStrength, 0.1f).OnComplete(() => transform.DOMove(originalPosition, 0.1f));
        }
    }
}
