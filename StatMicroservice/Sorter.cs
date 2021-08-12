using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatMicroservice
{
    public class Sorter
    {
        private List<KeyValuePair<string, int>>getValueIndexList(IEnumerable<Dictionary<string, string>> toSort, string field)
        {
            List<KeyValuePair<string, int>> result = new List<KeyValuePair<string, int>>();
            int index = 0;
            foreach (var record in toSort)
            {
                string key = null;

                var currentJSON = JObject.Parse(record["eventJson"]);

                JToken jToken;
                if (currentJSON.TryGetValue(field, out jToken) && jToken.Type == JTokenType.String)
                {
                    key = jToken.Value<string>() ?? null;
                }

                result.Add(new KeyValuePair<string, int>(key, index));

                index++;
            }
            return result;
        }

        public void Sort(ref List<Dictionary<string, string>>toSort, string field)
        {
            List<KeyValuePair<string, int>> ValueIndexList = getValueIndexList(toSort,field);

            // Сортируем ValueIndexList
            ValueIndexList.Sort((pair1, pair2) => string.Compare(pair1.Key, pair2.Key));

            // Создаём новый список
            var result = new List<Dictionary<string, string>>();
            
            // Наполняем новый список
            foreach(var onePair in ValueIndexList)
            {
                result.Add( toSort[onePair.Value] );
            }
            toSort = result;
        }
    }
}
