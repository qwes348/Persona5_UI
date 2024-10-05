using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandMenuController : MonoBehaviour
{
    [SerializeField]
    private List<Button> commandButtons;
    [SerializeField]
    private FocusEffectController focusEffect;
    [SerializeField]
    private Transform focusedParent;

    private Vector3 originScale = Vector3.one * 1.27f;

    private void Awake()
    {
        focusEffect.gameObject.SetActive(false);
    }

    public void OnFocuesd(int index)
    {
        var btn = commandButtons[index];
        focusEffect.transform.position = btn.transform.position;
        btn.transform.SetParent(focusedParent);
        focusEffect.gameObject.SetActive(true);
        btn.transform.DOScale(1.5f, 0.25f);

        focusEffect.DoScaleTween();

        SoundManager.Instance.PlaySfx("select");
    }

    public void OnFocusOut(int index)
    {
        var btn = commandButtons[index];
        btn.transform.SetParent(transform);
        btn.transform.SetSiblingIndex(index);
        focusEffect.gameObject.SetActive(false);
        btn.transform.DOScale(originScale, 0.1f);
    }
}
