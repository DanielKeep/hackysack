// 
//  HackyCpu.cs
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
    public class HackyCpu
    {
        public HackyCpu()
        {
            alu = new HackyAlu();
        }
        
        #region "Public members"
        public void SetDecodeFlag(bool decode)
        {
            this.decode = decode;
        }
        
        public void SetResetFlag(bool reset)
        {
            this.reset = reset;
        }
        
        public void PreClock()
        {
            clearOutputs();
            
            if( reset )
            {
            }
            else if( decode )
            {
                memAddr = pc;
                memLoad = true;
            }
            else
            {
                memAddr = a;
                memLoad = isComputeInstr && useMemory;
            }
        }
        
        public void Clock()
        {
            if( reset )
            {
                a = 0;
                d = 0;
                pc = 0;
                instr = 0;
            }
            else if( decode )
            {
                instr = memIn;
            }
            else if( isLoadInstr )
            {
                a = loadedWord;
                pc ++;
            }
            else
            {
                alu.SetLeftInput(useMemory ? memIn : a);
                alu.SetRightInput(d);
                alu.SetFlags(aluFlags);
                alu.Evaluate();
                
                var dst = dstMask;
                var leg = alu.GetCmpMask();
                var o = alu.GetResult();
                
                if( (dst & DstMask.A) != DstMask.None ) a = o;
                if( (dst & DstMask.D) != DstMask.None ) d = o;
                if( (dst & DstMask.M) != DstMask.None )
                {
                    memOut = o;
                    memSet = true;
                }
                
                if( (leg & jmpMask) != CmpMask.Never )
                {
                    pc = a;
                }
                else
                {
                    pc ++;
                }
            }
        }
        
        public void SetMemoryIn(ushort word)
        {
            memIn = word;
        }
        
        public ushort GetMemoryOut()
        {
            return memOut;
        }
        
        public ushort GetMemoryAddress()
        {
            return memAddr;
        }
        
        public bool GetMemorySet()
        {
            return memSet;
        }
        
        public bool GetMemoryLoad()
        {
            return memLoad;
        }
        
        public ushort ARegister { get { return a; } }
        public ushort DRegister { get { return d; } }
        public ushort PCRegister { get { return pc; } }
        public ushort InstrRegister { get { return instr; } }
        #endregion
        
        #region "Private members"
        ushort a, d, pc, instr;
        HackyAlu alu;
        
        bool decode, reset;
        ushort memIn, memOut, memAddr;
        bool memSet, memLoad;
        
        private bool isLoadInstr { get { return (instr & 0x8000) == 0; } }
        
        private bool isComputeInstr { get { return (instr & 0x8000) != 0; } }
        
        private bool useMemory { get { return (instr & 0x1000) != 0; } }
        
        private byte aluFlags { get { return (byte)((instr >> 6) & 0x3F); } }
        
        private DstMask dstMask { get { return (DstMask)((instr >> 3) & 0x7); } }
        
        private CmpMask jmpMask { get { return (CmpMask)(instr & 0x7); } }
        
        private ushort loadedWord { get { return instr; } }
        
        private void clearOutputs()
        {
            memOut = 0;
            memAddr = 0;
            memSet = false;
            memLoad = false;
        }
        #endregion
    }
    
    public enum DstMask
    {
        None = 0,
        A = 4,
        M = 2,
        D = 1,
    }
}