// 
//  HackyKb.cs
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
    public class HackyKb : MemoryDevice
    {
        public HackyKb()
        {
        }
        
        public bool Available()
        {
            return Console.KeyAvailable;
        }
        
        public ushort ReadAscii()
        {
            if( !Available() )
                return 0;
            
            var ki = Console.ReadKey();
            var c = ki.KeyChar;
            
#if DEBUG
            /*
            switch( c )
            {
                case '\r':
                    //Console.Error.Write("<CR>");
                    DebugGlobals.DumpNextTtyWrite = true;
                    break;
                case '\n':
                    //Console.Error.Write("<LF>");
                    DebugGlobals.DumpNextTtyWrite = true;
                    break;
            }
            // */
#endif
            
            return (ushort) c;
        }
        
        public void Clear()
        {
            while( Console.KeyAvailable )
                Console.Read();
        }
        
        public void WriteWord(ushort address, ushort word)
        {
            switch( address & 0x3 )
            {
                case 3:
                    Clear();
                    break;
            }
        }
        
        public ushort ReadWord(ushort address)
        {
            switch( address & 0x3 )
            {
                case 0:
                    return 1;
                    
                case 1:
                    return Available() ? (ushort)1 : (ushort)0;
                    
                case 2:
                    return ReadAscii();
                    
                default:
                    return 0;
            }
        }
    }
}