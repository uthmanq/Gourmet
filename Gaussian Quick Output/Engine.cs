using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Document;
using Xceed.Words;

namespace Gaussian_Quick_Output
{
    class Engine
    {
        public Engine ()
        {
            
        }
        private static string ProcessText (string filepath)
        {
            string text = "";
            try
            {
                
                //Word Document processing
                if (filepath.EndsWith(".docx"))
                {
                    using (var document = Xceed.Words.NET.DocX.Load(filepath))
                    {
                        text = document.Text;

                    }
                }
                //Everything else
                else
                {
                    text = System.IO.File.ReadAllText(filepath);
                }
            }
            catch (Exception ex)
            {
                
            }
            return text;
        }


        public static string Read(CustomFunction c, string filepath)
        {
            string file = ProcessText(filepath);
            return c.ReadFunction(file);
        }
        public static void Execute (CustomFunction c, string filepath)
        {      
            c.DoFunction(filepath);
        }
        //Future function for graphing and parsing data
        public static void Graph (CustomFunction c, string filepath)
        {

        }

        
    }
}
