using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TFAMinigameController : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI clockTime;
    public GameObject phoneLockScreen;
    public GameObject phoneAppScreen;
    public GameObject phoneSmsScreen;
    public GameObject phoneLocationScreen;
    public TextMeshProUGUI phoneSmsValue;
    public Button biometricButton;
    public TextMeshProUGUI appTimeout;
    public Image appTimeoutClock;
    public TextMeshProUGUI appCodeField;
    public Transform hwKey;
    public GameObject hwKeyDropZone;
    public Transform keyDropZoneParent;
    public TextMeshProUGUI laptopFieldValue;
    public GameObject laptopFieldObject;
    public Image progressBar;
    public GameObject laptopSubmit;
    public GameObject laptopLabelPassword;
    public GameObject laptopLabelAppCode;
    public GameObject laptopLabelHWKey;
    public GameObject laptopLabelSMS;
    public GameObject laptopLabelLocation;
    public GameObject laptopLabelDone;


    private const float appTimeoutDuration = 15f;
    private const int numberOfPhases = 5;

    private string appCode = "";
    private string smsCode;
    private float appTime = 16f;
    private string laptopField = "";
    private int phaseOfTheGame = 0;


    void Start()
    {
        smsCode = Random.Range(1000, 9999).ToString();
        phoneSmsValue.text = smsCode;

        progressBar.fillAmount = 1 / (float)numberOfPhases;

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
        Debug.Log("Phone unlocked");
        biometricButton.interactable = false;

        phoneLockScreen.transform.DOLocalMoveY(700, .6f).SetEase(Ease.InQuart).OnComplete(() =>
        {
            phoneLockScreen.SetActive(false);
            phoneAppScreen.SetActive(true);
        });
    }

    public void KeyboardPressed(string value)
    {
        Debug.Log("Key pressed: " + value);

        if (value == "Clear")
        {
            laptopField = "";
        }
        else if (value == "Enter" || value == "Submit")
        {
            if (CodeIsCorrect(laptopField, phaseOfTheGame))
                GoToNextPhase();

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

    public void PhoneLocationConfirm(bool isItMe)
    {
        Debug.Log("Phone location confirmed: " + isItMe);

        if (isItMe)
        {
            GoToNextPhase();
        }
        else
        {
            // todo
        }
    }

    public void HWKeyInsert()
    {
        Debug.Log("HW key inserted");

        hwKeyDropZone.SetActive(false);
        hwKey.transform.SetParent(keyDropZoneParent);
        hwKey.DOLocalMoveX(57f, 1f).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            GoToNextPhase();
        });
    }

    private bool CodeIsCorrect(string code, int phase)
    {
        switch (phase)
        {
            case 0:
                return code == "1234"; // password on sticker note
            case 1:
                return code == appCode;
            case 2:
                return false; // hw key
            case 3:
                return false; // location
            case 4:
                return code == smsCode;
            default:
                return false; // finish
        }
    }

    public void GoToNextPhase()
    {
        phaseOfTheGame++;

        progressBar.DOFillAmount((phaseOfTheGame + 1) / (float)(numberOfPhases + 1), 1f).SetEase(Ease.OutQuart);

        switch (phaseOfTheGame)
        {
            case 1: // app code
                laptopLabelPassword.SetActive(false);
                laptopLabelAppCode.SetActive(true);
                break;
            case 2: // hw key
                laptopLabelAppCode.SetActive(false);
                laptopLabelHWKey.SetActive(true);
                laptopFieldObject.SetActive(false);
                laptopSubmit.SetActive(false);

                hwKeyDropZone.SetActive(true);
                break;
            case 3: // location
                laptopLabelHWKey.SetActive(false);
                laptopLabelLocation.SetActive(true);

                phoneAppScreen.SetActive(false);
                phoneLocationScreen.SetActive(true);
                break;
            case 4: // sms code
                laptopLabelLocation.SetActive(false);
                laptopLabelSMS.SetActive(true);
                laptopFieldObject.SetActive(true);
                laptopSubmit.SetActive(true);

                phoneLocationScreen.SetActive(false);
                phoneSmsScreen.SetActive(true);
                break;
            case 5: // finish
                laptopLabelSMS.SetActive(false);
                laptopLabelDone.SetActive(true);
                laptopFieldObject.SetActive(false);
                laptopSubmit.SetActive(false);

                FinishMinigame();
                break;
        }
    }

    private void FinishMinigame()
    {
        int score = 2;
        LoggingService.Log(LoggingService.LogCategory.Minigame, "{\"message\":\"TFA minigame completed\",\"score\":" + score + "}");
        Store.Instance.minigameStars = score;
        int scoreForStore = score == 0 ? 0b000 : score == 1 ? 0b100 : 0b110;
        Store.Instance.SetLevelScore(Store.Level.TFA, scoreForStore);
        Store.Instance.quizToLoad = Store.Quiz.TFA;

        DOVirtual.DelayedCall(2, () => SceneManager.LoadScene("Quiz"));
    }
}
