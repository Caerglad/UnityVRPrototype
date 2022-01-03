using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFrontDoor : MonoBehaviour
{
    public kontroler Game_kontroler;
    void Start()
    {
        Game_kontroler = FindObjectOfType<kontroler>();
    }
    private void OnMouseDown()
    {
        Game_kontroler.NewFrontDoor(true);
        Debug.Log("dodawanie drzwi frontowych");
    }
}
