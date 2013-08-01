using System.Security.Principal;

namespace Zaz.Server.Advanced.Service.Security
{
    class BasicAuthenticationIdentity : GenericIdentity
    {
        public BasicAuthenticationIdentity(string name, string password)
            : base(name, "Basic")
        {
            Password = password;
        }

        public string Password { get; private set; }
    }
}