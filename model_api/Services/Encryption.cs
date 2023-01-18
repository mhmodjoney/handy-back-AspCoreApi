using Microsoft.Extensions.Options;
using model_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace model_api.Services
{
  
    public class Encryption
    {
   
        public static void EncreptpassMD5(Customer customer)
        {
            var df = MD5.Create();
            byte[] data = df.ComputeHash(Encoding.UTF8.GetBytes(customer.Password));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            customer.Password = sBuilder.ToString();
        }
        public static void EncreptpassMD5(Admin admin)
        {
            var df = MD5.Create();
            byte[] data = df.ComputeHash(Encoding.UTF8.GetBytes(admin.Password));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            admin.Password = sBuilder.ToString();
        }

        public static string generateID()
        {
            return Guid.NewGuid().ToString("N");
        }

    }
}
