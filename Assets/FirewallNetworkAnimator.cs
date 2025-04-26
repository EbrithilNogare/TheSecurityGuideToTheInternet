using DG.Tweening;
using UnityEngine;

public class FirewallNetworkAnimator : MonoBehaviour
{
    public Transform fire1;
    public Transform fire2;
    public Transform package;

    private bool initialized = false;
    private Vector3 fire1StartPos;
    private Vector3 fire2StartPos;
    private Vector3 packageStartPos;
    private CanvasGroup fire1CanvasGroup;
    private CanvasGroup fire2CanvasGroup;
    private CanvasGroup packageCanvasGroup;

    private const float fadeDuration = 0.4f;
    private const float moveSpeed = 200f;


    void Start()
    {
        initialized = true;

        fire1StartPos = fire1.transform.localPosition;
        fire2StartPos = fire2.transform.localPosition;
        packageStartPos = package.transform.localPosition;

        StartAnimation();
    }
    private void InitPositions()
    {
        fire1.transform.localPosition = fire1StartPos;
        fire2.transform.localPosition = fire2StartPos;
        package.transform.localPosition = packageStartPos;
        fire1CanvasGroup = fire1.GetComponent<CanvasGroup>();
        fire2CanvasGroup = fire2.GetComponent<CanvasGroup>();
        packageCanvasGroup = package.GetComponent<CanvasGroup>();
        fire1CanvasGroup.alpha = 0f;
        fire2CanvasGroup.alpha = 0f;
        packageCanvasGroup.alpha = 0f;
    }

    private void StartAnimation()
    {
        InitPositions();
        MoveFire1();
    }

    private void MoveFire1()
    {
        float destination = -161.26f;
        fire1CanvasGroup.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            fire1.transform.DOLocalMoveX(destination, moveSpeed).SetSpeedBased().SetEase(Ease.InOutSine).OnComplete(() =>
            {
                fire1CanvasGroup.DOFade(0f, fadeDuration);
                MoveFire2();
            });
        });
    }

    private void MoveFire2()
    {
        Vector3 destination1 = new Vector3(81, -631.76f, 0);
        Vector3 destination2 = new Vector3(482.95f, -631.7594f, 0);
        fire2CanvasGroup.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            fire2.transform.DOLocalMove(destination1, moveSpeed * 1.5f).SetSpeedBased().SetEase(Ease.InOutSine).OnComplete(() =>
            {
                fire2.transform.DOLocalMove(destination2, moveSpeed * 1.5f).SetSpeedBased().SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    fire2CanvasGroup.DOFade(0f, fadeDuration);
                    MovePackage();
                });
            });
        });
    }

    private void MovePackage()
    {
        Vector3 destination1 = new Vector3(0, -631.7594f, 0);
        Vector3 destination2 = new Vector3(-483f, -631.7594f, 0);
        packageCanvasGroup.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            package.transform.DOLocalMove(destination1, moveSpeed).SetSpeedBased().SetEase(Ease.InOutSine).OnComplete(() =>
            {
                package.transform.DOLocalMove(destination2, moveSpeed).SetSpeedBased().SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    packageCanvasGroup.DOFade(0f, fadeDuration);
                    StartAnimation();
                });
            });
        });
    }

    private void OnEnable()
    {
        if (!initialized) return;
        StartAnimation();
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

    public void Cleanup()
    {
        DOTween.Kill(fire1);
        DOTween.Kill(fire2);
        DOTween.Kill(package);
        DOTween.Kill(fire1CanvasGroup);
        DOTween.Kill(fire2CanvasGroup);
        DOTween.Kill(packageCanvasGroup);

        InitPositions();
    }
}
