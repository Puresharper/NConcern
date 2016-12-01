using System;

namespace NConcern.Example
{
    public class Account
    {
        public string Name { get; private set; }

        public Account([Requiered] string name)
        {
            this.Name = name;
        }
    }
}
