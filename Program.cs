using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using Ionic.Zip;

namespace turbotax_explorer
{
    class Program
    {
        static void Main(string[] args)
        {
            string turbotax2017_data_container = @"Form_1040_Individual_Tax_Return.tax2017";

            ZipFile zf = new ZipFile(turbotax2017_data_container);
            zf.CompressionMethod = CompressionMethod.Deflate;
            zf.CompressionLevel = Ionic.Zlib.CompressionLevel.BestSpeed;
            zf.Encryption = EncryptionAlgorithm.WinZipAes256;
            zf.ExtractAll(".", ExtractExistingFileAction.OverwriteSilently);

            AesCryptoServiceProvider aes_csp = new AesCryptoServiceProvider();

            byte[] manif = File.ReadAllBytes("manifest.xml");

            aes_csp.Mode = CipherMode.CBC;
            aes_csp.Padding = PaddingMode.PKCS7;
            aes_csp.KeySize = 0x80;
            aes_csp.BlockSize = 0x80;
            aes_csp.IV = Encoding.UTF8.GetBytes("!QAZ2WSX#EDC4RFV");
            aes_csp.Key = Encoding.UTF8.GetBytes("5TGB@YHN7UJM(IK(");

            string str_manifest = "";
            using (ICryptoTransform transform = aes_csp.CreateDecryptor())
            {
                byte[] buffer = transform.TransformFinalBlock(manif, 0, manif.Length);

                str_manifest = Encoding.UTF8.GetString(buffer);
            }
            Console.WriteLine(str_manifest);

            int pos1 = str_manifest.IndexOf("<entityKey>");
            int pos2 = str_manifest.IndexOf("</entityKey>");
            string str_tax_return_xml_encrypted = str_manifest.Substring(pos1 + "<entityKey>".Length, pos2 - (pos1 + "<entityKey>".Length));

            byte[] tax_return_xml_encrypted = File.ReadAllBytes(str_tax_return_xml_encrypted);

            aes_csp.Mode = CipherMode.CBC;
            aes_csp.Padding = PaddingMode.PKCS7;
            aes_csp.KeySize = 0x80;
            aes_csp.BlockSize = 0x80;
            aes_csp.IV = Encoding.UTF8.GetBytes("!ASZ2WSX#EDC4RFV"); ;
            aes_csp.Key = Encoding.UTF8.GetBytes("4TGB@YHN7UJM(IK(");

            string str_tax_return = "";
            using (ICryptoTransform transform = aes_csp.CreateDecryptor())
            {
                byte[] buffer = transform.TransformFinalBlock(tax_return_xml_encrypted, 0, tax_return_xml_encrypted.Length);

                str_tax_return = Encoding.UTF8.GetString(buffer);
            }

            Console.WriteLine(str_tax_return);
            Console.WriteLine("Enjoy!");
        }
    }
}
