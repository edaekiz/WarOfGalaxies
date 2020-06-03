using UnityEngine;

namespace Assets.Scripts.Extends
{
    public static class TextExtends
    {
        public static string MakeItColorize(string content, string color)
        {
            return $"<color={color}>{content}</color>";
        }

        public static string MakeItColorize(string content, string endFixOfContent, string color)
        {
            return $"<color={color}>{content} {endFixOfContent} </color>";
        }

        public static string MakeItColorize(string content, string endFixOfContent, string color, string value, string valueColour)
        {
            return $"<color={color}>{content} {endFixOfContent} </color><color={valueColour}>{value}</color>";
        }

        public static string MakeItColorize(string content, string endFixOfContent, string color, string value)
        {
            return $"<color={color}>{content} {endFixOfContent} </color>{value}";
        }
    }
}
