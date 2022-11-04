using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.AttributeUsage( System. AttributeTargets.Field, AllowMultiple = true)] 
public class GroupAttribute : PropertyAttribute{
    public readonly string header;
    public readonly string colorString;
    public readonly Color color;
    public readonly float textHeightIncrease;
    public readonly bool showUnderLine;

    /// <summary> 
    /// Creates a header with a line separator with the color. With a Text Height Increase of 1. 
    /// </summary> 
    /// <param name="header">The header to display.</param> /// <param name="colorString">The color can be specified in the traditional HTML format #rrggbbaa.
    /// Can use a name for a list of names "https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual /StyledText.html"</param>
    public GroupAttribute(string header, string colorString) : this(header, false, colorString) { }

    /// <summary> 
    /// Creates a header with a line separator with the color and text height increase. 
    /// Defaults to a height increase of 1. 
    /// Defaults to lightblue if no color is given. 
    /// </summary> 
    /// <param name="header">The header to display.</param>
    /// <param name="textHeightIncrease">The amount to increase the height of the text by (Min value of 1).</param>
    /// <param name="colorString">The color can be specified in the traditional HTML format #rrggbbaa.
    /// Can use a name for a list of names "https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual /StyledText.html"</param>
    public GroupAttribute(string header, bool showLine = false, string colorString = "lightblue", float textHeightIncrease = 1){
        this.header = header; this.colorString = colorString;
        this.textHeightIncrease = Mathf.Max(1, textHeightIncrease);
        this.showUnderLine = showLine;

        if (!ColorUtility.TryParseHtmlString(colorString, out this.color)) 
        {
            this.color = new Color(173, 216, 230); 
            this.colorString = "lightblue";
        }
    }
}
#if UNITY_EDITOR
    [CustomPropertyDrawer( typeof( GroupAttribute))] 
    public class GroupDrawer : DecoratorDrawer {
        private const float barSize = 1f;
        #region Overrides of DecoratorDrawer
        /// <inheritdoc /> 
        public override void OnGUI(Rect position)
        {
            if (!(attribute is GroupAttribute headerAttribute)) return;
            position = EditorGUI.IndentedRect(position); 
            position.yMin += EditorGUIUtility.singleLineHeight * (headerAttribute.textHeightIncrease - 0.5f);

            if(string.IsNullOrEmpty(headerAttribute.header)){
                position.height = headerAttribute.textHeightIncrease;
                EditorGUI.DrawRect(position, headerAttribute.color);
                return;
            }

            GUIStyle style = new GUIStyle( EditorStyles. label) {richText = true}; 
            GUIContent label = new GUIContent(
                $"<color={headerAttribute.colorString}><size={style.fontSize + headerAttribute.textHeightIncrease}><b>{headerAttribute.header}</b></size></color>");
            EditorGUI.LabelField(position, label, style);

            Rect pos = new Rect(position.x, position.y + (position.height-barSize), position.width, barSize);
            EditorGUI.DrawRect(pos, headerAttribute.color);
        }
        /// <inheritdoc /> 
        public override float GetHeight(){
            GroupAttribute headerAttribute = attribute as GroupAttribute; 
            return (EditorGUIUtility.singleLineHeight * (headerAttribute?.textHeightIncrease + 0.5f ?? 0) + barSize);
        }
        #endregion
    }
#endif
