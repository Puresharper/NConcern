using System;
using System.Linq;

namespace NConcern.Example
{
    public class Forum : IForum
    {
        public Account CreateAccount(string name)
        {
            //Check name conflict.
            if (Storage.Query<Account>().Any(_Account => _Account.Name == name)) { throw new NotSupportedException("An account with same name already exists."); }
     
            //Create an account.
            var _account = new Account(name);

            //Add account into storage.
            Storage.Add(_account);

            //Return account.
            return _account;
        }

        public Discussion CreateDiscussion(string name)
        {
            //Check name conflict.
            if (Storage.Query<Discussion>().Any(_Discussion => _Discussion.Name == name)) { throw new NotSupportedException("A discussion with same name already exists."); }

            //Create an discussion.
            var _discussion = new Discussion(name);

            //Add discussion into storage.
            Storage.Add(_discussion);

            //Return discussion.
            return _discussion;
        }

        public Post CreatePost(string discussion, string account, string message)
        {
            //Query discussion by name.
            var _discussion = Storage.Query<Discussion>().SingleOrDefault(_Discussion => _Discussion.Name == discussion);

            //Query account by name.
            var _account = Storage.Query<Account>().SingleOrDefault(_Account => _Account.Name == account);

            //Create post.
            var _post = new Post(_discussion, _account, message);

            //Add post into storage.
            Storage.Add(_post);

            //Return post.
            return _post;
        }

        public Post[] GetPosts(string discussion)
        {
            //Query discussion by name.
            var _discussion = Storage.Query<Discussion>().SingleOrDefault(_Discussion => _Discussion.Name == discussion);

            //Query posts by discussion.
            var _posts = Storage.Query<Post>(_Post => _Post.Discussion == _discussion).ToArray();

            //Return posts.
            return _posts;
        }
    }
}
