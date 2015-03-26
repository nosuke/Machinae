using UnityEngine;
using System.Collections;

[System.Serializable]
public class Pouvoir {

	public GameObject 	munition;
	public GameObject 	grosseMunition;
	
	public Transform 	générationMunitions;
	public float 		cadenceTir;

	public AudioClip 	sonMunition;
	public AudioClip 	sonGrosseMunition;
	
	private float 		prochainTir = .0f;
	private float 		débutTempsChargement = .0f;

	public void commencerChargement() {
		débutTempsChargement = Time.time;
	}

	public float récupérerTempsChargement() {
		float tempsChargement = Time.time - débutTempsChargement;
		return tempsChargement >= 0 ? tempsChargement : 0;
	}

	public bool peutTirer() {
		if (Time.time > prochainTir) {
			return true;
		}
		return false;
	}

	public void tirer() {
		prochainTir = Time.time + cadenceTir;
		débutTempsChargement = 0;
	}

}



public class PersonnagePJP : MonoBehaviour {

	bool 						tournéVersDroite = true;		// Pour la détermination du sens vers lequel est tourné le joueur.

	[SerializeField] float		vitesseMax = 10f;				// La vitesse la plus rapide à laquelle le joueur peut bouger sur l'axe x.
	[SerializeField] float		forceSaut = 400f;				// Quantité de force ajoutée quand le joueur saute.	
	[SerializeField] float		forceSecondSaut = 200f;			// Quantité de force ajoutée quand le joueur double-saute.
	
	[SerializeField] bool		contrôleEnLAir = false;			// Si un joueur peut, ou pas, se diriger pendant le saut.
	[SerializeField] bool		peutDoubleSauter = true;		// Si un joueur peut sauter une seconde fois tandis qu'il est dans les airs.
	[SerializeField] LayerMask	quEstSol;						// Un masque déterminant qu'est-ce qui est considéré comme le sol pour le joueur.

	[SerializeField] float		tempsRechargeTéléport;
	[SerializeField] float		distanceTéléport;

	[SerializeField] Pouvoir	pouvoirActuel;
	
	public float				life = 100.0f;
	public float				duréeImmunité = 0.5f;
	private float				tempsImmunité;
	
	Transform					vérificateurSol;				// Une position marquant où vérifier si le joueur touche le sol.
	float						rayonSolTouché = 0.01f;			// Rayon du cercle servant à déterminer si le joueur touche le sol.
	bool						solTouché = false;				// Si le joueur touche le sol ou pas.
	Transform					vérificateurPlafond;			// Une position marquant où vérifier si le joueur touche le plafond.
	float						rayonPlafond = .01f;			// Rayon du cercle servant à déterminer si le joueur peut se lever.
	Animator					animateur;						// Référence au composant animateur du joueur.
	bool						peutSauter = true;
	float						prochainTéléport = .0f;

	Vector2						vitesseDash = new Vector2(60,0);
	bool						peutUtiliserDash = true;
	float						dashTempsChargement = 0.2f;
	float						duréeDash = 0.6f;
	bool						frameSaut = true;


	private GameObject			hpBar;
	private Vector2				hpScale;
	private float				currentLife;
	private float 				hpBarMinPos;
	private float				hpBarMaxPos;

	void Start() {
		tempsImmunité = Time.time;
		hpBar = GameObject.Find ("HideLifeBar");
		currentLife = life;
		hpScale = hpBar.transform.localScale;
		hpBarMinPos = Mathf.Abs (GameObject.Find ("HideLifeBarMin").transform.localPosition.x);//0.14f;
		hpBarMaxPos = Mathf.Abs (GameObject.Find ("HideLifeBarMax").transform.localPosition.x);//2.07;

		hpBar.transform.localScale = new Vector2(hpScale.x * (1 - (currentLife / life)), 1);
	}

    void Awake() {
		// Met à jour les références.
		vérificateurSol = transform.Find("Vérificateur sol");
		vérificateurPlafond = transform.Find("Vérificateur plafond");
		animateur = GetComponent<Animator>();
	}
	
	void FixedUpdate() {
		// Le joueur touche le sol si un cercle de diffusion autour du vérificateur sol entre en collision avec quoi que ce soit désigné comme étant le sol.
		solTouché = Physics2D.OverlapCircle(vérificateurSol.position, rayonSolTouché, quEstSol);
		animateur.SetBool("Sol", solTouché);
		
		// Change l'animation verticale.
		animateur.SetFloat("VitesseVerticale", rigidbody2D.velocity.y);

		// Change l'animation de saut.
		if (animateur.GetBool("Sol") && animateur.GetBool("Saut")) {
			if (frameSaut)
				frameSaut = false;
			else {
				animateur.SetBool("Saut", false);
				frameSaut = true;
				peutSauter = true;
				peutDoubleSauter = true;
			}
		}
	}

	// For enemy collision damage
	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Enemy") {
			TakeDamage(other.gameObject, tempsImmunité > Time.time);
		}
	}

	// For enemy collision when staying in contact with him
	void OnCollisionStay2D(Collision2D other) {
		if (other.gameObject.tag == "Enemy") {
			TakeDamage(other.gameObject, tempsImmunité > Time.time);
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "EnemyProjectile" || other.gameObject.tag == "BossProjectile") {
			TakeDamage(other.gameObject, false);
			Destroy(other.gameObject);
		}
	}

	// Only used for flying enemy, take damage when the player is in contact with the enemy
	void OnTriggerStay2D(Collider2D autre) {
		if (autre.gameObject.tag == "Enemy") {
			TakeDamage(autre.gameObject, tempsImmunité > Time.time);
		}
	}

	void TakeDamage(GameObject ennemi, bool immunité) {
		if (immunité == true)
			return;
		Damage d = ennemi.GetComponent<Damage> ();
		if (d == null)
			return;
		currentLife -= d.damage;

		hpBar.transform.localScale = new Vector2(hpScale.x * (1 - (currentLife / life)), 1);

		float pos = hpBarMaxPos - hpBarMinPos;
		pos = pos * (currentLife / life);
		pos = hpBarMinPos + pos;
		hpBar.transform.localPosition = new Vector2 (- pos, 0.0f);

		if (currentLife <= 0) {
			//animator.Play(dieAnimation.name);
			//Destroy(this.gameObject, dieAnimation.length);
			Application.LoadLevel (Application.loadedLevel);
		}
		tempsImmunité = Time.time + duréeImmunité;
	}

	public void SeDéplacer(float mouvement, bool saut, bool rayonEnCharge) {

		// Spécifie si le personnage est en train de charger son tir, ou pas, dans l'animateur.
		animateur.SetBool("RayonEnCharge", rayonEnCharge);

		// Permet le contrôle du joueur s'il touche le sol ou si le contrôle en l'air est activé.
		if (solTouché || contrôleEnLAir) {

			// Assigne la valeur absolue de la saisie horizontale au paramètre Vitesse de l'animateur.
			animateur.SetFloat("Vitesse", Mathf.Abs(mouvement));

			// Bouge le personnage.
			rigidbody2D.velocity = new Vector2(mouvement * vitesseMax, rigidbody2D.velocity.y);

			// Si la saisie fait bouger le personnage vers la droite et le joueur est tourné vers la gauche…
			if(mouvement > 0 && !tournéVersDroite)
				// … retourne le joueur.
				SeRetourner();
			// Cependant, si la saisie fait bouger le personnage vers la gauche et le joueur est tourné vers la droite…
			else if(mouvement < 0 && tournéVersDroite)
				// … retourne le joueur.
				SeRetourner();
		}

		// Si le joueur est censé sauter…
		if (saut && (peutSauter || (!peutSauter && peutDoubleSauter))) {
			// Ajoute une force verticale au joueur.
			if (peutSauter) {
				rigidbody2D.velocity = Vector2.zero;
				rigidbody2D.AddForce(new Vector2(0f, forceSaut));
				peutSauter = false;
			} else if (peutDoubleSauter) {
				rigidbody2D.velocity = Vector2.zero;
				rigidbody2D.AddForce(new Vector2(0f, forceSecondSaut));
				peutDoubleSauter = false;
			}

			// Met à jour l'animateur.
			animateur.SetBool("Saut", true);
        }

	}

	void SeRetourner() {
		// Change le sens vers lequel le joueur est tourné.
		tournéVersDroite = !tournéVersDroite;
		
		// Multiplie la graduation locale x du joueur par -1.
		Vector3 laGraduation = transform.localScale;
		laGraduation.x *= -1;
		transform.localScale = laGraduation;
	}


	public void CommencerChargementTir() {
		if (pouvoirActuel.peutTirer())
			pouvoirActuel.commencerChargement();
	}
	
	public void Tirer() {
		float tempsChargement = pouvoirActuel.récupérerTempsChargement();
		if (pouvoirActuel.peutTirer() && tempsChargement != 0.0f) {
			pouvoirActuel.tirer();
			
			GameObject cloneMunition;
			
			// Tire une grosse munition.
			if (tempsChargement >= 2.0f) {
				cloneMunition = Instantiate(pouvoirActuel.grosseMunition, pouvoirActuel.générationMunitions.position, pouvoirActuel.générationMunitions.rotation) as GameObject;
				audio.clip = pouvoirActuel.sonGrosseMunition;
			} else {
				cloneMunition = Instantiate(pouvoirActuel.munition, pouvoirActuel.générationMunitions.position, pouvoirActuel.générationMunitions.rotation) as GameObject;
				audio.clip = pouvoirActuel.sonMunition;
			}
			
			Mouvement mouvement = cloneMunition.GetComponent<Mouvement>();
			mouvement.ChangerDirection(tournéVersDroite ? Mouvement.Direction.DROITE : Mouvement.Direction.GAUCHE);
			mouvement.CommencerDéplacement();
			
			audio.Play();
		}
	}

	public void Epée() {
		StartCoroutine(AttendreEpée(0.0f, true));
		StartCoroutine(AttendreEpée(0.8f, false));
	}

	IEnumerator AttendreEpée(float tempsAttente, bool animation) {
		yield return new WaitForSeconds(tempsAttente);
		animateur.SetBool("Épée", animation);
	}

	public void Dash() {
		if (solTouché && peutUtiliserDash)
			StartCoroutine(AttendreDash(duréeDash));
	}

	IEnumerator AttendreDash(float tempsDash) {
		float temps = 0;
		peutUtiliserDash = false;
		animateur.SetBool("Dash", true);

		while(tempsDash > temps) {
			temps += Time.deltaTime;
			rigidbody2D.velocity = Vector2.zero;
			if (tournéVersDroite)
				rigidbody2D.AddForce(vitesseDash);
			else
				rigidbody2D.AddForce(-vitesseDash);
			yield return 0;
		}

		animateur.SetBool("Dash", false);
		yield return new WaitForSeconds(dashTempsChargement);
		peutUtiliserDash = true;
	}

	public void Téléport() {
		StartCoroutine(AttendreTéléport(0f, 1));
		StartCoroutine(Téléport(0.8f));
		StartCoroutine(AttendreTéléport(0.8f, 2));
		StartCoroutine(AttendreTéléport(1.7f, 0));
	}

	IEnumerator AttendreTéléport(float tempsAttente, int animation) {
		yield return new WaitForSeconds(tempsAttente);
		animateur.SetInteger("Téléportation", animation);
	}

	IEnumerator Téléport(float tempsAttente) {
		yield return new WaitForSeconds(tempsAttente);
		if (Time.time > prochainTéléport) {
			prochainTéléport = Time.time + tempsRechargeTéléport;
			transform.Translate(new Vector2(transform.localScale.x * distanceTéléport, 0.0f));
		}
	}
}