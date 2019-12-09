using kCura.SingleFileUpload.Core.Entities;
using kCura.SingleFileUpload.Core.Helpers;
using System;
using System.IO;
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
		private bool checkToIgnoreValue { get; set; }
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
					if (reader.Name == "doc_content")
					{
						XmlReader docChild = reader.ReadSubtree();

						while (docChild.Read())
						{
							if (docChild.Name == "target" && docChild.NodeType == XmlNodeType.Element)
							{
								checkToIgnoreValue = true;
							}
							string displayName = docChild.GetAttribute("display_name");
							if (docChild.NodeType == XmlNodeType.Element && !string.IsNullOrEmpty(displayName))
							{
								XmlReader child = docChild.ReadSubtree();

								while (child.Read())
								{
									if (child.NodeType == XmlNodeType.Text)
									{
										extractedTextBuilder.AppendLine($" {displayName} {child.Value}");
									}
								}
							}
							else
							{
								if (docChild.NodeType == XmlNodeType.Text)
								{
									if (checkToIgnoreValue)
									{
										checkToIgnoreValue = false;
									}
									else
									{
										extractedTextBuilder.AppendLine(docChild.Value);
									}
								}
							}
						}

					}
				}
			}
			result.ExtractedText = extractedTextBuilder.ToString();

			return result;
		}
		public void ConfigureOutsideIn()
		{
			string directoryPath, filePath;

			string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            directoryPath = Path.Combine(currentPath, "bin", "oi", "unmanaged");
            if (currentPath.Contains("Tests"))
            {
                directoryPath = Path.Combine(currentPath, "oi", "unmanaged");
            }
            filePath = Path.Combine(directoryPath, "oilink.exe");
            if (!File.Exists(filePath))
			{
				using (var outStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
				{
					outStream.Write(DeployableFiles.oilink, 0, DeployableFiles.oilink.Length);
				}
			}
            if (OutsideIn.OutsideIn.InstallLocation == null)
            {
                try
                {
                    OutsideIn.OutsideIn.InstallLocation = new DirectoryInfo(directoryPath);
                }
                catch (Exception ex)
                {
                    DocumentManager.Instance.LogError(ex, false);
                }
            }
		}
	}
}
