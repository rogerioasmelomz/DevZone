using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Core.Security
{
    public class Scramble
    {

        private string _hash;
        private string _P;

        public string Hash {
            get {return (_hash);}
        }

        private string Info {
            get { return (StringCipher.What (_hash,_P)  ); }
        }

        public Scramble(string A, string B, string C, string D, string E) {

            _hash = string.Format("{0}|{1}|{2}|{3}|{4}",A,B,C,D,E);
            _hash = StringCipher.Thing(_hash,B);
            _P = B;
        }

        public Scramble(string A, string B, int Mode=1)
        {
            if (Mode == 1)
            {
                _hash = A;
                _hash = StringCipher.Thing(_hash, B);
            }
            else
            {
                _hash = A;
            }
            _P = B;
        }

        public void DoThings(ILicenca Lic, int O) {

            Lic.MakeLic(this.Info ,O);
        }
        
        public bool IsEqual(string A) {

            return (A.Equals(_hash,StringComparison.CurrentCultureIgnoreCase));

        }

    }
}

