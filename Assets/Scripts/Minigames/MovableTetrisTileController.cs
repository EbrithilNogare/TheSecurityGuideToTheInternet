using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovableTetrisTileController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PhoneMinigameController.PieceType pieceType;

    private GameObject parent;
    private RectTransform parentRectTransform;
    private Transform defaultDropZone;
    private CanvasGroup parentCanvasGroup;
    private PhoneMinigameController phoneMinigameController;

    void Start()
    {
        parent = transform.parent.gameObject;
        parentRectTransform = parent.GetComponent<RectTransform>();
        parentCanvasGroup = parent.GetComponent<CanvasGroup>() != null ? parent.GetComponent<CanvasGroup>() : parent.AddComponent<CanvasGroup>();
        phoneMinigameController = FindObjectOfType<PhoneMinigameController>();
        defaultDropZone = parent.transform.parent.transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parent.transform.SetParent(phoneMinigameController.dragAndDropTemporaryParent.transform);
        parentCanvasGroup.blocksRaycasts = false;
        parentCanvasGroup.alpha = 0.7f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        parentRectTransform.anchoredPosition += eventData.delta / (new Vector2(parentRectTransform.lossyScale.x, parentRectTransform.lossyScale.y));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        parentCanvasGroup.blocksRaycasts = true;
        parentCanvasGroup.alpha = 1f;
        EvaluateDrop();
    }

    private void EvaluateDrop()
    {
        bool isOverBoard = RectTransformUtility.RectangleContainsScreenPoint(phoneMinigameController.mainDropZone, Input.mousePosition);
        if (!isOverBoard)
        {
            // Reset the piece to its original drop zone
            parent.transform.SetParent(defaultDropZone);
            parentRectTransform.anchoredPosition = Vector2.zero;
            return;
        }

        // Get the local position relative to the main drop zone
        RectTransformUtility.ScreenPointToLocalPointInRectangle(phoneMinigameController.mainDropZone, Input.mousePosition, null, out Vector2 localPointerPosition);

        // Calculate the board position taking into account the parent anchor being in the center
        float cellWidth = phoneMinigameController.mainDropZone.rect.width / 7;
        float cellHeight = phoneMinigameController.mainDropZone.rect.height / 7;

        Vector2 boardPosition = new Vector2(
            Mathf.Floor(localPointerPosition.x / cellWidth),
            Mathf.Floor(localPointerPosition.y / cellHeight)
        );

        // Adjust the rounded position to account for the center anchor
        Vector2 roundedPosition = new Vector2(
            boardPosition.x * cellWidth - phoneMinigameController.mainDropZone.rect.width / 2 + cellWidth / 2,
            boardPosition.y * cellHeight - phoneMinigameController.mainDropZone.rect.height / 2 + cellHeight / 2
        );

        // Get the shape offsets for the current piece
        List<Vector2> shapeOffsets = phoneMinigameController.pieceShapes[pieceType];
        bool hasCollision = false;

        // Check for collisions and ensure the piece fits within the board
        foreach (var offset in shapeOffsets)
        {
            int x = (int)(boardPosition.x + offset.x);
            int y = (int)(boardPosition.y + offset.y);

            if (x < 0 || x >= 7 || y < 0 || y >= 7 ||
                (phoneMinigameController.currentBooard[x, y] != PhoneMinigameController.PieceType.None &&
                 phoneMinigameController.currentBooard[x, y] != pieceType))
            {
                hasCollision = true;
                break;
            }
        }

        if (hasCollision)
        {
            // Reset the piece to its original drop zone
            parent.transform.SetParent(defaultDropZone);
            parentRectTransform.anchoredPosition = Vector2.zero;
            return;
        }

        // Move the piece to the rounded position
        parentRectTransform.anchoredPosition = roundedPosition;
        parent.transform.SetParent(phoneMinigameController.mainDropZone);

        // Update the board state
        foreach (var offset in shapeOffsets)
        {
            int x = (int)(boardPosition.x + offset.x);
            int y = (int)(boardPosition.y + offset.y);
            phoneMinigameController.currentBooard[x, y] = pieceType;
        }

        // Notify the controller of the move
        phoneMinigameController.DoMove(pieceType, boardPosition);
    }

}
