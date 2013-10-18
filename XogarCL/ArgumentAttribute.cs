using System;

namespace XogarCL
{
    public class ArgumentAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Example { get; set; }
    }
}
