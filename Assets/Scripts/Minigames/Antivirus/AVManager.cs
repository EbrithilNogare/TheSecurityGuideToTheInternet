using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AVManager : MonoBehaviour {
    public RectTransform sourceRow;
    public RectTransform dropZoneOK;
    public RectTransform dropZoneOKRow;
    public RectTransform dropZoneScan;
    public RectTransform dropZoneScanRow;
    public AVScanManager dropZoneScanManager;
    public RectTransform dropZoneNotOK;
    public RectTransform dropZoneNotOKRow;
    public GameObject submitButton;
    public GameObject feedbackMessageTextOK;
    public GameObject feedbackMessageTextFail;
    public GameObject dragAndDropTemporaryParent;

    public Canvas canvas;

    private bool wonWithoutMistake = true;

    public void EvaluateDrop(AVFileStructure item) {
        if (RectTransformUtility.RectangleContainsScreenPoint(dropZoneOK, Input.mousePosition, canvas.worldCamera)) {
            HandleDropOnOK(item);
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(dropZoneScan, Input.mousePosition, canvas.worldCamera)) {
            HandleDropOnScan(item);
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(dropZoneNotOK, Input.mousePosition, canvas.worldCamera)) {
            HandleDropOnNotOK(item);
        }
        else {
            ReturnToSource(item);
        }

        if (sourceRow.childCount == 0 && dropZoneScanRow.childCount == 0) {
            ShowSubmitButton();
        }
        else {
            HideSubmitButton();
        }

        if (dropZoneScanManager.isScanning && dropZoneScanRow.childCount == 0)
            dropZoneScanManager.OnFileRemoved();
    }

    private void ShowSubmitButton() {
        submitButton.SetActive(true);
    }
    private void HideSubmitButton() {
        submitButton.SetActive(false);
    }

    public void HandleDropOnOK(AVFileStructure item) {
        item.transform.SetParent(dropZoneOKRow, false);
    }

    public void HandleDropOnScan(AVFileStructure item) {
        if (dropZoneScanRow.childCount > 0) {
            ReturnToSource(item);
            return;
        }

        item.transform.SetParent(dropZoneScanRow, false);

        // scanning for viruses
        dropZoneScanManager.StartScanning(item);
    }

    public void HandleDropOnNotOK(AVFileStructure item) {
        item.transform.SetParent(dropZoneNotOKRow, false);
    }

    public void ReturnToSource(AVFileStructure item) {
        if (transform.parent == sourceRow)
            return;

        item.transform.SetParent(sourceRow, false);
    }

    public void Submit() {
        bool OKDropZOneIsCorrect = dropZoneOKRow.GetComponentsInChildren<AVFileStructure>().All(item => !item.isVirus);
        bool NotOKDropZOneIsCorrect = dropZoneNotOKRow.GetComponentsInChildren<AVFileStructure>().All(item => item.isVirus);

        if (!OKDropZOneIsCorrect || !NotOKDropZOneIsCorrect) {
            wonWithoutMistake = false;
            DisplayErrorMessage();
            return;
        }

        DisplaySuccessMessage();
    }

    private void DisplayErrorMessage() {
        feedbackMessageTextOK.SetActive(false);
        feedbackMessageTextFail.SetActive(true);
    }

    private void DisplaySuccessMessage() {
        feedbackMessageTextOK.SetActive(true);
        feedbackMessageTextFail.SetActive(false);
        FinishMinigame();
    }

    public void FinishMinigame() {
        int score = wonWithoutMistake ? 2 : 1;
        LoggingService.Log(LoggingService.LogCategory.Minigame, "{\"message\":\"Malware minigame completed\",\"score\":" + score + "}");
        Store.Instance.minigameStars = score;
        int scoreForStore = score == 1 ? 0b100 : 0b110;
        Store.Instance.SetLevelScore(Store.Level.Malware, scoreForStore);
        Store.Instance.quizToLoad = Store.Quiz.Malware;

        DOVirtual.DelayedCall(2, () => SceneManager.LoadScene("Quiz"));
    }
}
