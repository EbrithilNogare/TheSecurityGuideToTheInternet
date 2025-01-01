using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public struct QuizQuestion
    {
        public bool isVertialLayouot;
        public string textQuestion;
        public string[] textAnswers;
        public int[] imageAnswerIndices;
        public int correctAnswerIndex;

#nullable enable
        public QuizQuestion(bool isVertialLayouot, string textQuestion, string[]? textAnswers, int[]? imageAnswerIndices, int correctAnswerIndex)
        {
            this.isVertialLayouot = isVertialLayouot;
            this.textQuestion = textQuestion;
            this.textAnswers = textAnswers ?? new string[0];
            this.imageAnswerIndices = imageAnswerIndices ?? new int[0];
            this.correctAnswerIndex = correctAnswerIndex;
        }
    }
#nullable disable

    [Header("External variables")]
    public Sprite[] quizSprites;
    public Color correctAnswerColor;
    public Color incorrectAnswerColor;
    public TMPro.TextMeshProUGUI quizProgress;
    public TMPro.TextMeshProUGUI quizQuestionText;
    public TMPro.TextMeshProUGUI scoreCounterText;
    public RectTransform scoreCounterPosition;
    public GameObject verticalAnswersContainer;
    public GameObject horizontalAnswersContainer;
    public GameObject[] verticalQuestionGameObjects;
    public TMPro.TextMeshProUGUI[] verticalQuestionTexts;
    public GameObject[] horizontalQuestionGameObjects;
    public Image[] horizontalQuestionImages;
    public GameObject particleContainer;
    public Image countdown;
    public TMPro.TextMeshProUGUI categoryLabel;


    private StringTable stringTable;
    private QuizQuestion[] currentQuiz;
    private string currentQuizCategory;
    private int currentQuestionIndex;
    private bool isEvaluatingInProgress;
    private int score;
    private float defaultTimeToRespond = 30f;
    private float timeToRespond;
    private bool allQuizesMode = false;
    private int allQuizesModeIndex = 0;

    void Start()
    {
        stringTable = LocalizationSettings.StringDatabase.GetTable("QuizStrings");
        Store.Instance.quizScore = 0;

        Store.Quiz quizCategoryFromStore = Store.Instance.quizToLoad;

        if (quizCategoryFromStore == Store.Quiz.None)
            quizCategoryFromStore = Store.Quiz.Phishing; // for development purposes

        if (quizCategoryFromStore == Store.Quiz.All)
        {
            allQuizesMode = true;
            quizCategoryFromStore = Store.Quiz.Malware; // full quiz
        }


        StartQuizCategory(quizCategoryFromStore);

        DoWelcomeAnimation();
    }

    private void StartQuizCategory(Store.Quiz quizName)
    {
        currentQuiz = allQuizes[quizName];
        currentQuizCategory = "";

        switch (quizName)
        {
            case Store.Quiz.Malware: currentQuizCategory = "malwareCategory"; break;
            case Store.Quiz.Firewall: currentQuizCategory = "firewallCategory"; break;
            case Store.Quiz.Phishing: currentQuizCategory = "phishingCategory"; break;
            case Store.Quiz.Cookies: currentQuizCategory = "cookiesCategory"; break;
            case Store.Quiz.Phone: currentQuizCategory = "phoneCategory"; break;
            case Store.Quiz.AI: currentQuizCategory = "aiCategory"; break;
            case Store.Quiz.Passwords: currentQuizCategory = "passwordsCategory"; break;
            case Store.Quiz.TFA: currentQuizCategory = "tfaCategory"; break;
            default: Debug.LogError("unknown quiz category: " + quizName.ToString()); break;
        }

        categoryLabel.text = currentQuizCategory != "" ? stringTable.GetEntry(currentQuizCategory).GetLocalizedString() : "";

        currentQuestionIndex = 0;
        isEvaluatingInProgress = false;
        DisplayQuestion(currentQuiz[currentQuestionIndex]);
        quizProgress.text = $"{currentQuestionIndex + 1} / {currentQuiz.Length}";
    }

    private void Update()
    {
        timeToRespond += Time.deltaTime;
        countdown.fillAmount = Mathf.Clamp(1f - timeToRespond / defaultTimeToRespond, 0f, 1f);
    }

    private void DoWelcomeAnimation()
    {
        float duration = 1f;
        quizQuestionText.transform.DOMoveY(quizQuestionText.transform.position.y, duration).From(quizQuestionText.transform.position.y - 300).SetEase(Ease.OutBack);
        if (currentQuiz[currentQuestionIndex].isVertialLayouot)
            verticalAnswersContainer.transform.DOMoveY(verticalAnswersContainer.transform.position.y, duration).From(verticalAnswersContainer.transform.position.y - 300).SetEase(Ease.OutBack);
        else
            horizontalAnswersContainer.transform.DOMoveY(horizontalAnswersContainer.transform.position.y, duration).From(horizontalAnswersContainer.transform.position.y - 300).SetEase(Ease.OutBack);
    }
    private void DoEndAnimation()
    {
        float duration = 1.5f;
        if (currentQuiz[currentQuestionIndex - 1].isVertialLayouot)
            verticalAnswersContainer.transform.DOMoveY(verticalAnswersContainer.transform.position.y - 1000, duration).From(verticalAnswersContainer.transform.position.y).SetEase(Ease.InBack);
        else
            horizontalAnswersContainer.transform.DOMoveY(horizontalAnswersContainer.transform.position.y - 1000, duration).From(horizontalAnswersContainer.transform.position.y).SetEase(Ease.InBack);
        quizQuestionText.transform.DOMoveY(quizQuestionText.transform.position.y - 1000, duration).From(quizQuestionText.transform.position.y).SetEase(Ease.InBack).OnComplete(() =>
        {
            Store.Instance.quizScore = score;
            SceneManager.LoadScene("LevelSelection");
        });
    }

    private void HandleEndOfQuiz()
    {
        if (allQuizesMode && System.Enum.IsDefined(typeof(Store.Quiz), allQuizesModeIndex + 1))
        {
            Store.Instance.quizScore += score;
            allQuizesModeIndex++;
            StartQuizCategory((Store.Quiz)allQuizesModeIndex);
        }
        else
        {
            DoEndAnimation();
        }
    }

    private void DoParticlesAnimation(RectTransform buttonPosition, float duration)
    {
        for (int i = 0; i < particleContainer.transform.childCount; i++)
        {
            Transform particle = particleContainer.transform.GetChild(i);
            particle.gameObject.SetActive(true);
            particle.position = buttonPosition.position + new Vector3(Random.Range(buttonPosition.rect.width / 2, -buttonPosition.rect.width / 2), Random.Range(buttonPosition.rect.height / 2, -buttonPosition.rect.height / 2), 0);
            var endPosition = scoreCounterPosition.position;
            particle
                .DOMove(endPosition, Random.Range(duration * .9f, duration * 1.1f))
                .SetEase(Ease.InCubic)
                .OnComplete(() => particle.gameObject.SetActive(false));
        }
        int currentAmount = score;
        score = score + Mathf.Clamp((int)(100f - timeToRespond + defaultTimeToRespond), 0, 100);
        DOTween.To(() => currentAmount, x => scoreCounterText.text = x.ToString(), score, duration / 2).SetDelay(duration / 2);
    }
    public void ChooseAnswer(int answerIndex)
    {
        if (isEvaluatingInProgress)
            return;

        isEvaluatingInProgress = true;

        bool isVerticalLayout = currentQuiz[currentQuestionIndex].isVertialLayouot;
        int correctAnswerIndex = currentQuiz[currentQuestionIndex].correctAnswerIndex;
        bool isCorrectAnswer = answerIndex == correctAnswerIndex;

        LoggingService.Log(LoggingService.LogCategory.Quiz, $"{{category:{currentQuizCategory},questionIndex:{currentQuestionIndex},answerIndex:{answerIndex},isCorrectAnswer:{isCorrectAnswer},timeToRespond:{Mathf.Floor(timeToRespond)}}}");

        GameObject answerButton = currentQuiz[currentQuestionIndex].isVertialLayouot
            ? verticalQuestionGameObjects[answerIndex]
            : horizontalQuestionGameObjects[answerIndex];
        Color targetColor = isCorrectAnswer ? correctAnswerColor : incorrectAnswerColor;
        Color originalColor = answerButton.GetComponent<Image>().color;

        if (!isCorrectAnswer)
        {
            GameObject correctAnswerButton = currentQuiz[currentQuestionIndex].isVertialLayouot
            ? verticalQuestionGameObjects[correctAnswerIndex]
            : horizontalQuestionGameObjects[correctAnswerIndex];

            Color targetColorOfCorrectAnswer = correctAnswerColor;
            Color originalColorOfCorrectAnswer = correctAnswerButton.GetComponent<Image>().color;

            DOTween.Sequence()
                .Append(correctAnswerButton.GetComponent<Image>().DOColor(targetColorOfCorrectAnswer, 0.2f))
                .AppendInterval(1f)
                .OnComplete(() =>
                {
                    correctAnswerButton.GetComponent<Image>().color = originalColorOfCorrectAnswer;
                });
        }

        Sequence sequence = DOTween.Sequence()
            .Append(answerButton.GetComponent<Image>().DOColor(targetColor, 0.2f))
            .AppendCallback(() =>
            {
                if (isCorrectAnswer)
                    DoParticlesAnimation(answerButton.GetComponent<RectTransform>(), 1f);
            })
            .AppendInterval(1f)
            .OnComplete(() =>
            {
                answerButton.GetComponent<Image>().color = originalColor;

                currentQuestionIndex++;

                if (currentQuestionIndex < currentQuiz.Length)
                {
                    isEvaluatingInProgress = false;
                    quizProgress.text = $"{currentQuestionIndex + 1} / {currentQuiz.Length}";
                    DisplayQuestion(currentQuiz[currentQuestionIndex]);
                }
                else
                {
                    HandleEndOfQuiz();
                }
            });
    }

    void DisplayQuestion(QuizQuestion quizQuestion)
    {
        timeToRespond = 0;
        quizQuestionText.text = stringTable.GetEntry(quizQuestion.textQuestion).GetLocalizedString();

        if (quizQuestion.isVertialLayouot)
        {
            verticalAnswersContainer.SetActive(true);
            horizontalAnswersContainer.SetActive(false);

            for (int i = 0; i < quizQuestion.textAnswers.Length; i++)
            {
                verticalQuestionGameObjects[i].SetActive(true);
                verticalQuestionTexts[i].text = stringTable.GetEntry(quizQuestion.textAnswers[i]).GetLocalizedString();
            }

            for (int i = quizQuestion.textAnswers.Length; i < verticalQuestionGameObjects.Length; i++)
            {
                verticalQuestionGameObjects[i].SetActive(false);
            }

        }
        else
        {
            verticalAnswersContainer.SetActive(false);
            horizontalAnswersContainer.SetActive(true);

            for (int i = 0; i < quizQuestion.imageAnswerIndices.Length; i++)
            {
                horizontalQuestionGameObjects[i].SetActive(true);
                horizontalQuestionImages[i].sprite = quizSprites[quizQuestion.imageAnswerIndices[i]];
            }

            for (int i = quizQuestion.imageAnswerIndices.Length; i < horizontalQuestionGameObjects.Length; i++)
            {
                horizontalQuestionGameObjects[i].SetActive(false);
            }
        }
    }

    private Dictionary<Store.Quiz, QuizQuestion[]> allQuizes = new Dictionary<Store.Quiz, QuizQuestion[]>(){
        { Store.Quiz.Malware , new QuizQuestion[]{
            new QuizQuestion(true, "malware_question_0", new string[]{ "malware_answer_0_0", "malware_answer_0_1", "malware_answer_0_2" }, null, 1),
            new QuizQuestion(true, "malware_question_1", new string[]{ "malware_answer_1_0", "malware_answer_1_1", "malware_answer_1_2" }, null, 1),
            new QuizQuestion(true, "malware_question_2", new string[]{ "malware_answer_2_0", "malware_answer_2_1", "malware_answer_2_2" }, null, 2),
            new QuizQuestion(true, "malware_question_3", new string[]{ "malware_answer_3_0", "malware_answer_3_1", "malware_answer_3_2" }, null, 1),
            new QuizQuestion(true, "malware_question_4", new string[]{ "malware_answer_4_0", "malware_answer_4_1", "malware_answer_4_2" }, null, 0),
        }},
        { Store.Quiz.Firewall , new QuizQuestion[]{
            new QuizQuestion(true, "firewall_question_0", new string[]{ "firewall_answer_0_0", "firewall_answer_0_1", "firewall_answer_0_2" }, null, 1),
            new QuizQuestion(true, "firewall_question_1", new string[]{ "firewall_answer_1_0", "firewall_answer_1_1", "firewall_answer_1_2" }, null, 1),
            new QuizQuestion(true, "firewall_question_2", new string[]{ "firewall_answer_2_0", "firewall_answer_2_1", "firewall_answer_2_2" }, null, 0),
            new QuizQuestion(true, "firewall_question_3", new string[]{ "firewall_answer_3_0", "firewall_answer_3_1", "firewall_answer_3_2" }, null, 0),
            new QuizQuestion(false, "firewall_question_4", null, new int[]{0, 1}, 0),
        }},
        { Store.Quiz.Phishing , new QuizQuestion[]{
            new QuizQuestion(true, "phishing_question_0", new string[]{ "phishing_answer_0_0", "phishing_answer_0_1", "phishing_answer_0_2" }, null, 0),
            new QuizQuestion(true, "phishing_question_1", new string[]{ "phishing_answer_1_0", "phishing_answer_1_1", "phishing_answer_1_2" }, null, 0),
            new QuizQuestion(true, "phishing_question_2", new string[]{ "phishing_answer_2_0", "phishing_answer_2_1", "phishing_answer_2_2" }, null, 2),
            new QuizQuestion(true, "phishing_question_3", new string[]{ "phishing_answer_3_0", "phishing_answer_3_1", "phishing_answer_3_2" }, null, 0),
            new QuizQuestion(true, "phishing_question_4", new string[]{ "phishing_answer_4_0", "phishing_answer_4_1", "phishing_answer_4_2" }, null, 1),
        }},
        { Store.Quiz.Cookies , new QuizQuestion[]{
            new QuizQuestion(true, "cookies_question_0", new string[]{ "cookies_answer_0_0", "cookies_answer_0_1", "cookies_answer_0_2" }, null, 1),
            new QuizQuestion(true, "cookies_question_1", new string[]{ "cookies_answer_1_0", "cookies_answer_1_1", "cookies_answer_1_2" }, null, 0),
            new QuizQuestion(true, "cookies_question_2", new string[]{ "cookies_answer_2_0", "cookies_answer_2_1", "cookies_answer_2_2" }, null, 1),
            new QuizQuestion(true, "cookies_question_3", new string[]{ "cookies_answer_3_0", "cookies_answer_3_1", "cookies_answer_3_2" }, null, 1),
            new QuizQuestion(true, "cookies_question_4", new string[]{ "cookies_answer_4_0", "cookies_answer_4_1", "cookies_answer_4_2" }, null, 0),
        }},
        { Store.Quiz.Phone , new QuizQuestion[]{
            new QuizQuestion(true, "phone_question_0", new string[]{ "phone_answer_0_0", "phone_answer_0_1", "phone_answer_0_2" }, null, 0),
            new QuizQuestion(true, "phone_question_1", new string[]{ "phone_answer_1_0", "phone_answer_1_1", "phone_answer_1_2" }, null, 1),
            new QuizQuestion(true, "phone_question_2", new string[]{ "phone_answer_2_0", "phone_answer_2_1", "phone_answer_2_2" }, null, 0),
            new QuizQuestion(true, "phone_question_3", new string[]{ "phone_answer_3_0", "phone_answer_3_1", "phone_answer_3_2" }, null, 1),
            new QuizQuestion(true, "phone_question_4", new string[]{ "phone_answer_4_0", "phone_answer_4_1", "phone_answer_4_2" }, null, 0),
        }},
        { Store.Quiz.AI , new QuizQuestion[]{
            new QuizQuestion(true, "ai_question_0", new string[]{ "ai_answer_0_0", "ai_answer_0_1", "ai_answer_0_2" }, null, 1),
            new QuizQuestion(true, "ai_question_1", new string[]{ "ai_answer_1_0", "ai_answer_1_1", "ai_answer_1_2" }, null, 0),
            new QuizQuestion(true, "ai_question_2", new string[]{ "ai_answer_2_0", "ai_answer_2_1", "ai_answer_2_2" }, null, 1),
            new QuizQuestion(true, "ai_question_3", new string[]{ "ai_answer_3_0", "ai_answer_3_1", "ai_answer_3_2" }, null, 1),
            new QuizQuestion(true, "ai_question_4", new string[]{ "ai_answer_4_0", "ai_answer_4_1", "ai_answer_4_2" }, null, 0),
        }},
        { Store.Quiz.Passwords , new QuizQuestion[]{
            new QuizQuestion(true, "passwords_question_0", new string[]{ "passwords_answer_0_0", "passwords_answer_0_1", "passwords_answer_0_2" }, null, 0),
            new QuizQuestion(true, "passwords_question_1", new string[]{ "passwords_answer_1_0", "passwords_answer_1_1", "passwords_answer_1_2" }, null, 0),
            new QuizQuestion(true, "passwords_question_2", new string[]{ "passwords_answer_2_0", "passwords_answer_2_1", "passwords_answer_2_2" }, null, 0),
            new QuizQuestion(true, "passwords_question_3", new string[]{ "passwords_answer_3_0", "passwords_answer_3_1", "passwords_answer_3_2" }, null, 1),
            new QuizQuestion(true, "passwords_question_4", new string[]{ "passwords_answer_4_0", "passwords_answer_4_1", "passwords_answer_4_2" }, null, 1),
        }},
        { Store.Quiz.TFA , new QuizQuestion[]{
            new QuizQuestion(true, "tfa_question_0", new string[]{ "tfa_answer_0_0", "tfa_answer_0_1", "tfa_answer_0_2" }, null, 0),
            new QuizQuestion(true, "tfa_question_1", new string[]{ "tfa_answer_1_0", "tfa_answer_1_1", "tfa_answer_1_2" }, null, 1),
            new QuizQuestion(true, "tfa_question_2", new string[]{ "tfa_answer_2_0", "tfa_answer_2_1", "tfa_answer_2_2" }, null, 1),
            new QuizQuestion(true, "tfa_question_3", new string[]{ "tfa_answer_3_0", "tfa_answer_3_1", "tfa_answer_3_2" }, null, 0),
            new QuizQuestion(false, "tfa_question_4", null, new int[]{2, 3}, 1),
        }},
    };
}
