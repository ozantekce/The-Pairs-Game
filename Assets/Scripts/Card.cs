using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{


    private Image _frontImage;
    private Image _backImage;
    private Button _button;


    private float _openTime = 0.15f;
    private float _closeTime = 0.075f;

    // 0 -> joker
    private int _cardNo;

    private VisibleSide _visibleSide;

    private GameObject jokerEffect;

    private void Awake()
    {

        GameObject cardFrontGO = new GameObject("CardFrontGO");
        cardFrontGO.transform.SetParent(transform);
        GameObject cardBackGO = new GameObject("CardBackGO");
        cardBackGO.transform.SetParent(transform);

        _frontImage = cardFrontGO.AddComponent<Image>();
        _backImage = cardBackGO.AddComponent<Image>();

        _button = gameObject.AddComponent<Button>();

    }

    void Start()
    {
        gameObject.name = "Card " + No;
        transform.localScale = Vector3.one;

        _frontImage.sprite = GameManager.Instance.CardSprites.cardFrontSprites[_cardNo];
        _backImage.sprite = GameManager.Instance.CardSprites.cardBackSprites[
            GameManager.Instance.CardBackNo
            ];

        _button.onClick.AddListener(delegate { 
            if(_visibleSide == VisibleSide.Front)
            {
                //CloseTheCard();
            }
            else
            {
               GameManager.Instance.SelectCard(this);
            }
        });

        

        _visibleSide = VisibleSide.Front;
        _frontImage.gameObject.SetActive(true);
        _backImage.gameObject.SetActive(true);


        if (_visibleSide == VisibleSide.Front)
        {
            _frontImage.transform.eulerAngles = Vector3.zero;
            _backImage.transform.eulerAngles = new Vector3(0, 90, 0);
        }
        else
        {
            _frontImage.transform.eulerAngles = new Vector3(0, 90, 0);
            _backImage.transform.eulerAngles = Vector3.zero;
        }


    }


    private bool _opening;
    public void OpenTheCard()
    {
        if(_opening || _closing)
        {
            return;
        }

        _opening = true;
        _frontImage.gameObject.SetActive(true);
        _backImage.transform.DORotate(new Vector3(0, 90, 0), _openTime).OnComplete(delegate
        {
            _frontImage.transform.DORotate(new Vector3(0, 0, 0), _openTime).OnComplete(delegate
            {
                Invoke("ChangeStateFront", 0.05f);
                _backImage.gameObject.SetActive(false);
            });


        });

    }

    private bool _closing;

    public void CloseTheCard()
    {
        if (_opening || _closing)
        {
            return;
        }
        _closing = true;
        _backImage.gameObject.SetActive(true);
        _frontImage.transform.DORotate(new Vector3(0, 90, 0), _closeTime).OnComplete(delegate
        {
            _backImage.transform.DORotate(new Vector3(0, 0, 0), _closeTime).OnComplete(delegate
            {
                Invoke("ChangeStateBack", 0.05f);
                _frontImage.gameObject.SetActive(false);
            });


        });

    }

    private void ChangeStateFront()
    {


        _visibleSide = VisibleSide.Front;
        _opening = false;

        if (IsJoker)
        {
            if(jokerEffect!=null)
                jokerEffect.SetActive(true);
        }

    }
    private void ChangeStateBack()
    {

        _visibleSide = VisibleSide.Back;
        _closing = false;

        if (IsJoker)
        {
            if (jokerEffect!=null)
                jokerEffect.SetActive(false);
        }

    }


    public void PlayCorrectAnimation()
    {
        float duration = 0.5f;
        FrontImage.transform.DOScale(1.2f, duration/2f).SetLoops(2, LoopType.Yoyo);
        FrontImage.GetComponent<Image>().DOFade(0.75f, duration);

    }

    public void PlayInCorrectAnimation()
    {
        float duration = 0.5f;
        FrontImage.transform.DOShakePosition(duration, new Vector3(2f, 2f, 0), 10, 50);

    }


    #region GetterSetter
    public int No { get => _cardNo; set => _cardNo = value; }
    public Image BackImage { get => _backImage; set => _backImage = value; }
    public Image FrontImage { get => _frontImage; set => _frontImage = value; }
    public VisibleSide VisibleSide { get => _visibleSide; }

    public bool IsJoker { get { return No == 0; } }

    public bool Opening { get => _opening; set => _opening = value; }
    public bool Closing { get => _closing; set => _closing = value; }
    public GameObject JokerEffect { get => jokerEffect; set => jokerEffect = value; }

    #endregion



}
public enum VisibleSide
{
    Front,
    Back
}
