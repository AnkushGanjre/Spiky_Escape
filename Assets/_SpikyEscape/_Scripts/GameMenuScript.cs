using UnityEngine;
using UnityEngine.UI;

namespace DonzaiGamecorp.SpikyEscape
{
    public class GameMenuScript : MonoBehaviour
    {
        [Header("Singleton Reference")]
        public static GameMenuScript Instance;

        [Header("Game Panels")]
        [SerializeField] GameObject _introPanel;
        [SerializeField] GameObject _gamePanel;
        public GameObject GameOverPanel;
        public GameObject CountDownPanel;

        [Header("Play Quit Button")]
        [SerializeField] Button _playButton;
        [SerializeField] Button _quitButton;
        [SerializeField] Button _goQuitButton;

        [Header("Images")]
        [SerializeField] Image _backgroundImage;

        private void Awake()
        {
            Instance = Instance == null ? this : Instance;  // Setting Singleton Instance
            if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
            DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

            _introPanel = GameObject.Find("Into_Panel");
            _gamePanel = GameObject.Find("Game_Panel");
            GameOverPanel = GameObject.Find("Game_Over_Panel");
            CountDownPanel = GameObject.Find("Count_Down_Panel");

            _playButton = GameObject.Find("Play_Button").GetComponent<Button>();
            _quitButton = GameObject.Find("Quit_Button").GetComponent<Button>();
            _goQuitButton = GameObject.Find("GO_Quit_Button").GetComponent<Button>();

            _backgroundImage = GameObject.Find("BackGround_Image").GetComponent<Image>();
        }


        void Start()
        {
            _playButton.onClick.AddListener(() => { OnPlayBtnClicked(); });
            _quitButton.onClick.AddListener(() => { OnQuitBtnClicked(); });
            _goQuitButton.onClick.AddListener(() => { OnQuitBtnClicked(); });

            _gamePanel?.SetActive(false);
            GameOverPanel?.SetActive(false);
            CountDownPanel?.SetActive(false);

            _backgroundImage.color = new Color32(255, 255, 255, 255);
        }

        private void OnPlayBtnClicked()
        {
            _introPanel?.SetActive(false);
            _gamePanel?.SetActive(true);
            CountDownPanel?.SetActive(true);
            _backgroundImage.color = new Color32(255, 255, 255, 75);
            NewGameCountDownScript.Instance.OnCountDownStart();
        }

        private void OnQuitBtnClicked()
        {
            Application.Quit();
        }
    }
}

