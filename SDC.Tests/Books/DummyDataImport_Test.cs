using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace SDC.Tests.Books
{
    [TestClass]
    public class DummyDataImport_Test
    {
        [TestMethod]
        public void Import()
        {
            SDC.Library.DummyDataImport.ImportUtility import = new Library.DummyDataImport.ImportUtility();
            import.LoadData();

            Task t = import.Import();
            t.RunSynchronously();
        }
    }
}
