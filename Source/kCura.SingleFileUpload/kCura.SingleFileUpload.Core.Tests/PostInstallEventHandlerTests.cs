using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSerio.Relativity.Infrastructure;
using kCura.Relativity.Client;
using Moq;
using Relativity.API;
using kCura.SingleFileUpload.Resources.EventHandlers;
using NSerio.Relativity;
using kCura.SingleFileUpload.Core.Managers.Implementation;

namespace kCura.SingleFileUpload
{
    [TestClass]
    public class PostInstallEventHandlerTests
    {
        const string c_SQLServerName = "relativityvm";
        const string c_SQLUser = "eddsdbo";
        const string c_SQLPassword = "Nserio.1";

        const string c_ServerURL = "https://relativityvm/relativity.services";
        const string c_RelativityUser = "relativity.admin@kcura.com";
        const string c_RelativityPassword = "Nserio.1";
        IHelper helper;
        [TestInitialize]
        public void test_initialize()
        {
            helper = new StandaloneCustomPageHelper(-1, c_RelativityUser, c_RelativityPassword, c_SQLServerName, c_SQLUser, c_SQLPassword);
            RepositoryHelper.ConfigureRepository(helper);
        }

        [TestMethod]
        public void testExecution()
        {
            using (CacheContextScope scope = RepositoryHelper.InitializeRepository(-1))
            {
                var repo = new DocumentManager();
                var response = repo.SetDocumentCreateHref();
                Assert.IsTrue(response);
            }
        }
    }
}
