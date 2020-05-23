using System;
using System.Collections;
using System.Collections.Generic;

namespace WarOfGalaxiesApi
{
    public static class MailEncoder
    {
        public const string KEY_SHIP_KEY_VALUE_SEPERATOR = ":";
        public const string KEY_SHIP_SEPERATOR = "|";
        public const string KEY_VALUE_SEPERATOR = "=";
        public const string RECORD_SEPERATOR = ";";

        public const string KEY_ACTION_TYPE = "AT";
        public const string KEY_ACTION_TYPE_RETURN = "ATR";
        public const string KEY_MAIL_TYPE = "MT";

        public const string KEY_SENDERPLANETNAME = "S";
        public const string KEY_SENDERPLANETCORDINATE = "SC";
        public const string KEY_DESTINATIONPLANETNAME = "D";
        public const string KEY_DESTINATIONPLANETCORDINATE = "DC";
        public const string KEY_OLD_METAL = "OM";
        public const string KEY_OLD_CRYSTAL = "OC";
        public const string KEY_OLD_BORON = "OB";
        public const string KEY_NEW_METAL = "NM";
        public const string KEY_NEW_CRYSTAL = "NC";
        public const string KEY_NEW_BORON = "NB";

        public const string KEY_SHIPS_ATTACKER = "AS";
        public const string KEY_SHIPS_DEFENDER = "DS";

        public static string GetParam(string key, object value)
        {
            if (value.GetType() == typeof(double))
                return $"{key}{KEY_VALUE_SEPERATOR}{Math.Round((double)value)}";
            return $"{key}{KEY_VALUE_SEPERATOR}{value}";
        }

        public static string EncodeMail(IEnumerable<string> keyAndValues) => string.Join($"{RECORD_SEPERATOR}", keyAndValues);

    }
}
