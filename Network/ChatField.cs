using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Latte;
using System;
using System.Linq;

public class ChatField : MonoBehaviour
{
    InputField m_inputField;
    [SerializeField] Text chat_text;

    public static Text ChatText;

    private void Awake()
    {
        m_inputField = GetComponent<InputField>();
        ChatText = chat_text;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (m_inputField.text == "")
            {
                m_inputField.ActivateInputField();
                m_inputField.text = "";
            }
            else if (m_inputField.text == "\n")
            {
                m_inputField.DeactivateInputField();
                m_inputField.text = "";
            }
            else
            {
                if (m_inputField.text.Any(c => c == '\n'))
                {
                    Chat(m_inputField.text);
                }
            }
        }
    }

    public void Chat(string text)
    {
        text = text.Remove(text.Length - 1);

        if (text != "")
        {
            RoomTurn.Request($"chat {text}");
            m_inputField.text = "";

            var chats = ChatText.text.Split('\n').ToList();
            chats.Add($"{Host.Character.name}: {text}");
            if (chats.Count > 8)
            {
                ChatText.text = String.Join("\n", chats.Skip(1));
            }
            else if (ChatText.text == "")
            {
                ChatText.text = chats[1];
            }
            else
            {
                ChatText.text = String.Join("\n", chats);
            }
        }
    }
}
