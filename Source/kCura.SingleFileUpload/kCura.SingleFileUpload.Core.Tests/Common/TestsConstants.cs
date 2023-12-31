﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using kCura.SingleFileUpload.Core.Entities;
using kCura.SingleFileUpload.Core.Tests.Helpers;
using Relativity.Services;
using Relativity.Services.InstanceSetting;

namespace kCura.SingleFileUpload.Core.Tests.Constants
{
	public static class TestsConstants
	{
		public const int _WORKSPACE_ID = 10000;
		public const int _DOC_ARTIFACT_ID = 100000;
		public const int _DOC_FILE_ID = 10;
		public static readonly Guid _DOC_GUID = Guid.NewGuid();
		public const string _FILE_TYPE = ".7z";
		public const string _OI_FILE_TYPE_ = "Extensible Markup Language (XML)";
		public const string _FILE_NAME = "CTRL0192153.xml";

		public const string _WEB_API_URL = "https://test";
		public static readonly string _FILE_LOCATION = FileHelper.GetFileLocation(_FILE_NAME);
		public static readonly string _TEMP_FOLDER_LOCATION = FileHelper.GetTempFolderLocation();
		public const int _USER_ID = 777;
		public const string _DOC_CONTROL_NUMBER = "CTRL0192153";
		public const string _EXTRACTED_TEXT = "John\r\nDoe\r\n";

		public static readonly ExportedMetadata _EXP_METADATA = new ExportedMetadata
		{
			Native = File.ReadAllBytes(_FILE_LOCATION),
			FileName = "CTRL0192154.xml",
		};

		public static readonly DocumentExtraInfo _DOC_EXTRA_INFO = new DocumentExtraInfo
		{
			AvoidControlNumber = false,
			FromDocumentViewer = false,
			DocID = _DOC_ARTIFACT_ID,
			FolderID = _DOC_FILE_ID,
			WebApiUrl = _WEB_API_URL,
		};

        public static readonly InstanceSettingQueryResultSet _INSTANCE_SETTING_RESULT_SET = new InstanceSettingQueryResultSet
        {
            Success = true,
            Results = new List<Result<InstanceSetting>>
            {
                new Result<InstanceSetting>
                {
                    Success = true,
                    Artifact = new InstanceSetting
                    {
                        Value = "5",
                    }
                }
            }
        };

		public static readonly string _JSON_RESULT = @"{""fileExtension"":{ ""value"":""File %Extension"",""default"":""File Extension""},
						""fileName"":{ ""value"":""File % Name"",""default"":""File Name""},
						""fileSize"":{ ""value"":""File % Size"",""default"":""File Size""}}";
		
		public static DataTable _GetdataTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("FileID", typeof(int));
			dt.Columns.Add("DocumentArtifactID", typeof(int));
			dt.Columns.Add("FileName", typeof(string));
			dt.Columns.Add("Location", typeof(string));
			dt.Rows.Add(_DOC_FILE_ID, _DOC_ARTIFACT_ID, _FILE_NAME, _FILE_LOCATION);

			return dt;
		}
	}
}
