// 
//  HimgReader.cs
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
using System.IO;

namespace Hacky
{
    public class HimgReader : IDisposable
    {
        static HimgReader()
        {
            spaceSplit = new string[]{ " ", "\t" };
            lineSplit = new string[] { "\r\n", "\r", "\n" };
        }
        
        public HimgReader(string path)
        {
            fin = new FileStream(path, FileMode.Open, FileAccess.Read);
            tin = new StreamReader(fin);
            
            // Read header
            if( tin.ReadLine() != "v2.0 raw" )
                throw new InvalidOperationException("Invalid HIMG file");
            
            data = tin.ReadToEnd();
            nextLine();
        }
        
        ~HimgReader ()
        {
            Dispose(false);   
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if( !disposing ) return;
            
            tin.Close();
            fin.Close();
        }
        
        public bool HasMore { get { return line != null; } }
        
        public ushort ReadWord()
        {
            string[] parts;
            string word;
            
            parts = line.Split(spaceSplit, 2, StringSplitOptions.RemoveEmptyEntries);
            word = parts[0];
            
            line = (parts.Length > 1) ? parts[1] : null;
            if( line != null && line.Length == 0 )
                line = null;
            
            if( line == null )
                nextLine();
            
            return ushort.Parse(word, System.Globalization.NumberStyles.HexNumber);
        }
        
        private bool nextLine()
        {
            string[] parts;
            
            line = null;
            while( line == null )
            {
                if( data == null )
                    return false;
                
                parts = data.Split(lineSplit, 2, StringSplitOptions.RemoveEmptyEntries);
                
                line = parts[0].Trim();
                if( line.Length == 0 || line.StartsWith("#") )
                    line = null;
                
                data = (parts.Length > 1) ? parts[1] : null;
            }
            return true;
        }
        
        private string line;
        private string data;
        private FileStream fin;
        private TextReader tin;
        
        private static string[] spaceSplit;
        private static string[] lineSplit;
    }
}