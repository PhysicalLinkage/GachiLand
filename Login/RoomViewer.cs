using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Text;
using UnityEngine.UI;
using System.Linq;
using System;
using Latte;
using UnityEngine.SceneManagement;

public class RoomViewer : MonoBehaviour
{
    [SerializeField] GameObject button_prefab = null;

    private void Awake()
    {
        // スクロールする場所全体の形
        var scroll_rect = GetComponent<ScrollRect>();

        // コンテンツの全体の形
        var content_rect = scroll_rect.content.GetComponent<RectTransform>();

        var buttons = new List<GameObject>();
        float height = 0;

        World.AsObservable.Subscribe(_ =>
        {
            Debug.Log($"World size {World.Size}");
            Debug.Log($"World Numbers length {World.Numbers.Length}");
            buttons.ForEach(btn =>
            {
                btn.GetComponent<Button>().onClick.RemoveAllListeners();
                btn.SetActive(false);
            });

            Observable.Range(0, World.Numbers.Length).Subscribe(world_index =>
            {
                GameObject button_object = null;
                if (buttons.Count() > world_index)
                {
                    button_object = buttons[world_index];
                    button_object.SetActive(true);
                }
                else
                {
                    buttons.Add(button_object = Instantiate(button_prefab, scroll_rect.content.transform));

                    var new_button_rect = button_object.GetComponent<RectTransform>();
                    height = new_button_rect.sizeDelta.y;
                    new_button_rect.localPosition = new Vector3(new_button_rect.localPosition.x, -height * world_index);
                }

                var button = button_object.GetComponent<Button>();

                button.GetComponentInChildren<Text>().text = $"{World.Numbers[world_index]} / {World.Size}";

                button.OnClickAsObservable().Subscribe(__ =>
                {
                    var index = world_index;
                    Room.Request((UInt16)index);
                });
            });

            content_rect.sizeDelta = new Vector2(0, World.Numbers.Length * height);
        });

        Room.AsObservable.Subscribe(_ =>
        {

            buttons.ForEach(btn =>
            {
                btn.GetComponent<Button>().onClick.RemoveAllListeners();
                btn.SetActive(false);
            });

            Observable.Range(0, Room.Numbers.Length).Subscribe(room_index =>
            {
                GameObject button_object = null;
                if (buttons.Count() > room_index)
                {
                    button_object = buttons[room_index];
                    button_object.SetActive(true);
                }
                else
                {
                    buttons.Add(button_object = Instantiate(button_prefab, scroll_rect.content.transform));
                    var new_button_rect = button_object.GetComponent<RectTransform>();

                    new_button_rect.position = new Vector3(new_button_rect.localPosition.x, -height * room_index);
                }

                var new_button = button_object.GetComponent<Button>();

                new_button.GetComponentInChildren<Text>().text = $"{Room.Numbers[room_index]} / {Room.Size}";

                var button = new_button.GetComponent<Button>();

                button.OnClickAsObservable().Subscribe(__ =>
                {
                    Login.Request(Room.WorldIndex, (UInt16)room_index);
                });
            });
        });

        Login.AsObservable.Subscribe(_ =>
        {
            if (Login.IsSuccess)
            {
                SceneManager.LoadScene("Test");
            }
            else
            {
                UpdateRoom();
            }
        });
    }

    private void Start()
    {
        UpdateRoom();
    }

    public void UpdateRoom()
    {
        World.Request();
    }
}
