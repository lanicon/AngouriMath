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

using AngouriMath.Functions;
using System;
using static AngouriMath.Entity.Boolean;
using AngouriMath.Core;

namespace AngouriMath
{
    public abstract partial record Entity
    {
        partial record Boolean : BooleanNode
        {
            protected override Entity InnerEval() => this;
            internal override Entity InnerSimplify() => this;
        }

        partial record Notf : BooleanNode
        {
            protected override Entity InnerEval()
            {
                if (Evaled is Boolean)
                    return Evaled;
                if (Argument is Boolean b)
                    return !(bool)b; // there's no cost in casting
                return this;
            }
            internal override Entity InnerSimplify() => InnerEval();
        }

        partial record Andf : BooleanNode
        {
            protected override Entity InnerEval()
            {
                if (Evaled is Boolean)
                    return Evaled;
                if (Left is Boolean left && Right is Boolean right)
                    return (bool)left && (bool)right; // there's no cost in casting
                return this;
            }
            internal override Entity InnerSimplify() => InnerEval();
        }

        partial record Orf : BooleanNode
        {
            protected override Entity InnerEval()
            {
                if (Evaled is Boolean)
                    return Evaled;
                if (Left is Boolean left && Right is Boolean right)
                    return (bool)left || (bool)right; // there's no cost in casting
                return this;
            }
            internal override Entity InnerSimplify() => InnerEval();
        }

        partial record Xorf : BooleanNode
        {
            protected override Entity InnerEval()
            {
                if (Evaled is Boolean)
                    return Evaled;
                if (Left is Boolean left && Right is Boolean right)
                    return (bool)left ^ (bool)right; // there's no cost in casting
                return this;
            }
            internal override Entity InnerSimplify() => InnerEval();
        }

        partial record Impliesf : BooleanNode
        {
            protected override Entity InnerEval()
            {
                if (Evaled is Boolean)
                    return Evaled;
                if (Assumption is Boolean ass && Conclusion is Boolean conclusion)
                    return !(bool)ass || (bool)conclusion; // there's no cost in casting
                return this;
            }
            internal override Entity InnerSimplify() => InnerEval();
        }
    }
}
