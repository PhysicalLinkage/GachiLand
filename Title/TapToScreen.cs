using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Text))]
public class TapToScreen : MonoBehaviour
{
    [SerializeField] int interval;

    private void Awake()
    {
        var text = GetComponent<Text>();
        var color = text.color;
        var alpha0 = color;
        alpha0.a = 0;

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                text.color = Color.Lerp(color, alpha0, Mathf.PingPong(Time.time, 1));
            });
    }
}
