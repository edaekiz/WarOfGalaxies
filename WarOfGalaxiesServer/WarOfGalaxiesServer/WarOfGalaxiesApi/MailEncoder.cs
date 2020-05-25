using System;
using System.Collections;
using System.Collections.Generic;

namespace WarOfGalaxiesApi
{
    public static class MailEncoder
    {
        public const string KEY_VALUE_SEPERATOR = "=";
        public const string RECORD_SEPERATOR = ";";

        public const string KEY_MANY_ITEM_KEY_VALUE_SEPERATOR = ":";
        public const string KEY_MANY_ITEM_SEPERATOR = "|";

        public const string KEY_ACTION_TYPE = "AT";
        public const string KEY_ACTION_TYPE_RETURN = "ATR";
        public const string KEY_MAIL_TYPE = "MT";

        public const string KEY_SENDERPLANETNAME = "SM";
        public const string KEY_SENDERPLANETCORDINATE = "SC";
        public const string KEY_DESTINATIONPLANETNAME = "DM";
        public const string KEY_DESTINATIONPLANETCORDINATE = "DC";

        public const string KEY_NEW_METAL = "NM";
        public const string KEY_NEW_CRYSTAL = "NC";
        public const string KEY_NEW_BORON = "NB";

        public const string KEY_BUILDING_DEFENDER = "BD";

        public const string KEY_SHIPS_ATTACKER = "SA";

        public const string KEY_SHIPS_DEFENDER = "SD";

        public const string KEY_DEFENSES = "D";

        public const string KEY_RESEARCHES = "R";
       

        public static string GetParam(string key, object value)
        {
            if (value.GetType() == typeof(double))
                return $"{key}{KEY_VALUE_SEPERATOR}{Math.Round((double)value)}";
            return $"{key}{KEY_VALUE_SEPERATOR}{value}";
        }

        public static string EncodeMail(IEnumerable<string> keyAndValues) => string.Join($"{RECORD_SEPERATOR}", keyAndValues);

    }
}
