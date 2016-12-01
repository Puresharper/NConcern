using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConcern.Example
{
    public class Post
    {
        public Discussion Discussion { get; private set; }
        public Account Account { get; private set; }
        public string Message { get; private set; }
        public DateTime Datetime { get; private set; }

        public Post([Requiered] Discussion discussion, [Requiered] Account account, [Requiered] string message)
        {
            this.Discussion = discussion;
            this.Account = account;
            this.Message = message;
            this.Datetime = DateTime.Now;
        }
    }
}
