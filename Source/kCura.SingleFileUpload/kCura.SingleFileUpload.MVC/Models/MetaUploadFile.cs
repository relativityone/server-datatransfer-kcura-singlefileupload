﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kCura.SingleFileUpload.MVC.Models
{
	public class MetaUploadFile
	{
		public int fid { get; set; }
		public int did { get; set; }
		public bool fdv { get; set; }
		public bool force { get; set; }
	}
}