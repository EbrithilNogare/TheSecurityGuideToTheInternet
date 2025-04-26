using DG.Tweening;
using System;
using UnityEditor;
using UnityEngine;

public class DynamicSystemUI : MonoBehaviour
{
    public enum TweenType { Move, Scale, Rotate, Fade }
    public enum EaseType { Linear, EaseIn, EaseOut, EaseInOut, Elastic, Bounce }

    [Serializable]
    public class TweenSettings
    {
        public string tweenName = "New Tween";
        public bool autoStart = true;
        public TweenType tweenType = TweenType.Move;
        public EaseType easeType = EaseType.Linear;

        public float duration = 1f;
        public float delay = 0f;
        public float loopDelay = 0f;
        public bool loop = false;
        public LoopType loopType = LoopType.Restart;

        public Vector3 targetValue;
        public bool relative = false;

        [HideInInspector] public Tweener tweener;
        [HideInInspector] public Sequence sequence;
    }

    public TweenSettings[] tweenSettings = new TweenSettings[0];

    private CanvasGroup canvasGroup;

    private Vector3 defaultPosition;
    private Vector3 defaultRotation;
    private Vector3 defaultScale;
    private float defaultAlpha;
    private bool initialized = false;

    private void Start()
    {
        if (tweenSettings.Length == 0)
        {
            this.enabled = false;
            return; // die fast and save resources
        }

        defaultPosition = transform.localPosition;
        defaultRotation = transform.localEulerAngles;
        defaultScale = transform.localScale;
        canvasGroup = GetComponent<CanvasGroup>();
        defaultAlpha = canvasGroup != null ? canvasGroup.alpha : 1;
        initialized = true;
        AutoStartTweens();
    }

    private void OnEnable()
    {
        if (!initialized) return;
        AutoStartTweens();
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

    private void AutoStartTweens()
    {
        foreach (var tween in tweenSettings)
            if (tween.autoStart)
                StartTween(tween);
    }

    public void StartTweens()
    {
        foreach (var tween in tweenSettings)
            if (!tween.autoStart)
                StartTween(tween);
    }

    public void Cleanup()
    {
        foreach (var tween in tweenSettings)
        {
            StopTween(tween);
        }

        transform.localPosition = defaultPosition;
        transform.localEulerAngles = defaultRotation;
        transform.localScale = defaultScale;
        if (canvasGroup != null)
            canvasGroup.alpha = defaultAlpha;
    }

    public void StartTween(TweenSettings settings)
    {
        StopTween(settings);

        switch (settings.tweenType)
        {
            case TweenType.Move:
                StartSuperTween(settings, TweenType.Move);
                break;
            case TweenType.Scale:
                StartSuperTween(settings, TweenType.Scale);
                break;
            case TweenType.Rotate:
                StartSuperTween(settings, TweenType.Rotate);
                break;
            case TweenType.Fade:
                StartFadeTween(settings);
                break;
        }
    }

    public void StopTween(TweenSettings settings)
    {
        settings.tweener?.Kill();
        settings.sequence?.Kill();
    }

    private void StartSuperTween(TweenSettings settings, TweenType tweenType)
    {
        if (tweenType == TweenType.Fade && !canvasGroup)
        {
            canvasGroup = GetComponent<CanvasGroup>() != null ? GetComponent<CanvasGroup>() : gameObject.AddComponent<CanvasGroup>();
        }

        void tweenCreator()
        {
            settings.sequence = DOTween.Sequence()
            .Append(
                tweenType == TweenType.Move ? transform
                    .DOLocalMove(settings.relative ? transform.localPosition + settings.targetValue : settings.targetValue, settings.duration)
                    .SetEase(GetEase(settings.easeType)) :
                tweenType == TweenType.Scale ? transform
                    .DOScale(settings.relative ? transform.localScale + settings.targetValue : settings.targetValue, settings.duration)
                    .SetEase(GetEase(settings.easeType)) :
                tweenType == TweenType.Rotate ? transform
                    .DOLocalRotate(settings.relative ? transform.localEulerAngles + settings.targetValue : settings.targetValue, settings.duration, RotateMode.FastBeyond360)
                    .SetEase(GetEase(settings.easeType)) :
                canvasGroup.DOFade(settings.relative ? canvasGroup.alpha + settings.targetValue.x : settings.targetValue.x, settings.duration)
                    .SetEase(GetEase(settings.easeType))
                    .SetDelay(settings.delay)
                )
            .AppendInterval(settings.loopDelay)
            .SetLoops(settings.loop ? -1 : 0, settings.loopType);
        }
        ;

        if (settings.delay <= 0)
            tweenCreator();
        else
            settings.sequence = DOTween.Sequence()
                    .AppendInterval(settings.delay)
                    .OnComplete(tweenCreator);
    }

    private void StartFadeTween(TweenSettings settings)
    {
        if (!canvasGroup)
        {
            canvasGroup = GetComponent<CanvasGroup>() != null ? GetComponent<CanvasGroup>() : gameObject.AddComponent<CanvasGroup>();
        }

        settings.tweener = canvasGroup.DOFade(settings.relative ? canvasGroup.alpha + settings.targetValue.x : settings.targetValue.x, settings.duration)
            .SetEase(GetEase(settings.easeType))
            .SetDelay(settings.delay)
            .SetLoops(settings.loop ? -1 : 0, settings.loopType);
    }

    private Ease GetEase(EaseType ease)
    {
        return ease switch
        {
            EaseType.Linear => Ease.Linear,
            EaseType.EaseIn => Ease.InSine,
            EaseType.EaseOut => Ease.OutSine,
            EaseType.EaseInOut => Ease.InOutSine,
            EaseType.Elastic => Ease.OutElastic,
            EaseType.Bounce => Ease.OutBounce,
            _ => Ease.Linear,
        };
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DynamicSystemUI))]
public class DynamicSystemUIEditor : Editor
{
    private SerializedProperty tweenSettings;

    private void OnEnable()
    {
        tweenSettings = serializedObject.FindProperty("tweenSettings");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(tweenSettings, true);

        if (GUILayout.Button("Start All Tweens"))
        {
            if (Application.isPlaying)
            {
                var script = (DynamicSystemUI)target;
                foreach (var tween in script.tweenSettings)
                {
                    script.StartTween(tween);
                }
            }
            else
            {
                Debug.LogWarning("StartTween can only be called in Play Mode.");
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
