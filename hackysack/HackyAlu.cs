// 
//  HackyAlu.cs
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
    /// <summary>
    /// Implements the HACKY ALU in software. 
    /// </summary>
    public class HackyAlu
    {
        public HackyAlu()
        {
        }
        
        #region "Public members"
        public void SetLeftInput(ushort value)
        {
            this.a = value;
        }
        
        public void SetRightInput(ushort value)
        {
            this.b = value;
        }
        
        public void SetFlags(byte flags)
        {
            SetFlags((flags & 0x20) != 0,
                     (flags & 0x10) != 0,
                     (flags & 0x08) != 0,
                     (flags & 0x04) != 0,
                     (flags & 0x02) != 0,
                     (flags & 0x01) != 0);
        }
        
        public void SetFlags(bool za, bool na, bool zb, bool nb, bool fn, bool no)
        {
            this.za = za;
            this.na = na;
            this.zb = zb;
            this.nb = nb;
            this.fn = fn;
            this.no = no;
        }
        
        public void Evaluate()
        {
            ushort a = za ? (ushort)0 : this.a;
            ushort b = zb ? (ushort)0 : this.b;
            
            if( na ) a = (ushort) ~a;
            if( nb ) b = (ushort) ~b;
            if( fn )
            {
                o = (ushort)(a+b);
                ov = (a+b) > ushort.MaxValue;
            }
            else
            {
                o = (ushort)(a&b);
                ov = false;
            }
            if( no ) o = (ushort) ~o;
            
            leg = ((o < 0)  ? CmpMask.Lt : 0)
                | ((o == 0) ? CmpMask.Eq : 0)
                | ((o > 0)  ? CmpMask.Gt : 0);
            
#if DEBUG
            if( TraceEvaluation )
                Console.Error.WriteLine(String.Format("= {1:x4} {0} {2:x4} = {3:x4}",
                                                      fn ? "+" : "&", a, b, o));
#endif
        }
        
        public ushort GetResult()
        {
            return this.o;
        }
        
        public bool GetOverflow()
        {
            return this.ov;
        }
        
        public CmpMask GetCmpMask()
        {
            return this.leg;
        }
        
        public bool TraceEvaluation { get; set; }
        #endregion
        
        #region "Private members"
        private ushort a, b, o;
        private bool za, na, zb, nb, fn, no;
        private bool ov;
        private CmpMask leg;
        #endregion
    }
}