using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class Sciana
{
    [SerializeField]
    public string okna { get; set; }
    [SerializeField]
    public string drzwi { get; set; }
    [SerializeField]
    public string drzwifront { get; set; }
}

[System.Serializable]
class Wierzcholki
{
    [SerializeField]
    public int x { get; set; }
    [SerializeField]
    public int y { get; set; }
}

[System.Serializable]
class Pokoj
{
    [SerializeField]
    public Wierzcholki[] wierzcholki { get; set; }
    //public Sciana[] sciany { get; set; }
    
    public Pokoj()
    {
        // sciany = new Sciana[4];
        wierzcholki = new Wierzcholki[4];
    }
}

[System.Serializable]
class Dane
{
    public int[] wierzcholki;
    public string[] sciany;

    public Dane(int x) {
        wierzcholki = new int[x*4*2];
        sciany = new string[x * 4];
    }
}

public class RoomToJson : MonoBehaviour
{
    public int a;
    public void PrepareJson() {
        
        //dane.pokoje = new List<Pokoj>();
        var listapokoji = FindObjectsOfType<Room>();
        a = listapokoji.Length; 
        Dane dane = new Dane(a);
        Debug.Log(listapokoji.Length);

        int tmp = 0;
        foreach (var room in listapokoji)
        {
            for (int i = 0; i < 4; i++, tmp+=2)
            {
                dane.wierzcholki[tmp]= room.PunktyTablica[i, 0];
                dane.wierzcholki[tmp+1] = room.PunktyTablica[i, 1];
            }
        }
        Debug.Log(JsonUtility.ToJson(dane));
    }
}
