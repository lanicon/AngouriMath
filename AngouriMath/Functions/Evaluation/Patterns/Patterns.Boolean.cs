﻿/*
 * Copyright (c) 2019-2020 Angourisoft
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using System.Linq;
using static AngouriMath.Entity;
using static AngouriMath.Entity.Number;
using static AngouriMath.Entity.Boolean;
using System.Collections.Generic;
using static AngouriMath.Entity.Set;
using AngouriMath.Core;

namespace AngouriMath.Functions
{
    internal static partial class Patterns
    {
        private static bool IsLogic(Entity a)
            => a is Statement or Variable;

        private static bool IsLogic(Entity a, Entity b)
            => IsLogic(a) && IsLogic(b);

        private static bool IsLogic(Entity a, Entity b, Entity c)
            => IsLogic(a, b) && IsLogic(c);

        internal static Entity BooleanRules(Entity x) => x switch
        {
            Impliesf(var ass, _) when ass == False && IsLogic(ass) => True,
            Andf(Notf(var any1), Notf(var any2)) when IsLogic(any1, any2) => !(any1 | any2),
            Orf(Notf(var any1), Notf(var any2)) when IsLogic(any1, any2) => !(any1 & any2),
            Orf(Notf(var any1), var any1a) when any1 == any1a && IsLogic(any1) => True,
            Orf(Notf(var any1), var any2) when IsLogic(any1, any2) => any1.Implies(any2),
            Andf(var any1, var any1a) when any1 == any1a && IsLogic(any1) => any1,
            Orf(var any1, var any1a) when any1 == any1a && IsLogic(any1) => any1,
            Impliesf(var any1, var any1a) when any1 == any1a && IsLogic(any1) => True,
            Xorf(var any1, var any1a) when any1 == any1a && IsLogic(any1) => False,
            Notf(Notf(var any1)) when IsLogic(any1) => any1,

            Orf(var any1, var any2) when (any1 == True || any2 == True) && IsLogic(any1, any2) => True,
            Andf(var any1, var any2) when (any1 == False || any2 == False) && IsLogic(any1, any2) => False,

            Orf(Andf(var any1, var any2), Andf(var any1a, var any3)) when any1 == any1a && IsLogic(any1, any2, any3) => any1 & (any2 | any3),
            Andf(Orf(var any1, var any2), Orf(var any1a, var any3)) when any1 == any1a && IsLogic(any1, any2, any3) => any1 | (any2 & any3),
            Orf(Andf(var any1, var any2), Andf(var any3, var any1a)) when any1 == any1a && IsLogic(any1, any2, any3) => any1 & (any2 | any3),
            Andf(Orf(var any1, var any2), Orf(var any3, var any1a)) when any1 == any1a && IsLogic(any1, any2, any3) => any1 | (any2 & any3),
            Orf(Andf(var any2, var any1), Andf(var any1a, var any3)) when any1 == any1a && IsLogic(any1, any2, any3) => any1 & (any2 | any3),
            Andf(Orf(var any2, var any1), Orf(var any1a, var any3)) when any1 == any1a && IsLogic(any1, any2, any3) => any1 | (any2 & any3),
            Orf(Andf(var any2, var any1), Andf(var any3, var any1a)) when any1 == any1a && IsLogic(any1, any2, any3) => any1 & (any2 | any3),
            Andf(Orf(var any2, var any1), Orf(var any3, var any1a)) when any1 == any1a && IsLogic(any1, any2, any3) => any1 | (any2 & any3),

            Orf(var any1, Andf(var any1a, _)) when any1 == any1a && IsLogic(any1) => any1,
            Andf(var any1, Orf(var any1a, _)) when any1 == any1a && IsLogic(any1) => any1,

            Orf(var any1, Andf(Notf(var any1a), var any2)) when any1 == any1a && IsLogic(any1) => any1 | any2,
            Andf(var any1, Orf(Notf(var any1a), var any2)) when any1 == any1a && IsLogic(any1) => any1 & any2,
            Orf(var any1, Andf(var any1a, Notf(var any2))) when any1 == any1a && IsLogic(any1) => any1 | any2,
            Andf(var any1, Orf(var any1a, Notf(var any2))) when any1 == any1a && IsLogic(any1) => any1 & any2,
            Orf(Andf(Notf(var any1a), var any2), var any1) when any1 == any1a && IsLogic(any1) => any1 | any2,
            Andf(Orf(Notf(var any1a), var any2), var any1) when any1 == any1a && IsLogic(any1) => any1 & any2,
            Orf(Andf(var any1a, Notf(var any2)), var any1) when any1 == any1a && IsLogic(any1) => any1 | any2,
            Andf(Orf(var any1a, Notf(var any2)), var any1) when any1 == any1a && IsLogic(any1) => any1 & any2,

            Impliesf(Notf(var any1), Notf(var any2)) when IsLogic(any1, any2) => any2.Implies(any1),

            _ => x
        };
    }
}
