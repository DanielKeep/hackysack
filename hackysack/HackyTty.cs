// 
//  HackyTty.cs
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
    public class HackyTty : MemoryDevice
    {
        public HackyTty()
        {
        }
        
        #region "Public members"
        public void WriteAscii(ushort word)
        {
#if DEBUG
            if( DebugGlobals.DumpNextTtyWrite )
            {
                DebugGlobals.DumpNextTtyWrite = false;
                Console.Error.Write(String.Format("[tty:{0:x4};{1:x4}]", word, (ushort)'\r'));
            }
#endif
            
            // We have to handle newlines specially for Windows.
            if( word != 0x0d )
                Console.Write((char)word);
            else
            {
                Console.Write((char)0x0d);
                Console.Write((char)0x0a);
            }
        }
        
        public void Clear()
        {
#if DEBUG
            Console.WriteLine("");
            Console.WriteLine("".PadLeft(Console.WindowWidth-1, '-'));
#else
            Console.Clear();
#endif
        }
        
        public void WriteWord(ushort address, ushort word)
        {
            switch( address & 0x3 )
            {
                case 1:
                    WriteAscii(word);
                    break;
                    
                case 2:
                    if( word != 0 ) Clear();
                    break;
            }
        }
        
        public ushort ReadWord(ushort address)
        {
            switch( address & 0x3 )
            {
                case 0:
                    return 1;
                    
                default:
                    return 0;
            }
        }
        #endregion
    }
}