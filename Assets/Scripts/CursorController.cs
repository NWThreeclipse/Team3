using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Texture2D defaultCursorTexture;
    [SerializeField] private Texture2D holdingCursorTexture;

    [SerializeField] private Vector2 clickPosition = Vector2.zero;

    public static CursorController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Cursor.SetCursor(defaultCursorTexture, clickPosition, CursorMode.Auto);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetHoldingCursor();
        }

        // Check for mouse button release
        if (Input.GetMouseButtonUp(0))
        {
            SetDefaultCursor();
        }
    }

    private void SetHoldingCursor()
    {
        Cursor.SetCursor(holdingCursorTexture, clickPosition, CursorMode.Auto);
    }

    private void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursorTexture, clickPosition, CursorMode.Auto);
    }
}
