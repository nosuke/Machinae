using UnityEngine;
using System.Collections;

public class PersonnageAncien : MonoBehaviour {

	public Animator animateur;
	public float vitesse = 8f;

	public Transform verificationSol;
	private bool toucheLeSol = false;
	private float rayonSol = 0.3f;
	public LayerMask coucheSol;


	// Use this for initialization
	void Start () {
		animateur = GetComponent<Animator> ();
	}

	void FixedUpdate () {
		toucheLeSol = Physics2D.OverlapCircle (verificationSol.position, rayonSol, coucheSol);
		animateur.SetBool ("coucheSol", toucheLeSol);
	}

	// Update is called once per frame
	void Update () {
		Vector2 positionEcran = Camera.main.WorldToScreenPoint (transform.position);
		float abscisse = Input.GetAxis ("Horizontal");
		animateur.SetFloat ("vitesse", Mathf.Abs (abscisse));

		if (toucheLeSol && Input.GetButtonDown ("Jump")) {
			rigidbody2D.AddForce (new Vector2 (0, 430));
		}

		if (abscisse > 0) {
			transform.Translate (abscisse * vitesse * Time.deltaTime, 0, 0);
			transform.eulerAngles = new Vector2 (0, 0);
		}

		if (abscisse < 0) {
			transform.Translate (-abscisse * vitesse * Time.deltaTime, 0, 0);
			transform.eulerAngles = new Vector2 (0, 180);
		}

		if (positionEcran.y > Screen.height || positionEcran.x > Screen.width || positionEcran.y < 0 || positionEcran.x < 0) {
			TuerOX ();
		}
	}

	void OnCollisionEnter2D (Collision2D objet) {
		if (objet.gameObject.name == "Objet à obtenir") {
			TuerOX ();
		}
		if (objet.gameObject.name == "Mortelle 1") {
			TuerOX ();
		}
		if (objet.gameObject.name == "Temporaire 1") {
			Destroy(objet.gameObject);
		}
	}

	void TuerOX () {
		Application.LoadLevel(Application.loadedLevel);
	}

}