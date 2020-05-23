using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Pluigns
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

        public static string GetParam(string key, object value) => $"{key}{KEY_VALUE_SEPERATOR}{value}";

        public static string EncodeMail(params string[] keyAndValues) => string.Join($"{RECORD_SEPERATOR}", keyAndValues);

        public static MailDecodeDTO DecodeMail(string template)
        {
            MailDecodeDTO decode = new MailDecodeDTO();
            List<string> records = template.Split(new string[] { RECORD_SEPERATOR }, StringSplitOptions.RemoveEmptyEntries).ToList();
            records.ForEach(x =>
            {
                string[] keyAndValue = x.Split(new string[] { KEY_VALUE_SEPERATOR }, StringSplitOptions.RemoveEmptyEntries);
                decode.Records.Add(keyAndValue[0], keyAndValue[1]);
            });
            return decode;
        }

        public class MailDecodeDTO
        {
            /// <summary>
            /// Mail Dataları key value şeklinde.
            /// </summary>
            public Dictionary<string, string> Records { get; set; }

            public MailDecodeDTO()
            {
                Records = new Dictionary<string, string>();
            }

            public string GetValue(string key)
            {
                // Eğer kayıt yok ise boş dön.
                if (!Records.ContainsKey(key))
                    return string.Empty;

                // Value değerini buluyoruz.
                KeyValuePair<string, string> record = Records.FirstOrDefault(x => x.Key == key);

                #region Kaynaklarda  Noktalı olarak dönceğiz.

                // Kaynak değerleri noktalı olarak gösterilecek.
                if (record.Key == KEY_OLD_METAL || record.Key == KEY_OLD_CRYSTAL || record.Key == KEY_OLD_BORON ||
                    record.Key == KEY_NEW_METAL || record.Key == KEY_NEW_CRYSTAL || record.Key == KEY_NEW_BORON)
                {
                    if (double.TryParse(record.Value, out double val))
                        return ResourceExtends.ConvertToDottedResource(val);
                }

                #endregion

                #region Gemileri birleştirip döneceğiz.

                // Eğer data saldıran gemi ise key value şeklinde dönüyoruz.
                if (record.Key == KEY_SHIPS_ATTACKER || record.Key == KEY_SHIPS_DEFENDER)
                {
                    // Gemileri buluyoruz.
                    List<string> ships = record.Value.Split(new string[] { KEY_SHIP_SEPERATOR }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    // Her bir gemiyi satır satır alt alta basıyoruz.
                    return string.Join(Environment.NewLine, ships.Select(e =>
                     {
                         // 1.değer her zaman gemi idsi. 2. olan da miktarı.
                         string[] keyValue = e.Split(new string[] { KEY_SHIP_KEY_VALUE_SEPERATOR }, StringSplitOptions.RemoveEmptyEntries);
                         int shipId = int.Parse(keyValue[0]);
                         int shipCount = int.Parse(keyValue[1]);

                         // Gemi ismini alıp miktar ile basıyoruz.
                         return $"{LanguageController.LC.GetText($"S{shipId}")} : {shipCount}";
                     }));
                }

                #endregion

                // En son normal string ise normal stringi dönüyoruz.
                return record.Value;

            }

            public string GetMailType() => GetValue(KEY_MAIL_TYPE);

            public FleetTypes GetMailAction()
            {
                // Değeri alıyoruz.
                string action = GetValue(KEY_ACTION_TYPE);

                // Eğer mailde değer yok ise dönüş değerini arıyoruz.
                if (string.IsNullOrEmpty(action))
                {
                    // Dönüş değeri de aynı action type ancak dönüş olduğunu belli etmek için kullanıyorduk.
                    action = GetValue(KEY_ACTION_TYPE_RETURN);

                    // Eğer o da yok ise none dönüyoruz.
                    if (string.IsNullOrEmpty(action))
                        return FleetTypes.None;
                }

                // Eğer var ise türünü dönüyoruz.
                return (FleetTypes)int.Parse(action);
            }

            public bool IsReturnMail()
            {
                if (!string.IsNullOrEmpty(GetValue(KEY_ACTION_TYPE_RETURN)))
                    return true;
                return false;
            }

        }

    }
}
