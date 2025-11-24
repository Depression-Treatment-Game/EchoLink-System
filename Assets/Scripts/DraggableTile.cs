using UnityEngine;
using UnityEngine.InputSystem;

public class DraggableTile : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 originalPosition;

    [Header("Tile Type")]
    [SerializeField] private string tileType = "Grass";
    [SerializeField] private Color tileColor = new Color(0.4f, 0.78f, 0.4f);
    [SerializeField] private float distanceFromCamera = 10f;

    [Header("Input Actions")]
    [SerializeField] private InputAction clickAction = new InputAction(type: InputActionType.Button);
    [SerializeField] private InputAction positionAction = new InputAction(type: InputActionType.Value, expectedControlType: nameof(Vector2));

    private void OnEnable()
    {
        // Set up bindings
        clickAction.AddBinding("<Mouse>/leftButton");
        positionAction.AddBinding("<Mouse>/position");

        // Enable actions
        clickAction.Enable();
        positionAction.Enable();
    }

    private void OnDisable()
    {
        // Disable actions
        clickAction.Disable();
        positionAction.Disable();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        originalPosition = transform.position;
    }

    private void Update()
    {
        // Check for click press
        if (clickAction.WasPressedThisFrame())
        {
            TryStartDrag();
        }

        // Update position while dragging
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }

        // Check for click release
        if (clickAction.WasReleasedThisFrame())
        {
            TryStopDrag();
        }
    }

    private void TryStartDrag()
    {
        Vector2 mouseWorldPos = GetMouseWorldPosition();
        Collider2D hitCollider = GetComponent<Collider2D>();

        if (hitCollider != null && hitCollider.OverlapPoint(mouseWorldPos))
        {
            isDragging = true;
            offset = transform.position - (Vector3)mouseWorldPos;
            Debug.Log($"Started dragging {tileType}");
        }
    }

    private void TryStopDrag()
    {
        if (isDragging)
        {
            isDragging = false;

            // Check if dropped on a grid tile
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);
            Debug.Log($"Raycast found {hits.Length} objects");

            foreach (RaycastHit2D hit in hits)
            {
                Debug.Log($"Hit object: {hit.collider.gameObject.name}");

                GridTile gridTile = hit.collider.GetComponent<GridTile>();
                if (gridTile != null)
                {
                    Debug.Log($"Found GridTile! Setting to {tileType}");
                    gridTile.SetTileType(tileType, tileColor);
                    break;
                }
            }

            // Return to original position
            transform.position = originalPosition;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mouseScreenPos = positionAction.ReadValue<Vector2>();
        Vector3 mousePos = new Vector3(mouseScreenPos.x, mouseScreenPos.y, distanceFromCamera);
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
}