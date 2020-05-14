using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ActionType
{
    None,
    Normal,
    NormalPulse
}

public class ActionManager : MonoBehaviour
{
    private Dictionary<ActionType, Action> actions_;

    private void Awake()
    {
        var normal = gameObject.AddComponent<Normal>();
        var normalPulse = gameObject.AddComponent<NormalPulse>();

        actions_ = new Dictionary<ActionType, Action>
        {
            { ActionType.None, () => { } },
            { ActionType.Normal, normal.Action },
            { ActionType.NormalPulse,  normalPulse.Skill }
        };
    }

    public void Action(ActionType type)
    {
        Action action;
        if (actions_.TryGetValue(type, out action))
        {
            action();
        }
    }
}
