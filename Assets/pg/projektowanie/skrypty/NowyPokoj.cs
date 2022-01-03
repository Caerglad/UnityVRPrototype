using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NowyPokoj : MonoBehaviour
{
    public GameObject room_Object;
    public GameObject room_prefab;
    public kontroler Game_kontroler;
    public int counter = 0;
    void Start()
    {
        Game_kontroler = FindObjectOfType<kontroler>();
    }

    private void OnMouseDown()
    {
        if (Game_kontroler.newroom == true)
            return;

        Game_kontroler.NewRoom(true);
        counter = 0;
        Debug.Log("dodawanie lokoju");
        Instantiate(room_prefab);
        
    }
    public void addCorner(int x, int y) {
        if (counter < 4) {
            var tmp = FindObjectsOfType<Room>();
            foreach (var i in tmp)
            {
                if (i.isCreated == false)
                {
                    room_Object = i.gameObject;
                    break;
                }
            }

            room_Object.GetComponent<Room>().PunktyTablica[counter,0] = x;
            room_Object.GetComponent<Room>().PunktyTablica[counter,1] = y;
            Debug.Log("dodanie punktu " + x +"    "+y);
            counter++;
        }

        if (counter == 4)
        {
            room_Object.GetComponent<Room>().isCreated = true;
            Game_kontroler.etap = 1;
            room_Object.GetComponent<Room>().RepairPoints();
            room_Object = null;
            counter = 0;
            Game_kontroler.NewRoom(false);
        }
            
    }
}
