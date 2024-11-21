using System.Collections;
using UnityEngine;

public class SlideTransitionManager : MonoBehaviour
{
    [SerializeField] private GameObject[] slides;
    [SerializeField] private TransitionType[] transitions;
    private int currentSlideIndex = 0;

    void Start()
    {
        InitializeSlides();
        PlaySlide(currentSlideIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) NextSlide();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) PreviousSlide();
    }

    private void InitializeSlides()
    {
        for (int i = 0; i < slides.Length; i++)
            slides[i].SetActive(false);
    }

    private void PlaySlide(int index)
    {
        if (index >= 0 && index < slides.Length)
            slides[index].SetActive(true);
    }

    private void HideSlide(int index)
    {
        if (index >= 0 && index < slides.Length)
            slides[index].SetActive(false);
    }

    public void NextSlide()
    {
        if (currentSlideIndex < slides.Length - 1)
        {
            StartCoroutine(AnimateTransition(
                transitions[currentSlideIndex % transitions.Length],
                slides[currentSlideIndex],
                slides[currentSlideIndex + 1]));
            currentSlideIndex++;
        }
    }

    public void PreviousSlide()
    {
        if (currentSlideIndex > 0)
        {
            StartCoroutine(AnimateTransition(
                transitions[currentSlideIndex % transitions.Length],
                slides[currentSlideIndex],
                slides[currentSlideIndex - 1], reverse: true));
            currentSlideIndex--;
        }
    }

    private IEnumerator AnimateTransition(TransitionType transitionType, GameObject currentSlide, GameObject nextSlide, bool reverse = false)
    {
        RectTransform currentRect = currentSlide.GetComponent<RectTransform>();
        RectTransform nextRect = nextSlide.GetComponent<RectTransform>();
        CanvasGroup currentCanvas = currentSlide.GetComponent<CanvasGroup>();
        CanvasGroup nextCanvas = nextSlide.GetComponent<CanvasGroup>();

        if (currentRect == null) currentRect = currentSlide.AddComponent<RectTransform>();
        if (nextRect == null) nextRect = nextSlide.AddComponent<RectTransform>();
        if (currentCanvas == null) currentCanvas = currentSlide.AddComponent<CanvasGroup>();
        if (nextCanvas == null) nextCanvas = nextSlide.AddComponent<CanvasGroup>();

        nextSlide.SetActive(true);
        float elapsed = 0f, duration = 1f;

        Vector2 startPosition = Vector2.zero, targetPosition = Vector2.zero;
        switch (transitionType)
        {
            case TransitionType.None:
                nextSlide.SetActive(true);
                currentSlide.SetActive(false);
                yield break;

            case TransitionType.MoveUp:
                startPosition = reverse ? Vector2.down * Screen.height : Vector2.up * Screen.height;
                break;

            case TransitionType.MoveDown:
                startPosition = reverse ? Vector2.up * Screen.height : Vector2.down * Screen.height;
                break;

            case TransitionType.MoveRight:
                startPosition = reverse ? Vector2.left * Screen.width : Vector2.right * Screen.width;
                break;

            case TransitionType.MoveLeft:
                startPosition = reverse ? Vector2.right * Screen.width : Vector2.left * Screen.width;
                break;

            case TransitionType.FadeIn:
                currentCanvas.alpha = reverse ? 0f : 1f;
                nextCanvas.alpha = reverse ? 1f : 0f;
                break;
        }

        if (transitionType != TransitionType.FadeIn)
            nextRect.anchoredPosition = startPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (transitionType == TransitionType.FadeIn)
            {
                currentCanvas.alpha = Mathf.Lerp(reverse ? 0f : 1f, reverse ? 1f : 0f, t);
                nextCanvas.alpha = Mathf.Lerp(reverse ? 1f : 0f, reverse ? 0f : 1f, t);
            }
            else
            {
                currentRect.anchoredPosition = Vector2.Lerp(Vector2.zero, -startPosition, t);
                nextRect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            }

            yield return null;
        }

        currentSlide.SetActive(false);
    }
}

public enum TransitionType
{
    None,
    MoveUp,
    MoveDown,
    MoveRight,
    MoveLeft,
    FadeIn
}
