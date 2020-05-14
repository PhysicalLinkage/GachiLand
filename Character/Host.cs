using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using Latte;

public static class Host
{
    public static ModelType Model { get; private set; }

    public static Character Character { get; private set; }

    public static int ActionSize => 8;

    private static Subject<Vector3> TapUp { get; } = new Subject<Vector3>();
    public static IObserver<Vector3> TapUpAsObserver => TapUp;

    public static Vector3 TapDownPoint { get; set; }
    private static void Tap(Vector3 point) => TapUp.OnNext(TapDownPoint = point);

    private static Subject<IEnumerable<RaycastHit>> TapRaycastHits { get; } = new Subject<IEnumerable<RaycastHit>>();

    public static bool[] IsTapSkills = new bool[ActionSize];

    public static float time = 0f;

    public static void Subscribe(MonoBehaviour mono)
    {
        mono.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Where(_ => EventSystem.current.currentSelectedGameObject == null)
            .Subscribe(_ => TapDownPoint = Input.mousePosition)
            .AddTo(mono);

        mono.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonUp(0))
            .Where(_ => EventSystem.current.currentSelectedGameObject == null)
            .Subscribe(_ => TapUp.OnNext(Input.mousePosition))
            .AddTo(mono);

        TapUp
            .Where(point => Vector3.Distance(TapDownPoint, point) <= 24f)
            .Select(point => Camera.main.ScreenPointToRay(point))
            .Select(ray => Physics.RaycastAll(ray, 1000f))
            .Select(hits => hits.Where(hit => hit.collider != null))
            .Subscribe(hits => TapRaycastHits.OnNext(hits))
            .AddTo(mono);

        mono.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(1))
            .Subscribe(_ => Tap(Input.mousePosition))
            .AddTo(mono);

        TapRaycastHits
            .Select(hits =>
            {
                var hitGrounds = hits.Where(hit => hit.collider.tag.Split('/').Any(tag => tag == "Ground"));
                var hitTargets = hits.Where(hit => hit.collider.tag.Split('/').Any(tag => tag == "Character"));
                if (hitGrounds.Any() && hitTargets.Any())
                {
                return new Command(hitGrounds.First().point, hitTargets.First().collider.gameObject);
                }
                else if (hitGrounds.Any())
                {
                    return new Command(hitGrounds.First().point, null);
                }
                else if (hitTargets.Any())
                {
                    var target = hitTargets.First().collider.gameObject;
                    Debug.Log(target);
                    return new Command(target.transform.position, target);
                }
                else
                {
                    return null;
                }
            })
            .Where(command => command != null)
            .Subscribe(command => OnNext(command))
            .AddTo(mono);

        mono.UpdateAsObservable().Subscribe(_ =>
        {
            for (int i = 0; i < ActionTypes.Count() && i < ActionButtonManager.Buttons.Count(); i++)
            {
                var keyCode = KeyCode.Alpha1 + i;
                var isTapSkill = IsTapSkills[i];

                if (Input.GetKeyDown(keyCode) && !isTapSkill)
                {
                    OnNext(ActionTypes[i]);
                }

                if (Input.GetKeyDown(keyCode) && isTapSkill)
                {
                    OnNext(ActionTypes[i]);
                    Tap(Input.mousePosition);
                }
            }
        });

        ActionTypes = new ActionType[]
        {
            ActionType.NormalPulse,
            ActionType.None,
            ActionType.None,
            ActionType.None,
            ActionType.None,
            ActionType.None,
            ActionType.None,
            ActionType.None,
        };

        for (int i = 0; i < ActionTypes.Count() && i < ActionButtonManager.Buttons.Count(); i++)
        {
            var type = ActionTypes[i];

            ActionButtonManager.Buttons[i]?.OnClickAsObservable()
                .Subscribe(_ => OnNext(type));
        }
    }

    private static void OnNext(Command command)
    {
        Vector3 from = Character.transform.position;
        Vector3 to = command.Point;

        if (command.Target == null)
        {
            RoomTurn.Request($"command {from.x} {from.z} {to.x} {to.z}");
        }
        else
        {
            var target = command.Target.GetComponent<Character>();
            if (target != null)
            {
                RoomTurn.Request($"command {from.x} {from.z} {to.x} {to.z} {target.ID}");
            }
        }

        Character.CommandAction(command);
    }


    public static string Path => $"{Application.persistentDataPath}/player2.txt";

    public static void Setup(params string[] paramaters)
    {
        using (StreamWriter sw = File.CreateText(Path))
        {
            paramaters.ToList().ForEach(paramater =>
            {
                sw.WriteLine(paramater);
            });
        }
    }

    public static bool Load()
    {
        var lines = File.ReadLines(Path).ToArray();

        if (lines.Length != 2)
        {
            return false;
        }

        var modelName = lines[0];
        var characterName = lines[1];

        ModelType model;
        GameObject prefab;

        if (Enum.TryParse<ModelType>(modelName, out model) &&
            ObjectManager.PlayerModels.TryGetValue(model, out prefab))
        {
            Model = model;
            Character = UnityEngine.Object.Instantiate(prefab).AddComponent<Character>();
            Character.Initialize<HostActor>(Login.NumberIndex, characterName);
            Character.transform.position = Vector3.zero;

            float x = Character.transform.position.x;
            float z = Character.transform.position.z;

            LoginTurn.Request();

            return true;
        }
        else
        {
            return false;
        }
    }

    private static void OnNext(ActionType type)
    {
        Character.ActionManager.Action(type);
        Debug.Log($"action {(int)type}");
        var bytes = System.Text.Encoding.ASCII.GetBytes($"action {(int)type}");
        Debug.Log(bytes.Length);
        var message = System.Text.Encoding.ASCII.GetString(bytes);
        Debug.Log(message);
        RoomTurn.Request($"action {((int)type).ToString()}");
    }

    public static ActionType[] ActionTypes { get; private set; }
}
