using System.Collections;
using TMPro;
using UnityEngine;

public class NewGameCountDownScript : MonoBehaviour
{
    [Header("Singleton Reference")]
    public static NewGameCountDownScript Instance;

    [SerializeField] TextMeshProUGUI _countDownText;

    private void Awake()
    {
        Instance = Instance == null ? this : Instance;  // Setting Singleton Instance
        if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
        DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

        _countDownText = GameObject.Find("Count_Down_Text").GetComponent<TextMeshProUGUI>();
    }

    public void OnCountDownStart()
    {
        StartCoroutine(CountDownTimer());
    }

    public IEnumerator CountDownTimer()
    {
        _countDownText.text = "3";
        yield return new WaitForSeconds(1);
        _countDownText.text = "2";
        yield return new WaitForSeconds(1);
        _countDownText.text = "1";
        yield return new WaitForSeconds(1);
        _countDownText.text = "START";
        yield return new WaitForSeconds(1);
        PlayerScript.Instance.OnNewGameInitialize();
        GameMenuScript.Instance.CountDownPanel.SetActive(false);
        GameManagerScript.Instance.IsGamePlayOn = true;
        PlatformScript.Instance.TurnOffLayoutGroup();
        FallingSpikeScript.Instance.OnStartFallingSpikes();
        CoinsScript.Instance.OnStartCoinsSystem();
    }
}
