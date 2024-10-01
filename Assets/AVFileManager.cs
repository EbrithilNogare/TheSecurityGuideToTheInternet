using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AVFileManager : MonoBehaviour
{
    public RectTransform sourceRow;
    public RectTransform dropZoneOK;
    public RectTransform dropZoneOKRow;
    public RectTransform dropZoneScan;
    public RectTransform dropZoneScanRow;
    public TextMeshProUGUI dropZoneScanProgressInfo;
    public RectTransform dropZoneNotOK;
    public RectTransform dropZoneNotOKRow;
    public GameObject submitButton;

    public Canvas canvas;

    private string scanProgressReady = "Ready to scan file.";
    private string scanProgressScanning = "Scanning ";
    private string scanProgressOk = "File is safe.";
    private string scanProgressError1 = "Reading passwords!!!";
    private string scanProgressError2 = "Accessing webcam.";

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        dropZoneScanProgressInfo.SetText(scanProgressReady);
    }

    public void EvaluateDrop(AVFileStructure item)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(dropZoneOK, Input.mousePosition, canvas.worldCamera))
        {
            HandleDropOnOK(item);
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(dropZoneScan, Input.mousePosition, canvas.worldCamera))
        {
            HandleDropOnScan(item);
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(dropZoneNotOK, Input.mousePosition, canvas.worldCamera))
        {
            HandleDropOnNotOK(item);
        }
        else
        {
            ReturnToSource(item);
        }

        if (sourceRow.childCount == 1)
        {
            submitButton.SetActive(true);
        }
    }

    public void HandleDropOnOK(AVFileStructure item)
    {
        if (transform.parent == dropZoneOKRow)
            return;

        item.transform.SetParent(dropZoneOKRow, false);
    }

    public void HandleDropOnScan(AVFileStructure item)
    {
        if (transform.parent == dropZoneScanRow)
            return;

        if (dropZoneScanRow.childCount > 0)
        {
            ReturnToSource(item);
            return;
        }

        item.transform.SetParent(dropZoneScanRow, false);

        // scanning for viruses

        dropZoneScanProgressInfo.SetText(scanProgressScanning + ".....");
        dropZoneScanProgressInfo.maxVisibleCharacters = scanProgressScanning.Length;
        DOTween.To(() => dropZoneScanProgressInfo.maxVisibleCharacters,
                 x => dropZoneScanProgressInfo.maxVisibleCharacters = x,
                 dropZoneScanProgressInfo.text.Length,
                 1.0f).SetLoops(3, LoopType.Restart).OnComplete(() =>
                 {
                     dropZoneScanProgressInfo.maxVisibleCharacters = int.MaxValue;
                     if (item.isVirus)
                     {
                         switch (Random.Range(0, 2))
                         {
                             case 1:
                                 dropZoneScanProgressInfo.SetText(scanProgressError1); break;
                             case 2:
                                 dropZoneScanProgressInfo.SetText(scanProgressError2); break;
                         }
                     }
                     else
                     {
                         dropZoneScanProgressInfo.SetText(scanProgressOk);
                     }
                 });

        // todo handle scan animation better
    }

    public void HandleDropOnNotOK(AVFileStructure item)
    {
        if (transform.parent == dropZoneNotOKRow)
            return;

        item.transform.SetParent(dropZoneNotOKRow, false);
    }

    public void ReturnToSource(AVFileStructure item)
    {
        if (transform.parent == sourceRow)
            return;

        item.transform.SetParent(sourceRow, false);
    }

    public void Submit()
    {
        // todo
        Debug.Log("Submit");
        SceneManager.UnloadSceneAsync("Antivirus");
    }
}
