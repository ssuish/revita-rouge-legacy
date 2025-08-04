using System;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneChange : MonoBehaviour
{
    public int SceneIndex;
    
    //change of scenes
    private void OnTriggerEnter2D(Collider2D other)
    {
        print("Trigger Enter");

        if (other.tag == "Player")
        {
            print("Swithching to " + SceneIndex);
            SceneManager.LoadScene(SceneIndex, LoadSceneMode.Single);
        }
    }
}
