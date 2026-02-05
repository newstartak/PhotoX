using UnityEngine;
using System.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;

public class testScript : MonoBehaviour
{
    private void Start()
    {
        _ = StartAsync();
    }

    async Task StartAsync()
    {
        int curIndex = SceneManager.GetActiveScene().buildIndex;

        for(int i = 0; i < 100; i++)
        {
            Debug.Log(i);

            await Task.Delay(1000);

            if(curIndex != SceneManager.GetActiveScene().buildIndex)
            {
                break;
            }
        }
    }
}