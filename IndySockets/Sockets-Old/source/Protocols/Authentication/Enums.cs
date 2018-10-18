using System;

namespace Indy.Sockets.Protocols.Authentication
{
  [Flags]
  public enum AuthenticationSchemesEnum
  {
    Unknown,
    Base,
    Digest,
    NTLM
  }
  
  public enum AuthenticationWhatsNextEnum
  {
    AskTheProgram,
    DoRequest,
    Fail
  }
}
