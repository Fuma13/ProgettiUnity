using UnityEngine;
using System.Collections;
using System;

public class ForzaQuattro : MonoBehaviour {

    private const int Max_Righe = 8;
    private const int Max_Colonne = 9;
    private const int Max_Punti = 4;
    private const int Max_Celle = Max_Righe * Max_Colonne;

    GameObject[] oggetto_click = new GameObject[Max_Colonne]; //Barre verticali trasparenti x prendere il click  
    GameObject[] struttura = new GameObject[Max_Colonne]; //Barre verticale con celle

    int[] dischetto_cella = new int[Max_Celle]; //matrice della griglia

    //float[] dischetto_x = new float[Max_Celle]; //X dei dischetti inseriti (nello stesso ordine di dischetto)
    //float[] dischetto_y = new float[Max_Celle]; //Y dei dischetti inseriti (nello stesso ordine di dischetto)
    //Usato
    int[] colonna = new int[Max_Colonne]; //Posizione dell'ultimo dischetto in quella colonna (xkè 100??)

	int che_colonna; //# della colonna in cui sto inserendo il nuovo dischetto
	int vittoria = -1; //-1 nessuno, 0 ha vinto il primo, 1 il secondo
	int partenza_colore; //colore da inserire

    //Variabili mie
    int stato_gioco; //0 posso inserire, 1 inserisci dischetto, 2 dischetto in movimento, 3 dischetto fermo in attesa di controllo

    GameObject dischetto;

    DateTime startTime, endTime;
    TimeSpan duration;


	// Use this for initialization
	void Start () {

        for (int n = 0; n < Max_Colonne; n++)
        {
			oggetto_click[n] = Instantiate(Resources.Load("struttura_click2")) as GameObject;
			oggetto_click[n].transform.position = new Vector3(90-n*10,0,0); //TODO rivedere il posizionamento

			oggetto_click[n].renderer.enabled = false;
			oggetto_click[n].name = "colonna" + n;

			struttura[n] = Instantiate(Resources.Load("struttura_utile")) as GameObject;
			struttura[n].transform.position = new Vector3(90-n*10,0,0);

		}

		che_colonna = -1;
        stato_gioco = 0;
		for (int n=0; n<Max_Celle; n++) {
			dischetto_cella[n] = -1;
		}

        //StampaSchemaIniziale();
	
	}
	
	// Update is called once per frame
	void Update () {
		System.Threading.Thread.Sleep (27);

        startTime = System.DateTime.Now;

        //quando c'è il click dell'utente, selezione la colonna dove inserire la pedina
		click_colonna ();
        //se c'è stato il click e posso inserire -> inserisco
		inserisci_oggetto ();
        //se c'è un dischetto in movimento lo muovo
		disegna_oggetto();
        //se sono tutti fermi controllo se ha vinto
        controllo_vincita();

        endTime = System.DateTime.Now;
        duration = endTime - startTime;
        Debug.Log(duration.TotalMilliseconds);
	}

    void OnGUI()
    {
        if(vittoria != -1)
        {
            GUI.Box(new Rect(Screen.width/2 - 150, Screen.height / 2 - 75, 300, 150), "");
            GUI.Label(new Rect(Screen.width/2 - 100, Screen.height / 2 - 50, 200, 40), "Il vincitore è il giocatore: " + (vittoria == 1 ? "Blu":"Rosso"));

            if(GUI.Button(new Rect(Screen.width/2 - 100, Screen.height / 2 + 10, 200, 40), "Ricomincia"))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }
    }

	void click_colonna(){
        if (stato_gioco == 0 && Input.GetMouseButton(0) && vittoria == -1)
        {
			Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray2, out hit, 1000)){
				for(int n=0; n<Max_Colonne; n++){
					if(hit.collider.name == "colonna"+n){
						che_colonna = n;
                        stato_gioco = 1;
					}
				}
			}
		}
	}

	void inserisci_oggetto(){

		if (stato_gioco == 1){
			
			//if (che_colonna>-1 && che_colonna<Max_Colonne){

                //Se c'è ancora spazio
				if (colonna[che_colonna]<Max_Righe){

                    //Passo al prossimo
                    partenza_colore = 1 - partenza_colore;
					
					dischetto=Instantiate(Resources.Load("oggetto"+partenza_colore)) as GameObject;
                    dischetto.transform.position = new Vector3(90-che_colonna*10, 110, 0); //TODO da sistemare come posizionamente colonne

                    stato_gioco = 2;
				}
			//}
		}
	}

	void disegna_oggetto(){

        if(stato_gioco == 2)
        {
            int limite = (int)((colonna[che_colonna]) * 10 + 10 * .5f); //TODO da rivedere come posizionamento colonna
            Vector3 position = dischetto.transform.position;
            position.y -= 5;
            if(position.y < limite)
            {
                position.y = limite;
                int pos=colonna[che_colonna]+che_colonna*Max_Righe;
                dischetto_cella[pos] = partenza_colore;
                ++(colonna[che_colonna]);
                stato_gioco = 3;
            }

            dischetto.transform.position = position;
        }
	}

	void controllo_vincita(){
        //Se tutto è fermo
        if (stato_gioco == 3)
        {
            //if (che_colonna > 0 && vittoria == -1)
            {
                int limiteDestro, limiteSinistro, limiteSotto, limiteSopra;
                int partenzaDiagonalePrim, limiteDiagonalePrim, partenzaDiagonaleSec, limiteDiagonaleSec;
                int riga, col, pos;

                col = che_colonna;
                riga = colonna[col] - 1;
                pos = riga + col * Max_Righe;

                //Orizzontale
                limiteSinistro = col - (Max_Punti -1);
                if (limiteSinistro < 0)
                    limiteSinistro = 0;
                limiteDestro = (col + (Max_Punti - 1));
                if (limiteDestro >= Max_Colonne)
                    limiteDestro = Max_Colonne -1;
                //Verticale
                limiteSotto = riga - (Max_Punti - 1);
                if (limiteSotto < 0)
                    limiteSotto = 0;
                limiteSopra = riga + (Max_Punti - 1);
                if (limiteSopra >= Max_Righe)
                    limiteSopra = Max_Righe -1;

                //Debug.Log("LSX: " + limiteSinistro + " LDX: " + limiteDestro + " LSP: " + limiteSopra + " LDW: " + limiteSotto);

                //Diagonale primaria
                if ((col - limiteSinistro) < (riga - limiteSotto))
                    partenzaDiagonalePrim = col - limiteSinistro;
                else
                    partenzaDiagonalePrim = riga - limiteSotto;

                if ((limiteDestro - col) < (limiteSopra - riga))
                    limiteDiagonalePrim = limiteDestro - col;
                else
                    limiteDiagonalePrim = limiteSopra - riga;

                //Diagonale Secondaria
                if ((col - limiteSinistro) < (limiteSopra - riga))
                    partenzaDiagonaleSec = col - limiteSinistro;
                else
                    partenzaDiagonaleSec = limiteSopra - riga;

                if ((limiteDestro - col) < (riga - limiteSotto))
                    limiteDiagonaleSec = limiteDestro - col;
                else
                    limiteDiagonaleSec = riga - limiteSotto;

                //Debug.Log("pDsec: " + partenzaDiagonaleSec + " lDsec: " + limiteDiagonaleSec);

                int punti = 0;
                //Controllo orizzontale
                for (int i = limiteSinistro; i <= limiteDestro; i++)
                {
                    if (dischetto_cella[riga + i * Max_Righe] == dischetto_cella[pos])
                    {
                        punti++;

                        if (punti >= Max_Punti)
                        {
                            Debug.Log("Vince " + pos + " in oriz");
                            vittoria = dischetto_cella[pos];
                            i = 10000; //break;
                        }
                    }
                    else
                    {
                        punti = 0;
                    }
                }
                punti = 0;
                int temp_colonna = col * Max_Righe;
                //Controllo verticale
                for (int i = limiteSotto; i < colonna[col]; i++) //Sopra a colonna[col] non c'è nulla
                {
                    if (dischetto_cella[i + temp_colonna] == dischetto_cella[pos])
                    {
                        punti++;

                        if (punti >= Max_Punti)
                        {
                            Debug.Log("Vince " + pos + " in veticale");
                            vittoria = dischetto_cella[pos];
                            i = 10000; //break;
                        }
                    }
                    else
                    {
                        punti = 0;
                    }
                }

                punti = 0;
                //Controllo diagonale primaria
                int temp_riga = (riga - partenzaDiagonalePrim);
                temp_colonna = (col - partenzaDiagonalePrim);
                for (int i = 0; i <= (limiteDiagonalePrim + partenzaDiagonalePrim); i++)
                {
                    //Debug.Log("Prim Per pos: " + pos + " Controllo: " + (((riga - partenzaDiagonalePrim) + i) + ((col - partenzaDiagonalePrim) + i) * Max_Righe));
                    if (dischetto_cella[(temp_riga + i) + (temp_colonna + i) * Max_Righe] == dischetto_cella[pos])
                    {
                        punti++;

                        if (punti >= Max_Punti)
                        {
                            Debug.Log("Vince " + pos + " in diagonale prim");
                            vittoria = dischetto_cella[pos];
                            i = 10000; //break;
                        }
                    }
                    else
                    {
                        punti = 0;
                    }
                }

                punti = 0;
                temp_riga = (riga + partenzaDiagonaleSec);
                temp_colonna = (col - partenzaDiagonaleSec);
                int temp_limite = (limiteDiagonaleSec + partenzaDiagonaleSec);
                //Controllo diagonale secondaria
                for (int i = 0; i <= temp_limite; i++)
                {
                    //Debug.Log("Sec Per pos: " + pos + " Controllo: " + (((riga + partenzaDiagonaleSec) - i) + ((col - partenzaDiagonaleSec) + i) * Max_Righe));
                    if (dischetto_cella[( temp_riga - i) + (temp_colonna + i) * Max_Righe] == dischetto_cella[pos])
                    {
                        punti++;

                        if (punti >= Max_Punti)
                        {
                            Debug.Log("Vince " + pos + " in diagonale sec");
                            vittoria = dischetto_cella[pos];
                            i = 10000; //break;
                        }
                    }
                    else
                    {
                        punti = 0;
                    }
                }
                stato_gioco = 0;
                che_colonna = -1;
            }
        }
	}

    void StampaSchema()
    {
        string s = "";
        for (int riga = (Max_Righe -1); riga >= 0; riga--)
        {
            for (int col = 0; col < Max_Colonne; col++)
            {

                int num = col * Max_Righe + riga;
                if (dischetto_cella[num] >= 0)
                    s += "|_" + dischetto_cella[num];
                else
                    s += "|" + dischetto_cella[num];
            }
            s += "|\n";
        }
        Debug.Log(s);
    }

    void StampaSchemaIniziale()
    {
        string s = "";
        for (int riga = (Max_Righe - 1); riga >= 0; riga--)
        {
            for (int col = 0; col < Max_Colonne; col++)
            {

                int num = col * Max_Righe + riga;
                if (num > 9)
                    s += "|" + num;
                else
                    s += "|0" + num;
            }
            s += "|\n";
        }
        Debug.Log(s);
    }
}
