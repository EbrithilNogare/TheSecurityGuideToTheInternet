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
        Vector3 parentPosition = parent.transform.position;
        Vector3 parentOffset = new Vector3(parentRectTransform.rect.width / 2, -parentRectTransform.rect.height / 2, 0);
        Vector3 parentPositionWithOffset = parentPosition - parentOffset;

        Vector3 mainDropZonePosition = phoneMinigameController.mainDropZone.transform.position;
        Vector3 mainDropZoneOffset = new Vector3(phoneMinigameController.mainDropZone.rect.width / 2, -phoneMinigameController.mainDropZone.rect.height / 2, 0);
        Vector3 mainDropZonePositionWithOffset = mainDropZonePosition - mainDropZoneOffset;

        Vector3 diff = parentPositionWithOffset - mainDropZonePositionWithOffset;
        Vector2Int boardPosition = new Vector2Int((int)Mathf.Round(diff.x / 64.0f), -(int)Mathf.Round(diff.y / 64.0f));


        bool isValidlyOverBoard = true;
        List<Vector2> shapeOffsets = phoneMinigameController.pieceShapes[pieceType];
        foreach (var offset in shapeOffsets)
        {
            int x = (int)(boardPosition.x + offset.x);
            int y = (int)(boardPosition.y + offset.y);

            if (x < 0 || x >= 7 || y < 0 || y >= 7 || (phoneMinigameController.currentBooard[x, y] != PhoneMinigameController.PieceType.None && phoneMinigameController.currentBooard[x, y] != pieceType))
            {
                isValidlyOverBoard = false;
                break;
            }
        }

        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                if (phoneMinigameController.currentBooard[y, x] == pieceType)
                    phoneMinigameController.currentBooard[y, x] = PhoneMinigameController.PieceType.None;
            }
        }

        if (!isValidlyOverBoard)
        {
            parent.transform.SetParent(defaultDropZone);
            parentRectTransform.anchoredPosition = Vector2.zero;

            phoneMinigameController.EvaluateMove();
            return;
        }

        parent.transform.SetParent(phoneMinigameController.mainDropZone);
        parentRectTransform.anchoredPosition = new Vector2(boardPosition.x * 64 - mainDropZoneOffset.x + parentOffset.x, -boardPosition.y * 64 - mainDropZoneOffset.y + parentOffset.y);

        foreach (var offset in shapeOffsets)
        {
            int x = (int)(boardPosition.x + offset.x);
            int y = (int)(boardPosition.y + offset.y);
            phoneMinigameController.currentBooard[x, y] = pieceType; // reversed visualisation
        }

        // debug print the board
        //for (int y = 0; y < 7; y++)
        //{
        //    string row = "";
        //    for (int x = 0; x < 7; x++)
        //    {
        //        row += (int)phoneMinigameController.currentBooard[x, y] + " ";
        //    }
        //    Debug.Log(row);
        //}

        phoneMinigameController.EvaluateMove();
    }
}
