using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Latte;


public class Title : MonoBehaviour
{
    private void Start()
    {
        Server.Receive(NetworkConfig.Host, NetworkConfig.Port);

        GetComponent<Button>().OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (File.Exists(Host.Path))
                {
                    File.Delete(Host.Path);
                    SceneManager.LoadScene("Setup");
                    //SceneManager.LoadScene("Login");
                }
                else
                {
                    SceneManager.LoadScene("Setup");
                }
            });
    }


}
