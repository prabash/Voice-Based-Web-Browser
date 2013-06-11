using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace UWIC.FinalProject.Common
{
    public class Log
    {
        public static void ErrorLog(Exception ex)
        {
            string errorMessage = string.Empty;
            errorMessage = ex.Source + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
            CreateFile("ErrorLog", errorMessage);
        }

        private static char HexChar(int value)
        {
            value &= 0xF;
            if (value >= 0 && value <= 9) return (char)('0' + value);
            else return (char)('A' + (value - 10));
        }

        public static void MessageLog(string logMessage)
        {
            CreateFile("MessageLog", logMessage);
        }

        public static void ControlLog(string logMessage)
        {
            CreateFile("ControlLog", logMessage);
        }

        public static void InformationLog(string logMessage)
        {
            CreateFile("InformationLog", logMessage);
        }

        public static void CreateLog(string fileName, string logMessage)
        {
            CreateFile(fileName, logMessage);
        }

        public static void MessageLog(object obj)
        {
            string logMessage = SerializeToXML(obj);
            CreateFile("MessageLog", logMessage);
        }

        private static void CreateFile(string fileName, string logMessage)
        {
            string applicationPath = string.Empty;
            string filePath = string.Empty;
            string directoryPath = "\\Logs\\" + DateTime.Today.Date.ToString("dd-MM-yyyy") + "\\";

            try
            {
                //Get Application path
                applicationPath = AppDomain.CurrentDomain.BaseDirectory;
                filePath = applicationPath + directoryPath + fileName + " - " + DateTime.Today.Date.ToString("MMM-dd-yyyy") + ".txt";

                if (!File.Exists(filePath))
                {
                    if (!Directory.Exists(applicationPath + directoryPath))
                    {
                        //Create Directory
                        Directory.CreateDirectory(applicationPath + directoryPath);
                    }
                    //Create Log File
                    using (StreamWriter swLog = File.CreateText(filePath))
                    {
                        swLog.WriteLine("Created on - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                        swLog.WriteLine("==================================");
                        swLog.WriteLine();
                    }
                }

                //Following text is added to the file untill the data is changed
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff ") + logMessage);
                    sw.WriteLine();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string SerializeToXML(Object obj)
        {

            MemoryStream MemStream = null;

            try
            {
                XmlDocument XmlDoc = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                MemStream = new MemoryStream();

                serializer.Serialize(MemStream, obj);
                MemStream.Position = 0;
                XmlDoc.Load(MemStream);

                return FormatXml(XmlDoc);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                MemStream.Close();
            }
        }

        private static string FormatXml(XmlDocument doc)
        {
            var sb = new StringBuilder();
            var settings =
                new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = @"    ",
                    NewLineChars = Environment.NewLine,
                    NewLineHandling = NewLineHandling.Replace,
                };

            using (var writer = XmlWriter.Create(sb, settings))
            {
                if (doc.ChildNodes[0] is XmlProcessingInstruction)
                {
                    doc.RemoveChild(doc.ChildNodes[0]);
                }

                doc.Save(writer);
                return sb.ToString();
            }
        }
    }
}
