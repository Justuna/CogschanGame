using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class FXController : MonoBehaviour
{
    [field: SerializeField]
    public UnityEvent OnPlay { get; private set; }

    [field: SerializeField]
    [field: Tooltip("Oneshot FX can only be played once.")]
    public bool IsOneshot { get; set; }
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

    [Button("Test Play")]
    private void EditorPlay()
    {
        OnPlay.Invoke();
    }

    public async void Play()
    {
        if (IsOneshot)
        {
            if (_played) return;
            _played = true;
        }

        if (IsStandalone && Application.isPlaying)
            transform.SetParent(StandaloneParent, true);

        OnPlay.Invoke();

        if (IsStandalone)
        {
            await UniTask.WaitForSeconds(Lifetime, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
            if (gameObject != null && Application.isPlaying)
                Destroy(gameObject);
        }
    }
}
