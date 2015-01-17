using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

    public GameObject start;
    public GameObject option;

    private float width, height;
    private float rapporto, spostamento, distanza;
    private RaycastHit hit;
    private Ray ray;
	void Start () {

        distanza = 3;
	
	}
	void Update () {

        //Reset offset
        start.transform.renderer.material.mainTextureOffset = new Vector2(0, 0);
        option.transform.renderer.material.mainTextureOffset = new Vector2(0, 0);

        //prendo le misure dello schermo e calcolo il rapporto
        width = Screen.width;
        height = Screen.height;

        //if(width > height)
            rapporto = width / height;
        //else
        //    rapporto = height / width;

        Debug.Log(rapporto);
        //Calcolo lo spostamento 
        spostamento = (rapporto - .75f)*2;
        Debug.Log(spostamento);

        //Lo applico ai bottoni
        start.transform.localPosition = new Vector3(-0.5f - spostamento, 0, distanza);
        option.transform.localPosition = new Vector3(0.5f + spostamento, 0, distanza);

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit, 100f))
        {
            hit.transform.renderer.material.SetTextureOffset("_MainTex", new Vector2(0, .5f));
            if (Input.GetButton("Fire1"))
            {
                if (hit.collider.name == "start")
                {
                    Debug.Log("Carica Scena");
                }
                else if (hit.collider.name == "option")
                {
                    Debug.Log("Apri opzioni");
                }
            }
        }
	
	}
}
