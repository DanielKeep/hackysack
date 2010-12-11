// 
//  HackyBitop.cs
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
using System.Diagnostics;

namespace Hacky.Sack
{
    public class HackyBitop : MemoryDevice
    {
        public HackyBitop()
        {
        }
        
        #region "Public members"
        public void WriteWord(ushort address, ushort word)
        {
            switch( word & 0x3 )
            {
                case 0:
                    input = word;
                    break;
                    
                case 1:
                    shift = word;
                    break;
            }
        }
        
        public ushort ReadWord(ushort address)
        {
            switch( address & 0x3 )
            {
                case 0:
                    return input;
                    
                case 1:
                    return shift;
                    
                case 2:
                    return (ushort)(input << shift);
                    
                case 3:
                    return (ushort)(input >> shift);
                    
                default:
                    // This can't happen, you stupid compiler!
                    Debug.Assert(false, "OMGWTF?!");
                    return 0;
            }
        }
        #endregion
        
        #region "Private members"
        ushort input;
        ushort shift;
        #endregion
    }
}

