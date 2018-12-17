namespace ОдномерныйСлучайВетвления.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PolynomialTest
    {
        [TestMethod]
        public void RootsTest()
        {
            var test = new Polynomial(new double[] { 1, 0, 1 });
            test.RootsWithSturm();
            Assert.Fail();
        }

        [TestMethod]
        public void RootsTest1()
        {
            var test = new Polynomial(new double[] { 1, 2, 1 });
            test.RootsWithSturm();
            Assert.Fail();
        }
    }
}