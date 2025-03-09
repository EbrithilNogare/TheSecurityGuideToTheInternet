using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhishingMinigameManager : MonoBehaviour
{
    public GameObject urlTabContainer;
    public GameObject fieldTabContainer;
    public GameObject imageTabContainer;
    public Image urlTabButtonImage;
    public Image fieldTabButtonImage;
    public Image imageTabButtonImage;
    public Color tabButtonColorActive;
    public Color tabButtonColorInactive;
    public GameObject colorSelectorContainer;
    public string[] fakeURLs;
    public GameObject[] fields;
    public GameObject[] images;
    public Color[] colors;
    public Image[] colorChangeButtons;
    public GameObject[] dragableIcons;
    public GameObject templateContainer;
    public GameObject websiteContainer;
    public GameObject trashDropZone;
    public Image progressBar;
    public TMPro.TextMeshProUGUI scoreText;
    public GameObject TutorialGameObject;
    public GameObject SubmitGameObject;
    public Image[] scoreStars;
    public Sprite scoreStarFilled;
    public Sprite scoreStarEmpty;

    private List<System.Tuple<Vector2, itemName, Color>> objectsInTemplate;

    public enum itemName
    {
        Url,
        Pet, Flower, Plane, Beer, Game, Pool,
        Username, Email, SocialNumber, CardNumber, Password, Submit
    }

    void Start()
    {
        SpawnObjectsInTemplate();
        setActiveTab(0);
        changeColorOfButtons();
        ChangeColor(0);
    }

    void SpawnObjectsInTemplate()
    {
        objectsInTemplate = new List<System.Tuple<Vector2, itemName, Color>>();

        // Image 1
        SpawnObjectInTemplate(new Vector2(-400, -138), Random.Range(0, images.Length), true);

        // Image 2
        SpawnObjectInTemplate(new Vector2(400, -138), Random.Range(0, images.Length), true);

        // Field 1
        SpawnObjectInTemplate(new Vector2(0, 62), Random.Range(0, 3), false);

        // Field 2
        SpawnObjectInTemplate(new Vector2(0, -38), Random.Range(3, 5), false);

        // Field 3
        SpawnObjectInTemplate(new Vector2(0, -138), 5, false);
    }

    void SpawnObjectInTemplate(Vector3 position, int index, bool image)
    {
        GameObject instance = Instantiate(image ? images[index] : fields[index], templateContainer.transform);
        instance.transform.localPosition = position;
        if (image) { instance.GetComponent<Image>().color = colors[Random.Range(0, colors.Length)]; }
        instance.GetComponent<PhishingMinigameDragable>().enabled = false;
        instance.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        instance.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
        instance.GetComponent<RectTransform>().sizeDelta = image ? new Vector2(200, 200) : new Vector2(350, 80);
        objectsInTemplate.Add(new System.Tuple<Vector2, itemName, Color>(
            position,
            instance.GetComponent<PhishingMinigameDragable>().objectName,
            image ? instance.GetComponent<Image>().color : Color.white
        ));

    }

    public void ChangeColor(int colorIndex)
    {
        foreach (var item in dragableIcons)
        {
            item.GetComponent<Image>().color = colors[colorIndex];
        }
    }

    public void changeColorOfButtons()
    {
        for (int i = 0; i < colorChangeButtons.Length; i++)
        {
            colorChangeButtons[i].color = colors[i];
        }
    }

    public void setActiveTab(int tabIndex)
    {
        switch (tabIndex)
        {
            case 0:
                imageTabContainer.SetActive(true);
                colorSelectorContainer.SetActive(true);
                fieldTabContainer.SetActive(false);
                urlTabContainer.SetActive(false);
                imageTabButtonImage.color = tabButtonColorActive;
                fieldTabButtonImage.color = tabButtonColorInactive;
                urlTabButtonImage.color = tabButtonColorInactive;
                break;
            case 1:
                imageTabContainer.SetActive(false);
                colorSelectorContainer.SetActive(false);
                fieldTabContainer.SetActive(true);
                urlTabContainer.SetActive(false);
                imageTabButtonImage.color = tabButtonColorInactive;
                fieldTabButtonImage.color = tabButtonColorActive;
                urlTabButtonImage.color = tabButtonColorInactive;
                break;
            case 2:
                imageTabContainer.SetActive(false);
                colorSelectorContainer.SetActive(false);
                fieldTabContainer.SetActive(false);
                urlTabContainer.SetActive(true);
                imageTabButtonImage.color = tabButtonColorInactive;
                fieldTabButtonImage.color = tabButtonColorInactive;
                urlTabButtonImage.color = tabButtonColorActive;
                break;
        }
    }

    public void SubmitSolution()
    {
        int score = int.Parse(scoreText.text.Replace("%", "")) >= 96 ? 2 : 1;
        LoggingService.Log(LoggingService.LogCategory.Minigame, "Phishing Minigame completed with score: " + score);
        Store.Instance.minigameScore = score;
        Store.Instance.quizToLoad = Store.Quiz.Phishing;
        SceneManager.LoadScene("Quiz");
    }

    public void EvaluateTemplate()
    {
        float urlScore = 0;
        int childrensNotAactive = 0;
        float[] scoreTablePerComponent = new float[objectsInTemplate.Count];
        for (int webpageChildIndex = 0; webpageChildIndex < websiteContainer.transform.childCount; webpageChildIndex++)
        {
            var websiteChild = websiteContainer.transform.GetChild(webpageChildIndex);
            var dragableComponent = websiteChild?.GetComponent<PhishingMinigameDragable>();

            if (dragableComponent == null || !dragableComponent.isActiveAndEnabled)
            {
                childrensNotAactive++;
                continue;
            }

            if (dragableComponent.objectName == itemName.Url)
            {
                urlScore = .8f;
                continue;
            }

            float scoreOfBestComponent = 0;
            int indexOfBestComponent = 0;
            for (int templateChild = 0; templateChild < objectsInTemplate.Count; templateChild++)
            {
                System.Tuple<Vector2, itemName, Color> item = objectsInTemplate[templateChild];
                float distance = 1f - Mathf.Clamp((Vector2.Distance(websiteChild.transform.localPosition, item.Item1) - 20) / 200f, 0f, 1f);
                float type = dragableComponent.objectName == item.Item2 ? 1 : 0;
                float color =
                    websiteChild.GetComponent<Image>().color == item.Item3 ||
                    websiteChild.GetComponent<Image>().color == fields[0].GetComponent<Image>().color
                    ? 1
                    : 0;
                float scoreOfThisComponent = (distance + type + color) / 3f;
                if (scoreOfBestComponent < scoreOfThisComponent)
                {
                    scoreOfBestComponent = scoreOfThisComponent;
                    indexOfBestComponent = templateChild;
                }
            }
            scoreTablePerComponent[indexOfBestComponent] = Mathf.Max(scoreTablePerComponent[indexOfBestComponent], scoreOfBestComponent);
        }
        float score = scoreTablePerComponent.Sum() + urlScore;
        float finalScore = websiteContainer.transform.childCount <= childrensNotAactive
            ? 0
            : score / ((float)Mathf.Max(websiteContainer.transform.childCount - childrensNotAactive, objectsInTemplate.Count + 1)) * 100f;

        progressBar.fillAmount = finalScore / 100f;
        scoreText.text = ((int)finalScore).ToString() + "%";
        if (finalScore >= 85)
        {
            TutorialGameObject.SetActive(false);
            SubmitGameObject.SetActive(true);
            scoreStars[0].sprite = scoreStarFilled;
        }
        else
        {
            TutorialGameObject.SetActive(true);
            SubmitGameObject.SetActive(false);
            scoreStars[0].sprite = scoreStarEmpty;
        }
        if ((int)finalScore >= 96)
        {
            scoreStars[1].sprite = scoreStarFilled;
        }
        else
        {
            scoreStars[1].sprite = scoreStarEmpty;
        }

    }
}