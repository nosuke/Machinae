using UnityEngine;
using System.Collections;

public class Niveau : MonoBehaviour {

	public enum EndroitLancementNiveau {
		Début = 0, Niveau1, Niveau2, Niveau3
	}
	
	EndroitLancementNiveau apparitionJoueur;
	public GameObject joueur;


	void Awake() {
		if (LevelManager.GetParameter("EndroitLancementNiveau") == null) {
			apparitionJoueur = EndroitLancementNiveau.Début;
			joueur.transform.position = new Vector3(0, 0, 0);
		} else if (LevelManager.GetParameter("EndroitLancementNiveau").Equals("Niveau1")) {
			apparitionJoueur = EndroitLancementNiveau.Niveau1;
			joueur.transform.position = new Vector3(0.4f, -1.1f, 0);
		} else if (LevelManager.GetParameter("EndroitLancementNiveau").Equals("Niveau2")) {
			apparitionJoueur = EndroitLancementNiveau.Niveau2;
			joueur.transform.position = new Vector3(0.17f, -2.7f, 0);
		} else if (LevelManager.GetParameter("EndroitLancementNiveau").Equals("Niveau3")) {
			apparitionJoueur = EndroitLancementNiveau.Niveau3;
			joueur.transform.position = new Vector3(2.75f, -3.8f, 0);
		}
	}

}