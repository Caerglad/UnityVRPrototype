using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class punkt : MonoBehaviour
{
    public kontroler Game_kontroler;
    public NowyPokoj nowypokoj;
    public bool isCorner = false;
    public bool isWall = false;
    public bool isWindow = false;
    public bool isDoubleWall = false;
    public bool isDoor = false;
    public bool isFrontDoor = false;
    public int x;
    public int y;
    private int kierunek=2;

    punkty punktyCls;

    public Material mokno;
    public Material mdrzwi;
    public Material mdrzwifront;

    void Start()
    {
        Game_kontroler = FindObjectOfType<kontroler>();
        nowypokoj = FindObjectOfType<NowyPokoj>();
        punktyCls = FindObjectOfType<punkty>();
    }

    private void CreateWindow() {
        if (Game_kontroler.newwindow == false)
            return;

        for (int i = 0; i < Game_kontroler.WindowWidth; i++)
        {
            if (kierunek == 0) {punktyCls.tablica_punktow[x + i, y].isWindow = true;}
            else if (kierunek == 1){punktyCls.tablica_punktow[x, y + i].isWindow = true;}
        }
        kierunek = 2;
    }

    private void CreateDoor() {
        if (Game_kontroler.newdoor == false)
            return;

        for (int i = 0; i < Game_kontroler.DoorWidth; i++)
        {
            if (kierunek == 0){punktyCls.tablica_punktow[x + i, y].isDoor = true;}
            else if (kierunek == 1){punktyCls.tablica_punktow[x, y + i].isDoor = true;}
        }
        kierunek = 2;
    }

    private void CreateFrontDoor()
    {
        if (Game_kontroler.newfrontdoor == false)
            return;

        for (int i = 0; i < Game_kontroler.DoorWidth; i++)
        {
            if (kierunek == 0) { punktyCls.tablica_punktow[x + i, y].isFrontDoor = true; }
            else if (kierunek == 1) { punktyCls.tablica_punktow[x, y + i].isFrontDoor = true; }
        }
        kierunek = 2;
    }

    private void OnMouseDown()
    {
        if (Game_kontroler.newroom == true && nowypokoj.counter < 4 && GetComponent<SpriteRenderer>().color != Color.red) {
            GetComponent<SpriteRenderer>().color = Color.red;
            nowypokoj.addCorner(x,y);
            isCorner = true;
        }
        else if (Game_kontroler.newwindow == true && kierunek!=2) {
            CreateWindow();
            Game_kontroler.NewWindow(false);
        }

        else if (Game_kontroler.newdoor == true && kierunek != 2)
        {
            CreateDoor();
            Game_kontroler.NewDoor(false);
        }
        else if (Game_kontroler.newfrontdoor == true && kierunek != 2)
        {
            CreateFrontDoor();
            Game_kontroler.NewFrontDoor(false);
        }
    }

    private void PlaceOnOff(int kierunek, bool onof, string new_o) { //kierunek 0 -os X 1- os Y

        Material materialo=null;
        int width = 0;
        if (new_o == "drzwi")
        {
            materialo = mdrzwi;
            width = Game_kontroler.DoorWidth;
        }
        else if (new_o == "okno")
        {
            materialo = mokno;
            width = Game_kontroler.WindowWidth;
        }
        else if (new_o == "drzwi_front") {
            materialo = mdrzwifront;
            width = Game_kontroler.DoorWidth;
        }

        for (int i = 0; i < width; i++) {
            if (kierunek == 0)
            {
                var test = punktyCls.tablica_punktow[x + i, y].gameObject.transform.GetChild(0);

                if (onof == true)
                {
                    test.gameObject.SetActive(true);
                    test.gameObject.GetComponent<MeshRenderer>().material = materialo;
                    this.kierunek = kierunek;
                }
                else
                {
                    test.gameObject.SetActive(false);
                }
            }
            else if (kierunek == 1)
            {
                var test = punktyCls.tablica_punktow[x, y + i].gameObject.transform.GetChild(0);
                if (onof == true)
                {
                    test.gameObject.SetActive(true);
                    test.gameObject.GetComponent<MeshRenderer>().material = materialo;
                    this.kierunek = kierunek;
                }
                else
                {
                    test.gameObject.SetActive(false);
                }
            }
            else {
                this.kierunek = 2;
            }
        }
    }


    private bool TestPunkt(punkt test,string new_o) {
        if (new_o == "okno" || new_o== "drzwi_front")
        {
            if (test.isWall == false || test.isDoubleWall == true || test.isWindow == true || test.isCorner == true || test.isDoor == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else if (new_o == "drzwi")
        {
            if (test.isWall == false || test.isDoubleWall == false || test.isWindow == true || test.isCorner == true || test.isDoor == true)
            {
                return false;
            }
            else {
                return true;
            }
        }
        else {
            return false;
        }
    }

    private void CheckPlaceFor(string new_o) {
        Debug.Log("CheckPlaceFor "+new_o);
        int width=0;
        if (new_o == "okno") {
            width = Game_kontroler.WindowWidth;
        }
        else if(new_o=="drzwi" || new_o== "drzwi_front"){
            width = Game_kontroler.DoorWidth;
        }

        bool sprawdzenie = true;
        for (int i = 1; i < width; i++) {
            punkt test = punktyCls.tablica_punktow[x + i, y];
            if (TestPunkt(test, new_o) == false) {
                sprawdzenie = false;
                break;
            }
        }
        if (sprawdzenie == false)
        {
            sprawdzenie = true;
            for (int i = 1; i < width; i++)
            {
                punkt test = punktyCls.tablica_punktow[x, y + i];
                if (TestPunkt(test, new_o) == false)
                {
                    sprawdzenie = false;
                    break;
                }
            }
            if(sprawdzenie){
                PlaceOnOff(1, true, new_o);
            }
        }
        else {
            PlaceOnOff(0, true, new_o);
        }    
    }

    public void OnMouseOver() {
        if (Game_kontroler.newwindow == true && isCorner == false && isDoubleWall == false && isWindow == false && isWall== true && isDoor == false) {
            CheckPlaceFor("okno");
        }
        else if (Game_kontroler.newdoor == true && isCorner == false && isDoubleWall == true && isWindow == false && isWall == true && isDoor == false)
        {
            CheckPlaceFor("drzwi");
        }
        else if (Game_kontroler.newfrontdoor == true && isCorner == false && isDoubleWall == false && isWindow == false && isWall == true && isDoor == false)
        {
            CheckPlaceFor("drzwi_front");
        }
    }

    private void CheckOff(GameObject test, string obiekt) {
        test.SetActive(false);
        PlaceOnOff(0, false, obiekt);
        PlaceOnOff(1, false, obiekt);
        kierunek = 2;
    }

    public void OnMouseExit()
    {
        if (Game_kontroler.newwindow == true) {
            var test = this.gameObject.transform.GetChild(0);
            if (test.gameObject.activeSelf == true && isWindow == false&& kierunek != 2)
            {
                CheckOff(test.gameObject,"okno");
            }
        }

        if (Game_kontroler.newdoor == true)
        {
            var test = this.gameObject.transform.GetChild(0);
            if (test.gameObject.activeSelf == true && isDoor == false && kierunek != 2)
            {
                CheckOff(test.gameObject,"drzwi");
            }
        }

        if (Game_kontroler.newfrontdoor == true)
        {
            var test = this.gameObject.transform.GetChild(0);
            if (test.gameObject.activeSelf == true && isDoor == false && kierunek != 2)
            {
                CheckOff(test.gameObject, "drzwi_front");
            }
        }
    }
}
