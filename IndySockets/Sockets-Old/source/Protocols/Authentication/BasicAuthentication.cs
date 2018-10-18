using System;

using Indy.Sockets.Protocols.Encoding;

namespace Indy.Sockets.Protocols.Authentication {
  public class BasicAuthentication: AuthenticationBase {
    protected string _Realm;
    protected override AuthenticationWhatsNextEnum DoNext() {
      string S = ReadAuthInfo("Basic");
      Core.Global.Fetch(ref S);
      while (S.Length > 0) {
        _Params.Add(Core.Global.Fetch(ref S, ", ").Replace("=", _Params.NameValueSeparator));
      }
      _Realm = _Params.Values("realm").Substring(1);
      switch (_CurrentStep) {
        case 0: {
            if (UserName.Length > 0) {
              return AuthenticationWhatsNextEnum.DoRequest;
            } else {
              return AuthenticationWhatsNextEnum.AskTheProgram;
            }
          }
        case 1: {
            return AuthenticationWhatsNextEnum.Fail;
          }
      }
      return AuthenticationWhatsNextEnum.DoRequest;
    }

    protected override int GetSteps() {
      return 1;
    }

    public override string Authentication() {
      EncoderBase64 mime = new EncoderBase64();
      return "Basic " +
        mime.Encode(UserName + ":" + Password);
    }

    public override bool KeepAlive() {
      return false;
    }

    public override void Reset() {
      base.Reset();
    }

    public string Realm {
      get {
        return _Realm;
      }
      set {
        _Realm = value;
      }
    }

    public override void DecodeAuthParams(string AParams) {
      DecoderBase64 mime = new DecoderBase64();
      string TempString = mime.DecodeString(AParams);
      UserName = Core.Global.Fetch(ref TempString, ":");
      Password = TempString;
    }
  }
}
