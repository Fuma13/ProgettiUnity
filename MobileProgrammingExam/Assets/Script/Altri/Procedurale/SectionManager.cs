using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SectionManager : MonoBehaviour {

//	bool nextSceneDirectionUp= false;
	DifficoltyMod02 difficolty02 = DifficoltyMod02.facile;
	DifficoltyMod14 difficolty14 = DifficoltyMod14.facile;
//	DifficoltyMod3 difficolty3 = DifficoltyMod3.facile;
	DifficoltyBot difficoltyBot = DifficoltyBot.facile;
	int numOfEnemies=0;
	public GameObject section;
	public List<GameObject> tiles;
	public List<Material> matTiles;

	// probabilita' che capitino i pezzi
	const float DIFF_0_2 = 50f;
	const float DIFF_1_4 = 35f;
	const float DIFF_3 = 15f;
	const float PROB_BOT = 40f;

	const string PARENT= "Section";
	const int PROB = 200;

	// percentuale di probabilita' che capitino pezzi piu' difficili
	public enum Difficolty { facile, medio, difficile, ultra};
	enum DifficoltyMod02 { facile=60, medio=50, difficile= 40, ultra= 20 }; 
	enum DifficoltyMod14 { facile=30, medio=35, difficile= 40, ultra= 40 }; 
	enum DifficoltyMod3 { facile=10, medio=15, difficile= 20, ultra= 40 };
	enum DifficoltyBot { facile=40, medio=50, difficile=70, ultra=100};

	// Use this for initialization
	void Start () {

//		setDifficulty(Difficolty.medio);
//		createNewSection(50);
	}

	public int generateSection(Difficolty livelloDiDifficolta){
		section= GameObject.Find("Section");
		numOfEnemies=0;

		int numOfTiles = setDifficulty(livelloDiDifficolta);
		createNewSection(numOfTiles);

		return numOfEnemies;
	}

	private int setDifficulty(Difficolty livelloDiDifficolta){
		switch(livelloDiDifficolta){
		case Difficolty.facile:
			difficolty02 = DifficoltyMod02.facile;
			difficolty14 = DifficoltyMod14.facile;
//			difficolty3 = DifficoltyMod3.facile;
			difficoltyBot = DifficoltyBot.facile;
			return 20;
		case Difficolty.medio:
			difficolty02 = DifficoltyMod02.medio;
			difficolty14 = DifficoltyMod14.medio;
//			difficolty3 = DifficoltyMod3.medio;
			difficoltyBot = DifficoltyBot.medio;
			return 30;
		case Difficolty.difficile:
			difficolty02 = DifficoltyMod02.difficile;
			difficolty14 = DifficoltyMod14.difficile;
//			difficolty3 = DifficoltyMod3.difficile;
			difficoltyBot = DifficoltyBot.difficile;
			return 40;
		case Difficolty.ultra:
			difficolty02 = DifficoltyMod02.ultra;
			difficolty14 = DifficoltyMod14.ultra;
//			difficolty3 = DifficoltyMod3.ultra;
			difficoltyBot = DifficoltyBot.ultra;
			return 50;
		}

		return 0;
	}

	private void createNewSection(int numOfTile){

		int oldType= 3;

		for(int i=0; i< numOfTile; i++){
			oldType= placeANewTile(oldType, i);
		}

	}

	private int placeANewTile(int lastTile, int actualTileNumber){

		float probability02= (DIFF_0_2 + (float) difficolty02)/PROB;
		float probability14= (DIFF_1_4 + (float) difficolty14)/PROB;
//		float probability3= (DIFF_3 + (float) difficolty3)/PROB;

		float randomValue = Random.value;
		float randomValueForEnemy = Random.value;

		Vector3 position= new Vector3(3f*actualTileNumber,0f,0f);
		int newTile=-1;
		GameObject objectInstancied;

		if(randomValue < probability02){
			if(Random.value < 0.5f){
				objectInstancied = (GameObject) GameObject.Instantiate(tiles[0], position, Quaternion.identity);
				newTile= 0;
			}
			else{
				objectInstancied = (GameObject) GameObject.Instantiate(tiles[2], position, Quaternion.identity);
				newTile= 2;
			}
		}
		else if(randomValue < (probability02 + probability14)){
			if(Random.value < 0.5f){
				objectInstancied = (GameObject) GameObject.Instantiate(tiles[1], position, Quaternion.identity);
				newTile= 1;
			}
			else{
				objectInstancied = (GameObject) GameObject.Instantiate(tiles[4], position, Quaternion.identity);
				newTile= 4;
			}
		}
		else{
			objectInstancied = (GameObject) GameObject.Instantiate(tiles[3], position, Quaternion.identity);
			newTile= 3;
		}
		
		float probabilityEnemy= ((float) difficoltyBot)/100;

		// calcolo se il bot verra' disattivo
		if(randomValueForEnemy > probabilityEnemy){
			switch(newTile){
			case 0:
			case 3:
			case 4:
				objectInstancied.transform.Find("Enemy").gameObject.SetActive(false);
				break;
			case 1:
				if(Random.value < 0.5f)
					objectInstancied.transform.Find("EnemyS").gameObject.SetActive(false);
				else
					objectInstancied.transform.Find("EnemyD").gameObject.SetActive(false);
				// aggiungi un nemico al count
				numOfEnemies++;
				break;
			case 2:
				float random= Random.value;
				if(random < 0.5f)
					objectInstancied.transform.Find("Tile1D/Enemy").gameObject.SetActive(false);
				else
					objectInstancied.transform.Find("Tile1U/Enemy").gameObject.SetActive(false);
				// aggiungi un nemico al count
				numOfEnemies++;
				break;
			}
		}
		else{
			// se il tile e' la 2, aggiungo un nemico in piu'
			if(newTile == 2 || newTile == 1)
				numOfEnemies++;

			// aggiungi un nemico al count
			numOfEnemies++;
		}

		objectInstancied.name = actualTileNumber.ToString();
		GameObject lastObj = GameObject.Find((actualTileNumber-1).ToString());
		
		// adjust mesh
		switch(lastTile + "." + newTile){
		// analizzo il pezzo a destra dell'elemento precedente e quello a sinistra di quello nuovo,
		// 		rendendo consistente le texture
		// cambiamento mesh per tile precedente => 0
		case "0.0":
			lastObj.transform.Find("CubeD").GetComponent<Renderer>().material = matTiles[2];
			lastObj.transform.Find("EndD").gameObject.SetActive(false);
			objectInstancied.transform.Find("CubeS").GetComponent<Renderer>().material = matTiles[2];
			objectInstancied.transform.Find("EndS").gameObject.SetActive(false);
			break;
		case "0.1":
			lastObj.transform.Find("CubeD").GetComponent<Renderer>().material = matTiles[2];
			lastObj.transform.Find("EndD").gameObject.SetActive(false);
			objectInstancied.transform.Find("EndS").gameObject.SetActive(false);
			break;
		case "0.2":
			lastObj.transform.Find("CubeD").GetComponent<Renderer>().material = matTiles[2];
			lastObj.transform.Find("EndD").gameObject.SetActive(false);
			objectInstancied.transform.Find("Tile1D/CubeS").GetComponent<Renderer>().material = matTiles[2];
			objectInstancied.transform.Find("Tile1D/EndS").gameObject.SetActive(false);
			break;
		// cambiamento mesh per tile precedente => 1
		case "1.0":
			lastObj.transform.Find("EndD").gameObject.SetActive(false);
			objectInstancied.transform.Find("CubeS").GetComponent<Renderer>().material = matTiles[2];
			objectInstancied.transform.Find("EndS").gameObject.SetActive(false);
			break;
		case "1.1":
			lastObj.transform.Find("EndD").gameObject.SetActive(false);
			objectInstancied.transform.Find("EndS").gameObject.SetActive(false);
			break;
		case "1.2":
			lastObj.transform.Find("EndD").gameObject.SetActive(false);
			objectInstancied.transform.Find("Tile1D/CubeS").GetComponent<Renderer>().material = matTiles[2];
			objectInstancied.transform.Find("Tile1D/EndS").gameObject.SetActive(false);
			break;
		case "1.3":
		case "1.4":
			lastObj.transform.Find("CubeD").GetComponent<Renderer>().material = matTiles[0];
			//if(lastObj.transform.Find("EnemyD") != null)
				//lastObj.transform.Find("EnemyD").GetComponent<Enemy>().enemySpeed= 0;
			break;
		// cambiamento mesh per tile precedente => 2
		case "2.0":
			lastObj.transform.Find("Tile1D/CubeD").GetComponent<Renderer>().material = matTiles[2];
			lastObj.transform.Find("Tile1D/EndD").gameObject.SetActive(false);
			objectInstancied.transform.Find("CubeS").GetComponent<Renderer>().material = matTiles[2];
			objectInstancied.transform.Find("EndS").gameObject.SetActive(false);
			break;
		case "2.1":
			lastObj.transform.Find("Tile1D/CubeD").GetComponent<Renderer>().material = matTiles[2];
			lastObj.transform.Find("Tile1D/EndD").gameObject.SetActive(false);
			objectInstancied.transform.Find("EndS").gameObject.SetActive(false);
			break;
		case "2.2":
			lastObj.transform.Find("Tile1D/CubeD").GetComponent<Renderer>().material = matTiles[2];
			lastObj.transform.Find("Tile1D/EndD").gameObject.SetActive(false);
			objectInstancied.transform.Find("Tile1D/CubeS").GetComponent<Renderer>().material = matTiles[2];
			objectInstancied.transform.Find("Tile1D/EndS").gameObject.SetActive(false);
			lastObj.transform.Find("Tile1U/CubeD").GetComponent<Renderer>().material = matTiles[5];
			lastObj.transform.Find("Tile1U/EndD").gameObject.SetActive(false);
			objectInstancied.transform.Find("Tile1U/CubeS").GetComponent<Renderer>().material = matTiles[5];
			objectInstancied.transform.Find("Tile1U/EndS").gameObject.SetActive(false);
			break;
		case "2.4":
			lastObj.transform.Find("Tile1U/CubeD").GetComponent<Renderer>().material = matTiles[5];
			lastObj.transform.Find("Tile1U/EndD").gameObject.SetActive(false);
			objectInstancied.transform.Find("CubeS").GetComponent<Renderer>().material = matTiles[5];
			objectInstancied.transform.Find("EndS").gameObject.SetActive(false);
			break;
		// cambiamento mesh per tile precedente => 3
		case "3.1":
		// cambiamento mesh per tile precedente => 4
		case "4.1":
			objectInstancied.transform.Find("CubeS").GetComponent<Renderer>().material = matTiles[0];
			//if(objectInstancied.transform.Find("EnemyS") != null)
				//objectInstancied.transform.Find("EnemyS").GetComponent<Enemy>().enemySpeed= 0;
			break;
		case "4.2":
			lastObj.transform.Find("CubeD").GetComponent<Renderer>().material = matTiles[5];
			lastObj.transform.Find("EndD").gameObject.SetActive(false);
			objectInstancied.transform.Find("Tile1U/CubeS").GetComponent<Renderer>().material = matTiles[5];
			objectInstancied.transform.Find("Tile1U/EndS").gameObject.SetActive(false);
			break;
		case "4.4":
			lastObj.transform.Find("CubeD").GetComponent<Renderer>().material = matTiles[5];
			lastObj.transform.Find("EndD").gameObject.SetActive(false);
			objectInstancied.transform.Find("CubeS").GetComponent<Renderer>().material = matTiles[5];
			objectInstancied.transform.Find("EndS").gameObject.SetActive(false);
			break;
		}

		objectInstancied.transform.parent = section.transform;

		return newTile;
	}
}
