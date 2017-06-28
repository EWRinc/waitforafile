using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace waitforafile
{
    class Program
    {
        static int Main(string[] args)
        {
            string filemask = "*";
            if (args.Length == 0)
            {
                Console.Error.WriteLine("No arguments!  <directory> <filemask>");
                return 1;
            }
            if (!System.IO.Directory.Exists(args[0]))
            {
                Console.Error.WriteLine("Directory does not exit!");
                return 2;
            }
            if (args.Length > 1)
                filemask = args[1];
            int existingfiles = 0;
            System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(args[0]);

            existingfiles = d.GetFiles(filemask).Length;
            int newfiles = existingfiles;
            int i = 0;
            System.IO.Stream consoleinput = Console.OpenStandardInput();
            byte[] buff = new byte[256];
            Task<int> t = consoleinput.ReadAsync(buff, 0, 256);
            while (newfiles == existingfiles)
            {
                System.Threading.Thread.Sleep(150);
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    if (t.Result >= 0)
                    {
                        Console.WriteLine("Key pressed, file awaiting aborted.");
                        break;
                    }
                    else
                    {
                        throw new Exception("Unexpected task status : Ran to completion, but read no bytes");
                    }
                }
                d.Refresh();
                newfiles = d.GetFiles(filemask).Length;
                i++;
                if (i >= 21)
                {
                    System.GC.Collect(0);   //probably not needed
                    i = 0;
                }
            }
            

            return 0;
        }
    }
}
