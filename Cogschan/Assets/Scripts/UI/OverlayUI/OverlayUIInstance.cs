using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class OverlayUIInstance : MonoBehaviour
{
    [field: Header("Settings")]
    [field: SerializeField]
    public Vector3 WorldPosition { get; set; }
    [field: Tooltip("The worldspace offset of this UI instance relative to it's WorldPosition when the UI is visible within the screen.")]
    [field: SerializeField]
    public Vector3 VisibleWorldPositionOffset { get; set; }
    [field: Tooltip("The screenspace offset of this UI instance relative to it's WorldPosition when the UI is visible within the screen.")]
    [field: SerializeField]
    public Vector2 VisibleScreenPositionOffset { get; set; }
    [field: SerializeField]
    public bool WindowClamped { get; set; }
    [field: SerializeField]
    public bool HideWhenVisible { get; set; }

    [field: Header("Debug")]
    [field: ReadOnly]
    [field: SerializeField]
    public bool IsOffscreen { get; set; }
    [field: ReadOnly]
    [field: SerializeField]
    public float AngleToWorldPosition { get; set; }

    // NOTE: 
    // Movement of the OverlayUIInstance
    // is handled in OverlayUI
}