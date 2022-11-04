using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class TestDraw : MonoBehaviour {

    [Group("Touch Actions", "magenta")]
    [Note("Set action type to button\nAdd binding to Press (or other touch types)\nAdd hold interaction", NoteType.Info, true)]
    [SerializeField] protected InputAction TouchAction;
    [Note(NoteType.Info, "Set action type to value.\nSet value to Vector2 (or any)", true)]
    [SerializeField] protected InputAction TouchPosition;
    [SerializeField] protected Vector2 actionTouchPosition;
    [SerializeField] protected string ActionTouchHitName;
    [SerializeField] protected float minWaypointDistance;
    [SerializeField] protected LayerMask PathingLayerMask;
    protected Ray actionTouchPositionRay;
    protected RaycastHit actionTouchPositionRayHit;
    protected bool performing;
    protected LinkedList<Vector3> wayPoints = new LinkedList<Vector3>();

    void OnEnable(){

        TouchPosition.Enable();
        TouchAction.Enable();
        TouchAction.performed += PerformTouchAction;
        TouchAction.canceled += CanceledTouchAction;
    }
    void OnDisable(){

        TouchAction.performed -= PerformTouchAction;
        TouchAction.canceled -= CanceledTouchAction;
        TouchAction.Disable();
        TouchPosition.Disable();
    }
    protected void PerformTouchAction(InputAction.CallbackContext obj)
    {
        performing = true;
        wayPoints.Clear();
        Debug.Log("Perform Touch Action");
    }

    protected void CanceledTouchAction(InputAction.CallbackContext obj)
    {
        if(performing){
            performing = false;
            Ended();
            Debug.Log("Ended Touch Action");
            Debug.Log($"Waypoints: {wayPoints.Count}");
        }
    }
    protected virtual void Ended(){

    }

    void Update()
    {
        if (performing)
        {
            Perform();
        }
    }
    protected virtual void Perform(){
        
        SetPositionFromInput();
        SetRays();
        if(!PathingRayCast()){
            return;
        }
        
        if (!wayPoints.Any())
        {
            // Add first waypoint without checking distance
            wayPoints.AddFirst(actionTouchPositionRayHit.point);
            return;
        }
        var last = wayPoints.Last.Value;
        float distance = Vector3.Distance(last, actionTouchPositionRayHit.point);
        if (distance >= minWaypointDistance)
        {
            wayPoints.AddLast(actionTouchPositionRayHit.point);
        }
    }

    protected void SetPositionFromInput()
    {
        actionTouchPosition = TouchPosition.ReadValue<Vector2>();
    }
    protected void SetRays()
    {
        actionTouchPositionRay = Camera.main.ScreenPointToRay(actionTouchPosition);
    }
    protected bool PathingRayCast()
    {
        return RayCast(actionTouchPositionRay, PathingLayerMask, out actionTouchPositionRayHit);
    }

    protected static bool RayCast(Ray ray, LayerMask mask, out RaycastHit hit){
        return Physics.Raycast(ray, out hit, 100, mask);
    }
    protected static bool RayCast(Ray ray, LayerMask mask, ref string hitName, out RaycastHit hit)
    {
        bool didHit = RayCast(ray, mask, out hit);
        hitName = didHit ? hit.transform.name : ">_<";
        return didHit;
    }

    protected void DrawRays()
    {
        Draw(actionTouchPositionRay, actionTouchPositionRayHit, Color.magenta, .3f);
    }    
    protected static void Draw(Ray ray, RaycastHit hit, Color color, float size = 0.2f)
    {
        Debug.DrawRay(ray.origin, ray.direction, color);
        Gizmos.color = color;
        Gizmos.DrawWireSphere(hit.point, size);
    }
    public void OnDrawGizmos()
    {
        DrawRays();
        
        if(!wayPoints.Any())
            return;

        for(var node = wayPoints.First; node != null; node = node.Next) {
            var point = node.Value;
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(point, 0.05f);
            if(node.Next != null){
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(point, node.Next.Value);
            }
        }
    }
}   
