using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static GameManagerScript Instance;

    [Header("Boolean flags")]
    public bool IsGamePlayOn;

    private TextMeshProUGUI _scoreText;
    private int _scoreCount;

    private int _health;
    [SerializeField] Button _goRestartButton;
    [SerializeField] Transform _healthPanel;

    private void Awake()
    {
        Instance = Instance == null ? this : Instance;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

        _scoreText = GameObject.Find("ScorePanelText").GetComponent<TextMeshProUGUI>();
        _goRestartButton = GameObject.Find("GO_Restart_Button").GetComponent<Button>();
        _healthPanel = GameObject.Find("Health_Panel").transform;
    }

    private void Start()
    {
        _goRestartButton.onClick.AddListener(() => { OnGameRestartBtn(); });

        IsGamePlayOn = false;
        _scoreCount = 0;
        _scoreText.text = "Score: " + _scoreCount;

        _health = 3;
    }

    public void OnCoinCollect()
    {
        _scoreCount++;
        _scoreText.text = "Score: " + _scoreCount;
    }

    public void OnSpikeHit()
    {
        _healthPanel.GetComponent<HorizontalLayoutGroup>().enabled = false;
        _health--;
        _healthPanel.GetChild(_health).gameObject.SetActive(false);
        PlayerScript.Instance.OnPlayerBlink();
        if (_health <= 0)
        {
            OnGameOver();
            IsGamePlayOn = false;
        }
    }

    public void OnGameOver()
    {
        IsGamePlayOn = false;
        GameMenuScript.Instance.GameOverPanel.SetActive(true);
        GameObject.Find("Player").GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        FallingSpikeScript.Instance.OnGameOver();
        PlatformScript.Instance.OnGameOver();
        CoinsScript.Instance.OnGameOver();
    }

    private void OnGameRestartBtn()
    {
        _scoreCount = 0;
        _scoreText.text = "Score: " + _scoreCount;
        _health = 3;

        foreach (Transform t in _healthPanel.transform)
        {
            t.gameObject.SetActive(true);
        }

        for (int i = 0; i < PlatformScript.Instance.Platforms.Count; i++)
        {
            PlatformScript.Instance.Platforms[i].gameObject.SetActive(true);
            PlatformScript.Instance.Platforms[i].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }

        foreach (Transform t in FallingSpikeScript.Instance.TopSpikesPool)
        {
            t.gameObject.SetActive(false);
        }

        foreach(Transform t in CoinsScript.Instance.CoinsPoolTransform)
        {
            t.gameObject.SetActive(false);
        }

        GameMenuScript.Instance.CountDownPanel.SetActive(true);
        NewGameCountDownScript.Instance.OnCountDownStart();
        GameMenuScript.Instance.GameOverPanel.SetActive(false);
    }
}
