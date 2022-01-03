using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Next : MonoBehaviour
{
    public GameObject Game_kontroler;
    public GameObject noweOkno;
    public GameObject newRoom;
    public GameObject newdoor;
    public GameObject newdoorfront;

    private void OnMouseDown() {
        if (Game_kontroler.GetComponent<kontroler>().etap == 1)
        {
            noweOkno.SetActive(true);
            newRoom.SetActive(false);
            newdoor.SetActive(true);
            newdoorfront.SetActive(true);
            Game_kontroler.GetComponent<kontroler>().etap = 2;
            this.GetComponent<TextMesh>().text = "Generuj świat";
        }
        else if (Game_kontroler.GetComponent<kontroler>().etap == 2) {
            FindObjectOfType<RoomToJson>().PrepareJson();
        }
    }
}
