using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {

    public Personaggio eroe;
    public AINemico nemico;

    void Start()
    {
    }

    public void ColpisciNemico()
    {
        nemico.Colpito();
    }

    public void ColpisciEroe()
    {
        eroe.Colpito();
    }


    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 120, 60), "");
        GUI.Label(new Rect(20, 20, 100, 30), "Vita Eroe: " + eroe.GetVita());
        GUI.Label(new Rect(20, 40, 100, 30), "Vita Orco: " + nemico.GetVita());
    }
}
