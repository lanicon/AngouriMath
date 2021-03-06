﻿using AngouriMath;
using Xunit;

namespace UnitTests.Algebra
{
    public sealed class DerivativeTest
    {
        static readonly Entity.Variable x = MathS.Var(nameof(x));
        [Fact]
        public void Test1()
        {
            var func = MathS.Sqr(x) + 2 * x + 1;
            var derived = func.Differentiate(x);
            Assert.Equal(2 + 2 * x, derived.Simplify());
        }
        [Fact]
        public void TestSin()
        {
            var func = MathS.Sin(x);
            Assert.Equal(MathS.Cos(x), func.Differentiate(x).Simplify());
        }
        [Fact]
        public void TestCosCustom()
        {
            var func = MathS.Cos(MathS.Pow(x, 3));
            var expected = -3 * MathS.Sin(MathS.Pow(x, 3)) * MathS.Sqr(x);
            var actual = func.Differentiate(x).Simplify();
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void TestPow()
        {
            var func = MathS.Pow(MathS.e, x);
            Assert.Equal(func, func.Differentiate(x).Simplify());
        }
        [Fact]
        public void TestPoly()
        {
            var func = MathS.Pow(x, 4);
            Assert.Equal(4 * MathS.Pow(x, 3), func.Differentiate(x).Simplify());
        }
        [Fact]
        public void TestCusfunc()
        {
            var func = MathS.Sin(x).Pow(2);
            Assert.Equal(MathS.Sin(2 * x), func.Differentiate(x).Simplify(3));
        }
        [Fact]
        public void TestTan()
        {
            var func = MathS.Tan(2 * x);
            Assert.Equal(2 / MathS.Pow(MathS.Cos(2 * x), 2), func.Differentiate(x).Simplify());
        }
        [Fact]
        public void TestCoTan()
        {
            var func = MathS.Cotan(2 * x);
            Assert.Equal(-2 / MathS.Pow(MathS.Sin(2 * x), 2), func.Differentiate(x).Simplify());
        }
        [Fact]
        public void TestArc1()
        {
            var func = MathS.Arcsin(x);
            Assert.Equal(1 / MathS.Sqrt(1 - MathS.Sqr(x)), func.Differentiate(x).Simplify());
        }
        [Fact]
        public void TestArc2()
        {
            var func = MathS.Arcsin(2 * x);
            Assert.Equal(1 / MathS.Sqrt(1 - MathS.Sqr(2 * x)) * 2, func.Differentiate(x).Simplify());
        }
        [Fact]
        public void TestArc3()
        {
            var func = MathS.Arccos(2 * x);
            Assert.Equal((-1) / MathS.Sqrt(1 - MathS.Sqr(2 * x)) * 2, func.Differentiate(x).Simplify());
        }
        [Fact]
        public void TestArc4()
        {
            var func = MathS.Arctan(2 * x);
            Assert.Equal(2 / (1 + 4 * MathS.Sqr(x)), func.Differentiate(x).Simplify());
        }
        [Fact]
        public void TestArc5()
        {
            var func = MathS.Arccotan(2 * x);
            Assert.Equal(-2 / (1 + 4 * MathS.Sqr(x)), func.Differentiate(x).Simplify());
        }
        [Fact]
        public void TestNaN()
        {
            var func = MathS.Numbers.Create(double.NaN);
            Assert.Equal(MathS.Numbers.Create(double.NaN), func.Differentiate(x).Simplify());
        }
        [Fact]
        public void TestNaN2()
        {
            var func = MathS.Pow(21, MathS.Numbers.Create(double.NaN));
            Assert.Equal(MathS.Numbers.Create(double.NaN), func.Differentiate(x).Simplify());
        }

        [Fact]
        public void TestDerOverDer2()
        {
            var func = MathS.Derivative("x + 2", "y");
            var derFunc = func.Differentiate(x);
            Assert.Equal(MathS.Derivative(func, x), derFunc);
        }

        [Fact]
        public void TestSgnDer()
        {
            Entity func = "sgn(x + 2)";
            var derived = func.Differentiate("x");
            Assert.Equal(MathS.Derivative(func, x), derived);
        }

        [Fact]
        public void TestAbsDer()
        {
            Entity func = "abs(x + 2)";
            var derived = func.Differentiate("x");
            Assert.Equal(MathS.Signum("x + 2").Simplify(), derived.Simplify());
        }
    }
}
