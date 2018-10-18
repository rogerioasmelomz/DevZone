using System;
using System.Collections.Generic;

namespace Indy.Sockets.Web
{
	public class CookieList: List<NetscapeCookie>
	{
    public NetscapeCookie Cookie(int Index)
    {
      return (NetscapeCookie)this[Index];
    }

    public int IndexOfCookie(string name) {
      for (int i = 0; i < Count; i++) {
        if (String.Equals(this[i].CookieName, name, StringComparison.InvariantCultureIgnoreCase)) {
          return i;
        }
      }
      return -1;
    }
  }
}
