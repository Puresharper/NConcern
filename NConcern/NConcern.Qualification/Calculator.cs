using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConcern.Qualification
{
    public class Calculator
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        virtual public int Divide(int a, int b)
        {
            return a / b;
        }

        static public int Multiply(int a, int b)
        {
            return a * b;
        }
    }

    public class Derived : Calculator
    {
        override public int Divide(int a, int b)
        {
            return base.Divide(a, b);
        }
    }
}
