using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MiniGames
{

    public enum CardType
    {
        BrownTeddy,
        WhiteTeddy,
        Pillow,
        Heart,
    }

    public class MemoryGame : MonoBehaviour
    {
        
        
        [Header("UI References")]
        public GameObject memoryScene;
        public Petmanager PetManager;
        [SerializeField] private List<Button> cardButtons;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Game Settings")]
        [SerializeField] private List<CardType> cardSymbols;
        [SerializeField] private float delayBeforeFlippingBack = 0.8f;
        [SerializeField] private float delayAfterWin = 0.8f;

        //TODO should be moved to UIManager
        [SerializeField] public GameObject CardGridContainer;
        
        [SerializeField] public Sprite cardSprite;
        
        [SerializeField] public Sprite whiteTeddySprite;
        [SerializeField] public Sprite brownTeddySprite;
        [SerializeField] public Sprite pillowSprite;
        [SerializeField] public Sprite heartSprite;

        private const string DefaultStatus = "Find a match!";
        private const string NoMatch = "No match";
        private const string WinMessage = "You Win!";
        
        private Dictionary<CardType, Sprite> _cardSprites = new();
        
        private bool _gameOver = false;
        private int _score = 0;
        private const int ScoreTarget = 40;
        private const int MatchScoreBonus = 10;
        private const int MatchScorePenalty = 5;
        private int _totalPairs = 0;
        private int _matchedPairs = 0;

        //state tracking for the current turn
        private int _firstSelectedIndex = -1;
        private int _secondSelectedIndex = -1;
        private bool _isChecking = false; // Prevents clicking while waiting for mismatch reset

        //internal mapping of card button index to its secret symbol value
        private Dictionary<int, CardType> _cardValues = new();
        private Dictionary<int, bool> _matchedStatus = new();

        public void Init()
        {
            _gameOver = false;
            memoryScene.SetActive(true);
            CardGridContainer.SetActive(true);
            
            _score = 0;
            _matchedPairs = 0;
            _isChecking = false;
            _firstSelectedIndex = -1;
            _secondSelectedIndex = -1;

            FillCardSprites();

            if (statusText)
                statusText.text = DefaultStatus;
            
            if (scoreText)
                scoreText.text = "Score: 0";

            SetupAndShuffleCards();
        }

        private void FillCardSprites()
        {
            cardSymbols.Add(CardType.BrownTeddy);
            cardSymbols.Add(CardType.WhiteTeddy);
            cardSymbols.Add(CardType.Pillow);
            cardSymbols.Add(CardType.Heart);
            
            _cardSprites.Add(CardType.BrownTeddy, brownTeddySprite);
            _cardSprites.Add(CardType.WhiteTeddy, whiteTeddySprite);
            _cardSprites.Add(CardType.Pillow, pillowSprite);
            _cardSprites.Add(CardType.Heart, heartSprite);
        }

        void Update()
        {
            if (_gameOver)
                return;
            
            //TODO add timer here or something, ask what rest of team wants
        }

        private void SetupAndShuffleCards()
        {
            _cardValues.Clear();
            _matchedStatus.Clear();

            // build a pool of pairs
            var totalCards = cardButtons.Count;
            _totalPairs = totalCards / 2;

            List<CardType> activeSymbols = new();
            for (var i = 0; i < _totalPairs; i++)
            {
                var symbol = cardSymbols[i % cardSymbols.Count];
                activeSymbols.Add(symbol);
                activeSymbols.Add(symbol); // Add pair
            }

            // shuffle (Fisher-Yates)
            for (var i = activeSymbols.Count - 1; i > 0; i--)
            {
                var rnd = Random.Range(0, i + 1);
                (activeSymbols[i], activeSymbols[rnd]) = (activeSymbols[rnd], activeSymbols[i]);
            }

            // assign values to buttons and hook up click listeners
            for (var i = 0; i < cardButtons.Count; i++)
            {
                var index = i; // Local copy for closure
                _cardValues[index] = activeSymbols[i];
                _matchedStatus[index] = false;

                cardButtons[index].interactable = true;
            }
        }

        public void OnCardClicked(int index)
        {
            // ignore click if we are currently checking a mismatch, card is already matched or clicked the same card twice
            if (_isChecking || _matchedStatus[index] || index == _firstSelectedIndex)
                return;

            if (_firstSelectedIndex == -1)//first card selected
            {
                _firstSelectedIndex = index;
                if (cardButtons[index] && cardButtons[index].image)
                    cardButtons[index].image.sprite = _cardSprites[_cardValues[index]];// reveal symbol
            }
            else //second card selected
            {
                _secondSelectedIndex = index;
                if (cardButtons[index] && cardButtons[index].image)
                    cardButtons[index].image.sprite = _cardSprites[_cardValues[index]];// reveal symbol

                StartCoroutine(EvaluateMatchCoroutine());
            }
        }

        private IEnumerator EvaluateMatchCoroutine()
        {
            _isChecking = true;

            var firstSymbol = _cardValues[_firstSelectedIndex];
            var secondSymbol = _cardValues[_secondSelectedIndex];

            if (firstSymbol == secondSymbol)//match found
            {
                _matchedStatus[_firstSelectedIndex] = true;
                _matchedStatus[_secondSelectedIndex] = true;

                cardButtons[_firstSelectedIndex].interactable = false;
                cardButtons[_secondSelectedIndex].interactable = false;

                _matchedPairs++;
                _score += MatchScoreBonus;
                if (scoreText) scoreText.text = $"Score: {_score}";

                // check Win Condition
                if (_matchedPairs >= _totalPairs)
                {
                    _gameOver = true;
                    if (statusText)
                        statusText.text = WinMessage;
                    yield return new WaitForSeconds(delayAfterWin);
                    
                    PetManager.SetMultiplierBonus((float) _score / ScoreTarget);
                    memoryScene.SetActive(false);
                }

                ResetTurnState();
            }
            else //mismatch
            {
                _score = Mathf.Max(_score - MatchScorePenalty,0);
                if (scoreText) scoreText.text = $"Score: {_score}";
                
                if (statusText)
                    statusText.text = NoMatch;
                
                yield return new WaitForSeconds(delayBeforeFlippingBack);

                // Hide cards back down
                cardButtons[_firstSelectedIndex].image.sprite = cardSprite;
                cardButtons[_secondSelectedIndex].image.sprite = cardSprite;

                if (statusText)
                    statusText.text = DefaultStatus;
                ResetTurnState();
            }
        }

        private void ResetTurnState()
        {
            _firstSelectedIndex = -1;
            _secondSelectedIndex = -1;
            _isChecking = false;
        }
    }
}