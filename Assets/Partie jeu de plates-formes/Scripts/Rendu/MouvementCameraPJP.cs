using UnityEngine;
using System.Collections;

public class MouvementCameraPJP : MonoBehaviour {
	
	#region Attributs
	private Transform cible;
	public string nomJoueur;
	#endregion
	
	
	#region Méthodes Unity
	// Use this for initialization.
	void Start () {
		cible = GameObject.Find(nomJoueur).transform;
	}
	
	// Update is called once per frame.
	void Update () {
		transform.position = cible.position + new Vector3 (0, 0, -10);
	}
	#endregion
	
}