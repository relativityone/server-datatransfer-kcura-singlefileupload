using kCura.SingleFileUpload.Core.Entities;
using kCura.SingleFileUpload.Core.Helpers;
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
        private static readonly Lazy<ISearchExportManager> instance = new Lazy<ISearchExportManager>(() => new SearchExportManager());
        public static ISearchExportManager Instance
        {
            get
            {
                return instance.Value;
            }
        }

		private string FieldName { get; set; }
		private bool CheckToRemove { get; set; }
		public ExportedMetadata ExportToSearchML(string fileName, byte[] sourceFile)
        {
			ExportedMetadata result = new Entities.ExportedMetadata
			{
				FileName = fileName
			};
			using (OutsideIn.Exporter exporter = OutsideIn.OutsideIn.NewLocalExporter())
            {
                using (MemoryStream msMLS = new MemoryStream(sourceFile))
                {
                    using (MemoryStream msML = new MemoryStream())
                    {

                        exporter.SetPerformExtendedFI(true);
                        int timeZoneOffset = exporter.GetTimeZoneOffset();
                        exporter.SetSourceFile(msMLS);
                        exporter.SetDestinationFile(msML);
                        exporter.SetDestinationFormat(OutsideIn.FileFormat.FI_SEARCHML_LATEST);
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
                    ProcessReader(reader, extractedTextBuilder, result.Fields);
                }
            }
            result.ExtractedText = extractedTextBuilder.ToString();

            return result;
        }

        public void ConfigureOutsideIn()
        {
            string directoryPath, filePath;

            directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "oi", "unmanaged");
            filePath = Path.Combine(directoryPath, "oilink.exe");

            if (!File.Exists(filePath))
            {
                using (var outStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    outStream.Write(DeployableFiles.oilink, 0, DeployableFiles.oilink.Length);
                }
            }
            try
            {
                OutsideIn.OutsideIn.InstallLocation = new DirectoryInfo(directoryPath);
            }
            catch
            {
            }
        }
        

        private void ProcessReader(XmlReader reader, StringBuilder etBuilder, Dictionary<string, object> fields)
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    switch (reader.Name)
                    {
                        case "document":
                            AddToDictionary(fields, "Native Type", reader.GetAttribute("type"));
                            break;
						case "target":
							CheckToRemove = true;
							break;
						default:
                            if (reader.HasAttributes)
                            {
                                FieldName = reader.GetAttribute("type");
                            }
                            break;

                    }
                    if (FieldName == "hyperlink" || FieldName == "body" || FieldName == "bookmark")
                    {
                        FieldName = string.Empty;
                    }
                    break;
                case XmlNodeType.Text:
                    if (string.IsNullOrEmpty(FieldName))
                    {
						if (CheckToRemove)
						{
							CheckToRemove = false;
						}
						else
						{
							etBuilder.AppendLine(reader.Value);
						}
					}
                    else
                    {
                        object value = null;
                        switch (FieldName)
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
                        AddToDictionary(fields, FieldName, value);
                        FieldName = null;
                    }
                    break;
                default:
                    break;
            }
        }
        private void AddToDictionary(IDictionary<string, object> dictionary, string key, object value)
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
