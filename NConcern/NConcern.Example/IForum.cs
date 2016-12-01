using System;
using System.ServiceModel;

namespace NConcern.Example
{
    [ServiceContract]
    public interface IForum
    {
        [OperationContract]
        Account CreateAccount(string name);

        [OperationContract]
        Discussion CreateDiscussion(string name);

        [OperationContract]
        Post CreatePost(string discussion, string account, string message);

        [OperationContract]
        Post[] GetPosts(string discussion);
    }
}
