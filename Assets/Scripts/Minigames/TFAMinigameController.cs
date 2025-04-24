using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TFAMinigameController : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI clockTime;
    public GameObject phoneLockScreen, phoneAppScreen, phoneSmsScreen, phoneLocationScreen;
    public TextMeshProUGUI phoneSmsValue, appTimeout, appCodeField, laptopFieldValue;
    public Button biometricButton;
    public Image appTimeoutClock, progressBar;
    public Transform hwKey, keyDropZoneParent;
    public GameObject hwKeyDropZone, laptopFieldObject, laptopSubmit;
    public GameObject laptopLabelPassword, laptopLabelAppCode, laptopLabelHWKey, laptopLabelSMS, laptopLabelLocation, laptopLabelDone, laptopLabelFail;

    const float appTimeoutDuration = 15f;
    const int numberOfPhases = 5;

    string appCode = "", smsCode, laptopField = "";
    float appTime = 16f;
    int phase = 0;
    Vector3 originalKeyPos;
    Transform originalKeyParent;

    void Start()
    {
        smsCode = Random.Range(1000, 9999).ToString();
        phoneSmsValue.text = smsCode;

        progressBar.fillAmount = 1f / numberOfPhases;
        originalKeyPos = hwKey.position;
        originalKeyParent = hwKey.parent;
        hwKeyDropZone.SetActive(false);
    }

    void Update()
    {
        clockTime.text = System.DateTime.Now.ToString("H:mm");
        float newTime = Time.fixedTime % appTimeoutDuration;

        if (appTime > newTime)
        {
            appCode = Random.Range(1000, 9999).ToString();
            appCodeField.text = appCode;
        }

        appTime = newTime;
        appTimeout.text = (15 - (int)appTime).ToString();
        appTimeoutClock.fillAmount = 1f - newTime / appTimeoutDuration;
    }

    public void PhoneUnlocked()
    {
        biometricButton.interactable = false;
        var pos = phoneLockScreen.transform.localPosition;

        phoneLockScreen.transform.DOLocalMoveY(700, .6f).SetEase(Ease.InQuart).OnComplete(() =>
        {
            phoneLockScreen.SetActive(false);
            phoneAppScreen.SetActive(true);
            phoneLockScreen.transform.localPosition = pos;
        });

        LogEvent("Phone unlocked");
    }

    public void KeyboardPressed(string value)
    {
        if (value == "Clear")
        {
            laptopField = "";
            LogEvent("Keyboard clear");
        }
        else if (value is "Enter" or "Submit")
        {
            LogEvent("Keyboard submit", laptopField);
            if (IsCodeCorrect(laptopField, phase)) NextPhase();
            else FailLogin();

            laptopField = "";
        }
        else
        {
            laptopField += value;
            if (laptopField.Length > 4)
            {
                laptopField = laptopField.Substring(1, 4);
            }
        }
        laptopFieldValue.text = laptopField;
    }

    public void PhoneLocationConfirm(bool confirmed)
    {
        LogEvent(confirmed ? "Phone location confirmed" : "Phone location not confirmed");
        if (confirmed)
        {
            NextPhase();
        }
        else
        {
            FailLogin();
        }
    }

    public void HWKeyInsert()
    {
        LogEvent("HW key inserted");

        hwKeyDropZone.SetActive(false);
        hwKey.SetParent(keyDropZoneParent);
        hwKey.DOLocalMoveX(57f, 1f).SetEase(Ease.InOutCubic).OnComplete(NextPhase);
    }

    bool IsCodeCorrect(string code, int phase)
    {
        return phase switch
        {
            0 => code == "1234",
            1 => code == appCode,
            4 => code == smsCode,
            _ => false
        };
    }

    public void FailLogin()
    {
        LogEvent("Login failed");

        SetAllLabels(false);
        laptopLabelFail.SetActive(true);
        laptopFieldObject.SetActive(false);
        laptopSubmit.SetActive(false);
        phoneAppScreen.SetActive(false);
        phoneSmsScreen.SetActive(false);
        phoneLocationScreen.SetActive(false);

        hwKey.SetParent(originalKeyParent);
        hwKey.position = originalKeyPos;

        phase = -1;
        DOVirtual.DelayedCall(2, () => { phase = -1; NextPhase(); });
    }

    public void NextPhase()
    {
        phase++;
        progressBar.DOFillAmount((phase + 1f) / (numberOfPhases + 1), 1f).SetEase(Ease.OutQuart);
        SetAllLabels(false);

        switch (phase)
        {
            case 0:
                laptopLabelPassword.SetActive(true);
                phoneLockScreen.SetActive(true);
                biometricButton.interactable = true;
                laptopFieldObject.SetActive(true);
                laptopSubmit.SetActive(true);
                break;

            case 1:
                laptopLabelAppCode.SetActive(true);
                break;

            case 2:
                laptopLabelHWKey.SetActive(true);
                laptopFieldObject.SetActive(false);
                laptopSubmit.SetActive(false);
                hwKeyDropZone.SetActive(true);
                break;

            case 3:
                laptopLabelLocation.SetActive(true);
                phoneAppScreen.SetActive(false);
                phoneLocationScreen.SetActive(true);
                break;

            case 4:
                laptopLabelSMS.SetActive(true);
                laptopFieldObject.SetActive(true);
                laptopSubmit.SetActive(true);
                phoneLocationScreen.SetActive(false);
                phoneSmsScreen.SetActive(true);
                break;

            case 5:
                laptopLabelDone.SetActive(true);
                laptopFieldObject.SetActive(false);
                laptopSubmit.SetActive(false);
                CompleteGame();
                break;
        }
    }

    void CompleteGame()
    {
        int score = 2;
        LogEvent("TFA minigame completed", score.ToString());
        Store.Instance.minigameStars = score;
        Store.Instance.SetLevelScore(Store.Level.TFA, score == 0 ? 0b000 : score == 1 ? 0b100 : 0b110);
        Store.Instance.quizToLoad = Store.Quiz.TFA;

        DOVirtual.DelayedCall(2, () => SceneManager.LoadScene("Quiz"));
    }

    void SetAllLabels(bool active)
    {
        laptopLabelPassword.SetActive(active);
        laptopLabelAppCode.SetActive(active);
        laptopLabelHWKey.SetActive(active);
        laptopLabelSMS.SetActive(active);
        laptopLabelLocation.SetActive(active);
        laptopLabelFail.SetActive(active);
    }

    void LogEvent(string @event, string value = "")
    {
        var json = $"{{\"message\":\"TFA minigame event\",\"event\":\"{@event}\"" + (value != "" ? $",\"value\":\"{value}\"" : "") + "}";
        LoggingService.Log(LoggingService.LogCategory.Minigame, json);
    }
}
