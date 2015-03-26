using UnityEngine;



[RequireComponent(typeof(PersonnagePJP))]
public class ControlesJoueurPJP : MonoBehaviour {

	private PersonnagePJP	personnage;
    private bool 			saut;
	private bool 			tirEnCharge;
	private bool 			tir;
	private bool			épée;
	private bool			dash;
	private bool 			téléport;
	public string			endroitLancementNiveau;


	void Awake() {
		personnage = GetComponent<PersonnagePJP>();
	}

    void Update() {
        // Lit la saisie du saut dans Update pour que les boutons pressés soient pris en compte.
		#if CROSS_PLATFORM_INPUT
			if (CrossPlatformInput.GetButtonDown("Jump")) saut = true;
			if (CrossPlatformInput.GetKeyDown("J")) épée = true;
			if (CrossPlatformInput.GetKeyDown("K")) dash = true;
			if (CrossPlatformInput.GetKeyDown("L")) téléport = true;
			if (CrossPlatformInput.GetKeyDown("Escape")) {
				LevelManager.SetParameter("EndroitLancementNiveau", endroitLancementNiveau);
				LevelManager.LoadLevel("[PAR] Niveau général");
			}
		#else
			if (Input.GetButtonDown("Jump")) saut = true;
			if (Input.GetKeyDown("h")) tirEnCharge = true;
			if (Input.GetKeyUp("h")) tir = true;
			if (Input.GetKeyDown("j")) épée = true;
			if (Input.GetKeyDown("k")) dash = true;
			if (Input.GetKeyDown("l")) téléport = true;
			if (Input.GetKeyDown("escape")) {
				LevelManager.SetParameter("EndroitLancementNiveau", endroitLancementNiveau);
				LevelManager.LoadLevel("[PAR] Niveau général");
			}
		#endif
    }

	void FixedUpdate() {
		// Lit les saisies.
		bool tirEnCharge = Input.GetKey("h");
		#if CROSS_PLATFORM_INPUT
			float horizontal = CrossPlatformInput.GetAxis("Horizontal");
		#else
			float horizontal = Input.GetAxis("Horizontal");
		#endif

		// Envoie tous les paramètres au script de contrôle du personnage.
		personnage.SeDéplacer(horizontal, saut, tirEnCharge);

		if (téléport)
			personnage.Téléport();

		if (dash)
			personnage.Dash();

		if (épée)
			personnage.Epée();

		if (tirEnCharge)
			personnage.CommencerChargementTir();
		if (tir)
			personnage.Tirer();

        // Remet à zéro les variables de saisie une fois qu'elles ont été utilisées.
	    saut = false;
		tirEnCharge = false;
		tir = false;
		épée = false;
		dash = false;
		téléport = false;
	}

}