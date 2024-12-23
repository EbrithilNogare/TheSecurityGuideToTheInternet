using System.Collections.Generic;
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

    private List<System.Tuple<Vector3, itemName, Color>> objectsInTemplate;

    public enum itemName
    {
        Url,
        Pet, Flower, Plane, Beer, Game, Pool,
        Username, Email, SocialNumber, CardNumber, Password, Submit
    }

    void Start()
    {
        setActiveTab(0);
        changeColorOfButtons();
        ChangeColor(0);
        SpawnObjectsInTemplate();
    }

    void SpawnObjectsInTemplate()
    {
        objectsInTemplate = new List<System.Tuple<Vector3, itemName, Color>>();

        // Image 1
        Vector3 image1Position = new Vector3(200, -500, 0);
        GameObject image1 = Instantiate(images[0], image1Position, Quaternion.identity, templateContainer.transform);
        //image1.transform.localPosition = image1Position;
        image1.GetComponent<Image>().color = colors[Random.Range(0, colors.Length)];
        objectsInTemplate.Add(new System.Tuple<Vector3, itemName, Color>(image1Position, itemName.Pet, image1.GetComponent<Image>().color));

        // Image 2
        Vector3 image2Position = new Vector3(1000, 500, 0);
        GameObject image2 = Instantiate(images[1], image2Position, Quaternion.identity, templateContainer.transform);
        image2.transform.localPosition = image2Position;
        image2.GetComponent<Image>().color = colors[Random.Range(0, colors.Length)];
        objectsInTemplate.Add(new System.Tuple<Vector3, itemName, Color>(image2Position, itemName.Flower, image2.GetComponent<Image>().color));

        // Field 1
        Vector3 field1Position = new Vector3(600, -300, 0);
        GameObject field1 = Instantiate(fields[0], field1Position, Quaternion.identity, templateContainer.transform);
        field1.transform.localPosition = field1Position;
        objectsInTemplate.Add(new System.Tuple<Vector3, itemName, Color>(field1Position, itemName.Username, Color.white));

        // Field 2
        Vector3 field2Position = new Vector3(600, -400, 0);
        GameObject field2 = Instantiate(fields[1], field2Position, Quaternion.identity, templateContainer.transform);
        field2.transform.localPosition = field2Position;
        objectsInTemplate.Add(new System.Tuple<Vector3, itemName, Color>(field2Position, itemName.Password, Color.white));
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
}
