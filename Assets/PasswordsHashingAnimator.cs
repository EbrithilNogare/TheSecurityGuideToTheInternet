using DG.Tweening;
using UnityEngine;

public class PasswordsHashingAnimator : MonoBehaviour
{
    public Transform passwordObject;
    public Transform saltObject;
    public Transform hashObject;

    private const float duration = 8f;
    private const float delay = 1f;

    private bool initialized = false;
    private Vector3 defaultPositionOfPasswords;
    private Vector3 defaultPositionOfSalt;
    private Vector3 defaultPositionOfHash;
    private CanvasGroup passwordCanvasGroup;
    private CanvasGroup saltCanvasGroup;
    private CanvasGroup hashCanvasGroup;

    private void Start()
    {
        defaultPositionOfPasswords = passwordObject.transform.localPosition;
        defaultPositionOfSalt = saltObject.transform.localPosition;
        defaultPositionOfHash = hashObject.transform.localPosition;
        passwordCanvasGroup = passwordObject.GetComponent<CanvasGroup>();
        saltCanvasGroup = saltObject.GetComponent<CanvasGroup>();
        hashCanvasGroup = hashObject.GetComponent<CanvasGroup>();

        initialized = true;
        StartFade();
    }

    private void OnEnable()
    {
        if (!initialized) return;
        StartFade();
    }

    private void OnDisable()
    {
        if (!initialized) return;
        Cleanup();
    }

    private void OnDestroy()
    {
        if (!initialized) return;
        Cleanup();
    }

    private void InitPositions()
    {
        passwordObject.transform.localPosition = defaultPositionOfPasswords;
        saltObject.transform.localPosition = defaultPositionOfSalt;
        hashObject.transform.localPosition = defaultPositionOfHash;
        passwordCanvasGroup.alpha = 0f;
        saltCanvasGroup.alpha = 0f;
        hashCanvasGroup.alpha = 1f;
    }

    private void StartFade()
    {
        InitPositions();

        passwordCanvasGroup.DOFade(1f, delay);
        saltCanvasGroup.DOFade(1f, delay).OnComplete(StartTweens);
    }

    private void StartTweens()
    {
        passwordObject.DOLocalMoveX(-30f, duration).SetEase(Ease.InOutSine);
        saltObject.DOLocalMoveX(-30f, duration).SetEase(Ease.InOutSine);
        hashObject.DOLocalMoveX(95.5f, duration).SetEase(Ease.InOutSine).OnComplete(RerollTweens);
    }

    private void RerollTweens()
    {
        hashCanvasGroup.DOFade(0f, delay).SetDelay(5f).OnComplete(StartFade);
    }

    public void Cleanup()
    {
        DOTween.Kill(passwordObject);
        DOTween.Kill(saltObject);
        DOTween.Kill(hashObject);
        DOTween.Kill(passwordCanvasGroup);
        DOTween.Kill(saltCanvasGroup);

        InitPositions();
    }
}
