using System.Collections.Generic;
using UnityEngine;

public class FallingSpikeScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static FallingSpikeScript Instance;

    public Transform TopSpikesPool;

    private List<GameObject> _spikesPool;
    public GameObject _spikePrefab; // The GameObject to instantiate

    private float _canvasWidth;
    private float _padding = 50f;
    private int _initialPoolSize = 10;
    private float downwardForce = 200f;

    private void Awake()
    {
        Instance = Instance == null ? this : Instance;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

        TopSpikesPool = GameObject.Find("Top_Spikes_Pool").transform;
    }

    private void Start()
    {
        // Get a reference to the canvas and its dimensions
        RectTransform CanvasRect = GameObject.Find("Main_Canvas").GetComponent<RectTransform>();
        _canvasWidth = CanvasRect.rect.width;
        _spikePrefab = Resources.Load<GameObject>("SpikePrefab");
        CreateObjectPool();
    }

    public void OnStartFallingSpikes()
    {
        InvokeRepeating(nameof(SpawnObjectAtTop), 0f, 3f);
    }

    private void SpawnObjectAtTop()
    {
        // Set a random X position within the canvas width
        float randomX = Random.Range(_padding, (_canvasWidth-_padding));

        // Set the Y position at the Top
        float randomY = 175f;       // Value in anchor position

        // Create an instance of the image prefab
        GameObject spike = GetObjectFromPool();

        // Set the position of the instantiated image
        RectTransform imageRect = spike.GetComponent<RectTransform>();
        imageRect.anchoredPosition = new Vector2(randomX, randomY);
        Rigidbody2D rb = spike.GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.down * downwardForce, ForceMode2D.Impulse);
    }

    private void CreateObjectPool()
    {
        _spikesPool = new List<GameObject>();
        for (int i = 0; i < _initialPoolSize; i++)
        {
            GameObject obj = Instantiate(_spikePrefab, TopSpikesPool);
            obj.SetActive(false);
            _spikesPool.Add(obj);
        }
    }

    private GameObject GetObjectFromPool()
    {
        foreach (Transform t in TopSpikesPool)
        {
            if (!t.gameObject.activeInHierarchy)
            {
                t.gameObject.SetActive(true);
                return t.gameObject;
            }
        }
        return null;
    }

    public void OnGameOver()
    {
        CancelInvoke(nameof(SpawnObjectAtTop));
    }
}
