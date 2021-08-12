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
            
            // Идём по неотсортированному списку
            foreach (var record in toSort)
            {
                string key = null;

                // У каждой записи берём поле с JSON-ом и парсим его
                var currentJSON = JObject.Parse(record["json"]);

                // Берём значение этого ключевого поля и сохраняем в переменную key
                JToken jToken;
                if (currentJSON.TryGetValue(field, out jToken) && jToken.Type == JTokenType.String)
                {
                    key = jToken.Value<string>() ?? null;
                }

                // Записываем в список пару (Значение поля JSON-а, номер записи по порядку в неотсортированном списке)
                result.Add(new KeyValuePair<string, int>(key, index));

                index++;
            }
            return result;
        }

        public void Sort(ref List<Dictionary<string, string>>toSort, string field)
        {
            // Создаём список из таких пар: Pair<Значение поля в JSON-е, Индекс строки>
            List<KeyValuePair<string, int>> ValueIndexList = getValueIndexList(toSort,field);

            // Сортируем пары по значению поля JSON-а
            ValueIndexList.Sort((pair1, pair2) => string.Compare(pair1.Key, pair2.Key));

            // Создаём новый список
            var result = new List<Dictionary<string, string>>();
            
            // Наполняем новый список теми элементами, индекс которых указывают индексы в отсортированном
            //   списке пар
            foreach(var onePair in ValueIndexList)
            {
                result.Add( toSort[onePair.Value] );
            }
            toSort = result;
        }
    }
}
