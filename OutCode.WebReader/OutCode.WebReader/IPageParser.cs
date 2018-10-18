using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutCode.WebReader
{
    public interface IPageParser
    {
        void PageLoaded(string PageText);
    }
}
