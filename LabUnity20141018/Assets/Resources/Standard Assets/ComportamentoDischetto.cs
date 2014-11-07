using UnityEngine;
using System.Collections;

public class ComportamentoDischetto : MonoBehaviour 
{
    private Vector3 posizioneIniziale, posizioneFinale;
    private float tempoIniziale;
    private float velocita = 100.0F;
    private float lunghezzaViaggio;
    private ForzaQuattroRiscritto fqr;
    private bool fermo, vincitore;

	void Start () 
    {
        fqr = Camera.main.GetComponent<ForzaQuattroRiscritto>();
        fermo = false;
        vincitore = false;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!fermo)
        {
            float distCovered = (Time.time - tempoIniziale) * velocita;
            float fracJourney = distCovered / lunghezzaViaggio;
            if (fracJourney < 1.1f)
            {
                transform.position = Vector3.Lerp(posizioneIniziale, posizioneFinale, fracJourney);
            }
            else if (velocita != 0)
            {
                fqr.DischettoFermo();
                fermo = true;
            }
        }
        else if(vincitore)
        {
            transform.localScale = new Vector3(Mathf.PingPong(Time.time, 0.5f) + 1f, Mathf.PingPong(Time.time, 0.5f) + 1f, 1);
        }
	}

    public void SetStartAndEndPosition(Vector3 start, Vector3 end)
    {
        posizioneIniziale = start;
        posizioneFinale = end;

        lunghezzaViaggio = Vector3.Distance(posizioneIniziale, posizioneFinale);

        tempoIniziale = Time.time;
        transform.position = posizioneIniziale;
    }

    public void Vincitore()
    {
        vincitore = true;
    }
}
