using UnityEngine;



[RequireComponent(typeof(PersonnagePAR))]
public class ControlesJoueurPAR : MonoBehaviour {

	private PersonnagePAR	personnage;

	void Awake() {
		personnage = GetComponent<PersonnagePAR>();
	}

    void Update() {
        // Lit la saisie du saut dans Update pour que les boutons pressés soient pris en compte.
		#if CROSS_PLATFORM_INPUT
			if (CrossPlatformInput.GetKeyDown("1") || Input.GetKeyDown("[1]")) Application.LoadLevel("[PJP] Niveau 1 (Megaman)");
			if (CrossPlatformInput.GetKeyDown("2") || Input.GetKeyDown("[2]")) Application.LoadLevel("[PJP] Niveau 2 (Castlevania)");
			if (CrossPlatformInput.GetKeyDown("3") || Input.GetKeyDown("[3]")) Application.LoadLevel("[PJP] Niveau 3 (Metroid)");
			float v = CrossPlatformInput.GetAxis ("Vertical");
			float h = CrossPlatformInput.GetAxis ("Horizontal");
		#else
			if (Input.GetKeyDown("1") || Input.GetKeyDown("[1]")) LevelManager.LoadLevel("[PJP] Niveau 1 (Megaman)");
			if (Input.GetKeyDown("2") || Input.GetKeyDown("[2]")) LevelManager.LoadLevel("[PJP] Niveau 2 (Castlevania)");
			if (Input.GetKeyDown("3") || Input.GetKeyDown("[3]")) LevelManager.LoadLevel("[PJP] Niveau 3 (Metroid)");
			float v = Input.GetAxis ("Vertical");
			float h = Input.GetAxis ("Horizontal");
		#endif

		// Envoie tous les paramètres au script de contrôle du personnage.
		personnage.GestionMouvement(h, v);
    }

}