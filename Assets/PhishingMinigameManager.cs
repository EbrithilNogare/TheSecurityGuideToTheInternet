using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PhishingMinigameManager : MonoBehaviour
{
    public GameObject urlTabContainer;
    public GameObject fieldTabContainer;
    public GameObject imageTabContainer;
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
        Vector3 image1Position = new Vector2(200, 200);
        GameObject image1 = Instantiate(images[0], templateContainer.transform);
        image1.GetComponent<RectTransform>().anchoredPosition = image1Position;
        image1.GetComponent<Image>().color = colors[Random.Range(0, colors.Length)];
        image1.GetComponent<PhishingMinigameDragable>().enabled = false;
        image1.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        image1.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
        image1.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
        objectsInTemplate.Add(new System.Tuple<Vector2, itemName, Color>(image1Position, itemName.Pet, image1.GetComponent<Image>().color));

        // Image 2
        Vector3 image2Position = new Vector2(1000, 200);
        GameObject image2 = Instantiate(images[1], templateContainer.transform);
        image2.GetComponent<RectTransform>().anchoredPosition = image2Position;
        image2.GetComponent<Image>().color = colors[Random.Range(0, colors.Length)];
        image2.GetComponent<PhishingMinigameDragable>().enabled = false;
        image2.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        image2.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
        image2.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 200);
        objectsInTemplate.Add(new System.Tuple<Vector2, itemName, Color>(image2Position, itemName.Flower, image2.GetComponent<Image>().color));

        // Field 1
        Vector3 field1Position = new Vector2(600, 400);
        GameObject field1 = Instantiate(fields[0], templateContainer.transform);
        field1.GetComponent<RectTransform>().anchoredPosition = field1Position;
        field1.GetComponent<PhishingMinigameDragable>().enabled = false;
        field1.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        field1.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
        field1.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 80);
        objectsInTemplate.Add(new System.Tuple<Vector2, itemName, Color>(field1Position, itemName.Username, Color.white));

        // Field 2
        Vector3 field2Position = new Vector2(600, 300);
        GameObject field2 = Instantiate(fields[1], templateContainer.transform);
        field2.GetComponent<RectTransform>().anchoredPosition = field2Position;
        field2.GetComponent<PhishingMinigameDragable>().enabled = false;
        field2.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        field2.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
        field2.GetComponent<RectTransform>().sizeDelta = new Vector2(350, 80);
        objectsInTemplate.Add(new System.Tuple<Vector2, itemName, Color>(field2Position, itemName.Password, Color.white));
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
                urlTabContainer.SetActive(true);
                fieldTabContainer.SetActive(false);
                imageTabContainer.SetActive(false);
                colorSelectorContainer.SetActive(false);
                break;
            case 1:
                urlTabContainer.SetActive(false);
                fieldTabContainer.SetActive(true);
                imageTabContainer.SetActive(false);
                colorSelectorContainer.SetActive(false);
                break;
            case 2:
                urlTabContainer.SetActive(false);
                fieldTabContainer.SetActive(false);
                imageTabContainer.SetActive(true);
                colorSelectorContainer.SetActive(true);
                break;
        }
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

            if (urlScore == 0 && dragableComponent.objectName == itemName.Url)
            {
                urlScore = .5f;
                continue;
            }

            for (int templateChild = 0; templateChild < objectsInTemplate.Count; templateChild++)
            {
                System.Tuple<Vector2, itemName, Color> item = objectsInTemplate[templateChild];
                float distance = 1f - Mathf.Clamp((Vector2.Distance(websiteChild.GetComponent<RectTransform>().anchoredPosition, item.Item1) - 20) / 200f, 0f, 1f);
                float type = dragableComponent.objectName == item.Item2 ? 1 : 0;
                float color =
                    websiteChild.GetComponent<Image>().color == item.Item3 ||
                    websiteChild.GetComponent<Image>().color == fields[0].GetComponent<Image>().color
                    ? 1
                    : 0;
                scoreTablePerComponent[templateChild] = Mathf.Max(scoreTablePerComponent[templateChild], (distance + type + color) / 3f);
            }
        }

        float score = scoreTablePerComponent.Sum() + urlScore;
        float finalScore = websiteContainer.transform.childCount <= childrensNotAactive ? 0 : score / ((float)Mathf.Max(websiteContainer.transform.childCount - childrensNotAactive, 5)) * 100f;

        progressBar.fillAmount = finalScore / 100f;
        scoreText.text = finalScore.ToString("F0") + "%";
    }
}