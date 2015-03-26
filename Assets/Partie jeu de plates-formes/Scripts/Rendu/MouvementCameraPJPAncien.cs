using UnityEngine;
using System.Collections;



public class MouvementCameraAncien : MonoBehaviour {
	
	public Transform 	cible;
	public float 		amortissement = 1;
	public float 		facteurVision = 3;
	public float 		vitesseRetourVision = 0.5f;
	public float 		seuilDéplacementVision = 0.1f;
	
	float 		décalageZ;
	Vector3 	positionDernièreCible;
	Vector3 	vélocitéActuelle;
	Vector3 	positionVision;
	
	
	void Start() {
		positionDernièreCible = cible.position;
		décalageZ = (transform.position - cible.position).z;
		transform.parent = null;
	}
	
	void Update() {
		
		// Met à jour positionVision s'il y a accélération ou changement de direction.
		float xMouvementDelta = (cible.position - positionDernièreCible).x;
		
		bool cibleVisionMiseÀJour = Mathf.Abs(xMouvementDelta) > seuilDéplacementVision;
		
		if (cibleVisionMiseÀJour) {
			positionVision = facteurVision * Vector3.right * Mathf.Sign(xMouvementDelta);
		} else {
			positionVision = Vector3.MoveTowards(positionVision, Vector3.zero, Time.deltaTime * vitesseRetourVision);	
		}
		
		Vector3 positionCible = cible.position + positionVision + Vector3.forward * décalageZ;
		Vector3 nouvellePosition = Vector3.SmoothDamp(transform.position, positionCible, ref vélocitéActuelle, amortissement);
		
		transform.position = nouvellePosition;
		
		positionDernièreCible = cible.position;		
	}
	
}