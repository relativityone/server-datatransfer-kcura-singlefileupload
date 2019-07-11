using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Entities
{
	public class Imaging
	{
		public bool Fdv { get; set; }
		public bool Image { get; set; }
		public bool NewImage { get; set; }
		public int DocID { get; set; }
		public int ProfileID { get; set; }
		public int MyProperty { get; set; }
		public int ErrorFile { get; set; }
	}
}
