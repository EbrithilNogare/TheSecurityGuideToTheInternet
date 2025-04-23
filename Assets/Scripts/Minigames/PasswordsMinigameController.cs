using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PasswordsMinigameController : MonoBehaviour
{
    [Header("Settings")]
    public float writingDuration = 10f;

    [Header("References")]
    public RectTransform progressBar;
    public float progressBarFinalLength;
    public Image[] strikeImages;
    public RectTransform[] conesOfView;
    public Transform player;
    public Sprite strikeOff;
    public Sprite strikeOn;
    public Color strikeOffColor;
    public Color strikeOnColor;
    public GameObject submitButton;
    public Color strikeColorOfCone;
    public Color defaultColorOfCone;
    public HoldableButton writingButton;

    private const float coughtCooldownDuration = 1f;

    private bool isWriting;
    private float fillAmount;
    private bool passwordFilledSuccesfully;
    private int strikesCount;
    private float coughtCooldown;
    private float timeOfWriting;
    private float timeOfNotWriting;

    void Start()
    {
        GameRestart();
    }

    private void GameRestart()
    {
        isWriting = false;
        fillAmount = 0;
        passwordFilledSuccesfully = false;
        strikesCount = 0;
        coughtCooldown = 0;
        timeOfWriting = 0;
        timeOfNotWriting = 0;

        progressBar.sizeDelta = new Vector2(0, progressBar.sizeDelta.y);

        UpdateStrikes();
    }

    void Update()
    {
        if (passwordFilledSuccesfully)
            return;


        coughtCooldown += Time.deltaTime;

        if (isWriting)
        {
            timeOfWriting += Time.deltaTime;
            CheckForViolations();
            fillAmount = Mathf.Clamp01(fillAmount + Time.deltaTime / writingDuration);
            progressBar.sizeDelta = new Vector2(fillAmount * progressBarFinalLength, progressBar.sizeDelta.y);
        }
        else
        {
            timeOfNotWriting += Time.deltaTime;
        }

        if (fillAmount >= 1)
        {
            isWriting = false;
            passwordFilledSuccesfully = true;
            writingButton.enabled = false;
            submitButton.SetActive(true);
        }
    }

    public void Writing(bool writing)
    {
        isWriting = writing;
    }

    private void CheckForViolations()
    {
        if (coughtCooldown < coughtCooldownDuration)
            return;

        bool wasCought = false;

        for (int i = 0; i < conesOfView.Length; i++)
        {
            var cone = conesOfView[i];

            var angle = Mathf.Atan2(player.position.x - cone.position.x, player.position.y - cone.position.y) * Mathf.Rad2Deg * -1 + 180 % 360;
            var rotation = cone.rotation.eulerAngles.z;
            var diff = Mathf.Abs(angle - rotation);

            if (diff < 7f)
            {
                wasCought = true;
                var coneImage = cone.GetComponent<Image>();
                coneImage.color = strikeColorOfCone;
                coneImage.DOColor(defaultColorOfCone, 0.5f);
                //Debug.DrawLine(cone.position, player.position, Color.red);
                break;
            }
        }

        if (wasCought)
        {
            HasBeenCaught();
        }
    }

    private void HasBeenCaught()
    {
        if (coughtCooldown < coughtCooldownDuration)
            return;

        strikesCount++;
        isWriting = false;
        coughtCooldown = 0;

        if (strikesCount >= strikeImages.Length)
        {
            LoggingService.Log(LoggingService.LogCategory.Minigame, "{\"message\":\"Passwords minigame game over\",\"timeOfWriting\":" + timeOfWriting + ",\"timeOfNotWriting\":" + timeOfNotWriting + "}");
            GameRestart();
            return;
        }
        else
            UpdateStrikes();
    }

    private void UpdateStrikes()
    {
        for (int i = 0; i < strikeImages.Length; i++)
        {
            if (i < strikesCount)
            {
                strikeImages[i].color = strikeOnColor;
                strikeImages[i].sprite = strikeOn;
            }
            else
            {
                strikeImages[i].color = strikeOffColor;
                strikeImages[i].sprite = strikeOff;
            }
        }
    }

    public void FinishMinigame()
    {
        int score = strikesCount <= 1 ? 2 : 1;

        LoggingService.Log(LoggingService.LogCategory.Minigame, "{\"message\":\"Passwords minigame completed\",\"score\":" + score + ",\"timeOfWriting\":" + timeOfWriting + ",\"timeOfNotWriting\":" + timeOfNotWriting + ",\"strikes\":" + strikesCount + "}");
        Store.Instance.minigameStars = score;
        int scoreForStore = score == 1 ? 0b100 : 0b110;
        Store.Instance.SetLevelScore(Store.Level.Passwords, scoreForStore);
        Store.Instance.quizToLoad = Store.Quiz.Passwords;

        DOVirtual.DelayedCall(.5f, () => SceneManager.LoadScene("Quiz"));
    }
}
