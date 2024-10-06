using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyContent : MonoBehaviour
{
    [SerializeField]
    private GameObject outlineObject;

    public void SetActiveOutline(bool active)
    {
        outlineObject.SetActive(active);
    }
}
