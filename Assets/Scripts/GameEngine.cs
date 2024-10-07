using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEngine : MonoBehaviour
{
    public Store store;

    [Header("UI Elements")]
    public GameObject levelCompletedPopUp;
    public TextMeshProUGUI task1Text;
    public Image task1Icon;
    public TextMeshProUGUI task2Text;
    public Image task2Icon;
    public TextMeshProUGUI task3Text;
    public Image task3Icon;
    public Sprite taskDoneImage;

    [Header("Dektop Manager UI")]
    public Button internetButton;
    public Button antivirusButton;
    public Button gameButton1;
    public GameObject gameButton1Check;
    public GameObject gameButton1Cross;
    public Button gameButton2;
    public GameObject gameButton2Check;
    public GameObject gameButton2Cross;
    public Button gameButton3;
    public GameObject gameButton3Check;
    public GameObject gameButton3Cross;
    public Button backupButton;


    [Header("Window Manager UI")]
    public GameObject browserWindow;
    public Button downloadGame1Button;
    public Button downloadGame2Button;
    public Button downloadGame3Button;

    public GameObject gotHackedWindow;
    public Button tryAgainButton;


    private MalwareLevelScript malwareLevelScript;


    void Start()
    {
        malwareLevelScript = new MalwareLevelScript(this);
        malwareLevelScript.Init();
    }

    public void SetTasks(string task1, string task2, string task3)
    {
        task1Text.SetText(task1);
        task2Text.SetText(task2);
        task3Text.SetText(task3);
    }

    public void SetTaskDone(int index)
    {
        switch (index)
        {
            case 1:
                task1Icon.sprite = taskDoneImage;
                break;
            case 2:
                task2Icon.sprite = taskDoneImage;
                break;
            case 3:
                task3Icon.sprite = taskDoneImage;
                break;
        }
    }


}