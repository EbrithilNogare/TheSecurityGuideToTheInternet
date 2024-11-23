using DG.Tweening;
using System;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
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
        public bool loop = false;
        public LoopType loopType = LoopType.Restart;

        public Vector3 targetValue;
        public bool relative = false;

        [HideInInspector] public Tweener tweener;
        [HideInInspector] public Vector3 originalValue;
    }

    public TweenSettings[] tweenSettings = new TweenSettings[0];

    private CanvasGroup canvasGroup;

    private void Start()
    {
        foreach (var tween in tweenSettings)
        {
            if (tween.autoStart) StartTween(tween);
        }
    }

    public void StartTween(TweenSettings settings)
    {
        StopTween(settings);

        switch (settings.tweenType)
        {
            case TweenType.Move:
                StartMoveTween(settings);
                break;
            case TweenType.Scale:
                StartScaleTween(settings);
                break;
            case TweenType.Rotate:
                StartRotateTween(settings);
                break;
            case TweenType.Fade:
                StartFadeTween(settings);
                break;
        }
    }

    public void StopTween(TweenSettings settings)
    {
        settings.tweener?.Kill();
    }

    private void StartMoveTween(TweenSettings settings)
    {
        settings.originalValue = transform.localPosition;
        if (settings.relative)
        {
            settings.tweener = transform.DOLocalMove(transform.localPosition + settings.targetValue, settings.duration)
                .SetEase(GetEase(settings.easeType))
                .SetDelay(settings.delay)
                .SetLoops(settings.loop ? -1 : 0, settings.loopType);
        }
        else
        {
            settings.tweener = transform.DOLocalMove(settings.targetValue, settings.duration)
                .SetEase(GetEase(settings.easeType))
                .SetDelay(settings.delay)
                .SetLoops(settings.loop ? -1 : 0, settings.loopType);
        }
    }

    private void StartScaleTween(TweenSettings settings)
    {
        settings.originalValue = transform.localScale;
        if (settings.relative)
        {
            settings.tweener = transform.DOScale(transform.localScale + settings.targetValue, settings.duration)
                .SetEase(GetEase(settings.easeType))
                .SetDelay(settings.delay)
                .SetLoops(settings.loop ? -1 : 0, settings.loopType);
        }
        else
        {
            settings.tweener = transform.DOScale(settings.targetValue, settings.duration)
                .SetEase(GetEase(settings.easeType))
                .SetDelay(settings.delay)
                .SetLoops(settings.loop ? -1 : 0, settings.loopType);
        }
    }

    private void StartRotateTween(TweenSettings settings)
    {
        settings.originalValue = transform.localEulerAngles;
        if (settings.relative)
        {
            settings.tweener = transform.DOLocalRotate(transform.localEulerAngles + settings.targetValue, settings.duration, RotateMode.FastBeyond360)
                .SetEase(GetEase(settings.easeType))
                .SetDelay(settings.delay)
                .SetLoops(settings.loop ? -1 : 0, settings.loopType);
        }
        else
        {
            settings.tweener = transform.DOLocalRotate(settings.targetValue, settings.duration, RotateMode.FastBeyond360)
                .SetEase(GetEase(settings.easeType))
                .SetDelay(settings.delay)
                .SetLoops(settings.loop ? -1 : 0, settings.loopType);
        }
    }


    private void StartFadeTween(TweenSettings settings)
    {
        if (!canvasGroup)
        {
            canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        }

        settings.originalValue = new Vector3(canvasGroup.alpha, 0, 0);
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

        if (GUILayout.Button("Stop All Tweens"))
        {
            if (Application.isPlaying)
            {
                var script = (DynamicSystemUI)target;
                foreach (var tween in script.tweenSettings)
                {
                    script.StopTween(tween);
                }
            }
            else
            {
                Debug.LogWarning("StopTween can only be called in Play Mode.");
            }
        }

        foreach (SerializedProperty setting in tweenSettings)
        {
            int index = int.Parse(setting.propertyPath.Split('[')[1].Trim(']'));
            var tween = ((DynamicSystemUI)target).tweenSettings.Length > index + 1 ? ((DynamicSystemUI)target).tweenSettings[index] : null;

            if (tween != null && GUILayout.Button($"Save Current As Target ({tween.tweenName})"))
            {
                SaveCurrentAsTarget(tween);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void SaveCurrentAsTarget(DynamicSystemUI.TweenSettings settings)
    {
        var script = (DynamicSystemUI)target;

        switch (settings.tweenType)
        {
            case DynamicSystemUI.TweenType.Move:
                settings.targetValue = script.transform.position;
                break;
            case DynamicSystemUI.TweenType.Scale:
                settings.targetValue = script.transform.localScale;
                break;
            case DynamicSystemUI.TweenType.Rotate:
                settings.targetValue = script.transform.eulerAngles;
                break;
            case DynamicSystemUI.TweenType.Fade:
                if (!script.GetComponent<CanvasGroup>())
                {
                    script.gameObject.AddComponent<CanvasGroup>();
                }
                settings.targetValue = new Vector3(script.GetComponent<CanvasGroup>().alpha, 0, 0);
                break;
        }

        Debug.Log($"Saved current value as target for '{settings.tweenName}'.");
    }
}
#endif
