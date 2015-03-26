using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

class ControleurBlocApparaissant : MonoBehaviour
{
    // How far we can go up to get our of a collision
    public float limite = 16.0f;

    private Animator animateur = null;

    public void Apparaitre()
    {
        this.collider2D.enabled = true;
        this.renderer.enabled = true;

        this.animateur.SetTrigger("Apparition");
        VérifieCollisionJoueur();
    }

    public void Disparaitre()
    {
        this.collider2D.enabled = false;
        this.renderer.enabled = false;
    }


    private void Awake()
    {
        this.animateur = GetComponent<Animator>();
        Disparaitre();
    }


    private Rect GetBoxCollider2DRect(GameObject go)
    {
        BoxCollider2D box2d = go.GetComponent<BoxCollider2D>();
        Vector2 pos = box2d.transform.position;
        Rect rect = new Rect();
        rect.xMin = pos.x + box2d.center.x - box2d.size.x * 0.5f;
        rect.xMax = pos.x + box2d.center.x + box2d.size.x * 0.5f;
        rect.yMin = pos.y + box2d.center.y - box2d.size.y * 0.5f;
        rect.yMax = pos.y + box2d.center.y + box2d.size.y * 0.5f;
        return rect;
    }

    private Rect SommeMinkowski(Rect rc1, Rect rc2)
    {
        Rect rc = rc1;
        rc.xMin -= rc2.width * 0.5f;
        rc.xMax += rc2.width * 0.5f;
        rc.yMin -= rc2.height * 0.5f;
        rc.yMax += rc2.height * 0.5f;
        return rc;
    }

    private void VérifieCollisionJoueur()
    {
        // Does the player overlap with this appearing block?
        // If so, push the player out of the way
        // Prefer to put the player on top of the appearing block if you can
        // Because the player's box collider is disabled (he uses raycasts for collisions) then we can't rely on OnCollisionEnter type functions.

        GameObject joueur = (GameObject)GameObject.FindGameObjectWithTag("Player");
		Rect rcJoueur = GetBoxCollider2DRect(joueur);
        Rect rcBloc = GetBoxCollider2DRect(this.gameObject);

        // If there's an overlap than we must push the player out of the way
        // Prefer to push him up if we can, otherwise push left/right (never push down)
		if (rcBloc.Overlaps(rcJoueur))
        {
            // How much we move the player out of the way
            Vector2 playerDelta = Vector2.zero;

            // Reduce the player to a point
			Vector2 ptPlayer = rcJoueur.center;

            // Do a Minkowski sum on the block
			rcBloc = SommeMinkowski(rcBloc, rcJoueur);

            float up = rcBloc.yMax - ptPlayer.y;
            float right = rcBloc.xMax - ptPlayer.x;
            float left = ptPlayer.x - rcBloc.xMin;
            if (up <= this.limite)
            {
                // Prefer to push up
                playerDelta.y = up;
            }
            else if (left < right)
            {
                // Push left
                playerDelta.x = -left;
            }
            else
            {
                // Push right
                playerDelta.x = right;
            }

            // Move the player out of the way
			joueur.transform.Translate(playerDelta);
        }
    }

}