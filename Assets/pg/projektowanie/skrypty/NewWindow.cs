using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWindow : MonoBehaviour
{
    public kontroler Game_kontroler;
    void Start()
    {
        Game_kontroler = FindObjectOfType<kontroler>();
    }
    private void OnMouseDown()
    {
        Game_kontroler.NewWindow(true);
        Debug.Log("dodawanie okna");
    }
}
