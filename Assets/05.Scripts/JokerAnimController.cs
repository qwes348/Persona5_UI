using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JokerAnimController : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    [Button]
    public void PlayAnim(PersonaCanvasManager.MenuType animType)
    {
        switch (animType)
        {
            case PersonaCanvasManager.MenuType.None:
                anim.Play("Default");
                break;
            case PersonaCanvasManager.MenuType.MainMenu:
                anim.Play("MainMenu");
                break;
            case PersonaCanvasManager.MenuType.SkillMenu:
                anim.Play("SkillMenu");
                break;
        }
    }
}