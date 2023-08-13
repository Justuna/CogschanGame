using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(AspectRatioFitter))]
[RequireComponent(typeof(Image))]
public class ImageAspectRatioFitter : MonoBehaviour
{
    private void LateUpdate()
    {
        GetComponent<AspectRatioFitter>().aspectRatio = (float)GetComponent<Image>().sprite.texture.width / GetComponent<Image>().sprite.texture.height;
    }
}
