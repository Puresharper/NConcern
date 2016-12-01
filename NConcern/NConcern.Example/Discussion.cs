using System;

namespace NConcern.Example
{
    public class Discussion
    {
        public string Name { get; private set; }

        public Discussion([Requiered] string name)
        {
            this.Name = name;
        }
    }
}
