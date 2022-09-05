using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    private static ScreenManager instance;


    [SerializeField]
    private Screen[] _screens;
    [SerializeField]
    private PopUp[] _popUps;

    private Dictionary<string, Screen> _screenDictionary;
    private Dictionary<string, PopUp> _popUpDictionary;

    public Screen initialScreen;

    private Screen _currentScreen;

    private PopUp _currentPopUp;

    private bool _bloked;
    private bool _popUpBloked;

    private void Awake()
    {
        _currentScreen = initialScreen;

        _screenDictionary = new Dictionary<string, Screen>();
        foreach (Screen s in _screens)
        {
            _screenDictionary.Add(s.name, s);
            _screenDictionary[s.name].gameObject.SetActive(false);
        }

        _popUpDictionary = new Dictionary<string, PopUp>();
        foreach (PopUp s in _popUps)
        {
            _popUpDictionary.Add(s.name, s);
            _popUpDictionary[s.name].gameObject.SetActive(false);
        }
        MakeSingleton();

        LoadScreen(_currentScreen.name);

    }



    public void LoadScreen(string screenName)
    {
        if (_bloked)
            return;
        
        ClosePopUp();

        StartCoroutine(LoadScreenRoutine(_currentScreen, _screenDictionary[screenName]));

    }

    private IEnumerator LoadScreenRoutine(Screen willClose, Screen willOpen)
    {
        _bloked = true;
        if(willClose != null)
        {
            willClose.Close();
            yield return new WaitUntil(() => !willClose.Opened);
            Debug.Log(willClose.name + " closed");
            willClose.gameObject.SetActive(false);
        }

        _currentScreen = willOpen;

        willOpen.Open();

        yield return new WaitUntil(() => willOpen.Opened);
        willOpen.gameObject.SetActive(true);
        Debug.Log(willOpen.name + " opened");
        _bloked = false;

    }

    public void OpenPopUp(string popUpName)
    {
        if (_popUpBloked)
            return;

        StartCoroutine(OpenPopUpRoutine(_currentPopUp, _popUpDictionary[popUpName]));

    }

    private IEnumerator OpenPopUpRoutine(PopUp willClose, PopUp willOpen)
    {
        _popUpBloked = true;
        if (willClose != null)
        {
            willClose.Close();
            yield return new WaitUntil(() => !willClose.Opened);
            willClose.gameObject.SetActive(false);
        }

        _currentPopUp = willOpen;

        willOpen.Open();

        yield return new WaitUntil(() => willOpen.Opened);
        willOpen.gameObject.SetActive(true);

        _popUpBloked = false;

    }

    public void ClosePopUp()
    {
        StartCoroutine(ClosePopUpRoutine());
    }
    private IEnumerator ClosePopUpRoutine()
    {
        _popUpBloked = true;

        if (_currentPopUp != null)
        {
            _currentPopUp.Close();
            yield return new WaitUntil(() => !_currentPopUp.Opened);
            _currentPopUp.gameObject.SetActive(false);
        }
        
        _currentPopUp = null;

        _popUpBloked = false;

    }


    public void QuitApplication()
    {
        Application.Quit();
    }



    private void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    #region GetterSetter

    public static ScreenManager Instance { get => instance; }
    public Screen CurrentScreen { get => _currentScreen; }
    public PopUp CurrentPopUp { get => _currentPopUp;}


    #endregion


}
