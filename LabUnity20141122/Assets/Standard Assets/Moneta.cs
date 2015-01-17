using UnityEngine;
using System.Collections;

public class Moneta : MonoBehaviour {

    float angle;
    float oscilla;

    public GameObject eroe;
    private Vector3 posizioneIniziale;

    void Start()
    {
        posizioneIniziale = transform.position;
    }
	
	// Update is called once per frame
	void Update () {

        angle = angle + 500 * Time.deltaTime;
        transform.localEulerAngles = new Vector3(0, angle, 0);

        oscilla = oscilla + 5f * Time.deltaTime; ;
        transform.position = new Vector3(posizioneIniziale.x, posizioneIniziale.y + Mathf.Sin(oscilla) * 0.5f + 0.5f, posizioneIniziale.z);

        float distanza = (transform.position - eroe.transform.position).sqrMagnitude;
        

        if (distanza < 1)
        {
            DestroyImmediate(transform.gameObject);
        }
	
	}
}
