﻿
/* Copyright (c) 2019-2020 Angourisoft
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy,
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
 * is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using AngouriMath.Core;
using AngouriMath.Core.Exceptions;

namespace AngouriMath
{
    partial record Entity
    {
        /// <summary>
        /// This node represents all possible values a boolean node might be of
        /// </summary>
        public sealed partial record Boolean(bool Value) : Statement
        {
            public static readonly Boolean True = new Boolean(true);
            public static readonly Boolean False = new Boolean(false);
            public static implicit operator bool(Boolean b) => b.Value;
            public static Boolean Create(bool value) => value ? True : False; // to avoid reallocation

            public override Entity Replace(Func<Entity, Entity> func) 
                => func(this);
            public override Priority Priority => Priority.Leaf;
            protected override Entity[] InitDirectChildren() => Array.Empty<Entity>();

            /// <summary>
            /// Use this when parsing one boolean value
            /// </summary>
            /// <param name="expr">A string to parse from</param>
            /// <param name="dst">Where to store the result</param>
            /// <returns>
            /// true if the parsing completed successfully, 
            /// false otherwise
            /// </returns>
            public static bool TryParse(string expr, out Boolean dst)
            {
                switch (expr)
                {
                    case "false":
                        dst = False;
                        return true;
                    case "true":
                        dst = True;
                        return true;
                }
                dst = False;
                return false;
            }

            /// <summary>
            /// Unlike <see cref="TryParse"/> this will throw a
            /// <see cref="ParseException"/> if parsing is not successful
            /// </summary>
            public static Boolean Parse(string expr)
                => TryParse(expr, out var res) ? res : throw new ParseException($"Token '{expr}' is not valid for boolean");
        }

        #region Logical gates

        /// <summary>
        /// Whatever its argument is, the result will be inverted
        /// </summary>
        public sealed partial record Notf(Entity Argument) : Statement
        {
            public override Priority Priority => Priority.Negation;
            private Notf New(Entity negated) =>
                ReferenceEquals(Argument, negated) ? this : new(negated);
            public override Entity Replace(Func<Entity, Entity> func) => func(New(Argument.Replace(func)));
            protected override Entity[] InitDirectChildren() => new[] { Argument };
        }

        /// <summary>
        /// Is true iff both operands are true
        /// </summary>
        public sealed partial record Andf(Entity Left, Entity Right) : Statement
        {
            public override Priority Priority => Priority.Conjunction;
            private Andf New(Entity left, Entity right) =>
                ReferenceEquals(Left, left) && ReferenceEquals(Right, right) ? this : new(left, right);
            public override Entity Replace(Func<Entity, Entity> func)
                => func(New(Left.Replace(func), Right.Replace(func)));
            protected override Entity[] InitDirectChildren() => new[] { Left, Right };
        }

        /// <summary>
        /// Is true iff at least one operand is true,
        /// </summary>
        public sealed partial record Orf(Entity Left, Entity Right) : Statement
        {
            public override Priority Priority => Priority.Disjunction;
            private Orf New(Entity left, Entity right) =>
                ReferenceEquals(Left, left) && ReferenceEquals(Right, right) ? this : new(left, right);
            public override Entity Replace(Func<Entity, Entity> func)
                => func(New(Left.Replace(func), Right.Replace(func)));
            protected override Entity[] InitDirectChildren() => new[] { Left, Right };
        }

        /// <summary>
        /// Is true iff one operand is true
        /// </summary>
        public sealed partial record Xorf(Entity Left, Entity Right) : Statement
        {
            public override Priority Priority => Priority.XDisjunction;
            private Xorf New(Entity left, Entity right) =>
                ReferenceEquals(Left, left) && ReferenceEquals(Right, right) ? this : new(left, right);
            public override Entity Replace(Func<Entity, Entity> func)
                => func(New(Left.Replace(func), Right.Replace(func)));
            protected override Entity[] InitDirectChildren() => new[] { Left, Right };
        }

        /// <summary>
        /// Is true iff assumption is false or conclusion is true
        /// </summary>
        public sealed partial record Impliesf(Entity Assumption, Entity Conclusion) : Statement
        {
            public override Priority Priority => Priority.Impliciation;
            private Impliesf New(Entity left, Entity right) =>
                ReferenceEquals(Assumption, left) && ReferenceEquals(Conclusion, right) ? this : new(left, right);
            public override Entity Replace(Func<Entity, Entity> func)
                => func(New(Assumption.Replace(func), Conclusion.Replace(func)));
            protected override Entity[] InitDirectChildren() => new[] { Assumption, Conclusion };
        }

        #endregion

        #region Equality/inequality operators

        /// <summary>
        /// It is true if left and right are equal
        /// </summary>
        public sealed partial record Equalsf(Entity Left, Entity Right) : ComparisonSign
        {
            public override Priority Priority => Priority.Equal;
            public Equalsf New(Entity left, Entity right)
                => ReferenceEquals(Left, left) && ReferenceEquals(Right, right) ? this : new Equalsf(left, right);
            public override Entity Replace(Func<Entity, Entity> func)
                => func(New(Left.Replace(func), Right.Replace(func)));
            protected override Entity[] InitDirectChildren() => new[] { Left, Right };
        }

        /// <summary>
        /// It is true iff both parts are numeric and real, and left number is greater
        /// than the right one
        /// It is false iff both parts are numeric and real, and left number is less or equal 
        /// the right one
        /// It is NaN/unsimplified otherwise.
        /// </summary>
        public sealed partial record Greaterf(Entity Left, Entity Right) : ComparisonSign
        {
            public override Priority Priority => Priority.GreaterThan;
            public Greaterf New(Entity left, Entity right)
                => ReferenceEquals(Left, left) && ReferenceEquals(Right, right) ? this : new(left, right);
            public override Entity Replace(Func<Entity, Entity> func)
                => func(New(Left.Replace(func), Right.Replace(func)));
            protected override Entity[] InitDirectChildren() => new[] { Left, Right };
        }

        /// <summary>
        /// It is true iff both parts are numeric and real, and left number is greater
        /// than the right one or equal to it
        /// It is false iff both parts are numeric and real, and left number is less 
        /// the right one
        /// It is NaN/unsimplified otherwise.
        /// </summary>
        public sealed partial record GreaterOrEqualf(Entity Left, Entity Right) : ComparisonSign
        {
            public override Priority Priority => Priority.GreaterThan;
            public GreaterOrEqualf New(Entity left, Entity right)
                => ReferenceEquals(Left, left) && ReferenceEquals(Right, right) ? this : new(left, right);
            public override Entity Replace(Func<Entity, Entity> func)
                => func(New(Left.Replace(func), Right.Replace(func)));
            protected override Entity[] InitDirectChildren() => new[] { Left, Right };
        }

        /// <summary>
        /// It is true iff both parts are numeric and real, and left number is less
        /// than the right one
        /// It is false iff both parts are numeric and real, and left number is greater or equal 
        /// the right one
        /// It is NaN/unsimplified otherwise.
        /// </summary>
        public sealed partial record Lessf(Entity Left, Entity Right) : ComparisonSign
        {
            public override Priority Priority => Priority.GreaterThan;
            public Lessf New(Entity left, Entity right)
                => ReferenceEquals(Left, left) && ReferenceEquals(Right, right) ? this : new(left, right);
            public override Entity Replace(Func<Entity, Entity> func)
                => func(New(Left.Replace(func), Right.Replace(func)));
            protected override Entity[] InitDirectChildren() => new[] { Left, Right };
        }

        /// <summary>
        /// It is true iff both parts are numeric and real, and left number is less
        /// than the right one or equal to it
        /// It is false iff both parts are numeric and real, and left number is greater
        /// the right one
        /// It is NaN/unsimplified otherwise.
        /// </summary>
        public sealed partial record LessOrEqualf(Entity Left, Entity Right) : ComparisonSign
        {
            public override Priority Priority => Priority.GreaterThan;
            public LessOrEqualf New(Entity left, Entity right)
                => ReferenceEquals(Left, left) && ReferenceEquals(Right, right) ? this : new(left, right);
            public override Entity Replace(Func<Entity, Entity> func)
                => func(New(Left.Replace(func), Right.Replace(func)));
            protected override Entity[] InitDirectChildren() => new[] { Left, Right };
        }

        #endregion

        #region Set statements
        partial record Set
        {
            /// <summary>
            /// This node represents whether the given element is in the set
            /// </summary>
            public sealed partial record Inf(Entity Element, Entity SupSet) : Statement
            {
                public override Priority Priority => Priority.ContainsIn;
                public Inf New(Entity element, Entity supSet)
                    => ReferenceEquals(Element, element) && ReferenceEquals(SupSet, supSet) ? this : new(Element, SupSet);
                public override Entity Replace(Func<Entity, Entity> func)
                    => func(New(Element.Replace(func), SupSet.Replace(func)));
                protected override Entity[] InitDirectChildren() => new[] { Element, SupSet };
            }
        }
        #endregion
    }
}
