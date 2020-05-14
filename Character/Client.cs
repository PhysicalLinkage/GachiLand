using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Linq;
using System;
using UniRx;
using Latte;
using UnityEngine.UI;
using UnityEngine;

public class Client
{
    public static int Max = 3;
    public static Dictionary<UInt32, Client> Instances { get; } = new Dictionary<UInt32, Client>();

    public UInt32 ID { get; private set; }
    public Character Character { get; private set; }

    public Client(UInt32 id, Character character)
    {
        ID = id;
        Character = character;
    }

    public static void Subscribe()
    {
        RoomTurn.AsObservable.Subscribe(turn =>
        {
            Client client;

            if (!Instances.TryGetValue(turn.Destination, out client)) return;

            var messages = turn.Message.Split(' ');

            if (messages.Length < 1) return;

            if (messages[0] == "command" && messages.Length == 5)
            {
                float from_x, from_z, to_x, to_z;

                if (float.TryParse(messages[1], out from_x) &&
                    float.TryParse(messages[2], out from_z) &&
                    float.TryParse(messages[3], out to_x) &&
                    float.TryParse(messages[4], out to_z))
                {
                    Debug.Log($"(x, z) : ({to_x}, {to_z})");
                    var from = new Vector3(from_x, 0f, from_z);
                    var character = client.Character;
                    if (Vector3.Distance(client.Character.Rigidbody.position, from) < (character.Collider.bounds.size.z * character.transform.localScale.z / 4f))
                    {
                        client.Character.Rigidbody.MovePosition(new Vector3(from_x, 0f, from_z));
                    }
                    client.Character.CommandAction(new Command(new Vector3(to_x, 0f, to_z)));
                }
            }

            else if (messages[0] == "command" && messages.Length == 6)
            {
                float from_x, from_z, to_x, to_z;
                UInt32 id;
                Character target;

                if (float.TryParse(messages[1], out from_x) &&
                    float.TryParse(messages[2], out from_z) &&
                    float.TryParse(messages[3], out to_x) &&
                    float.TryParse(messages[4], out to_z) &&
                    UInt32.TryParse(messages[5], out id) &&
                    Character.Instances.TryGetValue(id, out target))
                {
                    var from = new Vector3(from_x, 0f, from_z);
                    var character = client.Character;
                    if (Vector3.Distance(client.Character.Rigidbody.position, from) < (character.Collider.bounds.size.z * character.transform.localScale.z / 4f))
                    {
                        client.Character.Rigidbody.MovePosition(new Vector3(from_x, 0f, from_z));
                    }
                    client.Character.CommandAction(new Command(new Vector3(to_x, 0f, to_z), target.gameObject));
                }
            }

            if (messages[0] == "action" && messages.Length == 2)
            {
                int actionType;

                if (int.TryParse(messages[1], out actionType))
                {
                    Debug.Log(actionType.ToString());
                    client.Character.ActionManager.Action((ActionType)actionType);
                }
            }

            if (messages[0] == "chat" && messages.Length > 1)
            {
                messages[0] = $"{client.Character.name}({client.ID}):";
                var msg = String.Join(" ", messages);

                var chats = ChatField.ChatText.text.Split('\n').ToList();
                chats.Add(msg);
                if (chats.Count > 8)
                {
                    ChatField.ChatText.text = String.Join("\n", chats.Skip(1));
                }
                else
                {
                    ChatField.ChatText.text = String.Join("\n", chats);

                }
            }

            if (turn.Message == "logout\0")
            {
                Debug.Log($"logout {client.ID}");
                Instances.Remove(client.ID);
                client.Character.Destroy();
            }

        });
    }

    public static bool Load(UInt32 id, UInt32 characterID, ModelType model_type, string name, Vector3 position)
    {
        if (Instances.Count() >= Max)
        {
            return false;
        }

        if (Instances.ContainsKey(id))
        {
            return false;
        }

        GameObject prefab;

        if (ObjectManager.PlayerModels.TryGetValue(model_type, out prefab))
        {
            var character = UnityEngine.Object.Instantiate(ObjectManager.PlayerModels[model_type])
                .AddComponent<Character>()
                .Initialize<ClientActor>(characterID, name);
            character.transform.position = position;
            Instances[id] = new Client(id, character);
        }

        return true;
    }
}
