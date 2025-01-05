using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class PersonaUITweenController : MonoBehaviour
{
    [SerializeField]
    private List<Image> sizeTweenStars;
    [SerializeField]
    private JokerAnimController jokerAnim;

    [Header("Panels")]
    [HorizontalLine]
    [SerializeField]
    private GameObject starPanel;
    [SerializeField]
    private GameObject whitePanel;
    [SerializeField]
    private GameObject blackPanel;

    [Header("Masks")]
    [HorizontalLine]
    [SerializeField]
    private RectTransform starMask;
    [SerializeField]
    private RectTransform whiteMask;
    [SerializeField]
    private Image transitionMaskSkillMenu;

    [Header("RawImages")]
    [HorizontalLine]
    [SerializeField]
    private JokerRawImages jokerRawImages;

    [Header("Rot")]
    [HorizontalLine]
    [SerializeField]
    private Vector3 starMaskInitRot;
    [SerializeField]
    private Vector3 starMaskMainMenuRot;
    [SerializeField]
    private Vector3 starMaskSkillMenuRot;

    [Header("Pos")]
    [HorizontalLine]
    [SerializeField]
    private Vector3 starMaskInitPos;
    [SerializeField]
    private Vector3 starMaskMainMenuPos;
    [SerializeField]
    private Vector3 starMaskSkillMenuPos;


    private List<Vector3> starsOriginSizeList;

    private void Init()
    {
        TurnOffOnInit();
        starsOriginSizeList = new List<Vector3>();
        for (int i = 0; i < sizeTweenStars.Count; i++)
        {
            starsOriginSizeList.Add(sizeTweenStars[i].transform.localScale);
            sizeTweenStars[i].transform.localScale = Vector3.zero;
        }

        starMask.rotation = Quaternion.Euler(starMaskInitRot);
        whiteMask.rotation = Quaternion.Euler(starMaskInitRot);
    }

    private void TurnOffOnInit()
    {
        foreach(var value in jokerRawImages.Containers)
        {
            value.raw.gameObject.SetActive(false);
        }
    }

    public void ActiveOnMainMenu()
    {
        Init();
        PlayStarsSizeTween();
        PlayMenuTween(PersonaCanvasManager.MenuType.MainMenu);

        starPanel.SetActive(true);
    }

    private void PlayStarsSizeTween()
    {
        for (int i = 0; i < sizeTweenStars.Count; i++)
        {
            sizeTweenStars[i].transform.DOScale(starsOriginSizeList[i], 0.4f);
        }
    }

    public void PlayMenuTween(PersonaCanvasManager.MenuType menu, bool isBackToMenu = false)
    {
        switch (menu)
        {
            case PersonaCanvasManager.MenuType.None:
                break;
            case PersonaCanvasManager.MenuType.MainMenu:
                MaskTween_MainMenu();
                if (!isBackToMenu)
                    Joker_MainMenu();
                else
                {
                    blackPanel.SetActive(false);
                    PersonaCanvasManager.Instance.OnBackToMenu(menu);
                }
                break;
            case PersonaCanvasManager.MenuType.SkillMenu:
                transitionMaskSkillMenu.material.SetFloat("_AlphaCutoff", 0f);
                MaskTween_SkillMenu();
                Joker_SkillMenu();
                break;
        }
    }

    private void MaskTween_MainMenu()
    {
        starMask.DOLocalMove(starMaskMainMenuPos, 0.3f);
        starMask.DORotate(starMaskMainMenuRot, 0.3f);
        whiteMask.DORotate(starMaskMainMenuRot, 0.4f);
    }

    private async void Joker_MainMenu()
    {        
        jokerAnim.PlayAnim(PersonaCanvasManager.MenuType.MainMenu);
        blackPanel.SetActive(false);
        jokerRawImages.GetContainer(PersonaCanvasManager.MenuType.MainMenu).raw.gameObject.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(0.4f));
        jokerRawImages.GetContainer(PersonaCanvasManager.MenuType.MainMenu).raw.gameObject.SetActive(false);
        PersonaCanvasManager.Instance.OnTweenComplete(PersonaCanvasManager.MenuType.MainMenu);
        jokerAnim.PlayAnim(PersonaCanvasManager.MenuType.None);
    }

    private void MaskTween_SkillMenu()
    {
        starMask.DORotate(starMaskSkillMenuRot, 0.4f);
        whiteMask.DORotate(starMaskSkillMenuRot, 0.3f);
    }

    private async void Joker_SkillMenu()
    {
        jokerAnim.PlayAnim(PersonaCanvasManager.MenuType.SkillMenu);
        blackPanel.SetActive(true);
        jokerRawImages.GetContainer(PersonaCanvasManager.MenuType.SkillMenu).raw.gameObject.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        await starMask.DOLocalMove(starMaskSkillMenuPos, 0.1f);
        jokerRawImages.GetContainer(PersonaCanvasManager.MenuType.SkillMenu).raw.gameObject.SetActive(false);
        PersonaCanvasManager.Instance.OnTweenComplete(PersonaCanvasManager.MenuType.SkillMenu);
        jokerAnim.PlayAnim(PersonaCanvasManager.MenuType.None);
    }

    public async UniTask TransitionSkillMenuOut()
    {
        await DOVirtual.Float(0f, 1f, 0.4f, v => transitionMaskSkillMenu.material.SetFloat("_AlphaCutoff", v));
    }
}

[Serializable]
public class JokerRawImages
{
    [SerializeField]
    private List<JokerRawImageContainer> containers;

    public List<JokerRawImageContainer> Containers { get => containers; }

    public JokerRawImageContainer GetContainer(PersonaCanvasManager.MenuType menu)
    {
        return containers.Find(con => con.menu == menu);
    }

    [Serializable]
    public class JokerRawImageContainer
    {
        public PersonaCanvasManager.MenuType menu;
        public RawImage raw;
    }
}
