using kCura.SingleFileUpload.Core.Entities;
using kCura.SingleFileUpload.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
	public class SearchExportManager : BaseManager, ISearchExportManager
	{
		private const int _START_INDEX = 2;
		private const int _SUBSTRING_LENGTH = 9;
		private static readonly Lazy<ISearchExportManager> _INSTANCE = new Lazy<ISearchExportManager>(() => new SearchExportManager());
		public static ISearchExportManager instance => _INSTANCE.Value;

		private string fieldName { get; set; }
		private string[] AdditionalFields
		{
			get
			{
				return new string[]
				{
					"mail_organizer", "mail_to", "mail_from", "mail_attendees",
					"mail_subject", "mail_conversation_topic", "mail_normalized_subject",
					"mail_appointment_duration", "mail_reqattendee","mail_location","mail_dtstart", "mail_dtend","mail_importance",
					"mail_client_submit_time"
				};
			}
		}
		private bool checkToRemove { get; set; }
		public ExportedMetadata ExportToSearchML(string fileName, byte[] sourceFile, OutsideIn.Exporter oIExporter)
		{
			ExportedMetadata result = new Entities.ExportedMetadata();
			result.FileName = fileName;
			using (oIExporter)
			{
				using (MemoryStream msMLS = new MemoryStream(sourceFile))
				{
					using (MemoryStream msML = new MemoryStream())
					{

						oIExporter.SetPerformExtendedFI(true);
						int timeZoneOffset = oIExporter.GetTimeZoneOffset();
						oIExporter.SetSourceFile(msMLS);
						oIExporter.SetDestinationFile(msML);
						oIExporter.SetDestinationFormat(OutsideIn.FileFormat.FI_SEARCHML_LATEST);
						oIExporter.Export();
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

			string currentPath = AppDomain.CurrentDomain.BaseDirectory;
			if (currentPath.Contains("Tests"))
			{
				directoryPath = Path.Combine(currentPath, "oi", "unmanaged");
			}
			else
			{
				directoryPath = Path.Combine(currentPath, "bin", "oi", "unmanaged");
			}

			filePath = Path.Combine(directoryPath, "oilink.exe");

			if (!File.Exists(filePath))
			{
				using (var outStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
				{
					outStream.Write(DeployableFiles.oilink, 0, DeployableFiles.oilink.Length);
				}
			}
			OutsideIn.OutsideIn.InstallLocation = new DirectoryInfo(directoryPath);
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
							checkToRemove = true;
							break;
						default:
							if (reader.HasAttributes)
							{
								fieldName = reader.GetAttribute("type");
							}
							break;
					}
					if (fieldName == "hyperlink" || fieldName == "body" || fieldName == "bookmark"
						|| AdditionalFields.Contains(fieldName?.ToLower() ?? string.Empty))
					{
						fieldName = string.Empty;
					}
					break;
				case XmlNodeType.Text:
					if (string.IsNullOrEmpty(fieldName))
					{
						if (checkToRemove)
						{
							checkToRemove = false;
						}
						else
						{
							etBuilder.AppendLine(reader.Value);
						}
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
									string preValue = reader.Value.Substring(_START_INDEX, reader.Value.Length - _SUBSTRING_LENGTH);
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
						AddToDictionary(fields, fieldName, value);
						fieldName = null;
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
