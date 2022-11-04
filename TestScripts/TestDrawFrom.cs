using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class TestDrawFrom : TestDraw {

    [Group("Starting From", "yellow")]
    [SerializeField] private LayerMask StartFromLayerMask;
    
    [SerializeField] private string StartingFromName;
    [Note("Zero is an infinite duration")]
    [Range(0,10)]
    [SerializeField] private float Duration;

    private bool hasHitStart;
    private bool hasMovedFromStart;
    private float timeLeft;
    protected override void Ended()
    {
        base.Ended();
        hasHitStart = false;
        hasMovedFromStart = false;
    }

    protected override void Perform()
    {
        SetPositionFromInput();
        SetRays();
        // Do first to get gizmo to show
        bool wasPathHit = PathingRayCast();
        // If we haven't hit a starting object, look for it
        if(!hasHitStart){
            hasHitStart = RayCast(actionTouchPositionRay, StartFromLayerMask, ref StartingFromName, out RaycastHit startHit);
        }
        // If we have hit a starting object, look for when we stop hitting it
        if(hasHitStart && !hasMovedFromStart){
            // when we no longer hit the starting object, then we can begin recording waypoints
            hasMovedFromStart = !RayCast(actionTouchPositionRay, StartFromLayerMask, out RaycastHit startHit);
            timeLeft = Duration;
        }

        // Only begin when the player has 'hit' a movable object and has then moved off the object during a single hold
        // If duration was set and timeleft reaches 0, stop recording
        if(!hasMovedFromStart || (Duration > 0 && timeLeft <= 0)){
            return;
        }
        // Only track time if duration is set
        if(Duration > 0)
            timeLeft -= Time.deltaTime;

        if (!wayPoints.Any())
        {
            
            if(!hasMovedFromStart || !wasPathHit)
                return;
            
            // Add starting point
            wayPoints.AddFirst(actionTouchPositionRayHit.point);
            return;
        }
        
        if (wasPathHit)
        {
            var last = wayPoints.Last.Value;
            float distance = Vector3.Distance(last, actionTouchPositionRayHit.point);
            if (distance >= minWaypointDistance)
            {
                wayPoints.AddLast(actionTouchPositionRayHit.point);
            }
        }
    }
}
