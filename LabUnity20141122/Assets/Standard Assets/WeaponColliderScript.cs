using UnityEngine;
using System.Collections;

public class WeaponColliderScript : MonoBehaviour {


    void OnTriggerEnter(Collider other)
    {
        if (transform.root.tag == "Eroe")
        {
            if (other.tag == "Enemy")
            {
                if (transform.root.GetComponent<Personaggio>().StoAttaccando())
                {
                    other.GetComponent<AINemico>().Colpito();
                }
            }
        }
        else if (transform.root.tag == "Enemy")
        {
            if (other.tag == "Eroe")
            {
                Debug.Log("COLPITO");
                if (transform.root.GetComponent<AINemico>().StoAttaccando())
                {
                    other.GetComponent<Personaggio>().Colpito();
                }
            }
        }
    }
}
