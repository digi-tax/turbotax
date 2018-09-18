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
            string turbotax2017_data_container = @"Form_1040_Individual_Tax_Return.tax2017"; // move your file to "/bin/Debug/" folder

            ZipFile zf = new ZipFile(turbotax2017_data_container); // unzip the file with Ionic.Zip
            zf.CompressionMethod = CompressionMethod.Deflate;
            zf.CompressionLevel = Ionic.Zlib.CompressionLevel.BestSpeed;
            zf.Encryption = EncryptionAlgorithm.WinZipAes256;
            zf.ExtractAll(".", ExtractExistingFileAction.OverwriteSilently); // extract tax2017 to "/bin/Debug/"

            AesCryptoServiceProvider aes_csp = new AesCryptoServiceProvider();

            byte[] manifest_xmlfile = File.ReadAllBytes("manifest.xml");

            aes_csp.Mode = CipherMode.CBC;
            aes_csp.Padding = PaddingMode.PKCS7;
            aes_csp.KeySize = 0x80;
            aes_csp.BlockSize = 0x80;
            aes_csp.IV = Encoding.UTF8.GetBytes("!QAZ2WSX#EDC4RFV");
            aes_csp.Key = Encoding.UTF8.GetBytes("5TGB@YHN7UJM(IK(");

            string str_manifest = "";
            using (ICryptoTransform crypto_transf = aes_csp.CreateDecryptor())
            {
                byte[] manifest_xml_data = crypto_transf.TransformFinalBlock(manifest_xmlfile, 0, manifest_xmlfile.Length);

                str_manifest = Encoding.UTF8.GetString(manifest_xml_data);
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
            using (ICryptoTransform crypto_transf = aes_csp.CreateDecryptor())
            {
                byte[] tax_return_2017 = crypto_transf.TransformFinalBlock(tax_return_xml_encrypted, 0, tax_return_xml_encrypted.Length);

                str_tax_return = Encoding.UTF8.GetString(tax_return_2017);
            }

            Console.WriteLine(str_tax_return);
            Console.WriteLine("Intuit's genius point of view on data security. 10 out of 10.");
        }
    }
}
