﻿/*
 * Copyright (c) 2019-2020 Angourisoft
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using PeterO.Numbers;
namespace AngouriMath
{
    using Core;
    using System.Text;

    partial record Entity
    {
        public abstract partial record Number
        {
            /// <summary>Use <see cref="Create(EInteger)"/> instead of the constructor for consistency with
            /// <see cref="Rational"/>, <see cref="Real"/> and <see cref="Complex"/>.</summary>
            public sealed record Integer : Rational, System.IComparable<Integer>
            {
                private Integer(EInteger value) : base(value) => EInteger = value;

                /// <summary>
                /// Represents PeterO number in EInteger
                /// </summary>
                public EInteger EInteger { get; }
                internal override Priority Priority => IsNegative ? Priority.Mul : Priority.Leaf;

                /// <summary>
                /// A zero, you can use it to avoid allocations
                /// </summary>
                public static readonly Integer Zero = new Integer(EInteger.Zero);

                /// <summary>
                /// A one, you can use it to avoid allocations
                /// </summary>
                public static readonly Integer One = new Integer(EInteger.One);

                /// <summary>
                /// A minus one, you can use it to avoid allocations
                /// </summary>
                public static readonly Integer MinusOne = new Integer(-EInteger.One);

                /// <summary>
                /// Creates an instance of Integer
                /// </summary>
                public static Integer Create(EInteger value) => new Integer(value);

                /// <summary>
                /// Deconstructs as record
                /// </summary>
                public void Deconstruct(out int? value) =>
                    value = EInteger.CanFitInInt32() ? EInteger.ToInt32Unchecked() : new int?();

                // TODO: When we target .NET 5, remember to use covariant return types
                /// <inheritdoc/>
                public override Real Abs() => Create(EInteger.Abs());

                /// <inheritdoc/>
                public override string Stringize() => EInteger.ToString();
                /// <inheritdoc/>
                public override string Latexise() => EInteger.ToString();
                internal static bool TryParse(string s,
                    [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Integer? dst)
                {
                    try
                    {
                        dst = EInteger.FromString(s);
                        return true;
                    }
                    catch
                    {
                        dst = null;
                        return false;
                    }
                }

#pragma warning disable CS1591
                public static bool operator >(Integer a, Integer b) => a.EInteger.CompareTo(b.EInteger) > 0;
                public static bool operator >=(Integer a, Integer b) => a.EInteger.CompareTo(b.EInteger) >= 0;
                public static bool operator <(Integer a, Integer b) => a.EInteger.CompareTo(b.EInteger) < 0;
                public static bool operator <=(Integer a, Integer b) => a.EInteger.CompareTo(b.EInteger) <= 0;
                public int CompareTo(Integer other) => EInteger.CompareTo(other.EInteger);
                public static Integer operator +(Integer a, Integer b) => OpSum(a, b);
                public static Integer operator -(Integer a, Integer b) => OpSub(a, b);
                public static Integer operator *(Integer a, Integer b) => OpMul(a, b);
                public static Real operator /(Integer a, Integer b) => (Real)OpDiv(a, b);
                public static Integer operator +(Integer a) => a;
                public static Integer operator -(Integer a) => OpMul(MinusOne, a);
                public static implicit operator Integer(sbyte value) => Create(value);
                public static implicit operator Integer(byte value) => Create(value);
                public static implicit operator Integer(short value) => Create(value);
                public static implicit operator Integer(ushort value) => Create(value);
                public static implicit operator Integer(int value) => Create(value);
                public static implicit operator Integer(uint value) => Create(value);
                public static implicit operator Integer(long value) => Create(value);
                public static implicit operator Integer(ulong value) => Create(value);
                public static implicit operator Integer(EInteger value) => Create(value);
#pragma warning restore CS1591

                /// <inheritdoc/>
                public override Domain Codomain { get; protected init; } = Domain.Integer;

                /// <inheritdoc/>
                public override Entity Substitute(Entity x, Entity value)
                    => this == x ? value : this;

                /// <inheritdoc/>
                public override string ToString() => Stringize();
            }
        }
    }
}