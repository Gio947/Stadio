﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFStadio
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost service = new ServiceHost(typeof(Service1));

            service.Open();
            Console.WriteLine("START WCF");
            Console.ReadLine();

            service.Close();
            Console.WriteLine("END WCF");
        }
    }
}
