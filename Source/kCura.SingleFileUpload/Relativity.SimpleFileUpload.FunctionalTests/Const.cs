using System;

namespace Relativity.SimpleFileUpload.FunctionalTests
{
	public static class Const
	{
		public const string FUNCTIONAL_TEMPLATE_NAME = "Functional Tests Template";
		public const string FUNCTIONAL_STANDARD_ACCOUNT_EMAIL_FORMAT = "sfu_func_user{0}@mail.com";

		public const string FUNCTIONAL_WORKSPACE_PREFIX = "FunctionalTests_";
		public const string UI_WORKSPACE_PREFIX = "UITests_";

		public class CustomPage
		{
			public static readonly Guid Guid = new Guid("1738CEB6-9546-44A7-8B9B-E64C88E47320"); 
		}

		public class App
		{
			public const string _NAME = "SimpleFileUpload";
			public const string _APP_GUID = "1738ceb6-9546-44a7-8b9b-e64c88e47320";
		}

		public class File
		{
			public const string _FILE_NAME = "CTRL0192153.xml";
			public const string _FILE_NAME_DOC = "SampleDOC.doc";
			public const string _FILE_NAME_TXT = "SampleTXT.txt";
			public const string _FILE_NAME_XLSX = "SampleXLSX.xlsx";
			public const string _FILE_NAME_PPTX = "SamplePPTX.pptx";
			public const string _FILE_NAME_PDF = "SamplePDF.pdf";
			public const string _FILE_NAME_PDF_INVALID_JS = "SamplePDF'.pdf";
			public const string _FILE_NAME_MSG = "SampleMSG.msg";
			public const string _FILE_NAME_EXE = "SampleEXE.exe";
			public const string _FILE_NAME_DLL = "SampleDLL.dll";
			public const string _FILE_NAME_JS = "SampleJS.js";
			public const string _FILE_NAME_HTM = "SampleHTM.htm";
			public const string _FILE_NAME_HTML = "SampleHTML.html";

			public const string _DOC_CONTROL_NUMBER = "CTRL0192153";

			public const string _FILE_NAME_PDF_XSS_JS = "';window.relativityXss=true;.pdf";
			public const string _FILE_NAME_PDF_XSS_HTML = "${constructor.constructor('window.relativityXSS=true')()};.pdf";
		}
	}
}
