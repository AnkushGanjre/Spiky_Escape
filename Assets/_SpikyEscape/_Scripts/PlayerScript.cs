using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DonzaiGamecorp.SpikyEscape
{
    public class PlayerScript : MonoBehaviour
    {
        [Header("Singleton Reference")]
        public static PlayerScript Instance;

        [Header("Componenets")]
        [SerializeField] RectTransform _playerRectTransform;
        private float _jumpDuration = 1f; // Adjust this for your desired jump duration.

        private Vector2 touchStartPos;
        private Vector2 touchEndPos;
        private bool isSwiping = false;
        private float swipeDistanceThreshold = 50f; // Adjust this threshold as needed.

        private void Awake()
        {
            Instance = Instance == null ? this : Instance;  // Setting Singleton Instance

            _playerRectTransform = GetComponent<RectTransform>();
        }
        void Update()
        {
            if (GameManagerScript.Instance.IsGamePlayOn)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            touchStartPos = touch.position;
                            isSwiping = true;
                            break;

                        case TouchPhase.Moved:
                            // Determine the swipe direction
                            touchEndPos = touch.position;
                            float swipeDistance = Vector2.Distance(touchEndPos, touchStartPos);

                            if (isSwiping && swipeDistance > swipeDistanceThreshold)
                            {
                                float swipeDirection = Mathf.Sign(touchEndPos.x - touchStartPos.x);

                                if (swipeDirection > 0) // Swipe right
                                {
                                    //Debug.Log("Swipe Right");

                                    int currentPlatformIndex = GetPlatformIndex();
                                    currentPlatformIndex++;
                                    int platformCount = PlatformScript.Instance.Platforms.Count;

                                    if (currentPlatformIndex < platformCount)
                                    {
                                        PlatformScript.Instance.CurrentPlatform = PlatformScript.Instance.Platforms[currentPlatformIndex];
                                    }

                                    StartCoroutine(Jump());
                                }
                                else if (swipeDirection < 0) // Swipe left
                                {
                                    //Debug.Log("Swipe Left");
                                    int currentPlatformIndex = GetPlatformIndex();
                                    currentPlatformIndex--;
                                    if (currentPlatformIndex >= 0)
                                    {
                                        PlatformScript.Instance.CurrentPlatform = PlatformScript.Instance.Platforms[currentPlatformIndex];
                                    }

                                    StartCoroutine(Jump());
                                }

                                isSwiping = false; // Reset the swiping flag
                            }
                            break;

                        case TouchPhase.Ended:
                            isSwiping = false;
                            break;
                    }
                }
                if (Input.GetKeyUp(KeyCode.A))
                {
                    // Going Left
                    int currentPlatformIndex = GetPlatformIndex();
                    currentPlatformIndex--;
                    if (currentPlatformIndex >= 0)
                    {
                        PlatformScript.Instance.CurrentPlatform = PlatformScript.Instance.Platforms[currentPlatformIndex];
                    }

                    StartCoroutine(Jump());
                }
                if (Input.GetKeyUp(KeyCode.D))
                {
                    int currentPlatformIndex = GetPlatformIndex();
                    currentPlatformIndex++;
                    int platformCount = PlatformScript.Instance.Platforms.Count;

                    if (currentPlatformIndex < platformCount)
                    {
                        PlatformScript.Instance.CurrentPlatform = PlatformScript.Instance.Platforms[currentPlatformIndex];
                    }

                    StartCoroutine(Jump());
                }
            }
            float pos = _playerRectTransform.localPosition.y;
            if (pos < -300)
                GameManagerScript.Instance.OnGameOver();
        }

        public void OnNewGameInitialize()
        {
            PlatformScript.Instance.CurrentPlatform = PlatformScript.Instance.Platforms[1].gameObject;
            Vector2 pos = PlatformScript.Instance.CurrentPlatform.transform.position;
            pos.y = pos.y + 45f;
            _playerRectTransform.position = pos;
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }

        private int GetPlatformIndex()
        {
            // getting the index of Current Platform;
            for (int i = 0; i < PlatformScript.Instance.Platforms.Count; i++)
            {
                if (PlatformScript.Instance.CurrentPlatform == PlatformScript.Instance.Platforms[i])
                {
                    return i;
                }
            }

            return -1;
        }

        private IEnumerator Jump()
        {
            GameManagerScript.Instance.IsGamePlayOn = false;

            Vector3 startPosition = _playerRectTransform.localPosition;
            Vector3 targetPosition = PlatformScript.Instance.CurrentPlatform.GetComponent<RectTransform>().localPosition;

            float journeyLength = Vector2.Distance(startPosition, targetPosition);

            float normalPos = 45f;              // Offset Value to keep player above Platform
            startPosition.y = normalPos;
            targetPosition.y = normalPos;
            float startTime = Time.time;


            while (Time.time - startTime < _jumpDuration)
            {
                float distanceCovered = (Time.time - startTime) * journeyLength / _jumpDuration;
                float fractionOfJourney = distanceCovered / journeyLength;
                float yOffset = 1 - Mathf.Pow(fractionOfJourney * 2 - 1, 2);

                Vector3 newPos = Vector2.Lerp(startPosition, targetPosition, fractionOfJourney);
                newPos.y += yOffset * 100;
                _playerRectTransform.anchoredPosition = newPos;

                yield return null;
            }

            if (PlatformScript.Instance.CurrentPlatform.activeInHierarchy)
            {
                GameManagerScript.Instance.IsGamePlayOn = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("FallingSpike"))
            {
                GameManagerScript.Instance.OnSpikeHit();
            }

            if (collision.CompareTag("Coins"))
            {
                collision.gameObject.SetActive(false);
                GameManagerScript.Instance.OnCoinCollect();
            }

            if (collision.CompareTag("BottomSpike"))
            {
                Debug.Log("Game Over");
                GameManagerScript.Instance.OnGameOver();
            }
        }

        public void OnPlayerBlink()
        {
            StartCoroutine(Blink());
        }

        private IEnumerator Blink()
        {
            Image playerimage = GetComponent<Image>();
            int blinkCount = 3;
            float blinkDuration = 0.5f;

            for (int i = 0; i < blinkCount; i++)
            {
                playerimage.color = new Color(playerimage.color.r, playerimage.color.g, playerimage.color.b, 0f);
                yield return new WaitForSeconds(blinkDuration / (2 * blinkCount));
                playerimage.color = new Color(playerimage.color.r, playerimage.color.g, playerimage.color.b, 1f);
                yield return new WaitForSeconds(blinkDuration / (2 * blinkCount));
            }
        }
    }
}

