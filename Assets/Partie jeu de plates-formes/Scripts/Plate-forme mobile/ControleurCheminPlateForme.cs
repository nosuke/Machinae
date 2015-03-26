using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

// Note: We take for granted the path is made of right-angles
public class ControleurCheminPlateForme : MonoBehaviour
{
    private static readonly float MovingSpeed = 1.0f * 60.0f;

    // The 'Tracker' is to help us stay on the path
    // The path that we are on is the normal of the line segment between point1 and point2
    // As the path changes direction, our tracker is rotated
    // The 'ahead' line segment helps us decide the direction to take when our path changes
    // (i.e. are we making a right or left turn?)
    Transform tracker = null;
    Transform trackerDirection = null;
    Transform trackerPoint1 = null;
    Transform trackerPoint2 = null;
    Transform trackerAhead1 = null;
    Transform trackerAhead2 = null;

    private Vector2 direction = new Vector2(0, 0);

    private void Awake()
    {
        this.tracker = this.transform.Find("Tracker");
        this.trackerDirection = tracker.Find("Direction");
        this.trackerPoint1 = tracker.Find("Point1");
        this.trackerPoint2 = tracker.Find("Point2");
        this.trackerAhead1 = tracker.Find("Ahead1");
        this.trackerAhead2 = tracker.Find("Ahead2");
    }

    private void Start()
    {
        // Put the platform on track to start off
        if (!SnapToTrack())
        {
            Debug.LogError("Need to place the platform close to a path to start.");
        }
    }

    private void FixedUpdate()
    {
        // Move our platform
		this.transform.Translate(this.direction * ControleurCheminPlateForme.MovingSpeed * Time.deltaTime);

        // We're on the path as long as we have collision between our trackers
        RaycastHit2D hit = GetPathCollision();
        if (!hit)
        {
            // We're off the path now. We need to change direction and find our way again.
            GetBackOnTrack();
        }
    }

    RaycastHit2D GetPathCollision()
    {
        RaycastHit2D hit = Physics2D.Linecast(this.trackerPoint1.position, this.trackerPoint2.position, 1 << LayerMask.NameToLayer("CheminsPlates-Formes"));
        return hit;
    }

    RaycastHit2D GetAheadCollision()
    {
		RaycastHit2D hit = Physics2D.Linecast(this.trackerAhead1.position, this.trackerAhead2.position, 1 << LayerMask.NameToLayer("CheminsPlates-Formes"));
        return hit;
    }

    private bool SnapToTrack()
    {
        RaycastHit2D hit = GetPathCollision();
        if (!hit)
        {
            this.tracker.Rotate(0, 0, 90);
            hit = GetPathCollision();
        }

        if (hit)
        {
            this.transform.position = hit.point;
            this.direction = this.trackerDirection.position - this.transform.position;
            return true;
        }
        return false;
    }

    private bool GetBackOnTrack()
    {
        RaycastHit2D hitAhead = new RaycastHit2D();

        // Look to the left of our current heading
        this.tracker.Rotate(0, 0, 90);
        hitAhead = GetAheadCollision();
        if (!hitAhead)
        {
            // Look to the right of current heading
            this.tracker.Rotate(0, 0, -180);
            hitAhead = GetAheadCollision();

            if (!hitAhead)
            {
                // Look behind our current heading
                this.tracker.Rotate(0, 0, -90);
                hitAhead = GetAheadCollision();
            }
        }

        if (!hitAhead)
        {
            // We can't find anything to attach to so stop moving
            this.direction = Vector2.zero;
        }
        else
        {
            // Our position needs to be calculated to get us back onto the path
            Vector2 delta = hitAhead.point - (Vector2)this.transform.position;
            delta = Vector2.Dot(delta, this.direction) * this.direction;
            this.transform.Translate(delta);

            this.direction = this.trackerDirection.position - this.transform.position;
            return true;
        }

        return false;
    }

}
