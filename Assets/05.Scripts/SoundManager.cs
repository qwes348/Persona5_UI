using Oni;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonSerializedMono<SoundManager>
{
    [SerializeField]
    private List<AudioClipInfo> clipInfos;
    [SerializeField]
    private AudioSource sfxSource;

    public void PlaySfx(string sfxName)
    {
        var info = clipInfos.Find(c => c.AudioName == sfxName);
        if (info == null)
            return;

        sfxSource.PlayOneShot(info.GetClip());
    }
}
