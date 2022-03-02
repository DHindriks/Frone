using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void SwitchScene(string SceneToLoad)
    {
        SceneManager.LoadScene(SceneToLoad);
    }

    public void RemoveSaves()
    {
        PlayerPrefs.DeleteAll();
    }

}
