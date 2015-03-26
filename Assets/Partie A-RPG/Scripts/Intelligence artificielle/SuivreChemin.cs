using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class SuivreChemin : MonoBehaviour {

	#region Attributs
	public Transform cheminASuivre;
	public TypesMouvement type = TypesMouvement.SensNormal;
	[SerializeField][Range(0.1F, 100.0F)] public float vitesse;
	[SerializeField][Range(0.0F, 60.0F)] public float tempsAttenteMinimum;
	[SerializeField][Range(0.0F, 60.0F)] public float tempsAttenteMaximum;

	// Animations.
	public string animationInactif = null;
	public string animationMarche = null;
	public List<string> autresAnimations = new List<string>();
	[SerializeField][Range(1, 100)] public int fréquenceAnimation = 50;
	
	private List<Transform> listeChemins = new List<Transform>();
	private int index = 1;
	private bool marche = false;
	private Transform trajectoireActuelle;
	private Transform dernièreTrajectoire;
	#endregion


	#region Méthodes Unity
	// Commence cette instance.
	void Start () {
		if (cheminASuivre == null) {
			Debug.LogError ("Un GameObject 'Chemin' doit etre renseigné dans le script 'SuivreChemin.cs'.");
		} else {
			ObtenirChemins ();
			switch (type) {
			case TypesMouvement.SensNormal:
				index = 1;
				break;
			case TypesMouvement.SensInverse:
				index = listeChemins.Count;
				break;
			case TypesMouvement.SensAléatoire:
				index = 1;
				break;
			}

			if (listeChemins.Count > 0) ObtenirNouvellePosition ();
		}
	}

	// Met à jour cette instance.
	void Update () {
		if (marche) CommencerMarche();
	}
	#endregion


	#region Autres méthodes
	// Obtient tous les chemins.
	void ObtenirChemins () {
		foreach (Transform chemin in cheminASuivre) {
				listeChemins.Add (chemin);
		}
		OrientationChemins ();
	}

	// Orientation des chemins.
	void OrientationChemins () {
		//int nombreChemins = listeChemins.Count ();
		//for (int i = 2; i <= nombreChemins; i++) {
			//Transform premier = listeChemins.Single (c => c.name == "Chemin" + (i-1));
			//Transform deuxieme = listeChemins.Single (c => c.name == "Chemin" + i);
			//premier.LookAt (deuxieme.position);
			//if (i == nombreChemins) deuxieme.LookAt (listeChemins.Single (c => c.name == "Chemin1").position);
		//}
	}

	// Obtenir la nouvelle position.
	void ObtenirNouvellePosition () {
		switch (type) {
		case TypesMouvement.SensNormal:
			trajectoireActuelle = listeChemins.Single (c => c.name == "Chemin" + index);
			index = (index < listeChemins.Count) ? index+1 : 1;
			break;
		case TypesMouvement.SensInverse:
			trajectoireActuelle = listeChemins.Single (c => c.name == "Chemin" + index);
			index = (index > 1) ? index-1 : listeChemins.Count;
			break;
		case TypesMouvement.SensAléatoire:
			trajectoireActuelle = listeChemins[Random.Range (0, listeChemins.Count)];
			break;
		}
		StartCoroutine (Attendre ());
	}

	// Commencer la marche.
	void CommencerMarche () {
		if (trajectoireActuelle != null) {
			transform.position = Vector2.MoveTowards (transform.position, trajectoireActuelle.position, Time.deltaTime*vitesse);
			if (VérifierDistance () <= 0.5f) {
				marche = false;
				dernièreTrajectoire = trajectoireActuelle;
				ObtenirNouvellePosition ();
			}
		}
	}

	// Vérifier la distance.
	float VérifierDistance () {
		return Vector2.Distance (transform.position, trajectoireActuelle.position);
	}

	// Temps d'attente.
	IEnumerator Attendre () {
		float temps = Random.Range (tempsAttenteMinimum, tempsAttenteMaximum);

		// Animation aléatoire.
		if (autresAnimations.Count () > 0 && Random.Range(1, 100) < fréquenceAnimation) {
			int animationIndex = Random.Range (0, autresAnimations.Count);
			string autreAnimation = autresAnimations[animationIndex];
			JouerAnimation (autreAnimation);

			yield return new WaitForSeconds (animation[autreAnimation].length);
			JouerAnimation (animationInactif);

			temps = 1.0f;
		}

		yield return new WaitForSeconds (temps);
		marche = true;
		JouerAnimation (animationMarche);
		//if (dernièreTrajectoire != null) transform.LookAt (trajectoireActuelle.position);
	}

	// Jouer l'animation.
	void JouerAnimation (string anim) {
		if (animation != null && animation[anim] != null) {
			animation.CrossFade (anim);
		}
	}
	#endregion
	
} // Fin de la classe.

public enum TypesMouvement {SensNormal, SensInverse, SensAléatoire}