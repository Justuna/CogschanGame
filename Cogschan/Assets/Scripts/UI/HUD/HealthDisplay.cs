using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class HealthDisplay : MonoBehaviour
{
    public Gradient FillColor { get => _fillColor; set => _fillColor = value; }

    [Header("Health Bar")]
    [Tooltip("The EntityServiceLocator of the entity to display health for.")]
    [SerializeField] private EntityServiceLocator _services;
    [Tooltip("The image being used for the fill.")]
    [SerializeField] private Image _fill;
    [Tooltip("The image being used for the background.")]
    [SerializeField] private Image _background;
    [Tooltip("An optional text label that displays the current health as a number.")]
    [SerializeField] private TextMeshProUGUI _healthText;
    [Tooltip("An optional text label that displays the max health as a number.")]
    [SerializeField] private TextMeshProUGUI _maxHealthText;
    [Tooltip("The color of the healthbar depending on the current health. Left is dead, right is full health.")]
    [SerializeField] private Gradient _fillColor;
    [Tooltip("Duration of health bar tween animation")]
    [SerializeField] private float _healthbarTweenDuration = 0.1f;
    [Header("Portrait")]
    [Tooltip("Cutoff to show damaged portrait")]
    [Range(0, 1f)]
    [SerializeField] private float _damagedPortraitCutoff = 0.5f;
    [Tooltip("Animator for the portrait")]
    [SerializeField] private Animator _portraitAnimator;

    private bool _isHurting;
    private OneShotTask _animateHealthBarTask;
    private float _prevPercentage = 1f;

    public void Init(EntityServiceLocator services)
    {
        _services = services;
        _services.HealthTracker.OnHealed += OnHealed;
        _services.HealthTracker.OnDamaged += OnDamaged;
        _services.HealthTracker.OnDefeat += OnDefeat;
        _prevPercentage = (float)_services.HealthTracker.Health / _services.HealthTracker.MaxHealth;
        UpdateBar(_prevPercentage);
    }

    public void SetSingleFillColor(Color color)
    {
        SetGradientFillColor(new Gradient()
        {
            alphaKeys = new GradientAlphaKey[] { new GradientAlphaKey(color.a, 0) },
            colorKeys = new GradientColorKey[] { new GradientColorKey(color, 0) },
            mode = GradientMode.Fixed
        });
    }

    public void SetGradientFillColor(Gradient gradient) => FillColor = gradient;

    private void OnHealed(float amount)
    {
        if (amount == 0) return;
        _isHurting = false;
        _animateHealthBarTask.Run();
    }

    private void OnDamaged(float amount)
    {
        if (amount == 0) return;
        _isHurting = true;
        _animateHealthBarTask.Run();
    }

    private void OnDefeat()
    {
        _portraitAnimator?.SetBool("dead", true);
    }

    private void Start()
    {
        if (Application.isEditor) return;

        _animateHealthBarTask = new OneShotTask(async (token) =>
        {
            var targetPercentage = (float)_services.HealthTracker.Health / _services.HealthTracker.MaxHealth;

            _portraitAnimator?.SetBool("damaged", targetPercentage <= _damagedPortraitCutoff);
            if (_isHurting)
                _portraitAnimator?.SetTrigger("wince");

            _healthText.text = _services.HealthTracker.Health.ToString();
            _maxHealthText.text = " / " + _services.HealthTracker.MaxHealth;

            await DOVirtual.Float(_prevPercentage, targetPercentage, _healthbarTweenDuration, UpdateBar).SetEase(Ease.OutQuad).WithCancellation(token);

            _prevPercentage = targetPercentage;
        });
    }

    private void UpdateBar(float percentage)
    {
        _fill.color = FillColor.Evaluate(percentage);
        _fill.rectTransform.sizeDelta = new Vector2(percentage * _background.rectTransform.sizeDelta.x, _fill.rectTransform.sizeDelta.y);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Application.isEditor)
            _fill.color = _fillColor.Evaluate(1f);
    }
#endif
}