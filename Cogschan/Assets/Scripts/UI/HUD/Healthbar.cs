using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
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
    [Tooltip("Whether or not the healthbar updates immediately. If this is off, the healthbar will try to smoothly glide between values instead.")]
    [SerializeField] private bool _isImmediate;
    [Tooltip("How fast to go between the previous health percentage to the current health percentage. If the healthbar is immediate, this does nothing.")]
    [SerializeField] private float _barVelocity;

    private float _percentage = 1;

    public void Construct(EntityServiceLocator services)
    {
        _services = services;
    }

    private void Update()
    {
        float truePercentage = _services.HealthTracker.Health / ((float)_services.HealthTracker.MaxHealth);
        if (_isImmediate)
        {
            _percentage = truePercentage;
        }
        else
        {
            int sign = (_percentage < truePercentage) ? 1 : -1;
            // Either change by a fixed delta/time, or by the amount remaining, whichever is smaller.
            _percentage += sign * Mathf.Min(_barVelocity * Time.deltaTime, Mathf.Abs(_percentage - truePercentage));
        }

        _fill.GetComponent<RectTransform>().sizeDelta =
            new Vector2(_background.GetComponent<RectTransform>().rect.width * _percentage, _background.GetComponent<RectTransform>().rect.height);
        _fill.color = _fillColor.Evaluate(_percentage);

        if (_healthText != null && _maxHealthText != null)
        {
            _healthText.text = _services.HealthTracker.Health.ToString();
            _maxHealthText.text = " / " + _services.HealthTracker.MaxHealth;
        }
    }
}