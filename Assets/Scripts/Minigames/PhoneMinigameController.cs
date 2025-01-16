using UnityEngine;

public class PhoneMinigameController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI[] pieceLabels;
    public RectTransform[] pieceDropZones;
    public RectTransform mainDropZone;

    private int[,] minimalSolution;
    private int[,] currentBooard;


    void Start()
    {
        currentBooard = new int[7, 7];
    }

    public void GenerateNewSolution()
    {

    }

    public void TryMovePiece(GameObject piece)
    {

    }
}
