using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anuluj : MonoBehaviour
{
    public GameObject Game_kontroler;

    private void OnMouseDown() {

        this.gameObject.SetActive(false);

        var kontroler_tmp = Game_kontroler.GetComponent<kontroler>();
        if (kontroler_tmp.newdoor == true || kontroler_tmp.newwindow == true || kontroler_tmp.newfrontdoor == true)
        {
            kontroler_tmp.nowedrzwi.gameObject.SetActive(true);
            kontroler_tmp.noweOkno.gameObject.SetActive(true);
            kontroler_tmp.next.gameObject.SetActive(true);
            kontroler_tmp.nowedrzwifront.gameObject.SetActive(true);
        }
        else if (kontroler_tmp.newroom == true) {
            Game_kontroler.GetComponent<kontroler>().newroom = false;
            var rooms = FindObjectsOfType<Room>();
            foreach (var test in rooms) {
                if (test.isCreated == false) {
                    Destroy(test.gameObject);
                    Game_kontroler.GetComponent<kontroler>().NewRoom(false);
                    break;
                }
            }
        }
    }
}
