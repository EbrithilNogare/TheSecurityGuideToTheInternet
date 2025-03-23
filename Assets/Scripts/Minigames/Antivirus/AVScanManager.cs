using DG.Tweening;
using TMPro;
using UnityEngine;

public class AVScanManager : MonoBehaviour {
    public bool isScanning = false;

    public GameObject scanProgressReady;
    public GameObject scanProgressScanning;
    public GameObject scanProgressOk;
    public GameObject scanProgressError1;
    public GameObject scanProgressError2;
    public TextMeshProUGUI scanProgressScanningText;

    private Tweener currentTween;


    void Awake() {
        ShowDropZoneText(scanProgressReady);
    }

    public void StartScanning(AVFileStructure item) {
        isScanning = true;
        ShowDropZoneText(scanProgressScanning);


        if (currentTween.IsActive()) { currentTween.Kill(); }

        currentTween = DOTween.To(() => 0,
                x => scanProgressScanningText.maxVisibleCharacters = scanProgressScanningText.text.Length - 5 + x,
                5,
                0.3f)
            .SetLoops(3, LoopType.Restart)
            .OnComplete(() => { EvaluateScanFinish(item); });
    }

    private void EvaluateScanFinish(AVFileStructure item) {
        isScanning = false;
        scanProgressScanningText.maxVisibleCharacters = int.MaxValue;
        if (item.isVirus) {
            switch (Random.Range(0, 2)) {
                case 0:
                    ShowDropZoneText(scanProgressError1); break;
                case 1:
                    ShowDropZoneText(scanProgressError2); break;
            }
        }
        else {
            ShowDropZoneText(scanProgressOk);
        }
    }

    public void OnFileRemoved() {
        isScanning = false;
        if (currentTween != null && currentTween.IsActive()) {
            currentTween.Kill();
        }
        ShowDropZoneText(scanProgressReady);
    }

    private void ShowDropZoneText(GameObject textToEnable) {
        scanProgressReady.SetActive(scanProgressReady == textToEnable);
        scanProgressScanning.SetActive(scanProgressScanning == textToEnable);
        scanProgressOk.SetActive(scanProgressOk == textToEnable);
        scanProgressError1.SetActive(scanProgressError1 == textToEnable);
        scanProgressError2.SetActive(scanProgressError2 == textToEnable);
    }
}
