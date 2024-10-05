using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

[Serializable]
public class AudioClipInfo
{
    [SerializeField]
    private string audioName;
    [SerializeField]
    private AudioClip clip;

    #region 프로퍼티
    public string AudioName { get => audioName; }
    #endregion

    public AudioClip GetClip()
    {
        return clip;
    }
}
