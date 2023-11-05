using UnityEngine;

public class SpikePrefabScript : MonoBehaviour
{
    [Header("Prefab Components")]
    private RectTransform _rectTransf;

    [Header("Prefab Properties")]
    private float canvasHeight;


    private void Start()
    {
        // Get a reference to the RectTransform.
        _rectTransf = GetComponent<RectTransform>();

        // Get the padding and canvas height from the PopTheBubblePanelScript.
        canvasHeight = GameObject.Find("Main_Canvas").GetComponent<RectTransform>().rect.height;
    }

    private void Update()
    {
        // Check if the image is above the screen and destroy it if it is.
        float yPos = _rectTransf.localPosition.y;
        if (Mathf.Abs(yPos) >= canvasHeight)
            gameObject.SetActive(false);
    }
}
