using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;

namespace FinalAssignment.DataAccess.Providers
{
    public static class FileProvider
    {
        public static string ReadFile(string fileLocation)
        {
            string result = null;

            try
            {
                if (System.IO.File.Exists(fileLocation) == true)
                {
                    string line;

                    // Read the file and display it line by line.  
                    System.IO.StreamReader file = new System.IO.StreamReader(fileLocation);
                    while ((line = file.ReadLine()) != null)
                    {
                        var tempString = line + "\n";
                        result += tempString;
                    }

                    file.Close();

                    return result;
                }
                else
                {
                    //file doesn't exist
                    return null;
                }
            }
            catch (IOException e)
            {
                var message = e.Message;
                return null;
            }
        }
    }
}
