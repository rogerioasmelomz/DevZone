using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OutCode.Security
{
    public static class StrCypher
    {

        private const int fdfsdfsf = 256;

        private const int asfasfsdfasdfg = 1000;

        public static string Do(string P1, string P2)
        {
            try
            {
                var yutrqwietyr = Generate256BitsOfRandomEntropy();
                var iuyruify = Generate256BitsOfRandomEntropy();
                var liugi = Encoding.UTF8.GetBytes(P1);
                using (var password = new Rfc2898DeriveBytes(P2, yutrqwietyr, asfasfsdfasdfg))
                {
                    var keyBytes = password.GetBytes(fdfsdfsf / 8);
                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = 256;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var dhfgajkshdgf = symmetricKey.CreateEncryptor(keyBytes, iuyruify))
                        {
                            using (var sdsfasd = new MemoryStream())
                            {
                                using (var defsdfdfg = new CryptoStream(sdsfasd, dhfgajkshdgf, CryptoStreamMode.Write))
                                {
                                    defsdfdfg.Write(liugi, 0, liugi.Length);
                                    defsdfdfg.FlushFinalBlock();

                                    var fdekfjdkfj = yutrqwietyr;
                                    fdekfjdkfj = fdekfjdkfj.Concat(iuyruify).ToArray();
                                    fdekfjdkfj = fdekfjdkfj.Concat(sdsfasd.ToArray()).ToArray();
                                    sdsfasd.Close();
                                    defsdfdfg.Close();
                                    return Convert.ToBase64String(fdekfjdkfj);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static string Undo(string P1, string P2)
        {

            var asdasdfasdfa = Convert.FromBase64String(P1);

            var jkhj = asdasdfasdfa.Take(fdfsdfsf / 8).ToArray();

            var asdfasdf = asdasdfasdfa.Skip(fdfsdfsf / 8).Take(fdfsdfsf / 8).ToArray();

            var asdfasdfffd = asdasdfasdfa.Skip((fdfsdfsf / 8) * 2).Take(asdasdfasdfa.Length - ((fdfsdfsf / 8) * 2)).ToArray();

            using (var rrfrrrffrrfrrfr = new Rfc2898DeriveBytes(P2, jkhj, asfasfsdfasdfg))
            {
                var keyBytes = rrfrrrffrrfrrfr.GetBytes(fdfsdfsf / 8);
                using (var qqaaawwsws = new RijndaelManaged())
                {
                    qqaaawwsws.BlockSize = 256;
                    qqaaawwsws.Mode = CipherMode.CBC;
                    qqaaawwsws.Padding = PaddingMode.PKCS7;
                    using (var decryptor = qqaaawwsws.CreateDecryptor(keyBytes, asdfasdf))
                    {
                        using (var hdffgdu = new MemoryStream(asdfasdfffd))
                        {
                            using (var dsasdasdf = new CryptoStream(hdffgdu, decryptor, CryptoStreamMode.Read))
                            {
                                var adasdfasdfaf = new byte[asdfasdfffd.Length];
                                var sasfasdfasdfasdfa = dsasdasdf.Read(adasdfasdfaf, 0, adasdfasdfaf.Length);
                                hdffgdu.Close();
                                dsasdasdf.Close();
                                return Encoding.UTF8.GetString(adasdfasdfaf, 0, sasfasdfasdfasdfa);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }

}
