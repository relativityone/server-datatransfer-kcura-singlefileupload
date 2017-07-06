using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSerio.Relativity.Infrastructure;
using kCura.Relativity.Client;
using Moq;
using Relativity.API;
using kCura.SingleFileUpload.Core.Managers;
using kCura.SingleFileUpload.Core.Managers.Implementation;
using System.IO;
using NSerio.Relativity;

namespace kCura.SingleFileUpload
{
    [TestClass]
    public class SearchExportManagerTest
    {
        const string c_SQLServerName = "relativityvm";
        const string c_SQLUser = "eddsdbo";
        const string c_SQLPassword = "Nserio.1";

        const string c_ServerURL = "https://relativityvm/relativity.services";
        const string c_RelativityUser = "relativity.admin@kcura.com";
        const string c_RelativityPassword = "Nserio.1";

        [TestInitialize]
        public void test_initialize()
        {
            var helper = new StandaloneCustomPageHelper(-1, c_RelativityUser, c_RelativityPassword, c_SQLServerName, c_SQLUser, c_SQLPassword);
            RepositoryHelper.ConfigureRepository(helper);
        }

        [TestMethod]
        public void test_export()
        {
            using (CacheContextScope scope = RepositoryHelper.InitializeRepository(-1))
            {
                ISearchExportManager sem = new SearchExportManager();
                var fileLocation = @"D:\\Documents\\LibPrendaM3\\Carta.docx";
                var act = sem.ExportToSearchML(Path.GetFileName(fileLocation), File.ReadAllBytes(fileLocation));
                Assert.IsNotNull(act.Fields["MAIL_FROM"] as string == "PasswordReset@kcura.com");
            }
        }
    }
}
