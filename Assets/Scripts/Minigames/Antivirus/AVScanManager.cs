using DG.Tweening;
using TMPro;
using UnityEngine;

public class AVScanManager : MonoBehaviour
{
    public TextMeshProUGUI dropZoneScanProgressInfo;
    public bool isScanning = false;

    private string scanProgressReady = "Ready for simulation.";
    private string scanProgressScanning = "Simulating ";
    private string scanProgressOk = "File is safe.";
    private string scanProgressError1 = "Reading passwords!!!";
    private string scanProgressError2 = "Accessing webcam.";

    private Tweener currentTween;

    void Awake()
    {
        dropZoneScanProgressInfo.SetText(scanProgressReady);
    }

    public void StartScanning(AVFileStructure item)
    {
        isScanning = true;
        dropZoneScanProgressInfo.SetText(scanProgressScanning + ".....");
        dropZoneScanProgressInfo.maxVisibleCharacters = scanProgressScanning.Length;

        if (currentTween.IsActive()) { currentTween.Kill(); }

        currentTween = DOTween.To(() => dropZoneScanProgressInfo.maxVisibleCharacters,
                 x => dropZoneScanProgressInfo.maxVisibleCharacters = x,
                 dropZoneScanProgressInfo.text.Length,
                 0.3f).SetLoops(3, LoopType.Restart).OnComplete(() =>
                 {
                     EvaluateScanFinish(item);
                 });

    }

    private void EvaluateScanFinish(AVFileStructure item)
    {
        isScanning = false;
        dropZoneScanProgressInfo.maxVisibleCharacters = int.MaxValue;
        if (item.isVirus)
        {
            switch (Random.Range(0, 2))
            {
                case 0:
                    dropZoneScanProgressInfo.SetText(scanProgressError1); break;
                case 1:
                    dropZoneScanProgressInfo.SetText(scanProgressError2); break;
            }
        }
        else
        {
            dropZoneScanProgressInfo.SetText(scanProgressOk);
        }
    }

    public void OnFileRemoved()
    {
        Debug.Log("OnFileRemoved");
        isScanning = false;
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
            dropZoneScanProgressInfo.maxVisibleCharacters = int.MaxValue;
            dropZoneScanProgressInfo.SetText(scanProgressReady);
        }
    }
}
