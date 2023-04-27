using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatesManager : MonoBehaviour
{
    public void ChangeScenesByName(string name)
    {
        if (name != null)
        {
            SceneManager.LoadScene(name);
            Time.timeScale = 1;
        }
    }
}
