using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;

public class ActionButtonManager : MonoBehaviour
{
    [SerializeField] Button[] Buttons_ = new Button[Host.ActionSize];

    public static Button[] Buttons { get; private set; }

    private void Awake()
    {
        Buttons = Buttons_;
    }
}
