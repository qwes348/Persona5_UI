using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButtonManager : MonoBehaviour
{
    public static BackButtonManager Instance;

    private Stack<Action> actionStack;
    public float a = 0f;
    public float b = 100f;
    public float c;

    private void Awake()
    {
        actionStack = new Stack<Action>();

        if(Instance == null)
            Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DoAction();
        }

        c = Mathf.Lerp(c, b, Time.deltaTime * 2f);
    }

    public void AddAction(Action act)
    {
        actionStack.Push(act);
    }

    public void DoAction()
    {
        if (actionStack.Count < 0)
            return;
        actionStack.Pop().Invoke();
    }
}
