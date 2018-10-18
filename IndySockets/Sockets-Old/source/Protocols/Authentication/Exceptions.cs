using System;

using Indy.Sockets.Core;

namespace Indy.Sockets.Protocols.Authentication
{
  public class AlreadyRegisteredAuthenticationMethodException: IndyException
  {
    public AlreadyRegisteredAuthenticationMethodException(string MethodTypeName):
      base(String.Format(ResourceStrings.AuthenticationMethodAlreadyRegistered, MethodTypeName))
    {
    }
  }
  
  public class InvalidAlgorithmException: IndyException
  {
    public InvalidAlgorithmException():base(ResourceStrings.InvalidHashMethod)
    {
    }
  }
}
