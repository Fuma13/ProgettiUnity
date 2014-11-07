using UnityEngine;
using System.Collections;

public class ForzaQuattroRiscritto : MonoBehaviour {

    private const int Max_Righe = 8;
    private const int Max_Colonne = 9;
    private const int Max_Punti = 4;
    private const int Max_Celle = Max_Righe * Max_Colonne;

    private struct Dischetto
    {
        public TipoDischetto tipo;
        public GameObject oggetto;
    }

    private Dischetto[] griglia = new Dischetto[Max_Celle];
    private int[] riempimento_colonna = new int[Max_Colonne];
    private int[] indici_dischetti_vincitori = new int[Max_Punti];

    private enum StatoDelGioco
    {
        IN_ATTESA = 0,
        INSERIMENTO_DISCHETTO = 1,
        VINCITORE = 2,
        POST_VINCITORE = 3
    }

    private enum TipoDischetto
    {
        NESSUNO = -1,
        GIOCATORE_0 = 0,
        GIOCATORE_1 = 1
    }

    private StatoDelGioco stato_gioco;
    private TipoDischetto vittoria;
    private TipoDischetto colore;

	// Use this for initialization
	void Start () 
    {
        stato_gioco = StatoDelGioco.IN_ATTESA;
        vittoria = TipoDischetto.NESSUNO;
        colore = TipoDischetto.GIOCATORE_0;

        GameObject oggetto;
        for (int n = 0; n < Max_Colonne; n++)
        {
            oggetto = Instantiate(Resources.Load("struttura_click2")) as GameObject;
            oggetto.transform.position = new Vector3(90 - n * 10, 0, 0); //TODO rivedere il posizionamento

            oggetto.renderer.enabled = false;
            oggetto.name = ""+ n;

            oggetto = Instantiate(Resources.Load("struttura_utile")) as GameObject;
            oggetto.transform.position = new Vector3(90 - n * 10, 0, 0);

        }

        for(int n = 0; n < Max_Celle; n++)
        {
            griglia[n].tipo = TipoDischetto.NESSUNO;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        ControlloClick();

        if(stato_gioco == StatoDelGioco.VINCITORE)
        {
            stato_gioco = StatoDelGioco.POST_VINCITORE;
            for(int n = 0; n < Max_Punti; n++)
            {
                griglia[indici_dischetti_vincitori[n]].oggetto.GetComponent<ComportamentoDischetto>().Vincitore();
            }
        }
	
	}

    void OnGUI()
    {
        if (stato_gioco == StatoDelGioco.POST_VINCITORE)
        {
            GUI.Box(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 75, 300, 150), "");
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 40), "Il vincitore è il giocatore: " + (vittoria == TipoDischetto.GIOCATORE_1 ? "Blu" : "Rosso"));

            if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 10, 200, 40), "Ricomincia"))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }
    }

    void ControlloClick()
    {
        if (stato_gioco == StatoDelGioco.IN_ATTESA && Input.GetMouseButton(0) && vittoria == TipoDischetto.NESSUNO)
        {
            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray2, out hit, 1000))
            {
                int n = int.Parse(hit.collider.name);
                InserisciDischetto(n);
                stato_gioco = StatoDelGioco.INSERIMENTO_DISCHETTO;
            }
        }
    }

    void InserisciDischetto(int colonna)
    {
        if(colonna < Max_Colonne)
        {
            if (riempimento_colonna[colonna] < Max_Righe)
            {
                Dischetto nuovo;
                nuovo.oggetto = Instantiate(Resources.Load("oggetto" + (int)colore)) as GameObject;
                nuovo.tipo = colore;

                Vector3 start = new Vector3(90 - colonna * 10, 110, 0);
                Vector3 end = new Vector3(90 - colonna * 10, (int)((riempimento_colonna[colonna]) * 10 + 10 * .5f), 0);

                nuovo.oggetto.AddComponent<ComportamentoDischetto>().SetStartAndEndPosition(start, end);

                griglia[riempimento_colonna[colonna] + colonna * Max_Righe] = nuovo;

                riempimento_colonna[colonna] += 1;

                colore = Prossimo();

                ControlloVincitore(colonna);
            }
        }
    }

    void ControlloVincitore(int colonna)
    {
        int limiteDestro, limiteSinistro, limiteSotto, limiteSopra;
        int partenzaDiagonalePrim, limiteDiagonalePrim, partenzaDiagonaleSec, limiteDiagonaleSec;
        int riga, posizione;

        riga = riempimento_colonna[colonna] - 1;
        posizione = riga + colonna * Max_Righe;

        //Orizzontale
        limiteSinistro = colonna - (Max_Punti - 1);
        if (limiteSinistro < 0)
            limiteSinistro = 0;
        limiteDestro = (colonna + (Max_Punti - 1));
        if (limiteDestro >= Max_Colonne)
            limiteDestro = Max_Colonne - 1;
        //Verticale
        limiteSotto = riga - (Max_Punti - 1);
        if (limiteSotto < 0)
            limiteSotto = 0;
        limiteSopra = riga + (Max_Punti - 1);
        if (limiteSopra >= Max_Righe)
            limiteSopra = Max_Righe - 1;

        //Debug.Log("LSX: " + limiteSinistro + " LDX: " + limiteDestro + " LSP: " + limiteSopra + " LDW: " + limiteSotto);

        //Diagonale primaria
        if ((colonna - limiteSinistro) < (riga - limiteSotto))
            partenzaDiagonalePrim = colonna - limiteSinistro;
        else
            partenzaDiagonalePrim = riga - limiteSotto;

        if ((limiteDestro - colonna) < (limiteSopra - riga))
            limiteDiagonalePrim = limiteDestro - colonna;
        else
            limiteDiagonalePrim = limiteSopra - riga;

        //Diagonale Secondaria
        if ((colonna - limiteSinistro) < (limiteSopra - riga))
            partenzaDiagonaleSec = colonna - limiteSinistro;
        else
            partenzaDiagonaleSec = limiteSopra - riga;

        if ((limiteDestro - colonna) < (riga - limiteSotto))
            limiteDiagonaleSec = limiteDestro - colonna;
        else
            limiteDiagonaleSec = riga - limiteSotto;

        //Debug.Log("pDsec: " + partenzaDiagonaleSec + " lDsec: " + limiteDiagonaleSec);
        ControlloOrizzontale(limiteSinistro, limiteDestro, posizione, riga);
        ControlloVerticale(limiteSotto, posizione, colonna);
        ControlloDiagonalePrincipale(partenzaDiagonalePrim, limiteDiagonalePrim, riga, colonna, posizione);
        ControlloDiagonaleSecondaria(partenzaDiagonaleSec, limiteDiagonaleSec, riga, colonna, posizione);
    }

    private void ControlloOrizzontale(int limiteSinistro, int limiteDestro, int posizione, int riga)
    {
        if (vittoria == TipoDischetto.NESSUNO)
        {
            int punti = 0;
            //Controllo orizzontale
            for (int i = limiteSinistro; i <= limiteDestro; i++)
            {
                if (griglia[riga + i * Max_Righe].tipo == griglia[posizione].tipo)
                {
                    indici_dischetti_vincitori[punti] = riga + i * Max_Righe;
                    punti++;

                    if (punti >= Max_Punti)
                    {
                        Debug.Log("Vince " + posizione + " in oriz");
                        vittoria = griglia[posizione].tipo;
                        i = 10000; //break;
                    }
                }
                else
                {
                    punti = 0;
                }
            }
        }
    }

    private void ControlloVerticale(int limiteSotto, int posizione, int colonna)
    {
        if (vittoria == TipoDischetto.NESSUNO)
        {
            int punti = 0;
            int temp_colonna = colonna * Max_Righe;
            //Controllo verticale
            for (int i = limiteSotto; i < riempimento_colonna[colonna]; i++) //Sopra a colonna[col] non c'è nulla
            {
                if (griglia[i + temp_colonna].tipo == griglia[posizione].tipo)
                {
                    indici_dischetti_vincitori[punti] = i + temp_colonna;
                    punti++;

                    if (punti >= Max_Punti)
                    {
                        Debug.Log("Vince " + posizione + " in veticale");
                        vittoria = griglia[posizione].tipo;
                        i = 10000; //break;
                    }
                }
                else
                {
                    punti = 0;
                }
            }
        }
    }

    private void ControlloDiagonalePrincipale(int partenzaDiagonalePrim, int limiteDiagonalePrim, int riga, int colonna, int posizione)
    {
        if (vittoria == TipoDischetto.NESSUNO)
        {
            int punti = 0;
            //Controllo diagonale primaria
            int temp_riga = (riga - partenzaDiagonalePrim);
            int temp_colonna = (colonna - partenzaDiagonalePrim);
            for (int i = 0; i <= (limiteDiagonalePrim + partenzaDiagonalePrim); i++)
            {
                //Debug.Log("Prim Per pos: " + pos + " Controllo: " + (((riga - partenzaDiagonalePrim) + i) + ((col - partenzaDiagonalePrim) + i) * Max_Righe));
                if (griglia[(temp_riga + i) + (temp_colonna + i) * Max_Righe].tipo == griglia[posizione].tipo)
                {
                    indici_dischetti_vincitori[punti] = (temp_riga + i) + (temp_colonna + i) * Max_Righe;
                    punti++;

                    if (punti >= Max_Punti)
                    {
                        Debug.Log("Vince " + posizione + " in diagonale prim");
                        vittoria = griglia[posizione].tipo;
                        i = 10000; //break;
                    }
                }
                else
                {
                    punti = 0;
                }
            }
        }
    }

    private void ControlloDiagonaleSecondaria(int partenzaDiagonaleSec, int limiteDiagonaleSec, int riga, int colonna, int posizione)
    {
        if (vittoria == TipoDischetto.NESSUNO)
        {
            int punti = 0;
            int temp_riga = (riga + partenzaDiagonaleSec);
            int temp_colonna = (colonna - partenzaDiagonaleSec);
            int temp_limite = (limiteDiagonaleSec + partenzaDiagonaleSec);
            //Controllo diagonale secondaria
            for (int i = 0; i <= temp_limite; i++)
            {
                //Debug.Log("Sec Per pos: " + pos + " Controllo: " + (((riga + partenzaDiagonaleSec) - i) + ((col - partenzaDiagonaleSec) + i) * Max_Righe));
                if (griglia[(temp_riga - i) + (temp_colonna + i) * Max_Righe].tipo == griglia[posizione].tipo)
                {
                    indici_dischetti_vincitori[punti] = (temp_riga - i) + (temp_colonna + i) * Max_Righe;
                    punti++;

                    if (punti >= Max_Punti)
                    {
                        Debug.Log("Vince " + posizione + " in diagonale sec");
                        vittoria = griglia[posizione].tipo;
                        i = 10000; //break;
                    }
                }
                else
                {
                    punti = 0;
                }
            }
        }
    }

    public void DischettoFermo()
    {
        if (vittoria != TipoDischetto.NESSUNO)
            stato_gioco = StatoDelGioco.VINCITORE;
        else
            stato_gioco = StatoDelGioco.IN_ATTESA;
    }

    TipoDischetto Prossimo()
    {
        if (colore == TipoDischetto.GIOCATORE_0)
            return TipoDischetto.GIOCATORE_1;
        else
            return TipoDischetto.GIOCATORE_0;
    }

}
