using UnityEngine;
using System.Collections;

public class ConeVue3D : MonoBehaviour {

	#region Attributs
	public float angle = 75f; // Angle d'ouverture, degré.
	public float distance = 10f;
	public int précision = 20; // Nombre de rayons lancés dans l'angle ci-dessus.
	public Material material; // Material appliqué au mesh de vue.
	public bool débug = false; // Dessine les rayons dans la scène.
	public float fréquence = 0.05F; // Fréquence de calcul. > 0.05F est déconseillé.
	public LayerMask masque; // Couches qui vont bloquer la vue.
	
	Vector3[] directions; // Va contenir les rayons, déterminés par précision, distance et angle.
	Mesh meshVue; // Le mesh dont les vertex seront modifiés selon les obstacles.
	Transform meshTransform;
	
	int nbPoints;
	int nbTriangles;
	int nbFaces;
	int nbIndices;
	int ligne;
	Vector3[] points;
	int[] indices;
	#endregion


	#region Méthodes Unity
	// Use this for initialization.
	void Start () {
		// Initialisation du cone.
		GameObject objetVue = new GameObject("ConeVue");
		meshVue = new Mesh();
		(objetVue.AddComponent( typeof( MeshFilter )) as MeshFilter).mesh = meshVue;
		(objetVue.AddComponent( typeof( MeshRenderer )) as MeshRenderer).material = material;
		meshTransform = transform; // Histoire de limiter les getComponent() dans la boucle.
		
		// Préparation des rayons.
		précision = précision > 1 ? précision : 2;
		directions = new Vector3[précision];
		float débutAngle = -angle*0.5F;
		float pasAngle = angle / (précision-1);
		for (int i = 0; i < précision; i++) {
			Matrix4x4 mat = Matrix4x4.TRS (Vector3.zero, Quaternion.Euler(0, (débutAngle + i*pasAngle), 0), Vector3.one);
			directions[i] = mat * Vector3.forward;
		}
		
		// Préparation des outils de manipulation du mesh.
		nbPoints =  précision*2;
		nbTriangles = nbPoints - 2;
		nbFaces = nbTriangles / 2;
		nbIndices =  nbTriangles * 3;
		ligne = nbFaces+1;
		
		points = new Vector3[nbPoints];
		indices = new int[nbIndices];
		
		for (int i = 0; i < nbFaces; i++) {
			indices[i*6+0] = i+0;
			indices[i*6+1] = i+1;
			indices[i*6+2] = i+ligne;
			indices[i*6+3] = i+1;
			indices[i*6+4] = i+ligne+1;
			indices[i*6+5] = i+ligne;
		}
		
		meshVue.vertices = new Vector3[nbPoints];
		meshVue.uv = new Vector2[nbPoints];
		meshVue.triangles = indices;      
		
		StartCoroutine ("Scan");
	}
	#endregion


	#region Autres méthodes
	// Appelle la modification du mesh tous les "fréquence" secondes.
	IEnumerator Scan () {
		while (true) {
			MettreAJourMeshVue ();
			yield return new WaitForSeconds (fréquence);
		}
	}

	// Fonction qui modifie le mesh.
	private void MettreAJourMeshVue () {         
		// Lance les rayons pour placer les vertices le plus loin possible.
		for (int i = 0; i < précision; i++) {
			Vector3 direction = meshTransform.TransformDirection (directions[i]); // Repère objet.
			RaycastHit raycast;
			float distanceRaycast = distance;
			if (Physics.Raycast (meshTransform.position, direction, out raycast, distance, masque)) // Si on touche, on rétrécit le rayon.
				distanceRaycast = raycast.distance;
			
			if (débug) Debug.DrawRay (meshTransform.position, direction*distanceRaycast);
			
			// Positionnement du vertex.
			points[i] = meshTransform.position + direction*distance;
			points[i+précision] = meshTransform.position;
		}
		
		// On réaffecte les vertices.
		meshVue.vertices = points;
		// Normales doivent être calculées pour tout changement du tableau "vertices", même si on travaille sur un plan.
		meshVue.RecalculateNormals ();
	}
	#endregion

}