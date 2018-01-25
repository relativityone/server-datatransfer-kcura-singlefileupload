extern alias outsidein;
using kCura.SingleFileUpload.Core.Entities;
using OutsideIn;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
    public class SearchExportManager : BaseManager, ISearchExportManager
    {
        public ExportedMetadata ExportToSearchML(string fileName, byte[] sourceFile)
        {
            ExportedMetadata result = new Entities.ExportedMetadata();
            result.FileName = fileName;
            using (outsidein::OutsideIn.Exporter exporter = outsidein::OutsideIn.OutsideIn.NewLocalExporter())
            {
                using (MemoryStream msMLS = new MemoryStream(sourceFile))
                {
                    using (MemoryStream msML = new MemoryStream())
                    {
                        exporter.SetPerformExtendedFI(true);
                        int timeZoneOffset = exporter.GetTimeZoneOffset();
                        exporter.SetSourceFile(msMLS);
                        exporter.SetDestinationFile(msML);
                        exporter.SetDestinationFormat(outsidein::OutsideIn.FileFormat.FI_SEARCHML_LATEST);
                        exporter.Export();
                        ProcessSearchMLString(msML.ToArray(), result);
                    }
                }
            }
            result.Native = sourceFile;
            return result;
        }

        public ExportedMetadata ProcessSearchMLString(byte[] searchML, ExportedMetadata result = null)
        {
            result = result ?? new ExportedMetadata();
            StringBuilder extractedTextBuilder = new StringBuilder();
            using (MemoryStream ms = new MemoryStream(searchML))
            using (XmlReader reader = XmlReader.Create(ms))
            {
                while (reader.Read())
                {
                    processReader(reader, extractedTextBuilder, result.Fields);
                }
            }
            result.ExtractedText = extractedTextBuilder.ToString();

            return result;
        }

        string fieldName;



        private void processReader(XmlReader reader, StringBuilder etBuilder, Dictionary<string, object> fields)
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    switch (reader.Name)
                    {
                        case "document":
                            addToDictionary(fields, "Native Type", reader.GetAttribute("type"));
                            break;
                        default:
                            if (reader.HasAttributes)
                            {
                                fieldName = reader.GetAttribute("type");
                            }
                            break;
                        
                    }
                    if (fieldName == "hyperlink" || fieldName == "body" || fieldName == "bookmark")
                    {
                        fieldName = string.Empty;
                    }
                    break;
                case XmlNodeType.Text:
                    if (string.IsNullOrEmpty(fieldName))
                    {
                        etBuilder.AppendLine(reader.Value);
                    }
                    else
                    {
                        object value = null;
                        switch (fieldName)
                        {
                            case "creation date":
                            case "revision date":
                            case "Last Saved Date":
                            case "DTTM Created":
                            case "DTTM Revised":
                                DateTime endValue = DateTime.MinValue;
                                if (!DateTime.TryParseExact(reader.Value, "M/d/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out endValue))
                                {
                                    string preValue = reader.Value.Substring(2, reader.Value.Length - 9);
                                    if (!DateTime.TryParseExact(preValue, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out endValue))
                                    {
                                        DateTime.TryParse(reader.Value, out endValue);
                                    }
                                }
                                if (endValue > DateTime.MinValue)
                                {
                                    value = endValue;
                                }
                                break;
                            default:
                                value = reader.Value;
                                break;
                        }
                        addToDictionary(fields, fieldName, value);
                        fieldName = null;
                    }
                    break;
                default:
                    break;
            }
        }
        private void addToDictionary(IDictionary<string, object> dictionary, string key, object value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
    }
}
