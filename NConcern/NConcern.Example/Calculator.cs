using System;
using System.ServiceModel;

namespace NConcern.Example
{
    [ServiceContract]
    public class Calculator
    {
        [OperationContract]
        public int Add(int a, int b)
        {
            return a + b;
        }

        [OperationContract]
        public int Divide(int a, int b)
        {
            return a / b;
        }
    }
}
