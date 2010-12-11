// 
//  HackyMem.cs
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
    public class HackyMem : MemoryDevice
    {
        public HackyMem()
        {
            notcon = new StaticMem();
            rom = new HackyRom(32*1024);
            ram = new HackyRam(16*1024);
            dr = new HackyDebugRegisters();
            
            kb = new HackyKb();
            tty = new HackyTty();
            bitop = new HackyBitop();
        }
        
        #region "Public members"
        public void WriteWord(ushort address, ushort word)
        {
#if DEBUG
            if( TraceAccess )
                Console.Error.WriteLine("> {0:x4} = {1:x4}", address, word);
#endif
            var dev = deviceFromAddress(address, out address);
            dev.WriteWord(address, word);
        }
        
        public ushort ReadWord(ushort address)
        {
            var dev = deviceFromAddress(address, out address);
            var word = dev.ReadWord(address);
#if DEBUG
            if( TraceAccess )
                Console.Error.WriteLine("< {0:x4} = {1:x4}", address, word);
#endif
            return word;
        }
        
        public void LoadRomImage(string path)
        {
            rom.LoadImage(path);
        }
        
        public HackyDebugRegisters DebugRegisters { get { return dr; } }
        
        public bool TraceAccess { get; set; }
        #endregion
        
        #region "Private members"
        StaticMem   notcon;
        HackyRom    rom;
        HackyRam    ram;
        HackyDebugRegisters dr;
        
        HackyKb     kb;
        HackyTty    tty;
        HackyBitop  bitop;
        
        private MemoryDevice deviceFromAddress(ushort address, out ushort offset)
        {
            if( address < 0x4000 )
            {
                offset = (ushort)(address);
                return rom;
            }
            else if( 0x6000 <= address && address < 0x8000 )
            {
                offset = (ushort)(address - 0x6000);
                return ram;
            }
            else if( 0x4000 <= address && address < 0x4010 )
            {
                offset = (ushort)(address - 0x4000);
                return dr;
            }
            else if( 0x4010 <= address && address < 0x4020 )
            {
                offset = (ushort)(address - 0x4010);
                return kb;
            }
            else if( 0x4020 <= address && address < 0x4030 )
            {
                offset = (ushort)(address - 0x4020);
                return tty;
            }
            else if( 0x4030 <= address && address < 0x4040 )
            {
                offset = (ushort)(address - 0x4030);
                return bitop;
            }
            else
            {
                offset = 0;
                return notcon;
            }
        }
        #endregion
    }
}

