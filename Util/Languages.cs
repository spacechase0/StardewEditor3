using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.Util
{
    public class Languages
    {
        public static string[] GetList()
        {
            return new string[]
            {
                "Japanese",
                "Russian",
                "Chinese",
                "Portugese",
                "Spanish",
                "German",
                "Thai",
                "French",
                "Korean",
                "Italian",
                "Turkish",
                "Hungarian",
            };
        }

        public static string LanguageToCode(string lang)
        {
            switch ( lang.ToLower() )
            {
                case "japanese": return "ja";
                case "russian": return "ru";
                case "chinese": return "zh";
                case "portugese": return "pt";
                case "spanish": return "es";
                case "german": return "de";
                case "thai": return "th";
                case "french": return "fr";
                case "korean": return "ko";
                case "italian": return "it";
                case "turkish": return "tr";
                case "hungarian": return "hu";
            }
            throw new ArgumentException("Unsupported language");
        }

        public static string CodeToLanguage(string code)
        {
            switch (code.ToLower())
            {
                case "ja": return "Japanese";
                case "ru": return "Russian";
                case "zh": return "Chinese";
                case "pt": return "Portugese";
                case "es": return "Spanish";
                case "de": return "German";
                case "th": return "Thai";
                case "fr": return "French";
                case "ko": return "Korean";
                case "it": return "Italian";
                case "tr": return "Turkish";
                case "hu": return "Hungarian";
            }
            throw new ArgumentException("Unsupported language");
        }
    }
}
