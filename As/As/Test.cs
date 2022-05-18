using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace As
{
    public class Test : Parent
    {
        public Test()
        {
            if (MyProperty.HasValue == false)
            {
                MyProperty = 4;
            }
        }

        public int? MyProperty { get; set; }
    }

    public class Parent
    {
        public int Asd { get; set; }
    }
}
