using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class punkty : MonoBehaviour
{
    public GameObject single_point;
    public int count_x;
    public int count_y;
    public float distance;

    private float start_x;
    private float start_y;

    public punkt[,] tablica_punktow;

    void Start()
    {
        start_x = this.transform.position.x;  
        start_y = this.transform.position.y;
       
        tablica_punktow = new punkt[count_x, count_y];

        for (int i = 0; i< count_x; i++) {
            for (int k = 0; k< count_y; k++) { 
                var new_point = Instantiate(single_point);
                new_point.transform.position = new Vector3((float)start_x+i*distance, (float)start_y-k*distance,0);
                new_point.GetComponent<punkt>().x = i;
                new_point.GetComponent<punkt>().y = k;
                tablica_punktow[i,k] = new_point.GetComponent<punkt>();
            }
        }

    }
}
