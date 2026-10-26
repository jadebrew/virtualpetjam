using System.Collections;
using System.Collections.Generic;
using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace MiniGames
{
    public class ReactionGame : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject reactionGameScene;
        [SerializeField] private RectTransform gameAreaContainer; // The panel bounding the game field
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Game Settings")]
        [SerializeField] private int laneCount = 4;        // Variable lanes! Change this in Inspector to test.
        [SerializeField] private float gameSpeed = 100f;   // Speed objects move left (pixels per second)
        [SerializeField] private float spawnRate = 1.5f;   // Seconds between object spawns
        [SerializeField] private int shampooBonus = 10;
        [SerializeField] private int dirtPenalty = 5;

        [Header("Sprites")]
        [SerializeField] private Sprite petSprite;
        [SerializeField] private Sprite foodSprite;
        [SerializeField] private Sprite inedibleSprite;

        [Header("Cat Reference")]
        [SerializeField] private RectTransform catRectTransform;

        private const float CatX = 5.0f;
        
        private const string DefaultStatus = "Eat the food!";
        private const string TriedToEatSomethingInedible = "Ew!";
        private const string AteSomething = "Yummy!";
        
        private bool _gameOver = false;
        private int _score = 0;
        private int _currentLane = 0;
        private float _spawnTimer = 0f;

        private int _hitCounter = 0;

        // Active obstacle/collectible items tracking
        private class ActiveItem
        {
            public RectTransform rectTransform;
            public int lane;
            public bool isEdible;
        }
        private List<ActiveItem> _activeItems = new();

        private float _laneHeight;

        public void Init()
        {
            _gameOver = false;
            reactionGameScene.SetActive(true);
            _score = 0;
            _currentLane = laneCount / 2; // Start in the middle lane
            _spawnTimer = 0f;
            
            _hitCounter = 0;

            // Clear old items if restarting
            foreach (var item in _activeItems)
            {
                if (item.rectTransform != null) Destroy(item.rectTransform.gameObject);
            }
            _activeItems.Clear();

            CalculateLaneDimensions();
            UpdateCatPosition();

            if (statusText) statusText.text = DefaultStatus;
            if (scoreText) scoreText.text = "Score: 0";
        }

        private void CalculateLaneDimensions()
        {
            if (gameAreaContainer == null) return;
            // Calculate height of a single lane based on container size and lane count
            _laneHeight = gameAreaContainer.rect.height / laneCount;
            
            // Adjust cat size to fit the square lane height
            if (catRectTransform != null)
            {
                catTypeSetup();
            }
        }

        private void catTypeSetup()
        {
            catRectTransform.sizeDelta = new Vector2(_laneHeight, _laneHeight);
            // Anchor cat to the left side
            catRectTransform.anchorMin = new Vector2(0f, 0.5f);
            catRectTransform.anchorMax = new Vector2(0f, 0.5f);
            catRectTransform.pivot = new Vector2(0f, 0.5f);
            catRectTransform.anchoredPosition = new Vector2(CatX, GetLaneYPosition(_currentLane));
            
            var img = catRectTransform.GetComponent<Image>();
            if (img && petSprite) img.sprite = petSprite;
        }

        private float GetLaneYPosition(int laneIndex)
        {
            // Calculate vertical center/offset for the given lane index from bottom-up
            float totalHeight = gameAreaContainer.rect.height;
            float startY = -totalHeight / 2f + _laneHeight / 2f;
            return startY + (laneIndex * _laneHeight);
        }

        void Update()
        {
            
            if (_gameOver) return;

            HandleInput();
            HandleSpawning();
            MoveItems();
            CheckCollisions();
        }

        private void HandleInput()
        {
            // Ensure keyboard is connected and active
            if (Keyboard.current == null) return;

            // Move Up (W or Up Arrow)
            if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame)
            {
                if (_currentLane < laneCount - 1)
                {
                    _currentLane++;
                    UpdateCatPosition();
                }
            }
            // Move Down (S or Down Arrow)
            else if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame)
            {
                if (_currentLane > 0)
                {
                    _currentLane--;
                    UpdateCatPosition();
                }
            }
        }

        private void UpdateCatPosition()
        {
            if (catRectTransform == null) return;
            float targetY = GetLaneYPosition(_currentLane);
            catRectTransform.anchoredPosition = new Vector2(CatX, targetY);
        }

        private void HandleSpawning()
        {
            _spawnTimer += Time.deltaTime;
            if (_spawnTimer >= spawnRate)
            {
                _spawnTimer = 0f;
                SpawnObject();
            }
        }

        private void SpawnObject()
        {
            int randomLane = Random.Range(0, laneCount);
            bool isEdible = Random.value > 0.4f; // 60% chance edible, 40% chance inedible

            GameObject objGO = new GameObject(isEdible ? "edible" : "inedible", typeof(RectTransform), typeof(Image));
            objGO.transform.SetParent(gameAreaContainer, false);

            RectTransform rect = objGO.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(_laneHeight, _laneHeight);
            
            // Match the cat's pivot (left edge) so coordinates align perfectly
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);

            // Spawn on the far right edge
            float spawnX = gameAreaContainer.rect.width / 2f + _laneHeight;
            float spawnY = GetLaneYPosition(randomLane);
            rect.anchoredPosition = new Vector2(spawnX, spawnY);

            var img = objGO.GetComponent<Image>();
            img.sprite = isEdible ? foodSprite : inedibleSprite;

            _activeItems.Add(new ActiveItem
            {
                rectTransform = rect,
                lane = randomLane,
                isEdible = isEdible
            });
        }

        private void MoveItems()
        {
            for (int i = _activeItems.Count - 1; i >= 0; i--)
            {
                var item = _activeItems[i];
                if (item.rectTransform == null)
                {
                    _activeItems.RemoveAt(i);
                    continue;
                }

                // Move left
                Vector2 pos = item.rectTransform.anchoredPosition;
                pos.x -= gameSpeed * Time.deltaTime;
                item.rectTransform.anchoredPosition = pos;

                // Remove if off-screen to the left
                
                if (pos.x < -20)
                {
                    Destroy(item.rectTransform.gameObject);
                    _activeItems.RemoveAt(i);
                }
            }
        }

        private void CheckCollisions()
        {
            float catX = catRectTransform.anchoredPosition.x;

            for (var i = _activeItems.Count - 1; i >= 0; i--)
            {
                var item = _activeItems[i];
                if (item.rectTransform == null) continue;

                float itemX = item.rectTransform.anchoredPosition.x;

                // Since both use left-edge pivots, check if the item's left edge 
                // has met the cat's position and hasn't completely passed it yet
                bool isAtCatX = itemX <= catX + _laneHeight && itemX >= (catX - _laneHeight);

                if (!isAtCatX) continue;
                if (item.lane != _currentLane) continue;
                
                // Collision detected!
                if (item.isEdible)
                {
                    AudioManager.Instance.Play(SFXTag.Eat);
                    _score += shampooBonus;
                    if (statusText) statusText.text = AteSomething;
                }
                else
                {
                    AudioManager.Instance.Play(SFXTag.Annoyed);
                    _score -= dirtPenalty;
                    if (_score < 0) _score = 0;
                    if (statusText) statusText.text = TriedToEatSomethingInedible;
                }

                if (scoreText) scoreText.text = $"Score: {_score}";
                _hitCounter++;
                Destroy(item.rectTransform.gameObject);
                _activeItems.RemoveAt(i);
                
                if (_hitCounter >= 10)
                {
                    _gameOver = true;
                    reactionGameScene.SetActive(false);
                }
            }
        }
    }
}