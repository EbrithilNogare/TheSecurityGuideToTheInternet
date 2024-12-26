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
        public string[]? textAnswers;
        public Sprite[]? imageAnswers;
        public int correctAnswerIndex;

        public QuizQuestion(bool isVertialLayouot, string textQuestion, string[]? textAnswers, Sprite[]? imageAnswers, int correctAnswerIndex)
        {
            this.isVertialLayouot = isVertialLayouot;
            this.textQuestion = textQuestion;
            this.textAnswers = textAnswers;
            this.imageAnswers = imageAnswers;
            this.correctAnswerIndex = correctAnswerIndex;
        }
    }

    [Header("External variables")]
    public Color correctAnswerColor;
    public Color incorrectAnswerColor;
    public string quizToRun;
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


    private StringTable stringTable;
    private QuizQuestion[] currentQuiz;
    private int currentQuestionIndex;
    private bool isEvaluatingInProgress;

    void Start()
    {
        stringTable = LocalizationSettings.StringDatabase.GetTable("QuizStrings");
        currentQuiz = allQuizes["phishingQuiz"]; // todo choose by checking the Store
        currentQuestionIndex = 0;
        DisplayQuestion(currentQuiz[currentQuestionIndex]);
        quizProgress.text = $"{currentQuestionIndex + 1} / {currentQuiz.Length}";

        DoWelcomeAnimation();
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
            Store.Instance.quizScore = currentQuiz.Length == int.Parse(scoreCounterText.text) / 100 ? 1 : 0;
            SceneManager.LoadScene("LevelSelection");
        });
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
        int currentAmount = int.Parse(scoreCounterText.text);
        DOTween.To(() => currentAmount, x => scoreCounterText.text = x.ToString(), currentAmount + 100, duration / 2).SetDelay(duration / 2);
    }
    public void ChooseAnswer(int answerIndex)
    {
        if (isEvaluatingInProgress)
            return;

        isEvaluatingInProgress = true;

        bool isVerticalLayout = currentQuiz[currentQuestionIndex].isVertialLayouot;
        bool isCorrectAnswer = answerIndex == currentQuiz[currentQuestionIndex].correctAnswerIndex;

        LoggingService.Log(LoggingService.LogCategory.Quiz, $"{{category:{quizToRun},questionIndex:{currentQuestionIndex},answerIndex:{answerIndex},isCorrectAnswer:{isCorrectAnswer}}}");

        GameObject answerButton = currentQuiz[currentQuestionIndex].isVertialLayouot
            ? verticalQuestionGameObjects[answerIndex]
            : horizontalQuestionGameObjects[answerIndex];
        Color targetColor = isCorrectAnswer ? correctAnswerColor : incorrectAnswerColor;
        Color originalColor = answerButton.GetComponent<Image>().color;

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
                    DoEndAnimation();
                }
            });
    }

    void DisplayQuestion(QuizQuestion quizQuestion)
    {
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

            for (int i = 0; i < quizQuestion.imageAnswers.Length; i++)
            {
                horizontalQuestionGameObjects[i].SetActive(true);
                horizontalQuestionImages[i].sprite = quizQuestion.imageAnswers[i];
            }

            for (int i = quizQuestion.imageAnswers.Length; i < horizontalQuestionGameObjects.Length; i++)
            {
                horizontalQuestionGameObjects[i].SetActive(false);
            }
        }
    }

    private Dictionary<string, QuizQuestion[]> allQuizes = new Dictionary<string, QuizQuestion[]>(){
        { "phishingQuiz" , new QuizQuestion[]{
            new QuizQuestion(true, "phishing_question_0", new string[]{ "phishing_answer_0_0", "phishing_answer_0_1", "phishing_answer_0_2" }, null, 0),
            new QuizQuestion(true, "phishing_question_1", new string[]{ "phishing_answer_1_0", "phishing_answer_1_1", "phishing_answer_1_2" }, null, 0),
            new QuizQuestion(true, "phishing_question_2", new string[]{ "phishing_answer_2_0", "phishing_answer_2_1", "phishing_answer_2_2" }, null, 2),
            new QuizQuestion(true, "phishing_question_3", new string[]{ "phishing_answer_3_0", "phishing_answer_3_1", "phishing_answer_3_2" }, null, 0),
            new QuizQuestion(true, "phishing_question_4", new string[]{ "phishing_answer_4_0", "phishing_answer_4_1", "phishing_answer_4_2" }, null, 1),
        }}
    };
}
