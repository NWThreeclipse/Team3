using UnityEngine;

public class ThermalButton : MonoBehaviour
{
    [SerializeField] private ThermalScanner thermalScanner;
    private void OnMouseDown()
    {
        thermalScanner.EnableMiniGame();
    }
}
