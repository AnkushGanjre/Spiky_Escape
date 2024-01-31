using System.Collections.Generic;
using UnityEngine;

namespace DonzaiGamecorp.SpikyEscape
{
    public class CoinsScript : MonoBehaviour
    {
        [Header("Singleton Reference")]
        public static CoinsScript Instance;

        public Transform CoinsPoolTransform;

        private List<GameObject> _coinsPoolList;
        private int _coinPoolSize = 5;
        public GameObject _coinsPrefab; // The GameObject to instantiate

        private void Awake()
        {
            Instance = Instance == null ? this : Instance;  // Setting Singleton Instance
            if (Instance != this) Destroy(gameObject);  // If not Active Singleton, destroy it
            DontDestroyOnLoad(gameObject);  // Ensure that the Singleton persists across scene changes

            CoinsPoolTransform = GameObject.Find("Coins_Pool").transform;
        }

        private void Start()
        {
            _coinsPrefab = Resources.Load<GameObject>("CoinPrefab");
            CreateObjectPool();
        }

        public void OnStartCoinsSystem()
        {
            InvokeRepeating(nameof(SpawnCoins), 5f, 5f);
        }

        private void SpawnCoins()
        {
            // If more than 2 coins are active in herarchy the return
            int activeCoins = GetActiveCoins();
            if (activeCoins >= 2)
            {
                return;
            }

            // Set a random X position within the canvas width
            int randomIndex = Random.Range(0, PlatformScript.Instance.Platforms.Count);
            GameObject coinsSpawningPlatform = PlatformScript.Instance.Platforms[randomIndex];

            if (coinsSpawningPlatform == PlatformScript.Instance.CurrentPlatform)
            {
                // If it is the player's object, select another random object
                SpawnCoins();
                return;
            }

            Vector2 coinPos = PlatformScript.Instance.Platforms[randomIndex].transform.position;
            coinPos.y += 80f;       // So that it spawn above platform

            // Create an instance of the image prefab
            GameObject coin = GetObjectFromPool();

            // Set the position of the instantiated image
            RectTransform imageRect = coin.GetComponent<RectTransform>();
            imageRect.position = coinPos;

            // Rotate the coin
        }

        private void CreateObjectPool()
        {
            _coinsPoolList = new List<GameObject>();
            for (int i = 0; i < _coinPoolSize; i++)
            {
                GameObject obj = Instantiate(_coinsPrefab, CoinsPoolTransform);
                obj.SetActive(false);
                _coinsPoolList.Add(obj);
            }
        }

        private GameObject GetObjectFromPool()
        {
            foreach (Transform t in CoinsPoolTransform)
            {
                if (!t.gameObject.activeInHierarchy)
                {
                    t.gameObject.SetActive(true);
                    return t.gameObject;
                }
            }
            return null;
        }

        private int GetActiveCoins()
        {
            int activeCoins = 0;
            foreach (Transform t in CoinsPoolTransform)
            {
                if (t.gameObject.activeInHierarchy)
                {
                    activeCoins++;
                }
            }
            return activeCoins;
        }

        public void OnGameOver()
        {
            CancelInvoke(nameof(SpawnCoins));
        }
    }
}

