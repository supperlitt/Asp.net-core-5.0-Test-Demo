using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml;

namespace WebCoreTest5.Tool
{
    public class SignKey
    {
        public static SecurityKey GetSigningKey(IConfiguration configuration, string fileName)
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            var rsa = new RSACryptoServiceProvider(2048);
            if (File.Exists(file))
            {
                rsa.FromXmlString2(File.ReadAllText(file));
            }
            else
            {
                var contents = rsa.ToXmlString2(true);
                File.WriteAllText(file, contents);
            }

            return new RsaSecurityKey(rsa.ExportParameters(true)); ;
        }
    }

    internal static class RsaKeyExtensions
    {
        #region XML

        public static void FromXmlString2(this RSA rsa, string xmlString)
        {
            RSAParameters parameters = new RSAParameters();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue", StringComparison.InvariantCulture))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus":
                            parameters.Modulus = (string.IsNullOrEmpty(node.InnerText)
                                ? null
                                : Convert.FromBase64String(node.InnerText));
                            break;
                        case "Exponent":
                            parameters.Exponent = (string.IsNullOrEmpty(node.InnerText)
                                ? null
                                : Convert.FromBase64String(node.InnerText));
                            break;
                        case "P":
                            parameters.P = (string.IsNullOrEmpty(node.InnerText)
                                ? null
                                : Convert.FromBase64String(node.InnerText));
                            break;
                        case "Q":
                            parameters.Q = (string.IsNullOrEmpty(node.InnerText)
                                ? null
                                : Convert.FromBase64String(node.InnerText));
                            break;
                        case "DP":
                            parameters.DP = (string.IsNullOrEmpty(node.InnerText)
                                ? null
                                : Convert.FromBase64String(node.InnerText));
                            break;
                        case "DQ":
                            parameters.DQ = (string.IsNullOrEmpty(node.InnerText)
                                ? null
                                : Convert.FromBase64String(node.InnerText));
                            break;
                        case "InverseQ":
                            parameters.InverseQ = (string.IsNullOrEmpty(node.InnerText)
                                ? null
                                : Convert.FromBase64String(node.InnerText));
                            break;
                        case "D":
                            parameters.D = (string.IsNullOrEmpty(node.InnerText)
                                ? null
                                : Convert.FromBase64String(node.InnerText));
                            break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            rsa.ImportParameters(parameters);
        }

        public static string ToXmlString2(this RSA rsa, bool includePrivateParameters)
        {
            RSAParameters parameters = rsa.ExportParameters(includePrivateParameters);

            return string.Format(CultureInfo.InvariantCulture,
                "<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                parameters.Modulus != null ? Convert.ToBase64String(parameters.Modulus) : null,
                parameters.Exponent != null ? Convert.ToBase64String(parameters.Exponent) : null,
                parameters.P != null ? Convert.ToBase64String(parameters.P) : null,
                parameters.Q != null ? Convert.ToBase64String(parameters.Q) : null,
                parameters.DP != null ? Convert.ToBase64String(parameters.DP) : null,
                parameters.DQ != null ? Convert.ToBase64String(parameters.DQ) : null,
                parameters.InverseQ != null ? Convert.ToBase64String(parameters.InverseQ) : null,
                parameters.D != null ? Convert.ToBase64String(parameters.D) : null);
        }

        #endregion
    }
}
