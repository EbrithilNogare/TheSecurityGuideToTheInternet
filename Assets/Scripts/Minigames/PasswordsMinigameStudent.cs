using DG.Tweening;
using UnityEngine;

public class PasswordsMinigameStudent : MonoBehaviour
{
    [Header("Per instance")]
    public Transform player;
    public bool isWalker;
    public Transform[] WalkerPoints;

    [Header("Private")]
    public Transform coneOfView;
    public Transform person;

    private float turningSpeed = 30f;
    private float walkingSpeed = 350f;
    private bool isWalking = false;
    private float lookAtDefault;
    private float lookLengthDefault;
    private float timeOfOneStep = .2f;
    private float lastStepTime;
    private int animationStep = 0;

    void Start()
    {
        lookAtDefault = coneOfView.rotation.eulerAngles.z;

        lookLengthDefault = coneOfView.localScale.y;

        PlayAnimation();
    }

    private void PlayAnimation()
    {
        animationStep++;
        if (isWalker)
            switch (animationStep)
            {
                case 1:
                    transform.position = WalkerPoints[0].position;
                    GoToLocation(WalkerPoints[1].position).OnComplete(PlayAnimation);
                    break;
                case 2:
                    isWalking = false;
                    LookAtPlayer();
                    break;
                case 3:
                    isWalking = true;
                    GoToLocation(WalkerPoints[2].position, true).OnUpdate(() =>
                    {
                        if (coneOfView == null || !coneOfView.gameObject.activeSelf) return;
                        coneOfView.rotation = Quaternion.Euler(0, 0, GetAngleToPlayer());
                    }).OnComplete(PlayAnimation);
                    break;
                case 4:
                    LookBack(false);
                    GoToLocation(WalkerPoints[3].position).OnComplete(PlayAnimation);
                    break;
                default: animationStep = 0; PlayAnimation(); break;
            }
        else
            switch (animationStep)
            {
                case 1: DOVirtual.DelayedCall(Random.Range(.5f, 5f), PlayAnimation); break;
                case 2: coneOfView.DORotate(new Vector3(0, 0, Mathf.Lerp(lookAtDefault, GetAngleToPlayer(), 0.2f)), 1.5f).SetEase(Ease.InOutSine).OnComplete(PlayAnimation); break;
                case 3: LookBack(true); break;
                case 4:
                    coneOfView.DOScale(new Vector3(coneOfView.localScale.x * 2f, 6.5f, coneOfView.localScale.z), 2f).SetEase(Ease.InQuint);
                    LookAtPlayer();
                    break;
                case 5:
                    coneOfView.DOScale(new Vector3(coneOfView.localScale.x / 2f, lookLengthDefault, coneOfView.localScale.z), 2f).SetEase(Ease.OutQuint);
                    LookBack(true);
                    break;
                default: animationStep = 0; PlayAnimation(); break;
            }
    }

    void OnDisable()
    {
        DOTween.Kill(this);
        DOTween.Kill(coneOfView);
    }

    void Update()
    {
        WalkingAnimation();
    }

    private void WalkingAnimation()
    {
        if (isWalking)
            lastStepTime += Time.deltaTime;

        if (lastStepTime > timeOfOneStep)
        {
            lastStepTime -= timeOfOneStep;
            person.localScale = new Vector3(-person.localScale.x, person.localScale.y, person.localScale.z);
        }
    }

    private float GetAngleToPlayer()
    {
        return Mathf.Atan2(player.position.x - transform.position.x, player.position.y - transform.position.y) * Mathf.Rad2Deg * -1 + 180 % 360;
    }

    private void LookAtPlayer()
    {
        float endAngle = GetAngleToPlayer();
        coneOfView.DOLocalRotate(new Vector3(0, 0, endAngle), turningSpeed).SetEase(Ease.InOutCubic).SetSpeedBased(true).OnComplete(PlayAnimation);
    }

    private void LookBack(bool hasContinue)
    {
        float endAngle = lookAtDefault;
        coneOfView.DORotate(new Vector3(0, 0, endAngle), turningSpeed * 2f).SetEase(Ease.InOutCubic).SetSpeedBased(true).OnComplete(hasContinue ? PlayAnimation : () => { });
    }

    private Tween GoToLocation(Vector3 to, bool slowMotion = false)
    {
        return transform.DOMove(to, slowMotion ? walkingSpeed * .85f : walkingSpeed).SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(() => isWalking = false).OnStart(() => isWalking = true);
    }
}
