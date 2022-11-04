using System;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class TestInputActions : MonoBehaviour {
    [Group("Old Input", "red")]
    [Note("This returns a position regardless of using Touch Simulator or Device Simulation")]
    [SerializeField] private Vector2 oldMousePosition;
    [SerializeField] private string OldInputHitName;
    private Ray oldInputRay;
    private RaycastHit oldInputRayHit;
    [SerializeField] private bool drawOldInput;
    
    

    [Group("Mouse Actions", "cyan")]
    [Note(NoteType.Info, "Set action type to button\nAdd Binding to left mouse button\nOr use any other button", true)]
    [SerializeField]
    private InputAction ActionButton;

    [SerializeField]
    [Space(5)]
    [Note(NoteType.Info, "Set action type to value\nSet value to Vector2 (or any)\nAdd Binding to Mouse Position", true)]
    private InputAction MousePosition;
    
    [Space(5)]
    [Note("Only returns value if mouse was used.\n")]
    [Note(NoteType.Warning, "Simulating touch or using Device Simulator converts mouse to touch, rendering this 0,0.", true)]
    [SerializeField] public Vector2 actionMousePosition;
    [SerializeField] private string ActionMouseHitName;
    private Ray actionPositionRay;
    private RaycastHit actionPositionRayHit;
    [SerializeField] private bool drawActionMouse;


    [Group("Touch Actions", "magenta")]
    [Note("Set action type to button\nAdd binding to Press (or other touch types)", NoteType.Info, true)]
    [SerializeField] private InputAction TouchAction;
    [Note(NoteType.Info, "Set action type to value.\nSet value to Vector2 (or any)", true)]
    [SerializeField] private InputAction TouchPosition;
    [SerializeField] private Vector2 actionTouchPosition;
    [SerializeField] private string ActionTouchHitName;
    private Ray actionTouchPositionRay;
    private RaycastHit actionTouchPositionRayHit;
    [SerializeField] private bool drawActionTouch;

    [Group("Current Mouse")]        
    [Note("Replicates old mouse position\nException: Only returns value if mouse was used.\nSimulating touch or using Device Simulator converts mouse to touch, rendering this 0,0.")]
    [SerializeField] private Vector2 currentMousePosition;
    [SerializeField] private string CurrentMouseHitName;
    private Ray currentMouseRay;
    private RaycastHit currentMouseRayHit;
    [SerializeField] private bool drawCurrentMouse;

    
    void OnEnable(){
        // Manually simulate touch (in game preview tab) wtihout using device simulator
        // TouchSimulation.Enable();
        MousePosition.Enable();
        ActionButton.Enable();
        ActionButton.performed += PerformAction;

        TouchPosition.Enable();
        TouchAction.Enable();
        TouchAction.performed += PerformTouchAction;
    }
    void OnDisable(){
        ActionButton.performed -= PerformAction;
        ActionButton.Disable();
        MousePosition.Disable();

        TouchAction.performed -= PerformTouchAction;
        TouchAction.Disable();
        TouchPosition.Disable();
    }
    private void PerformAction(InputAction.CallbackContext obj)
    {
        Debug.Log("Perform Action");
        SetPositionFromInput();
        SetRays();
        RayCast();
    }
    private void PerformTouchAction(InputAction.CallbackContext obj)
    {
        Debug.Log("Perform Touch Action");
        SetPositionFromInput();
        SetRays();
        RayCast();
    }

    void Update()
    {
        // Old input action
        if (drawOldInput && Input.GetMouseButtonDown(0))
        {
            SetPositionFromInput();
            SetRays();
            RayCast();
        }
    }
    private void SetPositionFromInput()
    {
        oldMousePosition = Input.mousePosition;
        currentMousePosition = Mouse.current.position.ReadValue();

        actionMousePosition = MousePosition.ReadValue<Vector2>();
        actionTouchPosition = TouchPosition.ReadValue<Vector2>();
    }
    private void SetRays()
    {
        oldInputRay = Camera.main.ScreenPointToRay(oldMousePosition);
        currentMouseRay = Camera.main.ScreenPointToRay(currentMousePosition);

        actionPositionRay = Camera.main.ScreenPointToRay(actionMousePosition);
        actionTouchPositionRay = Camera.main.ScreenPointToRay(actionTouchPosition);
    }
    private void RayCast()
    {
        RayCast(oldInputRay, ref OldInputHitName, out oldInputRayHit);
        RayCast(currentMouseRay, ref CurrentMouseHitName, out currentMouseRayHit);
        RayCast(actionPositionRay, ref ActionMouseHitName, out actionPositionRayHit);
        RayCast(actionTouchPositionRay, ref ActionTouchHitName, out actionTouchPositionRayHit);
    }

    private static void RayCast(Ray ray, ref string hitName, out RaycastHit hit)
    {
        if (Physics.Raycast(ray, out hit))
            hitName = hit.transform.name;
        else
            hitName = ">_<";
    }

    private void DrawRays()
    {
        if(drawOldInput)
            Draw(oldInputRay, oldInputRayHit, Color.red, .15f);
        if(drawCurrentMouse)
            Draw(currentMouseRay, currentMouseRayHit, Color.yellow, .2f);
        if(drawActionMouse)
            Draw(actionPositionRay, actionPositionRayHit, Color.cyan, .25f);
        if(drawActionTouch)
            Draw(actionTouchPositionRay, actionTouchPositionRayHit, Color.magenta, .3f);
    }    
    private static void Draw(Ray ray, RaycastHit hit, Color color, float size = 0.2f)
    {
        Debug.DrawRay(ray.origin, ray.direction, color);
        Gizmos.color = color;
        Gizmos.DrawWireSphere(hit.point, size);
    }
    public void OnDrawGizmos()
    {
        DrawRays();
    }
}   