using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace waitforafile
{
    class Program
    {
        public static bool abortflag = false;
        static int Main(string[] args)
        {
            string filemask = "*";
            Console.WriteLine(Environment.Version.ToString());

            if (args.Length == 0)
            {
                Console.Error.WriteLine("waitforafile.exe  <directory> [filemask]");
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
            System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(Environment.CurrentDirectory);
            try
            {
                d = new System.IO.DirectoryInfo(args[0]);
            }catch(Exception ex)
            {
                Console.Error.WriteLine("Unexpected directory error!");
                Console.Error.WriteLine(ex.Message);
                return 3;
            }
            try
            {
                existingfiles = d.GetFiles(filemask).Length;
            }catch(Exception ex)
            {
                Console.Error.WriteLine("Unexpected filemask error!");
                Console.WriteLine(ex.Message);
                return 4;
            }
            int newfiles = existingfiles;
            int i = 0;
            Console.CancelKeyPress += Console_CancelKeyPress;
            // The below commented out code, will abort waiting when ANY input is sent to this application.   This includes piped input.
            //  This may not be ideal, so now it will only abort on ctrl+c
            //System.IO.Stream consoleinput = Console.OpenStandardInput();
            //byte[] buff = new byte[256];
            //buff = Enumerable.Repeat((byte)0, 256).ToArray();
            //Task<int> t = consoleinput.ReadAsync(buff, 0, 256);
            while (newfiles == existingfiles)
            {
                System.Threading.Thread.Sleep(150);
                /*if (t.Status == TaskStatus.RanToCompletion)
                {
                    if (t.Result >= 0)
                    {
                        if(buff.Contains((byte)3))
                        {
                            // Ctrl+C?? 
                        }
                        Console.WriteLine("Key pressed, file awaiting aborted.");
                        break;
                    }
                    else
                    {
                        throw new Exception("Unexpected task status : Ran to completion, but read no bytes");
                    }
                }*/
                if (abortflag)
                    return 0;
                d.Refresh();
                newfiles = d.GetFiles(filemask).Length;
                i++;
                if (i >= 50)
                {
                    System.GC.Collect(0);   //probably not needed
                    i = 0;
                }
            }
            

            return 0;
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Key pressed, file awaiting aborted.");
        }
    }
}
