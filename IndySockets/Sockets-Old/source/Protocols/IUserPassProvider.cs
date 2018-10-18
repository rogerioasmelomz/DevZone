using System;
using System.Collections.Generic;
using System.Text;

namespace Indy.Sockets.Protocols {
	public interface IUserPassProvider {
		string UserName {
			get;
		}

		string Password {
			get;
		}
	}
}
