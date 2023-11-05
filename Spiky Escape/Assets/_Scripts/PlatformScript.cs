using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static PlatformScript Instance;

    public GameObject CurrentPlatform;

    private Transform _platformParent;
    public List<GameObject> Platforms;
    private float _blinkDuration = 2f;
    private int _blinkCount = 5;
    private float _deactiveDuration = 5f;

    private void Awake()
    {
        Instance = Instance == null ? this : Instance;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

        _platformParent = GameObject.Find("Platform_Panel").transform;
    }
    
    void Start()
    {
        foreach (Transform t in _platformParent)
        {
            Platforms.Add(t.gameObject);
        }
    }

    public void TurnOffLayoutGroup()
    {
        _platformParent.GetComponent<HorizontalLayoutGroup>().enabled = false;
        StartCoroutine(OnPlatformDisable());
    }

    private IEnumerator OnPlatformDisable()
    {
        yield return new WaitForSeconds(_deactiveDuration);

        // Turning on disabled Platform
        foreach (Transform t in _platformParent)
        {
            t.gameObject.SetActive(true);
        }

        // Generate a random index to select a GameObject from the list
        int randomIndex = Random.Range(0, Platforms.Count);
        Image targetImage = Platforms[randomIndex].GetComponent<Image>();

        for (int i = 0; i < _blinkCount; i++)
        {
            targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, 0f);
            yield return new WaitForSeconds(_blinkDuration / (2 * _blinkCount));
            targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, 1f);
            yield return new WaitForSeconds(_blinkDuration / (2 * _blinkCount));
        }

        // Disable the randomly selected GameObject
        Platforms[randomIndex].SetActive(false);
        if (Platforms[randomIndex] == CurrentPlatform)
        {
            GameManagerScript.Instance.IsGamePlayOn = false;
        }
        StartCoroutine(OnPlatformDisable());
    }

    public void OnGameOver()
    {
        StopAllCoroutines();
    }
}
