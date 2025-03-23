using DG.Tweening;
using System;
using System.Collections;
using System.Linq;
using TMPro;
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
    public TextMeshProUGUI feedbackMessageText;
    public GameObject dragAndDropTemporaryParent;

    public Canvas canvas;

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
    private IEnumerator DelayCoroutine(float delay, Action action) {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    public void Submit() {
        bool OKDropZOneIsCorrect = dropZoneOKRow.GetComponentsInChildren<AVFileStructure>().All(item => !item.isVirus);
        bool NotOKDropZOneIsCorrect = dropZoneNotOKRow.GetComponentsInChildren<AVFileStructure>().All(item => item.isVirus);

        if (!OKDropZOneIsCorrect || !NotOKDropZOneIsCorrect) {
            DisplayErrorMessage();
            return;
        }

        DisplaySuccessMessage();
    }

    private void DisplayErrorMessage() {
        feedbackMessageText.color = Color.red;
        feedbackMessageText.text = "Some files are still incorectly categorized!";
    }

    private void DisplaySuccessMessage() {
        feedbackMessageText.color = Color.green;
        feedbackMessageText.text = "You did a great job!";
        FinishMinigame();
    }

    public void FinishMinigame() {
        int score = 2; // todo do it better
        LoggingService.Log(LoggingService.LogCategory.Minigame, "{\"message\":\"Malware minigame completed\",\"score\":" + score + "}");
        Store.Instance.minigameStars = score;
        int scoreForStore = score == 0 ? 0b000 : score == 1 ? 0b100 : 0b110;
        Store.Instance.SetLevelScore((int)Store.Level.Malware, scoreForStore);
        Store.Instance.quizToLoad = Store.Quiz.Phone;

        DOVirtual.DelayedCall(2, () => SceneManager.LoadScene("Quiz"));
    }
}
