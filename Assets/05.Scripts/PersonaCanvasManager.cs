using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonaCanvasManager : MonoBehaviour
{
    public static PersonaCanvasManager Instance;

    public enum MenuType { None = -1, MainMenu, SkillMenu }

    [SerializeField]
    private PersonaUITweenController tweenController;
    [SerializeField]
    private RectTransform shakeParent;

    [Title("MenuPanels")]
    [SerializeField]
    private MenuPanels menuPanels;

    private void Awake()
    {
        Instance = this;        
    }

    private void Start()
    {
        ActiveCanvas(false);
    }

    [Button]
    public void ActiveCanvas(bool active)
    {
        if (active)
        {
            if (Instance == null)
                Awake();

            foreach(var pair in menuPanels.Containers)
            {
                pair.panelObject.SetActive(false);
            }
            tweenController.ActiveOnMainMenu();
            BackButtonManager.Instance.AddAction(() => ActiveCanvas(false));

            SoundManager.Instance.PlaySfx("openMenu");
        }
        else
        {
            BackButtonManager.Instance.AddAction(() => ActiveCanvas(true));
            SoundManager.Instance.PlaySfx("cancel");
        }

        gameObject.SetActive(active);        
    }

    public void OnTweenComplete(MenuType menu)
    {
        switch (menu)
        {
            case MenuType.None:
                break;
            case MenuType.MainMenu:
                menuPanels.GetContainer(menu).panelObject.SetActive(true);
                SoundManager.Instance.PlaySfx("kick");
                ShakeUI();
                break;
            case MenuType.SkillMenu:
                menuPanels.GetContainer(menu).panelObject.SetActive(true);
                ShakeUI();
                break;
        }        
    }

    public void OnBackToMenu(MenuType menu)
    {
        switch (menu)
        {
            case MenuType.None:
                break;
            case MenuType.MainMenu:
                menuPanels.GetContainer(menu).panelObject.SetActive(true);
                break;
            case MenuType.SkillMenu:
                break;
        }
    }

    [Button]
    private void ShakeUI()
    {
        shakeParent.DOShakePosition(0.3f, strength: new Vector3(10f, 10f), vibrato: 20);
    }

    public void ChangeMenu(int menu)
    {
        ChangeMenu((MenuType)menu);
    }

    public void ChangeMenu(MenuType menu)
    {
        switch (menu)
        {
            case MenuType.None:
                break;
            case MenuType.MainMenu:
                menuPanels.GetContainer(MenuType.SkillMenu).panelObject.SetActive(false);
                tweenController.PlayMenuTween(menu, isBackToMenu:true);
                break;
            case MenuType.SkillMenu:
                menuPanels.GetContainer(MenuType.MainMenu).panelObject.SetActive(false);
                tweenController.PlayMenuTween(menu);
                BackButtonManager.Instance.AddAction(() => ChangeMenu(MenuType.MainMenu));
                break;
        }
    }
}

[Serializable]
public class MenuPanels
{
    [SerializeField]
    private List<MenuPanelContainer> containers;

    public List<MenuPanelContainer> Containers { get => containers; }

    public MenuPanelContainer GetContainer(PersonaCanvasManager.MenuType menu)
    {
        return containers.Find(con => con.menu == menu);
    }

    [Serializable]
    public class MenuPanelContainer
    {
        public PersonaCanvasManager.MenuType menu;
        public GameObject panelObject;
    }
}

