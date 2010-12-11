// 
//  HackyMb.cs
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
    public class HackyMb
    {
        public HackyMb()
        {
            cpu = new HackyCpu();
            mem = new HackyMem();
        }
        
        #region "Public members"
        public void Reset()
        {
            cpu.SetResetFlag(true);
            cpu.Clock();
            cpu.SetResetFlag(false);
            decode = true;
            preClock = true;
        }
        
        public void MacroClock()
        {
            for( var i=0; i<4; ++i )
                Clock();
        }
        
        public void Clock()
        {
            // Load & decode next instruction
            cpu.SetDecodeFlag(decode);
            if( preClock )
            {
                cpu.PreClock();
                {
                    var addr = cpu.GetMemoryAddress();
                    var memLoad = cpu.GetMemoryLoad();
                    
                    if( memLoad ) cpu.SetMemoryIn(mem.ReadWord(addr));
                }
            }
            else
            {
                cpu.Clock();
                {
                    var addr = cpu.GetMemoryAddress();
                    var memOut = cpu.GetMemoryOut();
                    var memSet = cpu.GetMemorySet();
                    
                    if( memSet ) mem.WriteWord(addr, memOut);
                }
                
                decode = !decode;
            }
            
            preClock = !preClock;
        }
        
        public void LoadRomImage(string path)
        {
            mem.LoadRomImage(path);
        }
        
        public HackyCpu Cpu { get { return cpu; } }
        public HackyMem Memory { get { return mem; } }
        #endregion
        
        #region "Private members"
        HackyCpu cpu;
        HackyMem mem;
        
        bool decode = true;
        bool preClock = true;
        #endregion
    }
}

