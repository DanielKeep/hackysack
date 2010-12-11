//  
//  Main.cs
//  
//  Author:
//       Daniel Keep <daniel.keep@gmail.com>
// 
//  Copyright (c) 2010 Daniel Keep
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Reflection;
using System.Timers;
using Hacky.Sack;
using NDesk.Options;

namespace Hacky.Sack.Cli
{
    class MainClass
    {
        const string DEFAULT_ROM_PATH = "hacky-rom.himg";
        
        delegate void voidCall();
        
        public static int Main(string[] args)
        {
            bool exit = false;
            bool unclocked = true;
            bool trace = false;
            bool traceDebug = false;
            int clockLimit = 0;
            string romPath = DEFAULT_ROM_PATH;
            
            var optionSet = new OptionSet()
            {
                { "r|rom=",         path => romPath = path },
                { "u|unclocked",    _ => { unclocked = true; } },
                { "clocked",        _ => { unclocked = false; } },
                { "trace",          _ => { trace = true; } },
                { "trace-debug",    _ => { traceDebug = true; } },
                { "limit-clocks=",   _ => { clockLimit = int.Parse(_); } },
                { "help",           _ => { PrintHelp(); exit = true; } },
                { "version",        _ => { PrintVersion(); exit = true; } },
                { "licence",        _ => { PrintLicence(); exit = true; } },
            };
            
            var extraArgs = optionSet.Parse(args);
            
            foreach( var arg in extraArgs )
            {
                Console.Error.WriteLine("Error: unexpected argument '"+arg+"'");
                return 1;
            }
            
            if( exit ) return 0;
            
            var mb = new HackyMb();
            var stop = false;
            
            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e) {
               stop = true;
            };
            
            mb.LoadRomImage(romPath);
            mb.Reset();
            
            if( unclocked )
            {
                var useLimit = clockLimit > 0;
                var limit = clockLimit;
                
                var cpu = mb.Cpu;
                var mem = mb.Memory;
                var dr = mem.DebugRegisters;
                
                mem.TraceAccess = trace;
                
                if( traceDebug )
                {
                    mem.DebugRegisters.WriteTrap += delegate(Object sender, EventArgs e)
                    {
                        var ea = (WriteTrapEventArgs) e;
                        
                        Console.WriteLine(String.Format("                                                    "
                                                        +"  D:{0:x4} {1:x4} {2:x4} {3:x4}",
                                                        dr[0], dr[1], dr[2], dr[3]));
                    };
                }
                
                voidCall dumpRegs = delegate()
                {
                    Console.WriteLine(String.Format("PC:{2:x4},  A:{0:x4},  D:{1:x4},            I:{3:x4}"
                                                    +"  D:{4:x4} {5:x4} {6:x4} {7:x4}",
                                                    cpu.ARegister, cpu.DRegister, cpu.PCRegister, cpu.InstrRegister,
                                                    dr[0], dr[1], dr[2], dr[3]));
                    return;
                };
                
                while( !stop )
                {
                    if( useLimit && limit <= 0 )
                        break;
                    
                    /*
                     * The CPU uses four clocks per instruction.  These are decode preclock, decode, compute
                     * preclock, compute.  We want to dump registers between the decode and compute stages:
                     * it is at this point that the instruction has been decoded but BEFORE it has been executed.
                     */
                    mb.Clock();
                    mb.Clock();
                    if( trace ) dumpRegs();
                    mb.Clock();
                    mb.Clock();
                    if( useLimit ) --limit;
                }
                
                if( !stop && useLimit && limit <= 0 )
                    Console.Error.WriteLine("Stopped on clock limit.");
            }
            else
            {
                var clock = new Timer(1000.0/1.000000); // ms * 1MHz
                clock.AutoReset = false;
                clock.Elapsed += delegate(object sender, ElapsedEventArgs e) {
                    if( stop ) return;
                    mb.Clock();
                    clock.Enabled = true;
                };
                
                clock.Start();
                while( !stop ) { System.Threading.Thread.Sleep(0); }
                clock.Stop();
            }
            
            return 0;
        }
        
        public static void PrintHelp()
        {
            PrintVersion();
            PrintCopyright();
            PrintUsage();
            Console.WriteLine("");
            Console.WriteLine("Options:");
            Console.WriteLine("-r, --rom=PATH       Specify the ROM image to use.  If not specified, it");
            Console.WriteLine("                     defaults to '"+DEFAULT_ROM_PATH+"'.");
            Console.WriteLine("-u, --unclocked      Execute instructions as fast as possible. (default)");
            Console.WriteLine("    --clocked        Clock at 1MHz.");
            Console.WriteLine("    --trace          Trace each instruction executed.");
            Console.WriteLine("    --trace-debug    Trace writes to debug registers.");
            Console.WriteLine("    --limit-clocks=N  Limit execution to N macro clock cycles.");
            Console.WriteLine("    --help           Displays this message.");
            Console.WriteLine("    --version        Displays the version of the program.");
            Console.WriteLine("    --licence        Displays licensing information.");
        }
        
        public static void PrintUsage()
        {
            Console.WriteLine("Usage: hackysack-cli [OPTIONS]");
        }
        
        public static void PrintVersion()
        {
            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            Console.Write("HACKYSACK Version ");
            Console.WriteLine(ver.ToString());
        }
        
        public static void PrintCopyright()
        {
            var asm = Assembly.GetExecutingAssembly();
            var attribs = asm.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            Console.WriteLine(((AssemblyCopyrightAttribute)attribs[0]).Copyright);
        }
        
        public static void PrintLicence()
        {
            PrintCopyright();
            Console.Write(
"\n" +
"This program is free software: you can redistribute it and/or modify\n" +
"it under the terms of the GNU General Public License as published by\n" +
"the Free Software Foundation, either version 3 of the License, or\n" +
"(at your option) any later version.\n" +
"\n" +
"This program is distributed in the hope that it will be useful,\n" +
"but WITHOUT ANY WARRANTY; without even the implied warranty of\n" +
"MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the\n" +
"GNU General Public License for more details.\n" +
"\n" +
"You should have received a copy of the GNU General Public License\n" +
"along with this program.  If not, see <http://www.gnu.org/licenses/>.\n" +
                          "");
        }
    }
}
