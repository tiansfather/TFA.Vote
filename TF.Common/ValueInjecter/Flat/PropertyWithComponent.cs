using System.Reflection;

namespace TF.Common.ValueInjecter.Flat
{
    public class PropertyWithComponent
    {
        public PropertyInfo Property { get; set; }

        public object Component { get; set; }

        public int Level { get; set; }
    }
}