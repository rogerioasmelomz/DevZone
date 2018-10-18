using System;
using System.Collections.Generic;
using System.Text;

namespace Indy.Sockets.Core
{
  public class Yarn: IDisposable
  {
    public virtual void Dispose()
    {
      GC.SuppressFinalize(this);
    }
  }
}
