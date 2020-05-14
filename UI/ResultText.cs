using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class ResultText : MonoBehaviour
{
    static Text Text { get; set; }
    public static Subject<string> result { get; } = new Subject<string>();

    private void Awake()
    {
        Text = GetComponentInChildren<Text>();

        result.Subscribe(s =>
        {
            Text.text = s;
            gameObject.SetActive(s != "");
        });

        gameObject.SetActive(false);
    }
}
