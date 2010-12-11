// 
//  HackyRam.cs
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
    public class HackyRam : MemoryDevice
    {
        public HackyRam(int sizeInBytes)
        {
            if( (sizeInBytes & 1) == 1 )
                throw new ArgumentException("RAM size must be a multiple of two bytes", "sizeInBytes");
            data = new ushort[sizeInBytes/2];
        }
        
        #region "Public members"
        public void WriteWord(ushort address, ushort word)
        {
            data[address] = word;
        }
        
        public ushort ReadWord(ushort address)
        {
            return data[address];
        }
        #endregion
        
        #region "Private members"
        ushort[] data;
        #endregion
    }
}

