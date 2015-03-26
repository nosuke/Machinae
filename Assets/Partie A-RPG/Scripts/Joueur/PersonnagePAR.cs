using UnityEngine;
using System.Collections;

public class PersonnagePAR : MonoBehaviour {

	#region Attributs
	private Animator animateur;
	#endregion


	#region Méthodes Unity
	void Awake () {
		animateur = GetComponent<Animator> ();
	}

	void OnCollisionEnter2D (Collision2D objet) {
		if (objet.gameObject.name == "C") {

		}
	}
	#endregion


	#region Autres méthodes
	public void GestionMouvement (float horizontal, float vertical) {
		if (horizontal != 0f || vertical != 0f) {
			animateur.SetBool ("bouge", true);
			AnimerMarche (horizontal, vertical);
		} else {
			animateur.SetBool ("bouge", false);
		}
		rigidbody2D.velocity = new Vector3 (horizontal, vertical, 0);
	}
	
	void AnimerMarche (float h, float v) {
		if (animateur) {
			if ((v > 0) && (v > h)) {
				animateur.SetInteger ("direction", 1);
			}
			if ((h > 0) && (v < h)) {
				animateur.SetInteger ("direction", 2);
			}
			if ((v < 0) && (v < h)) {
				animateur.SetInteger ("direction", 3);
			}
			if ((h < 0 ) && (v > h)) {
				animateur.SetInteger ("direction", 4);
			}
		}
	}
	#endregion

}