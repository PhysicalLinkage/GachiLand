using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteManaSphereFactory : ManaSphereFactory
{
    protected override GameObject ManaSphere => ObjectManager.WhiteManaSphere;
}
