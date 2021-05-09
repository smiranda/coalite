using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace coahoarder
{

  // Example
  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length > 0 && (args[0] == "-h" || args[0] == "--help" || args[0] == "help" || args[0] == "-v" || args[0] == "--version" || args[0] == "version"))
      {
        var versionString = Assembly.GetEntryAssembly()
                                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                    .InformationalVersion
                                    .ToString();
        Console.WriteLine($"coakeygen v{versionString}");
        Console.WriteLine("-------------");
        Console.WriteLine("\nUsage:");
        Console.WriteLine("  coakeygen [key_length=4096] [priv_format=pkcs1|pkcs8] [pub_format=pkcs1|X.509] [key_name=key]");
        return;
      }
      var keyLength = args.Length > 0 ? Convert.ToInt32(args[0]) : 4096;
      var privFormat = args.Length > 1 ? args[1] : "pkcs1";
      var pubFormat = args.Length > 2 ? args[2] : "pkcs1";
      var keyName = args.Length > 3 ? args[3] : "key";

      RSA rsa = RSA.Create(keyLength);
      string privateKeyData = "";
      string publicKeyData = "";
      if (privFormat == "pkcs1")
      {
        privateKeyData = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
      }
      else if (privFormat == "pkcs8")
      {
        privateKeyData = Convert.ToBase64String(rsa.ExportPkcs8PrivateKey());
      }
      else
      {
        throw new NotImplementedException($"Format not supported: {privFormat}");
      }

      if (pubFormat == "pkcs1")
      {
        publicKeyData = Convert.ToBase64String(rsa.ExportRSAPublicKey());
      }
      else if (pubFormat == "X.509")
      {
        publicKeyData = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo());
      }
      else
      {
        throw new NotImplementedException($"Format not supported: {pubFormat}");
      }
      var privKeyName = $"{keyName}.{privFormat}.prv";
      var pubKeyName = $"{keyName}.{pubFormat}.pub";
      File.WriteAllText($"{privKeyName}", privateKeyData);
      File.WriteAllText($"{pubKeyName}", publicKeyData);
    }
  }
}
