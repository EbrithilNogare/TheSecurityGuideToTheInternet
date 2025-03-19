using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class PresentationController : MonoBehaviour {
    public string roomSceneName;
    public GameObject[] slides;

    private int currentSlideIndex;

    private void Start() {
        Assert.IsTrue(roomSceneName != "");
        Assert.IsTrue(slides.Length > 0);

        currentSlideIndex = 0;

        slides[currentSlideIndex].SetActive(true);
    }

    public void NextSlide() {
        slides[currentSlideIndex].SetActive(false);
        currentSlideIndex++;

        if (currentSlideIndex >= slides.Length) {
            SceneManager.LoadScene(roomSceneName);
        }
        else {
            slides[currentSlideIndex].SetActive(true);
        }
    }

    public void PreviousSlide() {
        slides[currentSlideIndex].SetActive(false);
        currentSlideIndex--;

        if (currentSlideIndex < 0) {
            SceneManager.LoadScene("LevelSelection");
        }
        else {
            slides[currentSlideIndex].SetActive(true);
        }
    }

    public void SkipSlides() {
        SceneManager.LoadScene(roomSceneName);
    }
}
