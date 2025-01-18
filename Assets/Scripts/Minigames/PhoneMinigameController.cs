using System.Collections.Generic;
using UnityEngine;

public class PhoneMinigameController : MonoBehaviour
{
    public enum PieceType
    {
        O, // Core
        L, // Location
        M, // Camera
        A, // Microphone
        S, // Contacts
        J, // Health data
        K, // Activity recognition
        T, // Notifications
        Z, // Calendar
        None
    }

    private struct PhoneTetrisLevelDescription
    {
        public string levelNameTextKey;
        public int[] correctpieces;
        public int[,] board;
    }

    public Color boardBagroundPieceColorFree;
    public Color boardBagroundPieceColorWall;
    public RectTransform[] pieceDropZones;
    public RectTransform mainDropZone;
    public GameObject boardBagroundPiecesparent;
    public GameObject dragAndDropTemporaryParent;

    [HideInInspector]
    public PieceType[,] currentBooard;

    private PhoneTetrisLevelDescription currentLevel;


    void Start()
    {
        currentLevel = levels[0];
        InitBoard();
    }

    public void DoMove(PieceType pieceType, Vector2 position)
    {

    }

    private void InitBoard()
    {
        currentBooard = new PieceType[7, 7];
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                currentBooard[i, j] = PieceType.None;
                boardBagroundPiecesparent.transform.GetChild(i * 7 + j).GetComponent<UnityEngine.UI.Image>().color = currentLevel.board[i, j] == 1 ? boardBagroundPieceColorFree : boardBagroundPieceColorWall;
            }
        }
    }

    public Dictionary<PieceType, List<Vector2>> pieceShapes = new Dictionary<PieceType, List<Vector2>>()
    {
        { PieceType.O, new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) } },
        { PieceType.L, new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2), new Vector2(1, 2) } },
        { PieceType.M, new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2), new Vector2(1, 1) } },
        { PieceType.A, new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2), new Vector2(1, 0) } },
        { PieceType.S, new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2), new Vector2(1, 2) } },
        { PieceType.J, new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2), new Vector2(1, 0) } },
        { PieceType.K, new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2), new Vector2(1, 1) } },
        { PieceType.T, new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2), new Vector2(1, 2) } },
        { PieceType.Z, new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2), new Vector2(1, 0) } }
    };

    private PhoneTetrisLevelDescription[] levels = new PhoneTetrisLevelDescription[] { new PhoneTetrisLevelDescription() {
        levelNameTextKey = "MessagingApp",
        correctpieces = new int[] { 0, 2, 3, 4, 7 },
        board = new int[,]{
            { 0,0,0,0,0,0,0 },
            { 0,1,1,1,1,1,0 },
            { 0,1,1,1,1,1,0 },
            { 0,1,1,1,1,1,0 },
            { 0,1,1,1,1,1,0 },
            { 0,1,1,1,1,1,0 },
            { 0,0,0,0,0,0,0 },
    } } };
}

