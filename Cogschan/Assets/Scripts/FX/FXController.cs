using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class FXController : MonoBehaviour
{
    [field: SerializeField]
    public UnityEvent OnPlay { get; private set; }

    [field: SerializeField]
    [field: Tooltip("Standalone FX unparent themselves on play and destroy themselves after their lifetime has passed.")]
    public bool IsStandalone { get; set; }
    [field: ShowIf(nameof(IsStandalone))]
    [field: SerializeField]
    public float Lifetime { get; set; }
    [field: ShowIf(nameof(IsStandalone))]
    [field: SerializeField]
    public Transform StandaloneParent { get; set; }

    private bool _played;

    public async void Play()
    {
        if (_played) return;
        _played = true;

        if (IsStandalone)
            transform.SetParent(StandaloneParent);

        OnPlay.Invoke();

        if (IsStandalone)
        {
            await UniTask.WaitForSeconds(Lifetime);
            if (gameObject != null)
                Destroy(gameObject);
        }
    }
}
