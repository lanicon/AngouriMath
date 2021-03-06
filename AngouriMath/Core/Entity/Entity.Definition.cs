/*
 * Copyright (c) 2019-2020 Angourisoft
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngouriMath.Core;
using AngouriMath.Core.Exceptions;
//[assembly: InternalsVisibleTo("Playground, PublicKey=")]

namespace AngouriMath.Core
{
#pragma warning disable CA1069 // Enums values should not be duplicated
    internal enum Priority
    { 
        BooleanOperation = 0x0000,

        Impliciation = 10 | BooleanOperation,
        Disjunction  = 30 | BooleanOperation,
        XDisjunction = 30 | BooleanOperation,
        Conjunction  = 50 | BooleanOperation,
        Negation     = 70 | BooleanOperation,

        EqualitySignsOperation = 0x1000,

        Equal          = 10 | EqualitySignsOperation,
        LessThan       = 20 | EqualitySignsOperation,
        GreaterThan    = 20 | EqualitySignsOperation,
        LessOrEqual    = 20 | EqualitySignsOperation,
        GreaterOrEqual = 20 | EqualitySignsOperation,

        SetOperation = 0x2000,

        ContainsIn   = 10 | SetOperation,
        SetMinus     = 20 | SetOperation,
        Union        = 30 | SetOperation,
        Intersection = 40 | SetOperation,

        NumericalOperation = 0x3000,

        Sum = 20       | NumericalOperation,
        Minus = 20     | NumericalOperation,
        Mul = 40       | NumericalOperation,

        Div = 40       | NumericalOperation,
        Pow = 60       | NumericalOperation,
        Factorial = 70 | NumericalOperation,
        Func = 80      | NumericalOperation,


        Leaf      = 100 | NumericalOperation,
    }
#pragma warning restore CA1069 // Enums values should not be duplicated

    /// <summary>
    /// Any class that supports converting to LaTeX format should implement this interface
    /// </summary>
    public interface ILatexiseable
    { 
        /// <summary>
        /// Converts the object to the LaTeX format
        /// That is, a string that can be later displayed and rendered as LaTeX
        /// </summary>
        public string Latexise(); 
    }
}

namespace AngouriMath
{

    /// <summary>
    /// This is the main class in AngouriMath.
    /// Every node, expression, or number is an <see cref="Entity"/>.
    /// However, you cannot create an instance of this class, look for the nested classes instead.
    /// </summary>
    public abstract partial record Entity : ILatexiseable
    {
        internal static RecordFieldCache caches = new();

        /// <inheritdoc/>
        protected abstract Entity[] InitDirectChildren();
        
        /// <summary>
        /// Represents all direct children of a node
        /// </summary>
        public IReadOnlyList<Entity> DirectChildren 
            => caches.GetValue(this, cache => cache.directChildren, cache => cache.directChildren = InitDirectChildren());

        /// <remarks>A depth-first enumeration is required by
        /// <see cref="AngouriMath.Functions.TreeAnalyzer.GetMinimumSubtree"/></remarks>
        public IEnumerable<Entity> Nodes => DirectChildren.SelectMany(c => c.Nodes).Prepend(this);

        /// <summary>
        /// Applies the given function to every node starting from the leaves
        /// </summary>
        /// <param name="func">
        /// The delegate that takes the current node as an argument and replaces the current node
        /// with the result of the delegate
        /// </param>
        /// <returns>Processed expression</returns>
        public abstract Entity Replace(Func<Entity, Entity> func);


        /// <summary>Replaces all <param name="x"/> with <param name="value"/></summary>
        public abstract Entity Substitute(Entity x, Entity value);


        // TODO: this function has no performance beneficial anymore, 
        // maybe need to think how it can be improved without defining
        // another virtual method?
        /// <summary>Replaces all <param name="replacements"/></summary>
        public Entity Substitute<TFrom, TTo>(IReadOnlyDictionary<TFrom, TTo> replacements) where TFrom : Entity where TTo : Entity
        {
            var res = this;
            foreach (var pair in replacements)
                res = res.Substitute(pair.Key, pair.Value);
            return res;
        }

        internal abstract Priority Priority { get; }

        /// <value>
        /// Whether both parts of the complex number are finite
        /// meaning that it could be safely used for calculations
        /// </value>
        public bool IsFinite
            => caches.GetValue(this, cache => cache.isFinite, cache => cache.isFinite =
            ThisIsFinite && DirectChildren.All(x => x.IsFinite)) ?? throw new AngouriBugException($"{IsFinite} cannot be null");

        /// <summary>
        /// Not NaN and not infinity
        /// </summary>
        protected virtual bool ThisIsFinite => true;       

        /// <value>Number of nodes in tree</value>
        // TODO: improve measurement of Entity complexity, for example
        // (1 / x ^ 2).Complexity() &lt; (x ^ (-0.5)).Complexity()
        public int Complexity 
            => caches.GetValue(this, 
            cache => cache.complexity,
            cache => cache.complexity = 1 + DirectChildren.Sum(x => x.Complexity)) ?? throw new AngouriBugException("Complexity cannot be null");

        /// <summary>
        /// Set of unique variables, for example 
        /// it extracts <c>`x`</c>, <c>`goose`</c> from <c>(x + 2 * goose) - pi * x</c>
        /// </summary>
        /// <returns>
        /// Set of unique variables excluding mathematical constants
        /// such as <see cref="MathS.pi"/> and <see cref="MathS.e"/>
        /// </returns>
        public IEnumerable<Variable> Vars => VarsAndConsts.Where(x => !x.IsConstant);
        
        /// <summary>
        /// Set of unique variables, for example 
        /// it extracts <c>`x`</c>, <c>`goose`</c> from <c>(x + 2 * goose) - pi * x</c>
        /// </summary>
        /// <returns>
        /// Set of unique variables and mathematical constants
        /// such as <see cref="MathS.pi"/> and <see cref="MathS.e"/>
        /// </returns>
        public IReadOnlyCollection<Variable> VarsAndConsts 
            => caches.GetValue(this, cache => cache.vars,
            cache => cache.vars = 
            new HashSet<Variable>(this is Variable v ? new[] { v } : DirectChildren.SelectMany(x => x.VarsAndConsts)));

        /// <summary>Checks if <paramref name="x"/> is a subnode inside this <see cref="Entity"/> tree.
        /// Optimized for <see cref="Variable"/>.</summary>
        public bool ContainsNode(Entity x) => x is Variable v ? VarsAndConsts.Contains(v) : Nodes.Contains(x);

        /// <summary>
        /// Implicit conversation from string to Entity
        /// </summary>
        /// <param name="expr">The source from which to parse</param>
        public static implicit operator Entity(string expr) => MathS.FromString(expr);

        /// <summary>
        /// Shows how simple the given expression is. The lower - the simpler the expression is.
        /// You might need it to pick the best expression to represent something. Unlike 
        /// <see cref="Complexity"/>, which shows the number of nodes, <see cref="SimplifiedRate"/> 
        /// shows how convenient it is to view the expression. This depends on 
        /// <see cref="MathS.Settings.ComplexityCriteria"/> which can be changed by user.
        /// </summary>
        public int SimplifiedRate => caches.GetValue(this, cache => cache.simplifiedRate, cache => cache.simplifiedRate = MathS.Settings.ComplexityCriteria.Value(this)) ?? throw new AngouriBugException("Sim cannot be null");
    }
}