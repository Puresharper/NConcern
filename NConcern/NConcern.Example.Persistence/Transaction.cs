using System;

namespace NConcern.Example.Persistence
{
    public class Transaction : IDisposable
    {
        public Transaction()
        {
            Console.WriteLine("Transaction created");
        }

        public void Commit()
        {
            Console.WriteLine("Transaction committed");
        }

        public void Dispose()
        {
            Console.WriteLine("Transaction disposed");
        }
    }
}
