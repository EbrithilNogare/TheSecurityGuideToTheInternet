using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhoneMinigameController : MonoBehaviour {
    public enum PieceType {
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

    private struct PhoneTetrisLevelDescription {
        public string levelNameTextKey;
        public PieceType[] correctpieces;
        public int[,] board;
    }

    public Color boardBagroundPieceColorFree;
    public Color boardBagroundPieceColorWall;
    public RectTransform[] pieceDropZones;
    public RectTransform mainDropZone;
    public GameObject boardBagroundPiecesparent;
    public GameObject dragAndDropTemporaryParent;
    public GameObject BoardSelection;
    public GameObject[] BoardCards;
    public TMPro.TextMeshProUGUI appNameLabel;

    [HideInInspector]
    public PieceType[,] currentBooard;

    private PhoneTetrisLevelDescription currentLevel;


    void Start() {
        for (int i = 0; i < BoardCards.Length; i++) {
            Transform boardCard = BoardCards[i].transform.GetChild(1);
            for (int x = 0; x < 7; x++) {
                for (int y = 0; y < 7; y++) {
                    boardCard.GetChild(y * 7 + x).GetComponent<UnityEngine.UI.Image>().color = levels[i].board[y, x] == 1 ? new Color(0.4f, 0.4f, 0.4f, 1) : new Color(0.2f, 0.2f, 0.2f, 1);
                }
            }
        }
    }

    public void SelectBoard(int boardIndex) {
        LoggingService.Log(LoggingService.LogCategory.Minigame, "Phone Minigame: selected board: " + boardIndex);


        foreach (var card in BoardCards) {
            if (card != BoardCards[boardIndex]) {
                card.GetComponent<Button>().interactable = false;
                card.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
            }
        }

        BoardCards[boardIndex].transform.DOLocalMove(new Vector3(0, 127.5f, 0), 2.0f).SetEase(Ease.InOutCubic).OnComplete(() => {
            currentLevel = levels[boardIndex];
            InitBoard();
            appNameLabel.text = BoardCards[boardIndex].transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
            BoardSelection.SetActive(false);
        });
    }

    public int EvaluateMove() {
        bool isUsingCorrectPieces = true;
        bool isInsideOfBoard = true;
        bool noEmptyCells = true;

        for (int x = 0; x < 7; x++) {
            for (int y = 0; y < 7; y++) {
                if (currentLevel.board[y, x] == 0 && currentBooard[x, y] != PieceType.None)
                    isInsideOfBoard = false;

                if (currentLevel.board[y, x] == 1 && currentBooard[x, y] == PieceType.None)
                    noEmptyCells = false;

                if (currentBooard[x, y] != PieceType.None && !Array.Exists(currentLevel.correctpieces, piece => piece == currentBooard[x, y]))
                    isUsingCorrectPieces = false;
            }
        }


        int toReturn = isUsingCorrectPieces && noEmptyCells ? isInsideOfBoard ? 2 : 1 : 0;

        if (toReturn == 2)
            FinishMinigame(toReturn); // todo do it better

        return toReturn;
    }

    private void InitBoard() {
        currentBooard = new PieceType[7, 7];
        for (int x = 0; x < 7; x++) {
            for (int y = 0; y < 7; y++) {
                currentBooard[x, y] = PieceType.None;
                boardBagroundPiecesparent.transform.GetChild(y * 7 + x).GetComponent<UnityEngine.UI.Image>().color = currentLevel.board[y, x] == 1 ? boardBagroundPieceColorFree : boardBagroundPieceColorWall;
            }
        }
    }
    public void FinishMinigame(int score) {
        LoggingService.Log(LoggingService.LogCategory.Minigame, "{\"message\":\"Phone minigame completed\",\"score\":" + score + "}");
        Store.Instance.minigameStars = score;
        int scoreForStore = score == 0 ? 0b000 : score == 1 ? 0b100 : 0b110;
        Store.Instance.SetLevelScore(Store.Level.Phone, scoreForStore);
        Store.Instance.quizToLoad = Store.Quiz.Phone;

        DOVirtual.DelayedCall(2, () => SceneManager.LoadScene("Quiz"));
    }

    public Dictionary<PieceType, List<Vector2>> pieceShapes = new Dictionary<PieceType, List<Vector2>>()
    {
        { PieceType.O, new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) } },
        { PieceType.L, new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2), new Vector2(1, 2) } },
        { PieceType.M, new List<Vector2> { new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0), new Vector2(3, 0) } },
        { PieceType.A, new List<Vector2> { new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1) } },
        { PieceType.S, new List<Vector2> { new Vector2(1, 0), new Vector2(2, 0), new Vector2(0, 1), new Vector2(1, 1) } },
        { PieceType.J, new List<Vector2> { new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 2), new Vector2(1, 2) } },
        { PieceType.K, new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 2) } },
        { PieceType.T, new List<Vector2> { new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0), new Vector2(1, 1) } },
        { PieceType.Z, new List<Vector2> { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1) } }
    };

    private PhoneTetrisLevelDescription[] levels = new PhoneTetrisLevelDescription[] {
        new PhoneTetrisLevelDescription() {
            levelNameTextKey = "MessagingApp",
            correctpieces = new PieceType[] { PieceType.O, PieceType.M, PieceType.A, PieceType.S, PieceType.T },
            board = new int[,]{
                { 0,0,0,0,0,0,0 },
                { 0,0,1,0,1,1,0 },
                { 0,1,1,1,1,1,0 },
                { 0,1,1,1,1,1,0 },
                { 0,0,1,1,1,0,0 },
                { 0,0,1,1,1,1,0 },
                { 0,0,0,0,0,0,0 },
            }
        },
        new PhoneTetrisLevelDescription() {
            levelNameTextKey = "VideoEditingApp",
            correctpieces = new PieceType[] { PieceType.O, PieceType.M, PieceType.A, PieceType.Z },
            board = new int[,]{
                { 0,0,0,0,0,0,0 },
                { 0,1,1,1,1,0,0 },
                { 0,1,1,1,1,0,0 },
                { 0,1,1,1,1,1,0 },
                { 0,0,1,1,1,0,0 },
                { 0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0 },
            }
        },
        new PhoneTetrisLevelDescription() {
            levelNameTextKey = "FitnessApp",
            correctpieces = new PieceType[] { PieceType.O, PieceType.L, PieceType.J, PieceType.K, PieceType.T  },
            board = new int[,]{
                { 0,0,0,0,0,0,0 },
                { 0,1,0,0,0,0,0 },
                { 0,1,1,1,1,0,0 },
                { 0,1,1,1,1,1,0 },
                { 0,1,1,1,1,1,0 },
                { 0,1,1,1,1,1,0 },
                { 0,0,0,0,0,0,0 },
            }
        },
    };
}

