using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WhiteManaSphereFactory))]
[RequireComponent(typeof(BlueManaSphereFactory))]
public class FactoryManager : MonoBehaviour
{
    public static ManaSphereFactory WhiteSphereFactory { get; private set; }
    public static ManaSphereFactory BlueSphereFactory { get; private set; }

    private void Awake()
    {
        WhiteSphereFactory = GetComponent<WhiteManaSphereFactory>();
        BlueSphereFactory = GetComponent<BlueManaSphereFactory>();
    }
}
