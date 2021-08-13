using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Tester
{
    class InputKeeper
    {
        List<JToken> _objects = null;

        public List<JToken> objects
        {
            get
            {
                if (_objects == null)
                {
                    GenerateObjects();
                }
                return _objects;
            }
        } 

        private void GenerateObjects()
        {
            _objects = new List<JToken>();
            for (int i=0;i<40;i++)
            {
                dynamic jsonObj = new JObject();
                jsonObj.field1 = string.Format("{0:D2}", 40 - i);
                jsonObj.field2 = string.Format("{0:D2}", 41 - i);

                dynamic oneObject = new JObject();
                oneObject.key = "some_key";
                oneObject.json = jsonObj;
                oneObject.dt = new DateTime(1980+i, 1, 2, 11, 30, 15).ToString();
                _objects.Add(oneObject);
            }
        }

    }
}
