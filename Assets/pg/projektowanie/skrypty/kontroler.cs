using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kontroler : MonoBehaviour
{ 
    public int etap = 0;
    public bool newroom = false;
    public bool newwindow = false;
    public bool newdoor = false;
    public bool newfrontdoor = false;

    public int WindowWidth = 5;
    public int DoorWidth = 4;

    public GameObject anuluj;
    public GameObject next;
    public GameObject nowedrzwi;
    public GameObject noweOkno;
    public GameObject nowedrzwifront;
    public GameObject nowypokoj;

    public void NewRoom(bool onof) {
        newroom = onof;
        anuluj.SetActive(onof);
        nowypokoj.SetActive(!onof);
        next.SetActive(false);
        if (onof == false && etap != 0) {
            next.SetActive(true);
        }
    }

    public void NewWindow(bool onof)
    {
        newwindow = onof;
        anuluj.SetActive(onof);
        next.SetActive(!onof);
        nowedrzwi.SetActive(!onof);
        nowedrzwifront.SetActive(!onof);
    }

    public void NewDoor(bool onof)
    {
        newdoor = onof;
        anuluj.SetActive(onof);
        next.SetActive(!onof);
        noweOkno.SetActive(!onof);
        nowedrzwifront.SetActive(!onof);
    }

    public void NewFrontDoor(bool onof)
    {
        newfrontdoor = onof;
        anuluj.SetActive(onof);
        next.SetActive(!onof);
        noweOkno.SetActive(!onof);
        nowedrzwi.SetActive(!onof);
    }
}
