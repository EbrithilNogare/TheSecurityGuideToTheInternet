using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TransitionType { None, MoveUp, MoveDown, MoveRight, MoveLeft, FadeIn }

public class SlideTransitionManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private float duration;
    [SerializeField] private string sceneBeforePresentation;
    [SerializeField] private string sceneAfterPresentation;
    [SerializeField] private GameObject[] slides;
    [SerializeField] private TransitionType[] transitions;
    private int currentSlideIndex;
    private bool isTransitioning;

    void Awake()
    {
        currentSlideIndex = 0;
        for (int i = 0; i < slides.Length; i++)
            slides[i].SetActive(false);
        slides[currentSlideIndex].SetActive(true);
    }

    public void NextSlide()
    {
        if (isTransitioning || currentSlideIndex >= slides.Length) return;

        if (currentSlideIndex < slides.Length - 1)
        {
            AnimateTransition(
                transitions[currentSlideIndex % transitions.Length],
                slides[currentSlideIndex],
                slides[currentSlideIndex + 1]);
            currentSlideIndex++;
        }
        else
        {
            HandleBeyondLastSlide();
        }
    }

    public void PreviousSlide()
    {
        if (isTransitioning || currentSlideIndex < 0) return;

        if (currentSlideIndex > 0)
        {
            AnimateTransition(
                transitions[currentSlideIndex % transitions.Length],
                slides[currentSlideIndex],
                slides[currentSlideIndex - 1], true);
            currentSlideIndex--;
        }
        else
        {
            HandleBeforeFirstSlide();
        }
    }

    private void AnimateTransition(TransitionType transitionType, GameObject currentSlide, GameObject nextSlide, bool reverse = false)
    {
        isTransitioning = true;
        if (transitionType == TransitionType.FadeIn)
            AnimateFadeTransition(currentSlide, nextSlide, reverse);
        else
            AnimateMoveTransition(transitionType, currentSlide, nextSlide, reverse);
    }

    private void AnimateMoveTransition(TransitionType transitionType, GameObject currentSlide, GameObject nextSlide, bool reverse)
    {
        RectTransform currentRect = currentSlide.GetComponent<RectTransform>();
        RectTransform nextRect = nextSlide.GetComponent<RectTransform>();

        if (currentRect == null) currentRect = currentSlide.AddComponent<RectTransform>();
        if (nextRect == null) nextRect = nextSlide.AddComponent<RectTransform>();

        nextSlide.SetActive(true);

        Vector2 startPosition = Vector2.zero, targetPosition = Vector2.zero;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float canvasHeight = canvasRect.rect.height;
        float canvasWidth = canvasRect.rect.width;

        switch (transitionType)
        {
            case TransitionType.MoveUp:
                startPosition = reverse ? Vector2.down * canvasHeight : Vector2.up * canvasHeight;
                break;
            case TransitionType.MoveDown:
                startPosition = reverse ? Vector2.up * canvasHeight : Vector2.down * canvasHeight;
                break;
            case TransitionType.MoveRight:
                startPosition = reverse ? Vector2.left * canvasWidth : Vector2.right * canvasWidth;
                break;
            case TransitionType.MoveLeft:
                startPosition = reverse ? Vector2.right * canvasWidth : Vector2.left * canvasWidth;
                break;
        }

        nextRect.anchoredPosition = startPosition;

        Sequence sequence = DOTween.Sequence();
        sequence.Join(currentRect.DOAnchorPos(-startPosition, duration).SetEase(Ease.InOutQuad));
        sequence.Join(nextRect.DOAnchorPos(targetPosition, duration).SetEase(Ease.InOutQuad));
        sequence.OnComplete(() =>
        {
            currentSlide.SetActive(false);
            isTransitioning = false;
        });
    }

    private void AnimateFadeTransition(GameObject currentSlide, GameObject nextSlide, bool reverse)
    {
        CanvasGroup currentCanvas = currentSlide.GetComponent<CanvasGroup>();
        CanvasGroup nextCanvas = nextSlide.GetComponent<CanvasGroup>();

        if (currentCanvas == null) currentCanvas = currentSlide.AddComponent<CanvasGroup>();
        if (nextCanvas == null) nextCanvas = nextSlide.AddComponent<CanvasGroup>();

        nextSlide.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence.Join(currentCanvas.DOFade(reverse ? 0f : 1f, duration).From(reverse ? 1f : 0f));
        sequence.Join(nextCanvas.DOFade(reverse ? 1f : 0f, duration).From(reverse ? 0f : 1f));
        sequence.OnComplete(() =>
        {
            currentSlide.SetActive(false);
            isTransitioning = false;
        });
    }

    private void HandleBeforeFirstSlide()
    {
        SceneManager.LoadScene(sceneBeforePresentation);
    }

    private void HandleBeyondLastSlide()
    {
        SceneManager.LoadScene(sceneAfterPresentation);
    }
}
