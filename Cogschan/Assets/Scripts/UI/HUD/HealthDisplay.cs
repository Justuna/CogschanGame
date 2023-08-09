using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
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
    [SerializeField] private float _healthbarTweenDuration = 0.1f;
    [Range(0, 1f)]
    [SerializeField] private float _damagedPortraitCutoff = 0.5f;
    [SerializeField] private Animator _portraitAnimator;

    private bool _isHurting;
    private OneShotTask _animateHealthBarTask;
    private float _prevPercentage = 1f;

    public void Construct(EntityServiceLocator services)
    {
        _services = services;
        _services.HealthTracker.OnHealed += OnHealed;
        _services.HealthTracker.OnDamaged += OnDamaged;
        _services.HealthTracker.OnDefeat += OnDefeat;
        _prevPercentage = (float)_services.HealthTracker.Health / _services.HealthTracker.MaxHealth;
        UpdateBar(_prevPercentage);
    }

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
        _portraitAnimator.SetBool("dead", true);
    }

    private void Start()
    {
        _animateHealthBarTask = new OneShotTask(async (token) =>
        {
            var targetPercentage = (float)_services.HealthTracker.Health / _services.HealthTracker.MaxHealth;

            _portraitAnimator.SetBool("damaged", targetPercentage <= _damagedPortraitCutoff);
            if (_isHurting)
                _portraitAnimator.SetTrigger("wince");

            _healthText.text = _services.HealthTracker.Health.ToString();
            _maxHealthText.text = " / " + _services.HealthTracker.MaxHealth;

            await DOVirtual.Float(_prevPercentage, targetPercentage, _healthbarTweenDuration, UpdateBar).SetEase(Ease.OutQuad).WithCancellation(token);

            _prevPercentage = targetPercentage;
        });
    }

    private void UpdateBar(float percentage)
    {
        _fill.color = _fillColor.Evaluate(percentage);
        _fill.rectTransform.sizeDelta = new Vector2(percentage * _background.rectTransform.sizeDelta.x, _fill.rectTransform.sizeDelta.y);
    }
}