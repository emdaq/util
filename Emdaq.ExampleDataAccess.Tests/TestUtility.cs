using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Emdaq.ExampleDataAccess.Tests
{
    public class TestUtility
    {
        #region singleton

        private static readonly Lazy<TestUtility> Singleton = new Lazy<TestUtility>(() => new TestUtility());
        public static TestUtility I { get { return Singleton.Value; } }

        #endregion

        public void AssertEqual<T>(T o1, T o2, bool lenientDates = true) where T : class
        {
            if (o1 == null && o2 == null)
            {
                return;
            }

            Assert.IsNotNull(o1);
            Assert.IsNotNull(o2);

            foreach (var prop in typeof(T).GetProperties().Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof(string)))
            {
                var o1Val = prop.GetValue(o1, null);
                var o2Val = prop.GetValue(o2, null);

                if (lenientDates && prop.PropertyType == typeof(DateTime))
                {
                    // mysql rounds to the nearest second, so let's just make sure dates are within 3 seconds of each other
                    var diff = ((DateTime)o1Val).Subtract((DateTime)o2Val);
                    Assert.IsTrue(Math.Abs(diff.TotalSeconds) < 3);
                }
                else
                {
                    Assert.IsTrue(Equals(o1Val, o2Val));
                }
            }
        }

        public void AsserListEquivalent<T>(IEnumerable<T> l1, IEnumerable<T> l2, Func<T, T, bool> equality = null)
        {
            if (equality == null)
            {
                CollectionAssert.AreEquivalent(l1, l2);
            }
            else
            {
                if (l1 == null && l2 == null)
                {
                    return;
                }

                Assert.IsNotNull(l1);
                Assert.IsNotNull(l2);

                Assert.That(l1, new CollectionEquivalentConstraint(l2).Using<T>((x, y) => equality(x, y) ? 0 : 1));
            }
        }
    }
}
