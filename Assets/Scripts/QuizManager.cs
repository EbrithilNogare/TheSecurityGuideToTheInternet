using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
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
    public string quizToRun;
    public TMPro.TextMeshProUGUI quizScore;
    public TMPro.TextMeshProUGUI quizQuestionText;
    public GameObject verticalAnswersContainer;
    public GameObject horizontalAnswersContainer;
    public GameObject[] verticalQuestionGameObjects;
    public TMPro.TextMeshProUGUI[] verticalQuestionTexts;
    public GameObject[] horizontalQuestionGameObjects;
    public Image[] horizontalQuestionImages;


    private StringTable stringTable;
    private int currentQuestionIndex;

    void Start()
    {
        stringTable = LocalizationSettings.StringDatabase.GetTable("QuizStrings");
        currentQuestionIndex = 0;
        DisplayQuestion(phishingQuiz[currentQuestionIndex]);
    }

    public void ChooseAnswer(int answerIndex)
    {
        bool isVerticalLayout = phishingQuiz[currentQuestionIndex].isVertialLayouot;
        bool isCorrectAnwer = answerIndex == phishingQuiz[currentQuestionIndex].correctAnswerIndex;

        LoggingService.Log(LoggingService.LogCategory.Quiz, $"{{category:{quizToRun},questionIndex:{currentQuestionIndex},answerIndex:{answerIndex},isCorrectAnwer:{isCorrectAnwer}}}");

        // todo display it is correct

        currentQuestionIndex++;

        if (currentQuestionIndex < phishingQuiz.Length)
        {
            DisplayQuestion(phishingQuiz[currentQuestionIndex]);
        }
        else
        {
            // todo end of quiz
        }
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

    private QuizQuestion[] phishingQuiz = new QuizQuestion[]{
        new QuizQuestion(true, "phishing_question_0", new string[]{ "phishing_answer_0_0", "phishing_answer_0_1", "phishing_answer_0_2" }, null, 0),
        new QuizQuestion(true, "phishing_question_1", new string[]{ "phishing_answer_1_0", "phishing_answer_1_1", "phishing_answer_1_2" }, null, 0),
        new QuizQuestion(true, "phishing_question_2", new string[]{ "phishing_answer_2_0", "phishing_answer_2_1", "phishing_answer_2_2" }, null, 2),
        new QuizQuestion(true, "phishing_question_3", new string[]{ "phishing_answer_3_0", "phishing_answer_3_1", "phishing_answer_3_2" }, null, 0),
        new QuizQuestion(true, "phishing_question_4", new string[]{ "phishing_answer_4_0", "phishing_answer_4_1", "phishing_answer_4_2" }, null, 1),
    };
}
