using Assets.Scripts.Enums;
using Assets.Scripts.Extends;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Pluigns
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

        public static string GetParam(string key, object value) => $"{key}{KEY_VALUE_SEPERATOR}{value}";

        public static string EncodeMail(params string[] keyAndValues) => string.Join($"{RECORD_SEPERATOR}", keyAndValues);

        public static MailDecodeDTO DecodeMail(string template)
        {
            MailDecodeDTO decode = new MailDecodeDTO();
            List<string> records = template.Split(new string[] { RECORD_SEPERATOR }, StringSplitOptions.None).ToList();
            records.ForEach(x =>
            {
                string[] keyAndValue = x.Split(new string[] { KEY_VALUE_SEPERATOR }, StringSplitOptions.None);
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
                if (record.Key == KEY_NEW_METAL || record.Key == KEY_NEW_CRYSTAL || record.Key == KEY_NEW_BORON)
                {
                    if (double.TryParse(record.Value, out double val))
                        return ResourceExtends.ConvertToDottedResource(val);
                }

                #endregion

                // En son normal string ise normal stringi dönüyoruz.
                return record.Value;

            }

            public MailTypes GetMailType()
            {
                string at = GetValue(KEY_MAIL_TYPE);
                if (int.TryParse(at, out int mailType))
                    return (MailTypes)mailType;
                return MailTypes.None;
            }

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

            public List<Tuple<T, int>> GetManyItem<T>(string key) where T : Enum, IConvertible
            {
                // Verilen keye ait dataları alıyoruz.
                string data = GetValue(key);

                // Eğer kayıt yok ise boş bir liste dönüyoruz.
                if (string.IsNullOrEmpty(data))
                    return new List<Tuple<T, int>>();

                // Var ise her bir key valueyi alıyoruz datadan.
                string[] items = data.Split(new string[] { KEY_MANY_ITEM_SEPERATOR }, StringSplitOptions.None);

                // Her birini dönüp gemi idsini ve miktarını alıyoruz.
                return items.Select(x =>
                {
                    // 1. Paramtre item idsi 2.parametre ise miktarı.
                    string[] itemWithQuantityQuantity = x.Split(new string[] { KEY_MANY_ITEM_KEY_VALUE_SEPERATOR }, StringSplitOptions.None);

                    // Tuple olarak geri dönüyoruz.
                    return new Tuple<T, int>((T)(object)int.Parse(itemWithQuantityQuantity[0]), int.Parse(itemWithQuantityQuantity[1]));

                }).ToList();
            }

        }

    }
}
