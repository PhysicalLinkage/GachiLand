using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueManaSphereFactory : ManaSphereFactory
{
    protected override GameObject ManaSphere => ObjectManager.BlueManaSphere;
}
