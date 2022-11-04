using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.AttributeUsage( System. AttributeTargets.Field, AllowMultiple = true)] 
public class BarAttribute : PropertyAttribute{
    public readonly string colorString;
    public readonly Color color;
    public readonly float size;

    /// <summary> 
    /// Creates a line separator with the color and text height increase. 
    /// Defaults to a height increase of 1. 
    /// Defaults to lightblue if no color is given. 
    /// </summary> 
    /// <param name="size">The amount to increase the height of the text by (Min value of 1).</param>
    /// <param name="colorString">The color can be specified in the traditional HTML format #rrggbbaa.
    /// Can use a name for a list of names "https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual /StyledText.html"</param>
    public BarAttribute(float size = 1, string colorString = "lightblue"){
        this.colorString = colorString;
        this.size = Mathf.Max(1, size);
        if (ColorUtility.TryParseHtmlString(colorString, out this.color)) return;
        this.color = new Color(173, 216, 230); 
        this.colorString = "lightblue";
    }
}
#if UNITY_EDITOR
    [CustomPropertyDrawer( typeof( BarAttribute))] 
    public class BarDrawer : DecoratorDrawer {

        #region Overrides of DecoratorDrawer
        /// <inheritdoc /> 
        public override void OnGUI(Rect position)
        {
            if (!(attribute is BarAttribute barAttribute)) return;
            position = EditorGUI.IndentedRect(position);
            position.yMin += barAttribute.size + 1;
            position.height += barAttribute.size + 1;
            EditorGUI.DrawRect(position, barAttribute.color);
        }
        /// <inheritdoc /> 
        public override float GetHeight(){
            BarAttribute barAttribute = attribute as BarAttribute;
            return (barAttribute?.size ?? 1);
        }
        #endregion
    }
#endif
