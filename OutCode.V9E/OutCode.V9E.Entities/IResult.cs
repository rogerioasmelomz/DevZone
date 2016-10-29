using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutCode.V9E.Interfaces
{
    public interface IResult
    {
        void SetResult(bool Erro,int ErrorNumber,string ErrorMessage);
    }

}
