using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModelType
{
    Misaki_SchoolUniform_WinterAvatar,
    Misaki_SchoolUniform_SummerAvatar,
    EthanAvatar,
    Cha_KnightAvatar,
    Chara_4HeroAvatar,
    None
}

public class ObjectManager : MonoBehaviour
{
    [SerializeField] GameObject misaki_SchoolUniform_WinterAvatar = null;
    [SerializeField] GameObject misaki_SchoolUniform_SummerAvatar = null;
    [SerializeField] GameObject ethanAvatar = null;
    [SerializeField] GameObject cha_KnightAvatar = null;
    [SerializeField] GameObject chara_4HeroAvatar = null;

    [SerializeField] GameObject statusCanvas = null;

    public static GameObject Misaki_SchoolUniform_WinterAvatar { get; private set; }
    public static GameObject Misaki_SchoolUniform_SummerAvatar { get; private set; }
    public static GameObject EthanAvatar { get; private set; }
    public static GameObject Cha_KnightAvatar { get; private set; }
    public static GameObject Chara_4HeroAvatar { get; private set; }

    public static GameObject StatusCanvas { get; private set; }

    public static Dictionary<ModelType, GameObject> PlayerModels = new Dictionary<ModelType, GameObject>();



    [SerializeField] GameObject whiteManaSphere = null;
    public static GameObject WhiteManaSphere { get; private set; }

    [SerializeField] GameObject blueManaSphere = null;
    public static GameObject BlueManaSphere { get; private set; }

    private void Awake()
    {
        Misaki_SchoolUniform_WinterAvatar = misaki_SchoolUniform_WinterAvatar;
        Misaki_SchoolUniform_SummerAvatar = misaki_SchoolUniform_SummerAvatar;
        EthanAvatar = ethanAvatar;
        Cha_KnightAvatar = cha_KnightAvatar;
        Chara_4HeroAvatar = chara_4HeroAvatar;

        PlayerModels.Add(ModelType.Misaki_SchoolUniform_WinterAvatar, Misaki_SchoolUniform_WinterAvatar);
        PlayerModels.Add(ModelType.Misaki_SchoolUniform_SummerAvatar, Misaki_SchoolUniform_SummerAvatar);
        PlayerModels.Add(ModelType.EthanAvatar, EthanAvatar);
        PlayerModels.Add(ModelType.Cha_KnightAvatar, Cha_KnightAvatar);
        PlayerModels.Add(ModelType.Chara_4HeroAvatar, Chara_4HeroAvatar);

        WhiteManaSphere = whiteManaSphere;
        BlueManaSphere = blueManaSphere;

        StatusCanvas = statusCanvas;
    }
}
