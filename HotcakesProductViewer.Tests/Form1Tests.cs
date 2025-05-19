using Microsoft.VisualStudio.TestTools.UnitTesting;
using HotcakesProductViewer;
using System.Reflection;
using System.Data;

namespace HotcakesProductViewer.Tests
{
    [TestClass]
    public class Form1Tests
    {
        private Form1 CreateForm1WithPrivateAccess()
        {
            var form = new Form1();
            return form;
        }

        [TestMethod]
        public void LoadConnectionString_Should_SetConnectionString()
        {
            var form = CreateForm1WithPrivateAccess();

            var method = typeof(Form1).GetMethod("LoadConnectionString", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(form, null);

            var connStr = typeof(Form1).GetField("_connectionString", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(form);

            Assert.IsFalse(string.IsNullOrEmpty((string)connStr));
        }

        [TestMethod]
        public void GetPropertyId_Should_ReturnMinusOne_WhenConnectionStringIsNull()
        {
            var form = CreateForm1WithPrivateAccess();

            typeof(Form1).GetField("_connectionString", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(form, null);

            var method = typeof(Form1).GetMethod("GetPropertyId", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = method.Invoke(form, new object[] { "NonExistingProperty" });

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void InitializeTagsDataGridView_Should_InitializeDataTable()
        {
            var form = CreateForm1WithPrivateAccess();

            var method = typeof(Form1).GetMethod("InitializeTagsDataGridView", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(form, null);

            var dataTable = typeof(Form1).GetField("_tagsDataTable", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(form) as DataTable;

            Assert.IsNotNull(dataTable);
        }

        [TestMethod]
        public void ClearAndDisableTagEditor_Should_RunWithoutError()
        {
            var form = CreateForm1WithPrivateAccess();

            var method = typeof(Form1).GetMethod("ClearAndDisableTagEditor", BindingFlags.NonPublic | BindingFlags.Instance);

            method.Invoke(form, null);

            // Ha ide eljut, akkor nincs hiba
            Assert.IsTrue(true);
        }

        // 💥 ÚJ TESZT, IDE ILL. BE:
        [TestMethod]
        public void GetPropertyId_Should_ReturnMinusOne_WhenPropertyNotFound()
        {
            var form = CreateForm1WithPrivateAccess();
            var method = typeof(Form1).GetMethod("GetPropertyId", BindingFlags.NonPublic | BindingFlags.Instance);

            var result = method.Invoke(form, new object[] { "NonExistingProperty" });

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void GetPropertyId_Should_ReturnMinusOne_WhenPropertyNameIsNull()
        {
            var form = CreateForm1WithPrivateAccess();
            var method = typeof(Form1).GetMethod("GetPropertyId", BindingFlags.NonPublic | BindingFlags.Instance);

            var result = method.Invoke(form, new object[] { null });

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void InitializeTagsDataGridView_Should_HandleRepeatedCalls()
        {
            var form = CreateForm1WithPrivateAccess();
            var method = typeof(Form1).GetMethod("InitializeTagsDataGridView", BindingFlags.NonPublic | BindingFlags.Instance);

            // Első hívás
            method.Invoke(form, null);

            // Második hívás (ismétlés tesztelése)
            method.Invoke(form, null);

            var dataTable = typeof(Form1).GetField("_tagsDataTable", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(form) as DataTable;

            Assert.IsNotNull(dataTable);
        }

        [TestMethod]
        public void ClearAndDisableTagEditor_Should_NotThrow_WhenCalledMultipleTimes()
        {
            var form = CreateForm1WithPrivateAccess();
            var method = typeof(Form1).GetMethod("ClearAndDisableTagEditor", BindingFlags.NonPublic | BindingFlags.Instance);

            // Első hívás
            method.Invoke(form, null);

            // Második hívás (ismétlés tesztelése)
            method.Invoke(form, null);

            Assert.IsTrue(true); // Ha ide eljut, nem volt kivétel
        }

    }
}
