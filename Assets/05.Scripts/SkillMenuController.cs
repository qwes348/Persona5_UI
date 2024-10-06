using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMenuController : MonoBehaviour
{
    [SerializeField]
    private List<PartyContent> contentsList;
    [SerializeField]
    private FocusEffectController focusEffect;
    [SerializeField]
    private Transform focuesParent;
    [SerializeField]
    private Transform originParent;

    private void OnEnable()
    {
        PlayEnabeTween();
    }

    private void PlayEnabeTween()
    {
        for (int i = 0; i < contentsList.Count; i++)
        {
            var content = contentsList[i];
            content.transform.localPosition += Vector3.left * 55f;
            content.transform.DOLocalMoveX(content.transform.localPosition.x + 55f, 0.125f).SetDelay(0.05f * i);
        }
    }

    public void OnFocused(int index)
    {
        var content = contentsList[index];
        focusEffect.transform.position = content.transform.position;
        content.transform.SetParent(focuesParent);
        focusEffect.gameObject.SetActive(true);
        content.transform.DOScale(1.2f, 0.25f);
        content.SetActiveOutline(false);

        focusEffect.DoScaleTween();

        SoundManager.Instance.PlaySfx("select");
    }

    public void OnFocusOut(int index)
    {
        var content = contentsList[index];
        content.transform.SetParent(originParent);
        content.transform.SetSiblingIndex(index);
        focusEffect.gameObject.SetActive(false);
        content.transform.DOScale(1f, 0.1f);
        content.SetActiveOutline(true);
    }
}
