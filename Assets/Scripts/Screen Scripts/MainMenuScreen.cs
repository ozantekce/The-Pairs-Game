using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class MainMenuScreen : Screen
{

    public GameObject logo;
    private Vector2 logoStartSize = new Vector2(0,0);
    private Vector2 logoEndSize = new Vector2(300,300);

    public Button playButton;
    private Vector2 playButtonStartPos = new Vector2(0,456);
    private Vector2 playButtonEndPos = new Vector2(0,-34f);

    public Button settingsButton;
    private Vector2 settingsButtonStartPos = new Vector2(0, 390);
    private Vector2 settingsButtonEndPos = new Vector2(0, -100f);


    public Button quitButton;
    private Vector2 quitButtonStartPos = new Vector2(0, 324);
    private Vector2 quitButtonEndPos = new Vector2(0, -166f);


    public RectTransform scorePanel;
    private Vector2 scorePanelStartPos = new Vector2(283f, -351f);
    private Vector2 scorePanelEndPos = new Vector2(283f, -250f);

    public TMP_Text highScoreText;
    public TMP_Text lastScoreText;

    private Sequence openingSequence;
    private Sequence closingSequence;


    private RectTransform rectTransform;


    public GameObject[] cardBackImages;
    public GameObject selectedIcon;


    private void Awake()
    {

        rectTransform = GetComponent<RectTransform>();

    }

    private void PlayOpeningAnimation()
    {

        if (openingSequence != null)
        {
            openingSequence.Kill();
        }

        openingSequence = DOTween.Sequence();

        openingSequence.Join(playButton.GetComponent<RectTransform>()
            .DOAnchorPos(playButtonEndPos, 1.5f).SetEase(Ease.OutBounce));

        openingSequence.Join(settingsButton.GetComponent<RectTransform>()
            .DOAnchorPos(settingsButtonEndPos, 1.5f).SetEase(Ease.OutBounce));

        openingSequence.Join(quitButton.GetComponent<RectTransform>()
            .DOAnchorPos(quitButtonEndPos, 1.5f).SetEase(Ease.OutBounce));


        openingSequence.Append(logo.GetComponent<RectTransform>().DOSizeDelta(logoEndSize, 1.25f).SetEase(Ease.OutBounce));

        openingSequence.Append(scorePanel.DOAnchorPos(scorePanelEndPos, 1f));

        openingSequence.OnStart(delegate
            {
                Debug.Log("1");
                playButton.enabled = false;
                settingsButton.enabled = false;
                quitButton.enabled = false;
                playButton.GetComponent<RectTransform>().anchoredPosition = playButtonStartPos;
                settingsButton.GetComponent<RectTransform>().anchoredPosition = settingsButtonStartPos;
                quitButton.GetComponent<RectTransform>().anchoredPosition = quitButtonStartPos;
                logo.GetComponent<RectTransform>().sizeDelta = logoStartSize;
                scorePanel.anchoredPosition = scorePanelStartPos;
                highScoreText.text = GameManager.Instance.HighScore.ToString();
                lastScoreText.text = GameManager.Instance.LastScore.ToString();
                SoundManager.Instance.StopMusic();
            }
        );

        openingSequence.OnComplete(delegate {

            Debug.Log("2");
            playButton.enabled = true;
            settingsButton.enabled = true;
            quitButton.enabled = true;
            SoundManager.Instance.PlayMusic();
            base.Open();
        }
        );


    }


    
    public void SelectCardBackImage(int sequence)
    {

        selectedIcon.transform.position = cardBackImages[sequence].transform.position;
        GameManager.Instance.CardBackNo = sequence;

    }

    public override void Close()
    {

        base.Close();
    }

    public override void Open()
    {

        gameObject.SetActive(true);
        PlayOpeningAnimation();

    }





}
