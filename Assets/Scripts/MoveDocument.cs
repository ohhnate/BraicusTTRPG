using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class MoveDocument : MonoBehaviour
{
    private DocumentControlActions documentControls;

    public Camera mainCamera;

    private bool mouseHeld = false;
    private bool documentClicked = false;
    private Vector3 mousePosition;
    private Vector3 startPosition;

    public float scrollAmount;

    private float currentScrollValue;
    private Vector3 minScale = new Vector3(1f, 1f, 1f);
    private Vector3 maxScale = new Vector3(3f, 3f, 3f);

    private Vector3 screenBounds;


    private void Awake()
    {
        documentControls = new DocumentControlActions();
    }

    private void OnEnable()
    {
        documentControls.Enable();
    }
  
    private void Start()
    {               
        documentControls.UI.Click.performed += context => MouseHeldChecker(context);
        documentControls.UI.Point.performed += context => GetMouseWorldPosition(context);
        documentControls.UI.ScrollWheel.performed += context => currentScrollValue = context.ReadValue<Vector2>().y;

        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
    }

    private void Update()
    {
        if (mouseHeld)
        {
            if (documentClicked)
            {
                //move document based on mouse position
                Vector3 newPosition = new Vector3(mousePosition.x - startPosition.x, mousePosition.y - startPosition.y, transform.position.z);
                transform.position = newPosition;                
            }
        }
        ScrollScale();
    }

    private void ScrollScale()
    {
        if (currentScrollValue != 0)
        {
            if (currentScrollValue > 0)
            {
                transform.localScale += Vector3.one * scrollAmount;
            }
            else if (currentScrollValue < 0)
            {
                transform.localScale += Vector3.one * -scrollAmount;
            }
            transform.localScale = Vector3.Max(transform.localScale, minScale);
            transform.localScale = Vector3.Min(transform.localScale, maxScale);
        }
    }

    private bool IsDocumentClicked(CallbackContext context)
    {
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit)
        {
            startPosition = mousePosition - transform.position;
            return true;
        }
        return false;
    }

    private void MouseHeldChecker(CallbackContext context)
    {
        if (context.performed)
        {
            mouseHeld = !mouseHeld;
        }
        if (mouseHeld)
        {
            documentClicked = IsDocumentClicked(context);
        }
    }  

    private void GetMouseWorldPosition(CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }

    private void OnDisable()
    {
        documentControls.Disable();
    }
}
