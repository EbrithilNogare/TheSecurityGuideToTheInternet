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

    private const float CELL_SIZE = 64.0f;

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
        Vector3 parentPosition = parent.transform.position;
        Vector3 parentScale = parent.transform.lossyScale;
        Vector3 parentInverseScale = new(1f / parentScale.x, 1f / parentScale.y, 1f / parentScale.z);
        Vector3 parentScaledPosition = Vector3.Scale(parentPosition, parentInverseScale);
        Vector3 parentOffset = new(parentRectTransform.rect.width / 2f, -parentRectTransform.rect.height / 2f, 0);
        Vector3 parentPositionWithOffset = parentScaledPosition - parentOffset;

        Vector3 mainDropZonePosition = phoneMinigameController.mainDropZone.transform.position;
        Vector3 mainDropZoneScale = phoneMinigameController.mainDropZone.lossyScale;
        Vector3 mainDropZoneInverseScale = new(1f / mainDropZoneScale.x, 1f / mainDropZoneScale.y, 1f / mainDropZoneScale.z);
        Vector3 mainDropZoneScalePosition = Vector3.Scale(mainDropZonePosition, mainDropZoneInverseScale);
        Vector3 mainDropZoneOffset = new(phoneMinigameController.mainDropZone.rect.width / 2f, -phoneMinigameController.mainDropZone.rect.height / 2f, 0);
        Vector3 mainDropZonePositionWithOffset = mainDropZoneScalePosition - mainDropZoneOffset;

        Vector3 diff = parentPositionWithOffset - mainDropZonePositionWithOffset;
        Vector2Int boardPosition = new Vector2Int(Mathf.RoundToInt(diff.x / CELL_SIZE), -Mathf.RoundToInt(diff.y / CELL_SIZE));

        //Debug.DrawLine(parentPosition, parentPositionWithOffset, Color.red, 2000f, false);
        //Debug.DrawLine(mainDropZonePosition, mainDropZonePositionWithOffset, Color.green, 2000f, false);
        //Debug.DrawLine(parentPositionWithOffset, mainDropZonePositionWithOffset, Color.blue, 2000f, false);

        // Clean up previous position
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                if (phoneMinigameController.currentBooard[y, x] == pieceType)
                    phoneMinigameController.currentBooard[y, x] = PhoneMinigameController.PieceType.None;
            }
        }

        bool isValidlyOverBoard = true;
        List<Vector2> shapeOffsets = phoneMinigameController.pieceShapes[pieceType];
        foreach (var offset in shapeOffsets)
        {
            int x = (int)(boardPosition.x + offset.x);
            int y = (int)(boardPosition.y + offset.y);

            if (x < 0 || x >= 7 || y < 0 || y >= 7 || (phoneMinigameController.currentBooard[x, y] != PhoneMinigameController.PieceType.None))
            {
                isValidlyOverBoard = false;
                break;
            }
        }

        if (!isValidlyOverBoard)
        {
            parent.transform.SetParent(defaultDropZone);
            parentRectTransform.anchoredPosition = Vector2.zero;
        }
        else
        {
            parent.transform.SetParent(phoneMinigameController.mainDropZone);
            parentRectTransform.anchoredPosition = new Vector2(boardPosition.x * CELL_SIZE - mainDropZoneOffset.x + parentOffset.x, -boardPosition.y * CELL_SIZE - mainDropZoneOffset.y + parentOffset.y);

            foreach (var offset in shapeOffsets)
            {
                int x = (int)(boardPosition.x + offset.x);
                int y = (int)(boardPosition.y + offset.y);
                phoneMinigameController.currentBooard[x, y] = pieceType; // reversed visualisation
            }
        }

        LoggingService.Log(LoggingService.LogCategory.Minigame, "{" +
                "\"message\":\"Phone minigame piece dropped\"," +
                "\"piece\":\"" + pieceType.ToString() + "\"," +
                "\"isValidlyOverBoard\":" + isValidlyOverBoard.ToString().ToLower() + "," +
                "\"currentLevelName\":\"" + phoneMinigameController.currentLevel.levelNameTextKey + "\"" +
            "}");

        phoneMinigameController.EvaluateMove();
    }
}
