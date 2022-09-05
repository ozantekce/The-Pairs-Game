using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField]
    private CardSprites _cardSprites;
    [SerializeField]
    private int _cardBackNo;

    [SerializeField]
    private GameObject _cardArea;
    private List<Card> _cardList = new List<Card>();
    private List<Card> _allCreatedCards = new List<Card>();


    public GameObject jokerEffect;

    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;


    public TextMeshProUGUI victoryYourScoreText, victoryHighScoreText;


    public TextMeshProUGUI loseYourScoreText, loseHighScoreText;

    public Button pauseButton;

    private TimeController _timeController;
    private int _currentScore;
    private int _combo = 1;
    private void IncreaseScore()
    {
        int earnedScore = _combo * 10;
        ShowEarnedScore(earnedScore.ToString());
        _currentScore += earnedScore;

    }

    public int LastScore
    {
        get { return PlayerPrefs.GetInt("LastScore",0); }
        set { PlayerPrefs.SetInt("LastScore",value); }

    }

    public int HighScore
    {
        get { return PlayerPrefs.GetInt("HighScore", 0); }
        set { PlayerPrefs.SetInt("HighScore", value); }

    }


    private void Start()
    {
        //9, 12, 15, 18
        //CreateGame(9);

    }





    private Card firstOpenedCard;
    private Card secondOpenedCard;

    public void SelectCard(Card card)
    {

        if(firstOpenedCard == null)
        {
            firstOpenedCard = card;
            firstOpenedCard.OpenTheCard();
            if(firstOpenedCard.IsJoker)
                SoundManager.Instance.PlaySoundClip("JokerFlip");
            else
                SoundManager.Instance.PlaySoundClip("CardFlip",0.5f);
        }
        else if(firstOpenedCard!=null &&secondOpenedCard == null && firstOpenedCard != card)
        {

            secondOpenedCard = card;
            secondOpenedCard.OpenTheCard();

            if (secondOpenedCard.IsJoker)
                SoundManager.Instance.PlaySoundClip("JokerFlip");
            else
                SoundManager.Instance.PlaySoundClip("CardFlip",0.5f);

            if (firstOpenedCard.IsJoker)
            {
                _cardList.Remove(firstOpenedCard);
                DestroyImmediate(firstOpenedCard.JokerEffect);
                firstOpenedCard.PlayCorrectAnimation();
                foreach (Card pairCard in _cardList)
                {
                    if(pairCard.No == secondOpenedCard.No && pairCard != secondOpenedCard)
                    {
                        firstOpenedCard=pairCard;
                        break;
                    }
                }
                firstOpenedCard.OpenTheCard();
                Debug.Log("fo : " + firstOpenedCard);

            }
            else if (secondOpenedCard.IsJoker)
            {
                _cardList.Remove(secondOpenedCard);
                DestroyImmediate(secondOpenedCard.JokerEffect);
                secondOpenedCard.PlayCorrectAnimation();
                foreach (Card pairCard in _cardList)
                {
                    if (pairCard.No == firstOpenedCard.No && pairCard != firstOpenedCard)
                    {
                        secondOpenedCard = pairCard;
                        break;
                    }
                    
                }
                secondOpenedCard.OpenTheCard();
            }


            if (secondOpenedCard.No == firstOpenedCard.No)
            {
                //Debug.Log("Pair");
                _cardList.Remove(firstOpenedCard);
                _cardList.Remove(secondOpenedCard);
                StartCoroutine(CorrectPair());

                if(_cardList.Count == 0)
                {
                    TerminateGame();
                }else if(_cardList.Count == 1)
                {
                    _cardList[0].OpenTheCard();
                    _cardList[0].PlayCorrectAnimation();
                    _cardList.Clear();
                    IncreaseScore();
                    TerminateGame();
                }


            }
            else
            {
                //Debug.Log("No Pair");
                StartCoroutine(InCorrectPair());
            }


        }


    }


    public void TerminateGame()
    {
        Debug.Log("game over");

        pauseButton.enabled = false;

        _timeController.State = TimeController.TimeState.Pause;

        if(_timeController.TimeRemaining > 0)
        {
            SoundManager.Instance.PlaySoundClip("Victory");
            StartCoroutine(ConvertRemainingTimeToScore());
        }
        else
        {
            SoundManager.Instance.PlaySoundClip("Lose");
            LastScore = _currentScore;
            Invoke("OpenLosePopUp", 0.5f);
        }
        

    }


    private IEnumerator ConvertRemainingTimeToScore()
    {
        yield return new WaitForSeconds(0.7f);
        while (TimeController.TimeRemaining > 0)
        {
            TimeController.TimeRemaining--;
            if (TimeController.TimeRemaining < 1)
                TimeController.TimeRemaining = 0;
            TimeController.DisplayTime(TimeController.TimeRemaining);
            _currentScore += 2;
            scoreText.text = _currentScore.ToString();
            SoundManager.Instance.PlaySoundClip("Increase");
            yield return new WaitForSeconds(0.2f);
        }

        LastScore = _currentScore;
        if (_currentScore > HighScore)
        {
            HighScore = _currentScore;
        }
        Invoke("OpenVictoryPopUp", 0.5f);
    }


    private void OpenVictoryPopUp()
    {
        victoryHighScoreText.text = HighScore.ToString();
        victoryYourScoreText.text = _currentScore.ToString();
        ScreenManager.Instance.OpenPopUp("VictoryPopUp");
    }


    private void OpenLosePopUp()
    {
        loseHighScoreText.text = HighScore.ToString();
        loseYourScoreText.text = _currentScore.ToString();
        ScreenManager.Instance.OpenPopUp("LosePopUp");
    }


    [SerializeField]
    private TMP_FontAsset _popUpMessageFont;
    private Sequence showEarnedScoreSequence;
    private void ShowEarnedScore(string text)
    {
        GameObject textGameObject = new GameObject("PopUpMessage");
        Screen currentScreen = ScreenManager.Instance.CurrentScreen;
        textGameObject.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
        TextMeshProUGUI textMeshPro = textGameObject.AddComponent<TextMeshProUGUI>();
        textMeshPro.rectTransform.anchoredPosition = Vector2.zero;
        textMeshPro.fontSize = 90;
        
        if (_popUpMessageFont != null)
            textMeshPro.font = _popUpMessageFont;
        textMeshPro.transform.localScale = textMeshPro.transform.localScale * 3;
        textMeshPro.margin = new Vector4(-50, -50, -50, -50);
        textMeshPro.alignment = TextAlignmentOptions.Center;
        textMeshPro.text = text;
        textMeshPro.color = Color.white;

        if (showEarnedScoreSequence != null)
        {
            showEarnedScoreSequence.Complete();
            showEarnedScoreSequence.Kill();
        }

        

        showEarnedScoreSequence = DOTween.Sequence();

        showEarnedScoreSequence.Append(textMeshPro.transform.DOScale(4f, 0.5f));

        showEarnedScoreSequence.Append(textMeshPro.DOFade(0.1f, 0.8f).OnComplete(() =>
        {
            scoreText.text = _currentScore.ToString();
            
        }
        ));
        showEarnedScoreSequence.Join(textMeshPro.transform.DOScale(0.1f, 1f)).OnComplete(delegate
        {
            Destroy(textGameObject,0.2f);
        });
        showEarnedScoreSequence.Join(textMeshPro.transform.DOMove(scoreText.transform.position, 1f));


    }


    private IEnumerator InCorrectPair()
    {
        SoundManager.Instance.PlaySoundClip("Incorrect");
        yield return new WaitUntil(() => !secondOpenedCard.Opening && !firstOpenedCard.Opening);

        firstOpenedCard?.PlayInCorrectAnimation();
        secondOpenedCard?.PlayInCorrectAnimation();

        yield return new WaitForSeconds(0.75f);
        firstOpenedCard?.CloseTheCard();
        secondOpenedCard?.CloseTheCard();

        yield return new WaitUntil(()
            => ((firstOpenedCard == null || !secondOpenedCard.Closing) && (firstOpenedCard == null || !secondOpenedCard.Closing))
        );

        firstOpenedCard=null;
        secondOpenedCard=null;
        _combo = 1;
    }


    private IEnumerator CorrectPair()
    {
        SoundManager.Instance.PlaySoundClip("Correct");
        IncreaseScore();
        yield return new WaitUntil(() => !secondOpenedCard.Opening && !firstOpenedCard.Opening);

        yield return new WaitForSeconds(0.1f);

        firstOpenedCard.PlayCorrectAnimation();
        secondOpenedCard.PlayCorrectAnimation();

        yield return new WaitForEndOfFrame();

        firstOpenedCard = null;
        secondOpenedCard = null;
        _combo++;
    }


    private int lastGameCardCount=9;
    public void CreateGame(int cardCount)
    {
        lastGameCardCount = cardCount;
        _currentScore = 0;
        _combo = 1;
        StartCoroutine(CreateGameRoutine(cardCount));
    }

    public void ReCreateCurrentGame()
    {
        CreateGame(lastGameCardCount);
    }

    private IEnumerator CreateGameRoutine(int cardCount)
    {
        pauseButton.enabled = false;
        DestroyCards();
        timeText.text = "01:00";
        scoreText.text = "0";

        Destroy(_timeController);

        CreateCards(cardCount);
        yield return new WaitUntil(() => _cardList[0].FrontImage!=null);
        PlayGameStartingAnimation();
        PlayCooldownAnimation();
        yield return new WaitForSeconds(3.5f);
        _timeController = gameObject.AddComponent<TimeController>();
        _timeController.ResetTimeController(60, timeText);
        _timeController.State = TimeController.TimeState.Start;

        pauseButton.enabled=true;
    }

    private void CreateCards(int cardCount)
    {
        
        GameObject cardGO;

        List<int> indexes = new List<int>();
        for (int i = 0; i < (cardCount / 2 + 1); i++)
        {
            indexes.Add(i);
            if (i != 0)
                indexes.Add(i);
        }

        int row = -1;
        int bound = Mathf.CeilToInt(cardCount / 3.0f);
        Debug.Log("cc : " + cardCount + " bound : " + bound);
        for (int i = 0; i < cardCount; i++)
        {
            if(i% bound == 0)
                row++;

            int currentIndex = indexes[Random.Range(0, indexes.Count)];
            indexes.Remove(currentIndex);

            cardGO = new GameObject();
            cardGO.AddComponent<RectTransform>();
            Debug.Log(row);
            cardGO.transform.SetParent(_cardArea.transform.GetChild(row));
            Card card1 = cardGO.AddComponent<Card>();
            card1.No = currentIndex;
            _cardList.Add(card1);
            if (currentIndex == 0)
            {   
                card1.JokerEffect = GameObject.Instantiate(this.jokerEffect, card1.transform);
                card1.JokerEffect.SetActive(false);
                continue;
            }
                

        }
        _allCreatedCards = new List<Card>(_cardList);

    }


    public TMP_Text cooldownText;
    private void PlayCooldownAnimation()
    {
        StartCoroutine(PlayCooldownAnimationRoutine());
    }
    private IEnumerator PlayCooldownAnimationRoutine()
    {

        cooldownText.gameObject.SetActive(true);
        cooldownText.transform.localScale = Vector3.one;
        cooldownText.color = Color.white;
        cooldownText.text = "3";
        cooldownText.DOFade(0, 1f);
        cooldownText.transform.DOScale(2f, 1f);
        yield return new WaitForSeconds(1);
        cooldownText.transform.localScale = Vector3.one;
        cooldownText.color = Color.white;
        cooldownText.text = "2";
        cooldownText.DOFade(0, 1f);
        cooldownText.transform.DOScale(2f, 1f);
        yield return new WaitForSeconds(1);
        cooldownText.transform.localScale = Vector3.one;
        cooldownText.color = Color.white;
        cooldownText.text = "1";
        cooldownText.DOFade(0, 1f);
        cooldownText.transform.DOScale(2f, 1f);
        yield return new WaitForSeconds(1);
        cooldownText.gameObject.SetActive(false);

    }

    private void PlayGameStartingAnimation()
    {
        StartCoroutine(PlayRevealAnimationRoutine());
    }
    private IEnumerator PlayRevealAnimationRoutine()
    {

        float fadeOutTime = 3f / _cardList.Count;
        List<Image> images = new List<Image>();
        Color transparentColor = new Color(255, 255, 255, 0);

        foreach (Card card in _cardList)
        {
            Image temp = card.FrontImage;
            temp.color = transparentColor;
            images.Add(temp);
        }

        for (int i = 0; i < images.Count; i++)
        {
            images[i].DOFade(1f, fadeOutTime);
            yield return new WaitForSeconds(fadeOutTime - 0.05f);
            // sadece joker kaldi
            if (i == images.Count - 1)
            {
                yield return new WaitForSeconds(1.5f);
            }
        }

        foreach (Card card in _cardList)
        {
            card.CloseTheCard();
        }

    }


    public void DestroyCards()
    {
        foreach(Card card in _allCreatedCards)
        {
            Destroy(card.gameObject);
        }
        _allCreatedCards.Clear();
        _cardList.Clear();

    }


    #region GetterSetter
    public CardSprites CardSprites { get => _cardSprites; set => _cardSprites = value; }
    public static GameManager Instance { get => instance; set => instance = value; }
    public int CardBackNo { 
        get => _cardBackNo; set
        {
            if (_allCreatedCards.Count > 0)
            {
                _cardBackNo = value;
                foreach (Card card in _allCreatedCards)
                {
                    card.BackImage.sprite = CardSprites.cardBackSprites[_cardBackNo];
                }
            }

        }
    }

    public TimeController TimeController { get => _timeController; set => _timeController = value; }

    #endregion

}

