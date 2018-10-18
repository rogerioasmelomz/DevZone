using System;
using System.Collections.Generic;

namespace Indy.Sockets.Web
{
	public class RFC2109Cookie: NetscapeCookie
	{
    protected long _MaxAge;
	  protected string _Version = "";
	  protected string _Comment = "";
	  protected override string GetClientCookie()
	  {
	    return base.GetClientCookie();
	  }
	  
	  protected override string GetCookie()
	  {
	    string TempResult = base.GetCookie();
	    
	    if (_MaxAge > -1
	      &&_Expires.Length > 0)
      {
        TempResult = Cookie.AddCookieProperty("max-age", _MaxAge.ToString(), TempResult);
      }
      
      TempResult = Cookie.AddCookieProperty("comment", _Comment, TempResult);
	    TempResult = Cookie.AddCookieProperty("version", _Version, TempResult);
	    return TempResult;
	  }
	  
	  protected override void SetExpires(string AValue)
	  {
	    if (AValue.Length > 0)
	    {
	      try
	      {
					_MaxAge = ((Http.GmtToLocalDateTime(AValue).Subtract(new TimeSpan(DateTime.Now.Ticks)).Ticks / 10) * Core.Global.MSecsPerDay / 1000);
	      }
	      catch
	      {
	      }
	    }
      base.SetExpires(AValue);
	  }
	  
    protected override void LoadProperties(SortedList<string, string> APropertyList)
    {
      base.LoadProperties(APropertyList);
      _MaxAge = Core.Global.StrToInt64Def(APropertyList["MAX-AGE"], -1);
      _Version = APropertyList["VERSION"];
      _Comment = APropertyList["COMMENT"];
      if (_Expires.Length == 0)
      {
        _InternalVersion = CookieVersionEnum.Netscape;
        if (_MaxAge >= 0)
        {
					_Expires = Http.DateTimeGmtToHttpStr(DateTime.Now.Subtract(new TimeSpan((Http.OffsetFromUtc().Ticks + _MaxAge * 1000))));
        }
      }
    }
    
    public RFC2109Cookie()
    {
      _InternalVersion = CookieVersionEnum.RFC2109;
      _MaxAge = Cookie.MaxAge;
    }
    
    public string Comment
    {
      get
      {
        return _Comment;
      }
      set
      {
        _Comment = value;
      }
    }
    
    public long MaxAge
    {
      get
      {
        return _MaxAge;
      }
      set
      {
        _MaxAge = value;
      }
    }
    
    public string Version
    {
      get
      {
        return _Version;
      }
      set
      {
        _Version = value;
      }
    }
	}
}
