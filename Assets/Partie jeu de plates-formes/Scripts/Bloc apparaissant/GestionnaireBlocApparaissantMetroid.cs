using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

// A simple class that goes through our tagged appearing blocks and controls when they're active
// Takes for granted we have 3 such groups of blocks, which is fine for a demo.
class GestionnaireBlocApparaissantMetroid : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(RoutineBloc());
    }

	private IEnumerator RoutineBloc()
    {
        yield return new WaitForSeconds(2.0f);

        const float tempsAttente = 1.05f;
        while (true)
        {
            // Group 1 appears
			FaireApparaitreBlocs(ObtenirControleursBloc(1));
			yield return new WaitForSeconds(tempsAttente);

            // Group 2 appears
			FaireApparaitreBlocs(ObtenirControleursBloc(2));
			yield return new WaitForSeconds(tempsAttente);

            // Group 1 dissappears, Group 3 appears
			FaireDisparaitreBlocs(ObtenirControleursBloc(1));
			FaireApparaitreBlocs(ObtenirControleursBloc(3));
			yield return new WaitForSeconds(tempsAttente);

            // Group 2 disappears
			FaireDisparaitreBlocs(ObtenirControleursBloc(2));
			yield return new WaitForSeconds(tempsAttente);

            // Group 3 dissappears
			FaireDisparaitreBlocs(ObtenirControleursBloc(3));
        }
    }

	IList<ControleurBlocApparaissant> ObtenirControleursBloc(int nombreGroupe)
    {
		List<ControleurBlocApparaissant> controleurs = new List<ControleurBlocApparaissant>();

		string tag = String.Format("BlocsApparaissants{0}", nombreGroupe);
        GameObject[] objetsJeu = GameObject.FindGameObjectsWithTag(tag);

		if (objetsJeu != null)
        {
			foreach (GameObject objetJeu in objetsJeu)
            {
				ControleurBlocApparaissant[] comps = objetJeu.GetComponentsInChildren<ControleurBlocApparaissant>();
				controleurs.AddRange(comps);
                if (comps != null)
                {
					controleurs.AddRange(comps);
                }
            }
        }

		return controleurs;
    }

	private void FaireApparaitreBlocs(IList<ControleurBlocApparaissant> blocs)
    {
        foreach (var bloc in blocs)
        {
            bloc.Apparaitre();
        }
    }

	private void FaireDisparaitreBlocs(IList<ControleurBlocApparaissant> blocs)
    {
        foreach (var bloc in blocs)
        {
            bloc.Disparaitre();
        }
    }

}

