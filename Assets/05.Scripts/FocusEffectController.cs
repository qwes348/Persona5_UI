using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class FocusEffectController : MonoBehaviour
{
    [SerializeField]
    private Vector3 originScale;

    public void DoScaleTween()
    {
        transform.DOScale(Vector3.right * originScale.x * 2f, 0f);
        transform.DOScale(originScale, 0.2f);
    }

    [Button]
    public void UpdateOriginScale()
    {
        originScale = transform.localScale;
    }
}
