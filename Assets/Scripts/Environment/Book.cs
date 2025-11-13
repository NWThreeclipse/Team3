using UnityEngine;

public class Book : MonoBehaviour
{
    [SerializeField] private GameObject bookCanvas;
    private void OnMouseDown()
    {
        bookCanvas.SetActive(true);
    }

    public void HideCanvas()
    {
        bookCanvas.SetActive(false);
    }
}
