using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int[,] PunktyTablica=new int[4,2];


    public bool isCreated = false;
    private punkty punktyCls;

    void Start()
    {
        punktyCls = FindObjectOfType<punkty>();
    }

    private void ReplacePoint(int a, int b) {
        var tmp_a = PunktyTablica[a,0];
        var tmp_b = PunktyTablica[a, 1];

        PunktyTablica[a, 0] = PunktyTablica[b, 0];
        PunktyTablica[a, 1] = PunktyTablica[b, 1];

        PunktyTablica[b, 0] = tmp_a;
        PunktyTablica[b, 1] = tmp_b;
    }
    private void RepairMapDots() { 
        Debug.Log("wywołanie RepairMapDots w objekcie Room");
        foreach (punkt tmp in punktyCls.tablica_punktow)
        {
            if (tmp.x > PunktyTablica[0,0] && tmp.x < PunktyTablica[2,0] && tmp.y < PunktyTablica[0,1] && tmp.y > PunktyTablica[1,1]) { 
                //wyłączanie punktów wewnątrz pokoju
                GameObject asd = tmp.gameObject;
                asd.SetActive(false);
            }
            
            if ((tmp.x == PunktyTablica[0,0] && 
                tmp.y < PunktyTablica[0,1] && 
                tmp.y > PunktyTablica[1,1]) ||
                (tmp.x == PunktyTablica[2,0] &&
                tmp.y < PunktyTablica[0,1] &&
                tmp.y > PunktyTablica[1,1]) ||
                (tmp.y == PunktyTablica[0,1] &&
                tmp.x > PunktyTablica[0,0] &&
                tmp.x <= PunktyTablica[2,0]) ||
                (tmp.y == PunktyTablica[1,1] &&
                tmp.x > PunktyTablica[0,0] &&
                tmp.x < PunktyTablica[2,0])
                ) {

                GameObject asd = tmp.gameObject;
                if (asd.GetComponent<punkt>().isWall == true)
                    asd.GetComponent<punkt>().isDoubleWall = true;
                else {
                    asd.GetComponent<punkt>().isWall = true;
                }

                if (asd.GetComponent<punkt>().isCorner != true)
                    asd.GetComponent<SpriteRenderer>().color = Color.green;

            }
            if ((tmp.x == PunktyTablica[0, 0] && tmp.y == PunktyTablica[0, 1]) ||
                (tmp.x == PunktyTablica[1, 0] && tmp.y == PunktyTablica[1, 1]) ||
                (tmp.x == PunktyTablica[2, 0] && tmp.y == PunktyTablica[2, 1]) ||
                (tmp.x == PunktyTablica[3, 0] && tmp.y == PunktyTablica[3, 1])) {

                GameObject asd = tmp.gameObject;
                asd.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
        }
    }
        

    public void RepairPoints() { //sortowanie punktów według założeń
        Debug.Log("wywołanie RapeirPoint w objekcie Room");
        //punkt 0  -> v2(min,max)
        for (int i = 1; i < 4; i++) {
            if (PunktyTablica[0,0] > PunktyTablica[i,0])
            {
                ReplacePoint(0, i);
            }
            else if (PunktyTablica[0,0] == PunktyTablica[i,0] && PunktyTablica[0,1] < PunktyTablica[i,1]) {
                ReplacePoint(0, i);
            }
        }

        //punkt 1  -> v2(min,min)
        for (int i = 2; i < 4; i++)
        {
            if (PunktyTablica[1,0] > PunktyTablica[i,0])
            {
                ReplacePoint(1, i);
            }
            else if (PunktyTablica[1,0] == PunktyTablica[i,0] && PunktyTablica[1,1] > PunktyTablica[i,1])
            {
                ReplacePoint(1, i);
            }
        }

        //punkt 3 i 4 -> v2(max,max), v2(max,min)
        if (PunktyTablica[2,1] < PunktyTablica[3,1])
            ReplacePoint(2, 3);

        RepairMapDots();
    }
}
