using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class next_Game : MonoBehaviour
{
    public string next;

    public void nextGame(){
        SceneManager.LoadScene(next);

    }
}
