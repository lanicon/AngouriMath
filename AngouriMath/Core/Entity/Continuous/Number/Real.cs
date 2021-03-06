/*
 * Copyright (c) 2019-2020 Angourisoft
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using PeterO.Numbers;
//[assembly: InternalsVisibleTo("DotnetBenchmark")]

namespace AngouriMath
{
    using Core;
    using System.Text;

    partial record Entity
    {
        public abstract partial record Number
        {
            /// <summary>
            /// Represents a real number, such complex
            /// that its imaginary part equals 0
            /// </summary>
            public record Real : Complex, System.IComparable<Real>
            {
                /// <summary>
                /// Constructor does not downcast automatically. Use <see cref="Create(EDecimal)"/> for automatic downcasting.
                /// </summary>
                private protected Real(EDecimal @decimal) : base(null, null) => EDecimal = @decimal;

                /// <summary>
                /// The PeterO number representation in decimal
                /// </summary>
                public EDecimal EDecimal { get; }

                /// <summary>
                /// Deconstructs as record
                /// </summary>
                public void Deconstruct(out EDecimal @decimal) => @decimal = EDecimal;

                /// <inheritdoc/>
                public override Real RealPart => this;
                internal override Priority Priority => EDecimal.IsNegative ? Priority.Mul : Priority.Leaf;

                /// <inheritdoc/>
                public override bool IsExact => !EDecimal.IsFinite;

                /// <summary>Strictly less than 0</summary>
                public bool IsNegative => EDecimal.IsNegative;

                /// <summary>Strictly greater than 0</summary>
                public bool IsPositive => !EDecimal.IsNegative && !EDecimal.IsZero;

                /// <summary>
                /// Creates an instance of Real
                /// (one can do it by implicit conversation)
                /// </summary>
                public static Real Create(EDecimal value)
                {
                    if (!MathS.Settings.DowncastingEnabled)
                        return new Real(value);

                    if (!value.IsFinite)
                        return new Real(value);
                    var (intPart, intRest) = value.SplitDecimal();
                    // If the difference between value & round(value) is zero (see Number.IsZero), we consider value as an integer
                    if (intRest.LessThan(MathS.Settings.PrecisionErrorZeroRange))
                        return Integer.Create(intPart);
                    if (intRest.GreaterThan(1 - MathS.Settings.PrecisionErrorZeroRange.Value))
                        return Integer.Create(intPart.Increment());

                    var attempt = Rational.FindRational(value);
                    if (attempt is null ||
                        attempt.ERational.Numerator.Abs() > MathS.Settings.MaxAbsNumeratorOrDenominatorValue ||
                        attempt.ERational.Denominator.Abs() > MathS.Settings.MaxAbsNumeratorOrDenominatorValue)
                        return new Real(value);
                    else
                        return attempt;
                }

                /// <inheritdoc/>
                public override Real Abs() => Create(EDecimal.Abs());

                /// <inheritdoc/>
                public override string Stringize() => this switch
                {
                    { IsFinite: true } => EDecimal.ToString(),
                    { IsNaN: true } => "NaN",
                    { IsNegative: true } => "-oo",
                    _ => "+oo",
                };

                /// <inheritdoc/>
                public override string Latexise() => this switch
                {
                    { IsFinite: true } => EDecimal.ToString(),
                    { IsNaN: true } => @"\mathrm{undefined}",
                    { IsNegative: true } => @"-\infty ",
                    _ => @"\infty ",
                };

                internal static bool TryParse(string s,
                    [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Real? dst)
                {
                    try
                    {
                        dst = EDecimal.FromString(s);
                        return true;
                    }
                    catch
                    {
                        dst = null;
                        return false;
                    }
                }

                /// <summary>Negative Infinity (-oo)</summary>
                public static readonly Real NegativeInfinity = new Real(EDecimal.NegativeInfinity);

                /// <summary>Positive Infinity (+oo)</summary>
                public static readonly Real PositiveInfinity = new Real(EDecimal.PositiveInfinity);

                /// <summary>Not A Number (NaN)</summary>
                public static readonly Real NaN = new Real(EDecimal.NaN);

                /// <summary>
                /// Converts the given number to a double (not recommended in general unless you need a built-in type)
                /// </summary>
                public double AsDouble() => EDecimal.ToDouble();

#pragma warning disable CS1591
                public static bool operator >(Real a, Real b) => a.EDecimal.GreaterThan(b.EDecimal);
                public static bool operator >=(Real a, Real b) => a.EDecimal.GreaterThanOrEquals(b.EDecimal);
                public static bool operator <(Real a, Real b) => a.EDecimal.LessThan(b.EDecimal);
                public static bool operator <=(Real a, Real b) => a.EDecimal.LessThanOrEquals(b.EDecimal);
                public int CompareTo(Real other) => EDecimal.CompareTo(other.EDecimal);
                public static Real operator +(Real a, Real b) => OpSum(a, b);
                public static Real operator -(Real a, Real b) => OpSub(a, b);
                public static Real operator *(Real a, Real b) => OpMul(a, b);
                public static Real operator /(Real a, Real b) => (Real)OpDiv(a, b);
                public static Real operator +(Real a) => a;
                public static Real operator -(Real a) => OpMul(Integer.MinusOne, a);
                public static implicit operator Real(sbyte value) => Integer.Create(value);
                public static implicit operator Real(byte value) => Integer.Create(value);
                public static implicit operator Real(short value) => Integer.Create(value);
                public static implicit operator Real(ushort value) => Integer.Create(value);
                public static implicit operator Real(int value) => Integer.Create(value);
                public static implicit operator Real(uint value) => Integer.Create(value);
                public static implicit operator Real(long value) => Integer.Create(value);
                public static implicit operator Real(ulong value) => Integer.Create(value);
                public static implicit operator Real(EInteger value) => Integer.Create(value);
                public static implicit operator Real(ERational value) => Rational.Create(value);
                public static implicit operator Real(EDecimal value) => Create(value);
                public static implicit operator Real(float value) => Create(EDecimal.FromSingle(value));
                public static implicit operator Real(double value) => Create(EDecimal.FromDouble(value));
                public static implicit operator Real(decimal value) => Create(EDecimal.FromDecimal(value));
#pragma warning restore CS1591

                /// <inheritdoc/>
                public override Domain Codomain { get; protected init; } = Domain.Real;

                /// <inheritdoc/>
                public override Entity Substitute(Entity x, Entity value)
                    => this == x ? value : this;

                /// <inheritdoc/>
                public override string ToString() => Stringize();
            }
        }
    }
}