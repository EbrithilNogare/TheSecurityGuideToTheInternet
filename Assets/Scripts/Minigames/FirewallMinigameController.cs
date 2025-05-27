using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirewallMinigameController : MonoBehaviour
{
    private struct Package
    {
        public bool isIncoming;
        public string label;
        public bool shouldBeBlocked;
    }

    public Transform package;
    public TextMeshProUGUI packageLabel;
    public DragableItem packageDragableItemScript;
    public Transform[] incomingConveyorBeltWheels;
    public Transform[] outgoingConveyorBeltWheels;
    public Transform incomingSpawn;
    public Transform incomingDropZone;
    public Transform outgoingSpawn;
    public Transform outgoingDropZone;
    public Transform fireDropZone;
    public Image[] scoreImages;
    public Color scoreImageCorrect;
    public Color scoreImageIncorrect;

    private float packageAnimationDuration = 2.5f;
    private float wheelRotations = 2.546f;
    private int packageIndex = -1;
    private int numberOfCorrectlySorted = 0;


    void Start()
    {
        packageDragableItemScript.enabled = false;
        Shuffle(possiblePackages);
        SpawnNewPackage();
    }

    public void SpawnNewPackage()
    {
        packageIndex++;

        if (packageIndex >= possiblePackages.Length)
        {
            FinishMinigame();
            return;
        }

        bool isIncoming = possiblePackages[packageIndex].isIncoming;
        package.gameObject.SetActive(true);
        packageLabel.text = possiblePackages[packageIndex].label;

        if (isIncoming)
            PlayIncomingConveyorBeltAnimationSpawn();
        else
            PlayOutgoingConveyorBeltAnimationSpawn();
    }

    private void FinishMinigame()
    {
        int score = numberOfCorrectlySorted < 10 ? 0 : numberOfCorrectlySorted + 2 >= possiblePackages.Length ? 2 : 1;

        LoggingService.Log(LoggingService.LogCategory.Minigame, "{\"message\":\"Firewall minigame completed\",\"score\":" + score + ",\"numberOfCorrectlySorted\":" + numberOfCorrectlySorted + "}");
        Store.Instance.minigameStars = score;
        int scoreForStore = score == 1 ? 0b100 : 0b110;
        Store.Instance.SetLevelScore(Store.Level.Firewall, scoreForStore);
        Store.Instance.quizToLoad = Store.Quiz.Firewall;

        DOVirtual.DelayedCall(.2f, () => SceneManager.LoadScene("Quiz"));
    }

    public void EvaluatePackageDrop(bool isCorectlySorted)
    {
        numberOfCorrectlySorted += isCorectlySorted ? 1 : 0;
        scoreImages[packageIndex].color = isCorectlySorted ? scoreImageCorrect : scoreImageIncorrect;
        // todo
    }

    public void OnDropIntoIncoming()
    {
        if (possiblePackages[packageIndex].isIncoming)
            return;
        bool isCorrect = !possiblePackages[packageIndex].shouldBeBlocked;
        LogDrop("incoming", isCorrect);
        packageDragableItemScript.enabled = false;
        PlayIncomingConveyorBeltAnimationDespawn(isCorrect);
    }

    public void OnDropIntoOutgoing()
    {
        if (!possiblePackages[packageIndex].isIncoming)
            return;
        bool isCorrect = !possiblePackages[packageIndex].shouldBeBlocked;
        LogDrop("outgoing", isCorrect);
        packageDragableItemScript.enabled = false;
        PlayOutgoingConveyorBeltAnimationDespawn(isCorrect);
    }

    public void OnDropIntoFire()
    {
        packageDragableItemScript.enabled = false;
        bool isCorrect = possiblePackages[packageIndex].shouldBeBlocked;
        LogDrop("fire", isCorrect);
        package.DOLocalMoveY(fireDropZone.position.y - 200f, packageAnimationDuration / 2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            package.gameObject.SetActive(false);
            EvaluatePackageDrop(isCorrect);
            SpawnNewPackage();
        });
    }

    private void LogDrop(string location, bool isCorrect)
    {
        string packageLabel = possiblePackages[packageIndex].label;
        LoggingService.Log(LoggingService.LogCategory.Minigame, "{\"message\":\"Dropped package\",\"packageLabel\":\"" + packageLabel.Replace('\n', ' ') + "\",\"droppedInto\":\"" + location + "\",\"isCorrect\":" + isCorrect.ToString().ToLower() + "}"); ;
    }

    public void PlayIncomingConveyorBeltAnimationSpawn()
    {
        Debug.Log("Playing conveyor belt spawn animation");
        package.SetParent(incomingDropZone);
        package.position = incomingSpawn.position;
        foreach (Transform wheel in incomingConveyorBeltWheels)
        {
            wheel.DORotate(Vector3.forward * -360 * wheelRotations, packageAnimationDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
        }
        package.DOMove(incomingDropZone.position, packageAnimationDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            packageDragableItemScript.enabled = true;
        });
    }

    public void PlayIncomingConveyorBeltAnimationDespawn(bool isCorectlySorted)
    {
        Debug.Log("Playing conveyor belt despawn animation");
        package.position = incomingDropZone.position;
        foreach (Transform wheel in incomingConveyorBeltWheels)
        {
            wheel.DORotate(Vector3.forward * 360 * wheelRotations, packageAnimationDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
        }
        package.DOMove(incomingSpawn.position, packageAnimationDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            package.gameObject.SetActive(false);
            EvaluatePackageDrop(isCorectlySorted);
            SpawnNewPackage();
        });
    }

    public void PlayOutgoingConveyorBeltAnimationSpawn()
    {
        Debug.Log("Playing outgoing conveyor belt spawn animation");
        package.SetParent(outgoingDropZone);
        package.position = outgoingSpawn.position;
        foreach (Transform wheel in outgoingConveyorBeltWheels)
        {
            wheel.DORotate(Vector3.forward * 360 * wheelRotations, packageAnimationDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
        }
        package.DOMove(outgoingDropZone.position, packageAnimationDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            packageDragableItemScript.enabled = true;
        });
    }

    public void PlayOutgoingConveyorBeltAnimationDespawn(bool isCorectlySorted)
    {
        Debug.Log("Playing outgoing conveyor belt despawn animation");
        package.position = outgoingDropZone.position;
        foreach (Transform wheel in outgoingConveyorBeltWheels)
        {
            wheel.DORotate(Vector3.forward * -360 * wheelRotations, packageAnimationDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
        }
        package.DOMove(outgoingSpawn.position, packageAnimationDuration).SetEase(Ease.Linear).OnComplete(() =>
        {
            package.gameObject.SetActive(false);
            EvaluatePackageDrop(isCorectlySorted);
            SpawnNewPackage();
        });
    }

    private void Shuffle<T>(T[] array)
    {
        int n = array.Length;
        while (n-- > 1)
        {
            int k = Random.Range(0, n);
            (array[n], array[k]) = (array[k], array[n]);
        }
    }

    private Package[] possiblePackages = new Package[]
    {
        new Package { isIncoming = true, label = "chrome.exe", shouldBeBlocked = false },
        new Package { isIncoming = true, label = "minecraft.exe", shouldBeBlocked = false },
        new Package { isIncoming = true, label = "fortnite.exe", shouldBeBlocked = false },
        new Package { isIncoming = true, label = "80\nTCP", shouldBeBlocked = false },
        new Package { isIncoming = true, label = "443\nTCP", shouldBeBlocked = false },
        new Package { isIncoming = false, label = "192.168.0.9\n21\nTCP", shouldBeBlocked = false },
        new Package { isIncoming = false, label = "1.1.1.1\n67\nTCP", shouldBeBlocked = false },
        new Package { isIncoming = false, label = "8.8.8.8\n67\nTCP", shouldBeBlocked = false },
        new Package { isIncoming = false, label = "8.8.8.8\n80\nTCP", shouldBeBlocked = false},
        new Package { isIncoming = false, label = "74.125.24.93\n80\nTCP", shouldBeBlocked = false },
        new Package { isIncoming = false, label = "69.63.187.17\n443\nTCP", shouldBeBlocked = false },
        new Package { isIncoming = true, label = "powershell.exe", shouldBeBlocked = true },
        new Package { isIncoming = true, label = "cmd.exe", shouldBeBlocked = true },
        new Package { isIncoming = true, label = "explorer.exe", shouldBeBlocked = true },
        new Package { isIncoming = true, label = "7\nUDP", shouldBeBlocked = true },
        new Package { isIncoming = false, label = "192.168.0.9\n25\nTCP", shouldBeBlocked = true },
        new Package { isIncoming = false, label = "8.8.8.8\n80\nUDP", shouldBeBlocked = true },
        new Package { isIncoming = false, label = "74.125.24.93\n21\nTCP", shouldBeBlocked = true },
    };
}
