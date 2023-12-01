using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NavigationBarManager : MonoBehaviour
{
    [System.Serializable]
    public class InteractorMenu
    {
        public GameObject Menu;
        public GameObject Interactor;
    }

    public bool HideWithCanvasGroup;
    public bool SetFirstMenuOnEnable;
    public List<InteractorMenu> Navs;
    public Button NextPageButton;
    public Button PrevPageButton;
    public TMP_Text NavBarText;

    [Header("Text")]
    public bool ChangeTextColor;
    public Color EnabledColor = Color.white;
    public Color DisabledColor = Color.white;


    protected int _currentPage;
    protected GameObject _selectedMenu;

    void Awake()
    {
        foreach (var nav in Navs)
        {
            if (nav.Interactor != null)
            {
                if (nav.Interactor.TryGetComponent<Button>(out var button))
                {
                    button.onClick.AddListener(() => { OnNavInteractorClick(nav); });
                }
                else if (nav.Interactor.TryGetComponent<Toggle>(out var toggle))
                {
                    toggle.onValueChanged.AddListener((value) =>
                    {
                        if (value)
                            OnNavInteractorClick(nav);
                    });
                }
            }
        }

        OnNavInteractorClick(Navs[0]);
        if (NextPageButton) NextPageButton.onClick.AddListener(NextPage);
        if (PrevPageButton) PrevPageButton.onClick.AddListener(PrevPage);
    }

    private void PrevPage()
    {
        ChangePage(-1);
    }

    private void NextPage()
    {
        ChangePage(1);
    }


    private void ChangePage(int change)
    {
        if (!Navs.IndexWithinRange(_currentPage + change)) return;

        OpenNav(_currentPage + change);
    }


    private void OnNavInteractorClick(InteractorMenu targetNav)
    {
        _selectedMenu = targetNav.Menu;
        _currentPage = Navs.IndexOf(targetNav);

        if (targetNav.Interactor != null)
        {
            if (targetNav.Interactor.TryGetComponent<Button>(out var button))
            {

            }
            else if (targetNav.Interactor.TryGetComponent<Toggle>(out var toggle))
            {
                if (toggle.isOn == false)
                    toggle.isOn = true;
            }
        }


        foreach (var nav in Navs)
        {
            bool enable = targetNav == nav;
            if (HideWithCanvasGroup)
                nav.Menu.GetComponent<CanvasGroup>().EnableCanvasGroup(enable);
            else
                nav.Menu.gameObject.SetActive(enable);

            if (ChangeTextColor)
            {
                nav.Interactor.GetComponentInChildren<TMP_Text>().color = enable ? EnabledColor : DisabledColor;
            }
        }

        if (NextPageButton) NextPageButton.interactable = _currentPage < Navs.Count - 1;
        if (PrevPageButton) PrevPageButton.interactable = _currentPage > 0;
        if (NavBarText) NavBarText.text = $"{_currentPage + 1} / {Navs.Count}";
    }

    public void OpenNav(int index)
    {
        if (!Navs.IndexWithinRange(index))
        {
            Debug.LogWarning("Index is not within range, will just open the first menu");
            index = 0;
        }

        OnNavInteractorClick(Navs[index]);
    }

    // private InteractorMenu FindInteractorMenu(GameObject menu)
    // {
    //     return Navs.Find(n => n.Menu == menu);
    // }



    void OnEnable()
    {
        if (SetFirstMenuOnEnable) OnNavInteractorClick(Navs[0]);
        if (_selectedMenu == null) OnNavInteractorClick(Navs[0]);
    }
}

