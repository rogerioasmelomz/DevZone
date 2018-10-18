using ADODB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutCode.Database
{
    public interface ISqlExecute
    {
        void ExecutaSql(string SqlCommand, bool SilentMode=true);

        DataTable Consulta(string SqlQuery);

        void SetVersions(double PltVersion, double DBVersion);
    }
}
