using UnityEngine;

public class HelpMenuButton : MonoBehaviour
{
    [SerializeField] GameObject canvas;

    private void Start()
    {
        canvas.SetActive(false);
    }

    public void ToggleCanvas()
    {
        canvas.SetActive(!canvas.activeSelf);
    }
    private void OnMouseDown()
    {
        ToggleCanvas();
    }
}
