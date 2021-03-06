// 
//  CmpMask.cs
//  
//  Author:
//       Daniel Keep <daniel.keep@gmail.com>
//  
//  Copyright (c) 2010 Daniel Keep
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace Hacky.Sack
{
    public enum CmpMask
    {
        Lt = 4,
        Eq = 2,
        Gt = 1,
        Le = Lt | Eq,
        Ge = Gt | Eq,
        Ne = Lt | Gt,
        Always = Lt | Eq | Gt,
        Never = 0,
    }
}
