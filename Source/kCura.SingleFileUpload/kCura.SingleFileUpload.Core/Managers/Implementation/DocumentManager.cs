using kCura.SingleFileUpload.Core.Entities;
using System;
using System.IO;
using System.Linq;
using Client = kCura.Relativity.Client;
using kCura.SingleFileUpload.Core.SQL;
using System.Data;
using Services = Relativity.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Relativity.Services.ObjectQuery;
using kCura.Relativity.ImportAPI;
using kCura.Relativity.DataReaderClient;
using Relativity.API;
using NSerio.Relativity;
using DTOs = kCura.Relativity.Client.DTOs;
using Newtonsoft.Json.Linq;
using System.Text;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
    public class DocumentManager : BaseManager, IDocumentManager
    {
        private FileType[] _viewerSupportedFileType;
        public FileType[] ViewerSupportedFileTypes
        {
            get
            {

                if (_viewerSupportedFileType == null)
                {
                    _viewerSupportedFileType = new FileType[] {
                        new FileType() { TypeName = "7 Zip",TypeExtension= ".7z"},
                        new FileType() { TypeName = "AutoCAD Drawing",TypeExtension= ".dwg"},
                        new FileType() { TypeName = "Corel Presentations",TypeExtension= ".shw"},
                        new FileType() { TypeName = "Flexiondoc (XML)",TypeExtension= ".xml"},
                        new FileType() { TypeName = "Harvard Graphics",TypeExtension= ".cht"},
                        new FileType() { TypeName = "Microsoft Access",TypeExtension= ".accdb"},
                        new FileType() { TypeName = "Microsoft OneNote File",TypeExtension= ".one"},
                        new FileType() { TypeName = "PKZip",TypeExtension= ".zip"},
                        new FileType() { TypeName = "Quattro Pro Win",TypeExtension= ".qpw"},
                        new FileType() { TypeName = "7z Self Extracting exe",TypeExtension= ".exe"},
                        new FileType() { TypeName = "LZA Self Extracting Compress",TypeExtension= ".lza"},
                        new FileType() { TypeName = "LZH Compress",TypeExtension= ".lzh"},
                        new FileType() { TypeName = "Microsoft Office Binder",TypeExtension= ".obd"},
                        new FileType() { TypeName = "Microsoft Cabinet (CAB)",TypeExtension= ".cab"},
                        new FileType() { TypeName = "RAR",TypeExtension= ".rar"},
                        new FileType() { TypeName = "Self-extracting .exe",TypeExtension= ".exe"},
                        new FileType() { TypeName = "UNIX Compress",TypeExtension= ".z"},
                        new FileType() { TypeName = "UNIX GZip",TypeExtension= ".gz"},
                        new FileType() { TypeName = "UNIX tar",TypeExtension= ".tar"},
                        new FileType() { TypeName = "Uuencode",TypeExtension= ".uue"},
                        new FileType() { TypeName = "Zip",TypeExtension= ".zip"},
                        new FileType() { TypeName = "DataEase",TypeExtension= ".dba"},
                        new FileType() { TypeName = "DBase",TypeExtension= ".dbf"},
                        new FileType() { TypeName = "First Choice DB",TypeExtension= ".fol"},
                        new FileType() { TypeName = "Microsoft Access (text only)",TypeExtension= ".accdb"},
                        new FileType() { TypeName = "Microsoft Access (text only)",TypeExtension= ".mdb"},
                        new FileType() { TypeName = "Microsoft Works DB for DOS",TypeExtension= ".wdb"},
                        new FileType() { TypeName = "Microsoft Works DB for Macintosh",TypeExtension= ".wdb"},
                        new FileType() { TypeName = "Microsoft Works DB for Windows",TypeExtension= ".wdb"},
                        new FileType() { TypeName = "Microsoft Works DB for DOS",TypeExtension= ".wdb"},
                        new FileType() { TypeName = "Paradox for DOS",TypeExtension= ".db"},
                        new FileType() { TypeName = "Paradox for Windows",TypeExtension= ".db"},
                        new FileType() { TypeName = "Q&A Database",TypeExtension= ".db"},
                        new FileType() { TypeName = "R:Base",TypeExtension= ".rb1"},
                        new FileType() { TypeName = "R:Base",TypeExtension= ".rb2,"},
                        new FileType() { TypeName = "R:Base",TypeExtension= ".rb3"},
                        new FileType() { TypeName = "Reflex",TypeExtension= ".rdx"},
                        new FileType() { TypeName = "SmartWare II DB",TypeExtension= ".db"},
                        new FileType() { TypeName = "Apple Mail Message (EMLX)",TypeExtension= ".emlx"},
                        new FileType() { TypeName = "EML with Digital Signature",TypeExtension= ".eml"},
                        new FileType() { TypeName = "IBM Lotus Notes Domino XML Language DXL",TypeExtension= ".xml"},
                        new FileType() { TypeName = "IBM Lotus Notes NSF (Win32, Win64, Linux x86-32 and Oracle Solaris 32-bit only with Notes Client or Domino Server)",TypeExtension= ".nsf."},
                        new FileType() { TypeName = "IBM Lotus Notes NSF (Win32, Win64, Linux x86-32 and Oracle Solaris 32-bit only with Notes Client or Domino Server)",TypeExtension= ".ntf"},
                        new FileType() { TypeName = "MBOX Mailbox",TypeExtension= ".mbox"},
                        new FileType() { TypeName = "Microsoft Outlook (MSG)",TypeExtension= ".msg"},
                        new FileType() { TypeName = "Microsoft Outlook (OST )",TypeExtension= ".ost"},
                        new FileType() { TypeName = "Microsoft Outlook (PST)",TypeExtension= ".pst"},
                        new FileType() { TypeName = "Microsoft Outlook Express (EML)",TypeExtension= ".eml"},
                        new FileType() { TypeName = "Microsoft Outlook Forms Template (OFT)",TypeExtension= ".oft"},
                        new FileType() { TypeName = "Microsoft Outlook PST (Mac)",TypeExtension= ".pst"},
                        new FileType() { TypeName = "MSG with Digital Signature",TypeExtension= ".msg"},
                        new FileType() { TypeName = "AVI (Metadata only)",TypeExtension= ".avi"},
                        new FileType() { TypeName = "Flash (text extraction only)",TypeExtension= ".swf"},
                        new FileType() { TypeName = "MP3 (ID3 metadata only)",TypeExtension= ".mp3"},
                        new FileType() { TypeName = "MPEG – 1 Audio layer 3 V ID3 v1 (Metadata only)",TypeExtension= ".mpg"},
                        new FileType() { TypeName = "MPEG – 1 Audio layer 3 V ID3 v2 (Metadata only)",TypeExtension= ".mpg"},
                        new FileType() { TypeName = "MPEG – 4 (Metadata only)",TypeExtension= ".mpg"},
                        new FileType() { TypeName = "MPEG – 7 (Metadata only)",TypeExtension= ".mpg"},
                        new FileType() { TypeName = "Quick Time (Metadata only)",TypeExtension= ".mpg"},
                        new FileType() { TypeName = "WAV (Metadata only)",TypeExtension= ".wav"},
                        new FileType() { TypeName = "Windows Media ASF (Metadata only)",TypeExtension= ".asf"},
                        new FileType() { TypeName = "Windows Media Audio WMA (Metadata only)",TypeExtension= ".wma"},
                        new FileType() { TypeName = "Windows Media DVR-MS (Metadata only)",TypeExtension= ".dvr-ms"},
                        new FileType() { TypeName = "Windows Media Video WMV (Metadata only)",TypeExtension= ".wmv"},
                        new FileType() { TypeName = "Microsoft OneNote (text only)",TypeExtension= ".one"},
                        new FileType() { TypeName = "Microsoft Project (sheet view only, Gantt Chart, Network Diagram, and graph not supported)",TypeExtension= ".mpp"},
                        new FileType() { TypeName = "Microsoft Windows DLL",TypeExtension= ".dll"},
                        new FileType() { TypeName = "Microsoft Windows Executable",TypeExtension= ".dll"},
                        new FileType() { TypeName = "Trillian Text Log File (via text filter)",TypeExtension= ".txt"},
                        new FileType() { TypeName = "vCalendar",TypeExtension= ".vcs"},
                        new FileType() { TypeName = "vCard",TypeExtension= ".vcf"},
                        new FileType() { TypeName = "Yahoo! Messenger",TypeExtension= ".yps"},
                        new FileType() { TypeName = "Apple iWork Keynote (MacOS, text and PDF preview)",TypeExtension= ".key"},
                        new FileType() { TypeName = "Apple iWork Keynote (MacOS, text and PDF preview)",TypeExtension= ".keynote"},
                        new FileType() { TypeName = "Harvard Graphics Presentation DOS",TypeExtension= ".prs"},
                        new FileType() { TypeName = "Lotus Freelance",TypeExtension= ".prz"},
                        new FileType() { TypeName = "Microsoft PowerPoint for Macintosh",TypeExtension= ".ppt"},
                        new FileType() { TypeName = "Microsoft PowerPoint for Windows",TypeExtension= ".ppt"},
                        new FileType() { TypeName = "Microsoft PowerPoint for Windows Slideshow",TypeExtension= ".ppt"},
                        //new FileType() { TypeName = "Microsoft PowerPoint 2007/2008",TypeExtension= ".pptx"},
                        new FileType() { TypeName = "Microsoft PowerPoint for Windows Template",TypeExtension= ".pot"},
                        new FileType() { TypeName = "Novell Presentations",TypeExtension= ".shw"},
                        new FileType() { TypeName = "OpenOffice Impress",TypeExtension= ".sdd"},
                        new FileType() { TypeName = "Oracle Open Office Impress",TypeExtension= ".odp"},
                        new FileType() { TypeName = "StarOffice Impress",TypeExtension= ".sda"},
                        new FileType() { TypeName = "StarOffice Impress",TypeExtension= ".sdd"},
                        new FileType() { TypeName = "Adobe Photoshop",TypeExtension= ".psd"},
                        new FileType() { TypeName = "CALS Raster (GP4)",TypeExtension= ".cg4"},
                        new FileType() { TypeName = "CALS Raster (GP4)",TypeExtension= ".cal"},
                        new FileType() { TypeName = "Computer Graphics Metafile",TypeExtension= ".cgm"},
                        new FileType() { TypeName = "Encapsulated PostScript (EPS)",TypeExtension= ".eps"},
                        new FileType() { TypeName = "GEM Image (Bitmap)",TypeExtension= ".bmp"},
                        new FileType() { TypeName = "Graphics Interchange Format (GIF)",TypeExtension= ".gif"},
                        new FileType() { TypeName = "IBM Graphics Data Format (GDF)",TypeExtension= ".gdf"},
                        new FileType() { TypeName = "IBM Picture Interchange Format",TypeExtension= ".pif"},
                        new FileType() { TypeName = "JFIF (JPEG not in TIFF format)",TypeExtension= ".jfif"},
                        new FileType() { TypeName = "JPEG",TypeExtension= ".jpg"},
                        new FileType() { TypeName = "Kodak Flash Pix",TypeExtension= ".fpx"},
                        new FileType() { TypeName = "Kodak Photo CD",TypeExtension= ".pcd"},
                        new FileType() { TypeName = "Lotus PIC",TypeExtension= ".pic"},
                        new FileType() { TypeName = "Macintosh PICT",TypeExtension= ".bmp"},
                        new FileType() { TypeName = "Macintosh PICT2",TypeExtension= ".bmp"},
                        new FileType() { TypeName = "MacPaint",TypeExtension= ".pntg"},
                        new FileType() { TypeName = "Microsoft Windows Bitmap",TypeExtension= ".bmp"},
                        new FileType() { TypeName = "Microsoft Windows Icon",TypeExtension= ".ico"},
                        new FileType() { TypeName = "Paint Shop Pro (Win32 only)",TypeExtension= ".psp"},
                        new FileType() { TypeName = "PC Paintbrush (PCX)",TypeExtension= ".pcx"},
                        new FileType() { TypeName = "PC Paintbrush DCX (multi-page PCX)",TypeExtension= ".dcx"},
                        new FileType() { TypeName = "Portable Bitmap (PBM)",TypeExtension= ".pbm"},
                        new FileType() { TypeName = "Portable Graymap PGM",TypeExtension= ".pgm"},
                        new FileType() { TypeName = "Portable Network Graphics (PNG)",TypeExtension= ".png"},
                        new FileType() { TypeName = "Portable Pixmap (PPM)",TypeExtension= ".ppm"},
                        new FileType() { TypeName = "Progressive JPEG",TypeExtension= ".jpg"},
                        new FileType() { TypeName = "Progressive JPEG",TypeExtension= ".jpeg"},
                        new FileType() { TypeName = "Progressive JPEG",TypeExtension= ".jpe"},
                        new FileType() { TypeName = "Sun Raster",TypeExtension= ".srs"},
                        new FileType() { TypeName = "TIFF",TypeExtension= ".tif"},
                        new FileType() { TypeName = "TIFF",TypeExtension= ".tiff"},
                        new FileType() { TypeName = "TruVision TGA (Targa)",TypeExtension= ".tga"},
                        new FileType() { TypeName = "Word Perfect Graphics",TypeExtension= ".wpg"},
                        new FileType() { TypeName = "WordPerfect Graphics",TypeExtension= ".wpg"},
                        new FileType() { TypeName = "WordPerfect Graphics",TypeExtension= ".wpg2"},
                        new FileType() { TypeName = "X-Windows Bitmap",TypeExtension= ".xbm"},
                        new FileType() { TypeName = "X-Windows Dump",TypeExtension= ".xdm"},
                        new FileType() { TypeName = "X-Windows Pixmap",TypeExtension= ".xpm"},
                        new FileType() { TypeName = "Apple iWork Numbers (MacOS, text, and PDF preview)",TypeExtension= ".numbers"},
                        new FileType() { TypeName = "Lotus 1-2-3",TypeExtension= ".wk1"},
                        new FileType() { TypeName = "Lotus 1-2-3",TypeExtension= ".wk3"},
                        new FileType() { TypeName = "Lotus 1-2-3",TypeExtension= ".wk4"},
                        new FileType() { TypeName = "Lotus 1-2-3",TypeExtension= ".wks"},
                        new FileType() { TypeName = "Microsoft Excel",TypeExtension= ".xlsx"},
                        new FileType() { TypeName = "Microsoft Excel",TypeExtension= ".xls"},
                        new FileType() { TypeName = "Microsoft Excel for Windows (.xlsb)",TypeExtension= ".xlsb"},
                        new FileType() { TypeName = "Microsoft Works SS for DOS",TypeExtension= ".wks"},
                        new FileType() { TypeName = "Microsoft Works SS for Macintosh",TypeExtension= ".wks"},
                        new FileType() { TypeName = "Microsoft Works SS for Windows",TypeExtension= ".wks"},
                        new FileType() { TypeName = "Multiplan",TypeExtension= ".sylk"},
                        new FileType() { TypeName = "Novell PerfectWorks Spreadsheet",TypeExtension= ".wpw"},
                        new FileType() { TypeName = "OpenOffice Calc",TypeExtension= ".sdc"},
                        new FileType() { TypeName = "Oracle Open Office Calc",TypeExtension= ".sdc"},
                        new FileType() { TypeName = "QuattroPro for DOS",TypeExtension= ".wb1"},
                        new FileType() { TypeName = "QuattroPro for Windows",TypeExtension= ".qpw"},
                        new FileType() { TypeName = "QuattroPro for Windows",TypeExtension= ".wb3"},
                        new FileType() { TypeName = "QuattroPro for Windows",TypeExtension= ".wb2"},
                        new FileType() { TypeName = "QuattroPro for Windows",TypeExtension= ".wb1"},
                        new FileType() { TypeName = "SmartWare II SS",TypeExtension= ".def"},
                        new FileType() { TypeName = "SmartWare Spreadsheet",TypeExtension= ".def"},
                        new FileType() { TypeName = "StarOffice Calc",TypeExtension= ".sdc"},
                        new FileType() { TypeName = "SuperCalc",TypeExtension= ".cal"},
                        new FileType() { TypeName = "Symphony",TypeExtension= ".wrk"},
                        new FileType() { TypeName = "ANSI Text",TypeExtension= ".ans"},
                        new FileType() { TypeName = "ASCII Text",TypeExtension= ".asc"},
                        new FileType() { TypeName = "HTML (CSS rendering not supported)",TypeExtension= ".html"},
                        new FileType() { TypeName = "Rich Text Format (RTF)",TypeExtension= ".rtf"},
                        new FileType() { TypeName = "Unicode Text",TypeExtension= ".txt"},
                        new FileType() { TypeName = "Wireless Markup Language",TypeExtension= ".wml"},
                        new FileType() { TypeName = "XML (text only)",TypeExtension= ".xml"},
                        new FileType() { TypeName = "Adobe PDF",TypeExtension= ".pdf"},
                        new FileType() { TypeName = "Ami Draw",TypeExtension= ".sdw"},
                        new FileType() { TypeName = "AutoCAD Drawing",TypeExtension= ".dwg"},
                        new FileType() { TypeName = "AutoShade Rendering",TypeExtension= ".rnd"},
                        new FileType() { TypeName = "Corel Draw",TypeExtension= ".cdr"},
                        new FileType() { TypeName = "Corel Draw Clipart",TypeExtension= ".cmx"},
                        new FileType() { TypeName = "Enhanced Metafile (EMF)",TypeExtension= ".emf"},
                        new FileType() { TypeName = "Escher Graphics",TypeExtension= ".egr"},
                        new FileType() { TypeName = "FrameMaker Graphics (FMV)",TypeExtension= ".fmv"},
                        new FileType() { TypeName = "Gem File (Vector)",TypeExtension= ".img"},
                        new FileType() { TypeName = "Harvard Graphics Chart DOS",TypeExtension= ".ch3"},
                        new FileType() { TypeName = "Harvard Graphics for Windows",TypeExtension= ".prs"},
                        new FileType() { TypeName = "HP Graphics Language",TypeExtension= ".hp"},
                        new FileType() { TypeName = "HP Graphics Language",TypeExtension= ".hpg"},
                        new FileType() { TypeName = "IGES Drawing",TypeExtension= ".iges"},
                        new FileType() { TypeName = "Micrografx Designer",TypeExtension= ".dsf"},
                        new FileType() { TypeName = "Micrografx Designer",TypeExtension= ".drw"},
                        new FileType() { TypeName = "Micrografx Draw",TypeExtension= ".drw"},
                        new FileType() { TypeName = "OpenOffice Draw",TypeExtension= ".sda"},
                        new FileType() { TypeName = "Oracle Open Office Draw",TypeExtension= ".sda"},
                        new FileType() { TypeName = "SVG (processed as XML, not rendered)",TypeExtension= ".xml"},
                        new FileType() { TypeName = "Microsoft Visio:",TypeExtension= ".vsd"},
                        new FileType() { TypeName = "Visio (Page Preview mode WMF/EMF)",TypeExtension= ".wmf"},
                        new FileType() { TypeName = "Visio (Page Preview mode WMF/EMF)",TypeExtension= ".emf"},
                        new FileType() { TypeName = "Windows Metafile",TypeExtension= ".vmf"},
                        new FileType() { TypeName = "Adobe FrameMaker (MIF only)",TypeExtension= ".mif"},
                        new FileType() { TypeName = "Adobe Illustrator Postscript",TypeExtension= ".eps"},
                        new FileType() { TypeName = "Ami Pro for OS2",TypeExtension= ".sam"},
                        new FileType() { TypeName = "Ami Pro for Windows",TypeExtension= ".sam"},
                        new FileType() { TypeName = "Apple iWork Pages (MacOS, text and PDF preview)",TypeExtension= ".pages"},
                        new FileType() { TypeName = "DEC DX",TypeExtension= ".dx"},
                        new FileType() { TypeName = "DEC DX Plus",TypeExtension= ".wpl"},
                        new FileType() { TypeName = "First Choice WP",TypeExtension= ".pfx"},
                        new FileType() { TypeName = "Hangul",TypeExtension= ".hwp"},
                        new FileType() { TypeName = "IBM DCA/FFT",TypeExtension= ".dca"},
                        new FileType() { TypeName = "IBM DCA/FFT",TypeExtension= ".fft"},
                        new FileType() { TypeName = "IBM DisplayWrite",TypeExtension= ".rft"},
                        new FileType() { TypeName = "IBM Writing Assistant",TypeExtension= ".iwa"},
                        new FileType() { TypeName = "Ichitaro",TypeExtension= ".jtd"},
                        new FileType() { TypeName = "JustWrite",TypeExtension= ".jw"},
                        new FileType() { TypeName = "Kingsoft WPS Writer",TypeExtension= ".wps"},
                        new FileType() { TypeName = "Legacy",TypeExtension= ".leg"},
                        new FileType() { TypeName = "Lotus Manuscript",TypeExtension= ".manu"},
                        new FileType() { TypeName = "Lotus WordPro (text only)",TypeExtension= ".lwp"},
                        new FileType() { TypeName = "Lotus WordPro (text only)",TypeExtension= ".mwp"},
                        new FileType() { TypeName = "MacWrite II",TypeExtension= ".mcw"},
                        new FileType() { TypeName = "Mass 11",TypeExtension= ".m11"},
                        new FileType() { TypeName = "Microsoft Word",TypeExtension= ".doc"},
                        new FileType() { TypeName = "Microsoft Word",TypeExtension= ".docx"},
                        new FileType() { TypeName = "Microsoft WordPad",TypeExtension= ".rtf"},
                        new FileType() { TypeName = "Microsoft Works WP for DOS",TypeExtension= ".wps"},
                        new FileType() { TypeName = "Microsoft Write for Windows",TypeExtension= ".wri"},
                        new FileType() { TypeName = "MultiMate",TypeExtension= ".dox"},
                        new FileType() { TypeName = "Navy DIF",TypeExtension= ".dif"},
                        new FileType() { TypeName = "Nota Bene",TypeExtension= ".nb"},
                        new FileType() { TypeName = "Novell PerfectWorks Word Processor",TypeExtension= ".wpw"},
                        new FileType() { TypeName = "OpenOffice Writer",TypeExtension= ".sdw"},
                        new FileType() { TypeName = "Oracle Open Office Writer",TypeExtension= ".sdw"},
                        new FileType() { TypeName = "PFS: Write",TypeExtension= ".pfs"},
                        new FileType() { TypeName = "Q&A Write",TypeExtension= ".jw"},
                        new FileType() { TypeName = "Samna Word IV",TypeExtension= ".sam"},
                        new FileType() { TypeName = "SmartWare II WP",TypeExtension= ".def"},
                        new FileType() { TypeName = "Sprint",TypeExtension= ".spr"},
                        new FileType() { TypeName = "StarOffice Writer",TypeExtension= ".sdw"},
                        new FileType() { TypeName = "Wang IWP",TypeExtension= ".iwp"},
                        new FileType() { TypeName = "WordPerfect for DOS",TypeExtension= ".wpd"},
                        new FileType() { TypeName = "Wordstar 2000 for DOS",TypeExtension= ".wsd"},
                        new FileType() { TypeName = "Wordstar for Windows",TypeExtension= ".ws1"},
                        new FileType() { TypeName = "XyWrite",TypeExtension= ".xy"},
                        new FileType() { TypeName = "Apache Office Calc (ODF 1.2)",TypeExtension= ".odp"},
                        new FileType() { TypeName = "Apache Office Draw (ODF 1.2)",TypeExtension= ".odg"},
                        new FileType() { TypeName = "Apache Office Impress (ODF 1.2)",TypeExtension= ".odp"},
                        new FileType() { TypeName = "Apache Office Writer (ODF 1.2)",TypeExtension= ".odt"},
                        new FileType() { TypeName = "Apple iWork Keynote File",TypeExtension= ".key"},
                        new FileType() { TypeName = "Apple iWork Keynote File Preview",TypeExtension= ".key"},
                        new FileType() { TypeName = "Apple iWork Keynote Numbers File",TypeExtension= ".key"},
                        new FileType() { TypeName = "Apple iWork Keynote Numbers File Preview",TypeExtension= ".key"},
                        new FileType() { TypeName = "Apple iWork Pages File",TypeExtension= ".pages"},
                        new FileType() { TypeName = "Apple iWork Pages File Preview",TypeExtension= ".pages"},
                        new FileType() { TypeName = "Libre Office Calc (ODF 1.2)",TypeExtension= ".odt"},
                        new FileType() { TypeName = "Libre Office Draw (ODF 1.2)",TypeExtension= ".odg"},
                        new FileType() { TypeName = "Libre Office Impress (ODF 1.2)",TypeExtension= ".odp"},
                        new FileType() { TypeName = "Libre Office Writer (ODF 1.2)",TypeExtension= ".odt"},
                        new FileType() { TypeName = "Office Calc (ODF 1.2)",TypeExtension= ".odt"},
                        new FileType() { TypeName = "Encoded mail messages",TypeExtension= ".mht"},
                        new FileType() { TypeName = "Adobe Illustrator XMP",TypeExtension= ".xmp"},
                        new FileType() { TypeName = "Adobe InDesign XMP",TypeExtension= ".xmp"},
                        new FileType() { TypeName = "Adobe InDesign Interchange XMP only",TypeExtension= ".xmp"},
                        new FileType() { TypeName = "Stencil",TypeExtension= ".vsd"},
                        new FileType() { TypeName = "Template",TypeExtension= ".vsd"},
                        new FileType() { TypeName = "Macro Enabled Drawing",TypeExtension= ".vsd"},
                        new FileType() { TypeName = "Macro Enabled Stencil",TypeExtension= ".vsd"},
                        new FileType() { TypeName = "Macro Enabled Template",TypeExtension= ".vsd"}
                    };
                }

                return _viewerSupportedFileType;
            }
        }
        public bool IsFileTypeSupported(string fileExtension) => ViewerSupportedFileTypes.Any(x => x.TypeExtension.Equals(fileExtension.ToLower()));
        public async Task<Response> SaveSingleDocument(ExportedMetadata documentInfo, int folderID, string webApiUrl, int workspaceID, int userID)
        {
            string error = await ImportDocument(documentInfo, webApiUrl, workspaceID, folderID);
            if (string.IsNullOrEmpty(error))
            {
                CreateMetrics(documentInfo, Helpers.Constants.BUCKET_DocumentsUploaded);
                File.Delete(instanceFile(documentInfo.FileName, documentInfo.Native, false));
                return new Response() { Result = Path.GetFileNameWithoutExtension(documentInfo.FileName), Success = true };
            }
            return new Response() { Result = error, Success = false };
        }
        public async Task ReplaceSingleDocument(ExportedMetadata documentInfo, int docID, bool fromDocumentViewer, bool avoidControlNumber, bool isDataGrid, string webApiUrl, int workspaceID, int userID, int folderID = 0)
        {
            if (!avoidControlNumber || !fromDocumentViewer)
            {
                DTOs.Document replacedDocument = new DTOs.Document(docID);
                if (!avoidControlNumber)
                {
                    replacedDocument.TextIdentifier = Path.GetFileNameWithoutExtension(documentInfo.FileName);
                }
                if (!fromDocumentViewer)
                {
                    replacedDocument.ParentArtifact = new DTOs.Artifact(defineFolder(folderID));
                    await ChangeFolder(folderID, docID);
                }
            }

            updateNative(documentInfo, docID);
            await ImportDocument(documentInfo, webApiUrl, workspaceID, folderID, docID);
            CreateMetrics(documentInfo, Helpers.Constants.BUCKET_DocumentsUploaded);
            File.Delete(instanceFile(documentInfo.FileName, documentInfo.Native, false));
            UpdateDocumentLastModificationFields(docID, userID, false);
        }
        public bool ValidateDocImages(int docArtifactId)
        {
            bool result = false;

            DTOs.Document document = new DTOs.Document(docArtifactId);
            document.Fields.Add(new DTOs.FieldValue(DTOs.DocumentFieldNames.HasImages));
            var docResults = _Repository.RSAPIClient.Repositories.Document.Read(document);
            if (docResults.Success)
            {
                DTOs.Document documentArtifact = docResults.Results.FirstOrDefault().Artifact;
                result = documentArtifact.HasImages.Name.Equals("Yes");
            }

            return result;
        }
        public bool ValidateDocNative(int docArtifactId)
        {
            bool result = false;

            DTOs.Document document = new DTOs.Document(docArtifactId);
            document.Fields.Add(new DTOs.FieldValue(DTOs.DocumentFieldNames.HasNative));
            var docResults = _Repository.RSAPIClient.Repositories.Document.Read(document);
            if (docResults.Success)
            {
                DTOs.Document documentArtifact = docResults.Results.FirstOrDefault().Artifact;
                result = documentArtifact.HasNative.Value;
            }

            return result;
        }
        public string GetDocumentControlNumber(int docArtifactId)
        {
            string result = null;

            DTOs.Document document = new DTOs.Document(docArtifactId);
            document.Fields.Add(new DTOs.FieldValue(new Guid(Helpers.Constants.CONTROL_NUMBER_FIELD)));
            var docResults = _Repository.RSAPIClient.Repositories.Document.Read(document);
            if (docResults.Success)
            {
                DTOs.Document documentArtifact = docResults.Results.FirstOrDefault().Artifact;
                result = documentArtifact.Fields[0].Value.ToString();
            }

            return result;
        }

        public int GetDocumentArtifactIdByControlNumber(string controlNumber)
        {
            var result = _Repository.CaseDBContext.ExecuteSqlStatementAsScalar(Queries.GetDocumentArtifactIdByControlNumber, 
                new[] {
                    SqlHelper.CreateSqlParameter("@ControlNumber", controlNumber)
                });
            return int.Parse(result.ToString());
        }

        public bool ValidateHasRedactions(int docArtifactId)
        {
            var query = Queries.DocumentHasRedactions;
            return _Repository.CaseDBContext.ExecuteSqlStatementAsScalar<int>(query,
                 new[] {
                    SqlHelper.CreateSqlParameter("@DocumentID", docArtifactId),
                 }) > 0;
        }
        public void UpdateDocumentLastModificationFields(int docArtifactId, int userID, bool isNew)
        {
            _Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.UpdateDocumentLastModificationFields,
                 new[] {
                    SqlHelper.CreateSqlParameter("@DocumentID", docArtifactId),
                    SqlHelper.CreateSqlParameter("@UserID", userID),
                    SqlHelper.CreateSqlParameter("@New", isNew),
                 });
        }
        public void DeleteRedactions(int docArtifactId)
        {
            _Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.DeleteDocumentRedactions,
                 new[] {
                    SqlHelper.CreateSqlParameter("@DocumentID", docArtifactId),
                 });
        }
        public void DeleteExistingImages(int dArtifactId)
        {
            _Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.DeleteDocumentImages,
                new[] {
                    SqlHelper.CreateSqlParameter("@DocumentID", dArtifactId),
                });
        }
        public void InsertImage(FileInformation image)
        {
            _Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.InsertImageInFileTable,
                new[] {
                    SqlHelper.CreateSqlParameter("@DocumentID", image.DocumentArtifactID),
                    SqlHelper.CreateSqlParameter("@FileName", image.FileName),
                    SqlHelper.CreateSqlParameter("@Order", image.Order),
                    SqlHelper.CreateSqlParameter("@DocIdentifier", image.DocumentIdentifier),
                    SqlHelper.CreateSqlParameter("@Location", image.FileLocation),
                    SqlHelper.CreateSqlParameter("@Size", image.FileSize),
                    SqlHelper.CreateSqlParameter("@Type", image.FileType)
                });
        }
        public void UpdateHasImages(int dArtifactId)
        {
            _Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.UpdateHasImages,
                new[] {
                    SqlHelper.CreateSqlParameter("@DocumentID", dArtifactId),
                    SqlHelper.CreateSqlParameter("@HasImagesFieldGuid", Helpers.Constants.DOCUMENT_HAS_IMAGES_FIELD),
                    SqlHelper.CreateSqlParameter("@HasImagesCodeYesGuid", Helpers.Constants.DOCUMENT_HAS_IMAGES_YES_CHOICE)
                });
        }
        public FileInformation getFileByArtifactId(int docArtifactId)
        {
            FileInformation fInformation = null;

            System.Data.Common.DbDataReader reader = _Repository.CaseDBContext.ExecuteSqlStatementAsDbDataReader(Queries.GetFileInfoByDocumentArtifactID, new[] { SqlHelper.CreateSqlParameter("@documentArtifactId", docArtifactId) });

            if (reader.HasRows)
            {
                reader.Read();
                fInformation = new FileInformation()
                {
                    FileID = reader.GetInt32(0),
                    DocumentArtifactID = reader.GetInt32(1),
                    FileName = reader.GetString(2),
                    FileLocation = reader.GetString(3)
                };
            }

            reader.Close();

            return fInformation;
        }
        public int GetDocByName(string docName)
        {
            DTOs.Query<DTOs.Document> qDocs = new DTOs.Query<DTOs.Document>();
            qDocs.Condition = new Client.TextCondition(DTOs.DocumentFieldNames.TextIdentifier, Relativity.Client.TextConditionEnum.EqualTo, docName);
            qDocs.Fields = DTOs.FieldValue.NoFields;
            return _Repository.RSAPIClient.Repositories.Document.Query(qDocs, 1).Results.FirstOrDefault()?.Artifact?.ArtifactID ?? -1;
        }
        public void SetCreateInstanceSettings()
        {
            Repository.Instance.MasterDBContext.ExecuteNonQuerySQLStatement(Queries.InsertInstanceSettings);
            var isResult = Repository.Instance.MasterDBContext.ExecuteNonQuerySQLStatement(Queries.InsertFieldsInstanceSetting);

            // Read Default fields configuration from instance setting
            if (isResult > 0)
            {
                CreateWorkspaceFieldSettings();
            }
        }
        public async Task<bool> ValidateFileTypes(string extension)
        {
            ObjectQueryResultSet results;
            using (IObjectQueryManager _objectQueryManager = _Repository.CreateProxy<IObjectQueryManager>(ExecutionIdentity.System))
            {
                Query query = new Query { Fields = new[] { "Value", "Section" }, IncludeIdWindow = false, TruncateTextFields = true, Condition = $"'Name' IN ['RestrictedNativeFileTypes']" };
                results = await _objectQueryManager.QueryAsync(-1, (int)Client.ArtifactType.InstanceSetting, query, 1, int.MaxValue, new int[] { 1, 2, 3, 4, 5, 6 }, string.Empty);
            }
            var restricted = results.Data.DataResults[0].Fields[0].Value.ToString().Split(';').Select(x => x.ToLower()).ToList();
            restricted.AddRange(new[] { "dll", "exe", "js" });
            return !restricted.Contains(extension.Replace(".", ""));
        }
        public async Task<bool> IsDataGridEnabled(int workspaceID)
        {
            ObjectQueryResultSet results;
            using (IObjectQueryManager _objectQueryManager = _Repository.CreateProxy<IObjectQueryManager>(ExecutionIdentity.System))
            {
                Query query = new Query { Fields = new[] { DTOs.WorkspaceFieldNames.EnableDataGrid }, IncludeIdWindow = false, TruncateTextFields = true, Condition = $"'ArtifactID' IN [{workspaceID}]" };
                results = await _objectQueryManager.QueryAsync(-1, (int)Client.ArtifactType.Case, query, 1, int.MaxValue, new int[] { 1, 2, 3, 4, 5, 6 }, string.Empty);
            }
            var isDataGrid = (bool)results.Data.DataResults[0].Fields[0].Value;
            return isDataGrid;

        }
        public async Task<DataTable> GetDocumentDataTable(string identifierName)
        {
            DataTable DocumentsDataTable = new DataTable();

            DocumentsDataTable.Columns.Add(identifierName, typeof(string));
            DocumentsDataTable.Columns.Add("Extracted Text", typeof(string));
            

            if (await ToggleManager.Instance.GetCheckSFUFieldsAsync())
            {
                var wsResult = Repository.Instance.CaseDBContext.ExecuteSqlStatementAsScalar(Queries.GetFieldsWorspaceSetting);
                JObject wsFields = null;
                if (wsResult != null)
                {
                    wsFields = JObject.Parse(wsResult.ToString());
                }
                else
                {
                    CreateWorkspaceFieldSettings();
                    wsResult = Repository.Instance.CaseDBContext.ExecuteSqlStatementAsScalar(Queries.GetFieldsWorspaceSetting);

                    if (wsResult != null)
                    {
                        wsFields = JObject.Parse(wsResult.ToString());
                    }
                    else
                    {
                        throw new Exception("Fields Settings does not exist.");
                    }
                }

                foreach (var wsItem in wsFields)
                {
                    DocumentsDataTable.Columns.Add(wsItem.Value.ToString(), typeof(string));
                }
            }
            else
            {
                DocumentsDataTable.Columns.Add("Document Extension", typeof(string));
                DocumentsDataTable.Columns.Add("File Extension", typeof(string));
                DocumentsDataTable.Columns.Add("FileExtension", typeof(string));
                DocumentsDataTable.Columns.Add("File Name", typeof(string));
                DocumentsDataTable.Columns.Add("FileName", typeof(string));
                DocumentsDataTable.Columns.Add("File Size");
                DocumentsDataTable.Columns.Add("FileSize");
            }

            DocumentsDataTable.Columns.Add("Native File", typeof(string));
            return DocumentsDataTable;
        }
        public void WriteFile(byte[] file, FileInformation document)
        {
            File.WriteAllBytes(document.FileLocation, file);
        }
        public string GetRepositoryLocation()
        {
            return _Repository.MasterDBContext.ExecuteSqlStatementAsScalar<string>(Queries.GetRepoLocationByCaseID, new[] { SqlHelper.CreateSqlParameter("AID", _Repository.WorkspaceID) });
        }

        private void CreateWorkspaceFieldSettings()
        {

            var fieldNames = Repository.Instance.MasterDBContext.ExecuteSqlStatementAsScalar(Queries.GetFieldsInstanceSetting).ToString();

            if (!string.IsNullOrEmpty(fieldNames))
            {
                JObject fields = JObject.Parse(fieldNames);
                JObject wsFields = new JObject();
                StringBuilder builder = new StringBuilder();

                foreach (var fItem in fields)
                {
                    builder.Append(string.Format(Queries.GetFieldItem, fItem.Key, fItem.Value["value"].ToString()));
                    builder.Append("\nUNION\n");
                    wsFields.Add(fItem.Key, fItem.Value["default"].ToString());
                }

                var wsValue = builder.ToString();
                wsValue = wsValue.Substring(0, wsValue.LastIndexOf("UNION"));
                DataTable tblFields = Repository.Instance.CaseDBContext.ExecuteSqlStatementAsDataTable(wsValue);

                if (tblFields != null && tblFields.Rows.Count > 0)
                {
                    foreach (DataRow row in tblFields.Rows)
                    {
                        wsFields[row[0].ToString()] = row[1].ToString();
                    }
                }

                Repository.Instance.CaseDBContext.ExecuteNonQuerySQLStatement(string.Format(Queries.InsertFieldsWorspaceSetting, wsFields.ToString()));
            }
        }
        private async Task<string> ImportDocument(ExportedMetadata documentInfo, string webApiUrl, int workspaceID, int folderId = 0, int? documentId = null)
        {
            string returnValues = string.Empty;
            try
            {
                string value = getBearerToken(webApiUrl);
                webApiUrl = webApiUrl.Replace("/Relativity/", "/RelativityWebAPI/");
                ImportAPI iapi = new ExtendedImportAPI("XxX_BearerTokenCredentials_XxX", value, webApiUrl);
                var importJob = iapi.NewNativeDocumentImportJob();

                importJob.Settings.CaseArtifactId = workspaceID;
                importJob.Settings.ExtractedTextFieldContainsFilePath = false;
                importJob.Settings.DisableExtractedTextEncodingCheck = true;
                importJob.Settings.DisableExtractedTextFileLocationValidation = true;
                importJob.Settings.DisableNativeLocationValidation = true;
                importJob.Settings.DisableNativeValidation = true;
                importJob.Settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
                importJob.OnComplete += ImportJob_OnComplete;
                importJob.OnError += ImportJob_OnError;
                importJob.OnFatalException += ImportJob_OnFatalException;

                if (folderId != 0)
                {
                    importJob.Settings.DestinationFolderArtifactID = folderId;
                }

                var IdentityField = await GetDocumentIdentifier();

                importJob.Settings.SelectedIdentifierFieldName = IdentityField.Name;
                importJob.Settings.NativeFilePathSourceFieldName = "Native File";
                importJob.Settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
                importJob.Settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
                // Specify the ArtifactID of the document identifier field, such as a control number.
                importJob.Settings.IdentityFieldId = IdentityField.ArtifactId;

                DataTable dtDocument = await GetDocumentDataTable(IdentityField.Name);

                var extension = Path.GetExtension(documentInfo.FileName);
                var fileName = Path.GetFileNameWithoutExtension(documentInfo.FileName);
                var fullFileName = documentInfo.FileName;
                var fileSize = decimal.Parse(documentInfo.Native.LongLength.ToString());

                // Add file to load
                if (await ToggleManager.Instance.GetCheckSFUFieldsAsync())
                {
                    dtDocument.Rows.Add(
                    !string.IsNullOrEmpty(documentInfo.ControlNumber) ? documentInfo.ControlNumber : (documentId.HasValue ? GetDocumentControlNumber(documentId.Value) : Path.GetFileNameWithoutExtension(documentInfo.FileName)),
                    documentInfo.ExtractedText,
                    extension,
                    fullFileName,
                    fileSize,
                    instanceFile(documentInfo.FileName, documentInfo.Native, false));
                }
                else
                {
                    dtDocument.Rows.Add(
                    !string.IsNullOrEmpty(documentInfo.ControlNumber) ? documentInfo.ControlNumber : (documentId.HasValue ? GetDocumentControlNumber(documentId.Value) : Path.GetFileNameWithoutExtension(documentInfo.FileName)),
                    documentInfo.ExtractedText,
                    extension,
                    extension,
                    extension,
                    fullFileName,
                    fileName,
                    fileSize,
                    fileSize,
                    instanceFile(documentInfo.FileName, documentInfo.Native, false));
                }

                importJob.SourceData.SourceData = dtDocument.CreateDataReader();

                importJob.Execute();
            }
            catch (Exception ex)
            {
                LogError(ex);
                return ex.Message;
            }
            return returnValues;
        }
        public void CreateMetrics(ExportedMetadata documentInfo, string bucket)
        {
            if (!string.IsNullOrEmpty(bucket))
            {
                ITelemetryManager telManager = new TelemetryManager();
                telManager.LogCountAsync(bucket, 1L);
                telManager.LogCountAsync(Helpers.Constants.BUCKET_TotalSizeDocumentUploaded, documentInfo.Native.LongLength);
                //Create File tipe metric
                telManager.CreateMetricAsync(string.Concat(Helpers.Constants.BUCKET_FileType, Path.GetExtension(documentInfo.FileName)), $"Number of {Path.GetExtension(documentInfo.FileName).Remove(0, 1)} uploaded");
                telManager.LogCountAsync(string.Concat(Helpers.Constants.BUCKET_FileType, Path.GetExtension(documentInfo.FileName)), 1L);
            }
        }
        public string instanceFile(string fileName, byte[] fileBytes, bool isTemp, string baseRepo = null)
        {
            string folder = Path.Combine(baseRepo ?? Path.GetTempPath(), $"RV_{Guid.NewGuid()}");
            Directory.CreateDirectory(folder);
            string tmpFileName = Path.Combine(folder, isTemp ? $"{Guid.NewGuid()}" + fileName : fileName);
            File.WriteAllBytes(tmpFileName, fileBytes);
            return tmpFileName;
        }
        private async Task ChangeFolder(int folderID, int docID)
        {
            using (Services.Document.IDocumentManager docManager = _Repository.CreateProxy<Services.Document.IDocumentManager>())
            {
                await docManager.MoveDocumentsToFolderAsync(_Repository.WorkspaceID,
                    new Services.Folder.FolderRef(folderID), new List<Services.Document.DocumentRef>() {
                        new Services.Document.DocumentRef(docID)
                    });
            }
        }
        private int getFieldIDByNameAndType(Client.FieldType type, params string[] fNames)
        {
            int id = 0;
            if (fNames.Length > 0)
            {
                id = _Repository.CaseDBContext.ExecuteSqlStatementAsScalar<int>(string.Format(Queries.GetFieldIDByNameAndType, string.Join(",", fNames.Select(p => $"'{p}'"))), SqlHelper.CreateSqlParameter("Type", (int)type));
            }
            return id;
        }
        private void addFieldToNewDocument(ExportedMetadata documentInfo, DTOs.Document newDocument, string fieldName, Client.FieldType type, params string[] possibleMatch)
        {
            if (documentInfo.Fields.ContainsKey(fieldName))
            {
                int fid = getFieldIDByNameAndType(type, possibleMatch);
                if (fid > 0)
                {
                    newDocument.Fields.Add(new DTOs.FieldValue(fid, documentInfo.Fields[fieldName], false));
                }
            }
        }
        private int getDocumentFieldByCategory(Client.FieldCategory category)
        {
            return _Repository.CaseDBContext.ExecuteSqlStatementAsScalar<int>(Queries.GetDocumentIdentifierField, SqlHelper.CreateSqlParameter("CATEGORYID", (int)category));
        }
        private int defineFolder(int id)
        {
            return _Repository.CaseDBContext.ExecuteSqlStatementAsScalar<int>(Queries.GetDroppedFolder, SqlHelper.CreateSqlParameter("SupID", id));
        }
        private string getNativeTypeByFilename(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToString();
            string relativityNativeType = string.Empty;

            switch (extension)
            {
                case ".xml":
                    relativityNativeType = "Extensible Markup Language (XML)";
                    break;
                case ".pdf":
                    relativityNativeType = "Adobe Acrobat (PDF)";
                    break;
                case ".doc":
                    relativityNativeType = "Microsoft Word 2003/2004";
                    break;
                case ".docx":
                    relativityNativeType = "Microsoft Word 2010";
                    break;
                case ".xls":
                    relativityNativeType = "Microsoft Excel 2003";
                    break;
                case ".xlsx":
                    relativityNativeType = "Microsoft Excel 2010 Workbook";
                    break;
                case ".html":
                case ".htm":
                    relativityNativeType = "Internet HTML";
                    break;
                case ".ppt":
                    relativityNativeType = "Microsoft PowerPoint 2000/2002";
                    break;
                case ".pptx":
                    relativityNativeType = "Microsoft PowerPoint 2007/2008";
                    break;
                default:
                    break;
            }
            return relativityNativeType;
        }
        private void updateNative(ExportedMetadata documentInfo, int docID)
        {
            var file = getFileByArtifactId(docID);

            var workspaceRepo = GetRepositoryLocation();
            var replaceGuid = Guid.NewGuid().ToString();
            var newFileLocation = instanceFile(replaceGuid, documentInfo.Native, false, workspaceRepo);
            _Repository.CaseDBContext.ExecuteNonQuerySQLStatement(Queries.ReplaceNativeFile, new[]
            {
                SqlHelper.CreateSqlParameter("FID", file == null ? -1 : file.FileID),
                SqlHelper.CreateSqlParameter("AID", docID),
                SqlHelper.CreateSqlParameter("RG", replaceGuid),
                SqlHelper.CreateSqlParameter("FN", Path.GetFileName(documentInfo.FileName)),
                SqlHelper.CreateSqlParameter("LOC", newFileLocation),
                SqlHelper.CreateSqlParameter("SZ", documentInfo.Native.LongLength),
                SqlHelper.CreateSqlParameter("RNT", getNativeTypeByFilename(documentInfo.FileName))
            }, 300);
        }
        private void updateMatchedField(ExportedMetadata documentInfo, int docID)
        {
            var matched = _Repository.CaseDBContext.ExecuteSqlStatementAsDataTable(string.Format(Queries.GetMatchedFields, "'File Name','Document Extension','File Size'")).Rows.Cast<DataRow>().Select(p => p[0] as string).ToArray();
            if (matched.Length > 0)
            {
                string setString = string.Join(",", matched.Select(p => $"{p} = @{p}"));
                _Repository.CaseDBContext.ExecuteNonQuerySQLStatement($"UPDATE EDDSDBO.[Document] SET { setString } WHERE ArtifactID = @AID", new[]
                {
                    SqlHelper.CreateSqlParameter("AID", docID),
                    SqlHelper.CreateSqlParameter("FileName", Path.GetFileName(documentInfo.FileName)),
                    SqlHelper.CreateSqlParameter("FileSize", documentInfo.Native.LongLength),
                    SqlHelper.CreateSqlParameter("DocumentExtension", Path.GetExtension(documentInfo.FileName))
                }, 300);
            }
        }
        private async Task<DocumentIdentifierField> GetDocumentIdentifier()
        {
            ObjectQueryResultSet results;
            using (IObjectQueryManager _objectQueryManager = _Repository.CreateProxy<IObjectQueryManager>())
            {
                Query query = new Query()
                {
                    Condition = $"'FieldCategoryID' == 2 AND 'FieldArtifactTypeID' == 10",
                    Fields = new string[] { "ArtifactID", "DisplayName" },
                    IncludeIdWindow = false,
                    RelationalField = null,
                    SampleParameters = null,
                    SearchProviderCondition = null,
                    Sorts = new string[] { },
                    TruncateTextFields = false
                };
                results = await _objectQueryManager.QueryAsync(Repository.Instance.WorkspaceID, 14, query, 1, int.MaxValue, new int[] { 1, 2, 3, 4, 5, 6 }, string.Empty);
            }

            var restricted = results.Data.DataResults[0];

            return new DocumentIdentifierField()
            {
                ArtifactId = Convert.ToInt32(restricted.Fields[0].Value),
                Name = restricted.Fields[1].Value?.ToString(),
            };
        }
        private string getBearerToken(string instanceUrl)
        {
            string token = ExtensionPointServiceFinder.SystemTokenProvider.GetLocalSystemToken();
            return token;
        }
        private KeyValuePair<int, string> GetFieldinfo(string artifactGuid)
        {
            KeyValuePair<int, string> fieldData = default(KeyValuePair<int, string>);
            System.Data.Common.DbDataReader reader = _Repository.CaseDBContext.ExecuteSqlStatementAsDbDataReader(Queries.GetFieldInfoByGuid, new[] { SqlHelper.CreateSqlParameter("@artifactGuid", artifactGuid) });

            if (reader.HasRows)
            {
                reader.Read();
                fieldData = new KeyValuePair<int, string>(reader.GetInt32(0), reader.GetString(1));
            }

            reader.Close();
            return fieldData;
        }
        private Tuple<string, string> GetClientCredentials()
        {
            Tuple<string, string> clientData = default(Tuple<string, string>);
            System.Data.Common.DbDataReader reader = _Repository.MasterDBContext.ExecuteSqlStatementAsDbDataReader(Queries.GetClientCredentials);

            if (reader.HasRows)
            {
                reader.Read();
                clientData = new Tuple<string, string>(reader.GetString(0), reader.GetString(1));
            }

            reader.Close();
            return clientData;
        }
        private void ImportJob_OnError(System.Collections.IDictionary row)
        {
            LogError(new Exception(row["Message"].ToString()));
            throw new Exception(row["Message"].ToString().Split('.')[1].Trim());
        }
        private void ImportJob_OnComplete(JobReport jobReport)
        {
            Repository.Instance.GetLogFactory().GetLogger().LogInformation($"SFU: Document upload succes: {jobReport.TotalRows} row(s) created.");
        }
        private void ImportJob_OnFatalException(JobReport jobReport)
        {
            LogError(jobReport.FatalException);
            throw jobReport.FatalException;
        }
       
    }
}
