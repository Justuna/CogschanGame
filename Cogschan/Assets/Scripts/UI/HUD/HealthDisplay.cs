using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class HealthDisplay : MonoBehaviour
{
    [Header("Health Bar")]
    [Tooltip("The EntityServiceLocator of the entity to display health for.")]
    [SerializeField] private EntityServiceLocator _services;
    [Tooltip("The image being used for the fill that animates between values.")]
    [SerializeField] private Image _changeFill;
    [Tooltip("The image being used for the immediate fill. This fill is not animated.")]
    [SerializeField] private Image _immediateFill;
    [Tooltip("The image being used for the background.")]
    [SerializeField] private Image _background;
    [Tooltip("An optional text label that displays the current health as a number.")]
    [SerializeField] private TextMeshProUGUI _healthText;
    [Tooltip("An optional text label that displays the max health as a number.")]
    [SerializeField] private TextMeshProUGUI _maxHealthText;
    [Tooltip("The color of the healthbar that animated between values.")]
    [SerializeField] private Color _changeFillColor;
    [Tooltip("The color of the healthbar used for the immediate fill. This fill is not animated.")]
    [SerializeField] private Color _immediateFillColor;
    [Tooltip("Duration of health bar tween animation")]
    [SerializeField] private float _healthbarTweenDuration = 0.1f;
    [SerializeField] private bool _hideWhenNoChange = false;
    [ShowIf(nameof(_hideWhenNoChange))]
    [SerializeField] private float _hideAfterNoChangeDuration = 5f;
    [Header("Portrait")]
    [Tooltip("Cutoff to show damaged portrait")]
    [Range(0, 1f)]
    [SerializeField] private float _damagedPortraitCutoff = 0.5f;
    [Tooltip("Animator for the portrait")]
    [SerializeField] private Animator _portraitAnimator;

    private enum HealthChangeReason
    {
        Damaged,
        Healed,
        Reset,
    }

    private HealthChangeReason _healthChangeReason;
    private OneShotTask _animateHealthBarTask;
    private float _prevPercentage = 1f;
    private OneShotTask _hideAfterNoChangeTask;
    private RectTransform _fillBarParent;

    public Color ChangeFillColor { get => _changeFillColor; set => _changeFillColor = value; }
    public Color ImmediateFillColor { get => _immediateFillColor; set => _immediateFillColor = value; }

    public void Init(EntityServiceLocator services)
    {
        _services = services;
        _services.HealthTracker.OnHealthReset.AddListener(OnHealthReset);
        _services.HealthTracker.OnHealed.AddListener(OnHealed);
        _services.HealthTracker.OnDamaged.AddListener(OnDamaged);
        _services.HealthTracker.OnDefeat.AddListener(OnDefeat);
        _prevPercentage = (float)_services.HealthTracker.Health / _services.HealthTracker.MaxHealth;

        _animateHealthBarTask = new OneShotTask(async (token) =>
        {
            var targetPercentage = (float)_services.HealthTracker.Health / _services.HealthTracker.MaxHealth;

            if (_portraitAnimator != null)
            {
                _portraitAnimator?.SetBool("damaged", targetPercentage <= _damagedPortraitCutoff);
                if (_healthChangeReason == HealthChangeReason.Damaged)
                    _portraitAnimator?.SetTrigger("wince");
            }

            if (_healthText != null)
                _healthText.text = _services.HealthTracker.Health.ToString();
            if (_maxHealthText != null)
                _maxHealthText.text = " / " + _services.HealthTracker.MaxHealth;

            UpdateImmediateFillBar(targetPercentage);
            if (_changeFill != null)
                await DOVirtual.Float(_prevPercentage, targetPercentage, _healthbarTweenDuration, UpdateChangeFillBar).SetEase(Ease.OutQuad).WithCancellation(token);

            _prevPercentage = targetPercentage;
        });
        _hideAfterNoChangeTask = new OneShotTask(async (token) =>
        {
            await UniTask.WaitForSeconds(_hideAfterNoChangeDuration, cancellationToken: token);
            _background.gameObject.SetActive(false);
        });

        if (_hideWhenNoChange)
            Hide();

        if (_changeFill != null)
            _fillBarParent = _changeFill.rectTransform.parent.GetComponent<RectTransform>();
        else if (_immediateFill != null)
            _fillBarParent = _immediateFill.rectTransform.parent.GetComponent<RectTransform>();
        UpdateImmediateFillBar(_prevPercentage);
        UpdateChangeFillBar(_prevPercentage);
    }

    private void Hide()
    {
        _background.gameObject.SetActive(false);
    }

    private void Show()
    {
        _background.gameObject.SetActive(true);
    }

    private void OnHealthReset()
    {
        _healthChangeReason = HealthChangeReason.Reset;
        _hideAfterNoChangeTask.Run();
        Show();
        _animateHealthBarTask.Run();
    }

    private void OnHealed(float amount)
    {
        if (amount == 0) return;
        _hideAfterNoChangeTask.Run();
        Show();
        _healthChangeReason = HealthChangeReason.Healed;
        _animateHealthBarTask.Run();
    }

    private void OnDamaged(float amount)
    {
        if (amount == 0) return;
        _hideAfterNoChangeTask.Run();
        Show();
        _healthChangeReason = HealthChangeReason.Damaged;
        _animateHealthBarTask.Run();
    }

    private void OnDefeat()
    {
        if (_portraitAnimator != null)
            _portraitAnimator.SetBool("dead", true);
    }

    private void Start()
    {
        if (!Application.isPlaying) return;

        if (_services != null)
            Init(_services);
    }

    private void UpdateImmediateFillBar(float percentage)
    {
        if (_immediateFill == null) return;
        _immediateFill.color = _immediateFillColor;
        _immediateFill.rectTransform.sizeDelta = new Vector2(percentage * _fillBarParent.rect.size.x, 0);
    }

    private void UpdateChangeFillBar(float percentage)
    {
        if (_changeFill == null) return;
        _changeFill.color = _changeFillColor;
        _changeFill.rectTransform.sizeDelta = new Vector2(percentage * _fillBarParent.rect.size.x, 0);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Application.isEditor)
        {
            if (_immediateFill != null)
                _immediateFill.color = _immediateFillColor;
            if (_changeFill != null)
                _changeFill.color = _changeFillColor;
        }
    }
#endif

    private void OnDestroy()
    {
        _animateHealthBarTask?.Stop();
        _hideAfterNoChangeTask?.Stop();
    }
}