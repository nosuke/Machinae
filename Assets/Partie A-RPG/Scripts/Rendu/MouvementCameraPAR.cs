using UnityEngine;
using System.Collections;

public class MouvementCameraPAR : MonoBehaviour {

	#region Attributs
	Transform cible;
	#endregion


	#region Méthodes Unity
	// Use this for initialization.
	void Start () {
		cible = GameObject.Find ("Joueur").transform;
	}
	
	// Update is called once per frame.
	void Update () {
		transform.position = cible.position + new Vector3 (0, 0, -10);
	}
	#endregion

}