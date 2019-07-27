using System;
using System.Collections.Generic;
using System.Text;

namespace UniOrm
{
    public class RegistedModelTypes
    {
        public RegistedModelTypes()
        {

        }
        protected static List<Type> registedModelTypes { get; set; } = new List<Type>();
        public List<Type> GetAllTypes()
        {
            return registedModelTypes;
        }
        public void AddTypes(IEnumerable<Type> types)
        {
            foreach (var t in types)
            {
                if (!registedModelTypes.Contains(t))
                {
                    registedModelTypes.Add(t);
                }
            }
        }

        public void AddTypes(params Type[] types)
        {
            foreach (var t in types)
            {
                if (!registedModelTypes.Contains(t))
                {
                    registedModelTypes.Add(t);
                }
            }
        }
    }
}
