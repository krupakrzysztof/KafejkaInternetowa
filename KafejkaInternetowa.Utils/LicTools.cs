using System.Security.Cryptography;
using System.Text;

namespace KafejkaInternetowa.Utils
{
    public static class LicTools
    {
        /// <summary>
        /// Generowanie podpisu licencji
        /// </summary>
        /// <param name="licence">Licencja do podpisania</param>
        /// <returns></returns>
        internal static string GenerateSignature(Licence licence)
        {
            string hash = string.Empty;
            foreach (byte item in new SHA256Managed().ComputeHash(Encoding.ASCII.GetBytes(licence.GetLicenceWithoutSignature())))
            {
                hash += item.ToString("x2");
            }

            return hash.ToUpper();
        }
    }
}
