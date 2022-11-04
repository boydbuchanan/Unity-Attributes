using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public enum NoteType
{
    None,
    Info,
    Warning,
    Error,
}
[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
public class NoteAttribute : PropertyAttribute
{
    public readonly string text;
    public readonly bool foldout;

    // MessageType exists in UnityEditor namespace and can throw an exception when used outside the editor.
    // We spoof MessageType at the bottom of this script to ensure that errors are not thrown when
    // MessageType is unavailable.
    public readonly NoteType type;


    /// <summary>
    /// Adds a HelpBox to the Unity property inspector above this field. 
    /// Aprox: 50 chars per line
    /// Add \n to create new lines
    /// </summary>
    /// <param name="text">The help text to be displayed in the HelpBox.</param>
    /// <param name="type">The icon to be displayed in the HelpBox.</param>
    /// <param name="foldout">Hides info with foldout arrow next to it</param>
    public NoteAttribute(string text, NoteType type = NoteType.None, bool foldout = false)
    {
        this.text = text;
        this.type = type;
        this.foldout = foldout;
    }
        /// <summary>
    /// Adds a HelpBox to the Unity property inspector above this field.
    /// </summary>
    public NoteAttribute(string text, bool foldout) : this(text, NoteType.None, foldout) { }
    public NoteAttribute(NoteType type, string text) : this(text, type) { }
    public NoteAttribute(NoteType type, string text, bool foldout) : this(text, type, foldout) { }
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(NoteAttribute))]
public class NoteAttributeDrawer : DecoratorDrawer {
    private const float indent = 10f;
    private const float margin = 3f;
    public bool showContent = false;
    
    public override float GetHeight() {
        var helpBoxAttribute = attribute as NoteAttribute;
        if (helpBoxAttribute == null) return base.GetHeight();
        var helpBoxStyle = (GUI.skin != null) ? GUI.skin.GetStyle("helpbox") : null;
        if (helpBoxStyle == null) return base.GetHeight();
        return Mathf.Clamp(helpBoxStyle.CalcHeight(new GUIContent(!helpBoxAttribute.foldout || showContent ? helpBoxAttribute.text : string.Empty), EditorGUIUtility.currentViewWidth) + (margin*2), 4f, 120f);
    }
 
    public override void OnGUI(Rect position) {
        var helpBoxAttribute = attribute as NoteAttribute;
        if (helpBoxAttribute == null) return;
        string label = helpBoxAttribute.type == NoteType.None ? "Notes" : helpBoxAttribute.type.ToString();
        
        Rect pos = new Rect(position.x + indent, position.y, position.width - indent, margin);
        EditorGUI.DrawRect(pos, Color.clear);
        
        pos = new Rect(pos.x, pos.y + pos.height, position.width - (indent*2), position.height - (margin*2));
        if (helpBoxAttribute.foldout)
            showContent = EditorGUI.Foldout(pos, showContent, showContent ? string.Empty : label);

        if (!helpBoxAttribute.foldout || showContent){
            EditorGUI.HelpBox(pos, helpBoxAttribute.text, GetMessageType(helpBoxAttribute.type));
        }
        pos = new Rect(pos.x, pos.y + pos.height, position.width - indent, margin);
        EditorGUI.DrawRect(pos, Color.clear);
    }
 
    private MessageType GetMessageType(NoteType helpBoxMessageType) {
        switch (helpBoxMessageType) {
            default:
            case NoteType.None:      return MessageType.None;
            case NoteType.Info:      return MessageType.Info;
            case NoteType.Warning:   return MessageType.Warning;
            case NoteType.Error:     return MessageType.Error;
        }
    }
}
#endif
