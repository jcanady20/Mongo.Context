using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Mongo.Context.AppHost;
partial class Program
{
    //	An internal pointer to the current Instance of the class
    //	used to reference Program and avoid using Activator Create Instance Calls
    //	'this' will not work for static method calls
    protected static Program _this;
    protected static string _prompt = ":>";
    protected static Type _type = typeof(Program);
    protected static List<MethodInfo> _methods = null;

    internal Program()
    {
        _this = this;
    }

    [STAThread]
    static void Main(string[] args)
    {
        CreateMethodCache();
        AddTraceListeners();

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.White;
        Console.Title = "Mongo.TestHarness";

        if (args.Length > 0)
        {
            RunCommand(args);
            return;
        }

        MainLoop();
    }

    static void MainLoop()
    {
        Help();
        bool pContinue = true;
        while (pContinue)
        {
            Console.Write(_prompt);

            //string[] Coms = Console.ReadLine().Split(new char[] { ' ' });
            var parms = ParseInput(Console.ReadLine());
            if (parms.Length == 0)
            {
                Help();
                continue;
            }

            switch (parms[0].ToLower())
            {
                case "exit":
                case "quit":
                    pContinue = false;
                    break;
                default:
                    RunCommand(parms);
                    break;
            }
            Console.WriteLine("");
        }
    }
    static string[] ParseInput(string args)
    {
        var parts = Regex.Matches(args, @"[\""].+?[\""]|[^ ]+")
            .Cast<Match>()
            .Select(x => x.Value.Replace("\"", ""))
            .ToArray();
        return parts;
    }
    static void RunCommand(string[] args)
    {
        string CallingMethod = args[0];
        var parms = args.Skip(1).Take(args.Count() - 1).ToArray();

        var method = _methods.FirstOrDefault(x => String.Compare(x.Name, CallingMethod, true) == 0 && x.GetParameters().Length == parms.Length);
        //	Abandon the call if we didn't find a match
        if (method == null)
        {
            Console.WriteLine("Unknown Command");
            Program.Help();
            return;
        }

        try
        {
            method.Invoke(_this, parms);
            return;
        }
        catch (Exception e)
        {
            if (e.InnerException != null)
            {
                Console.WriteLine();
                WriteExceptions(e.InnerException);
            }
            return;
        }
    }

    [Description("Shows Help for All Commands")]
    static void Help()
    {
        Console.WriteLine("Valid Commands");
        foreach (MethodInfo m in _methods)
        {
            DescriptionAttribute[] attribs = (DescriptionAttribute[])m.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attribs != null && attribs.Length > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(m.Name);
                ParameterInfo[] parm = m.GetParameters();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("(");
                for (int i = 0; i < parm.Length; i++)
                {
                    if (i > 0)
                        Console.Write(", ");

                    Console.Write("({0}){1}", parm[i].ParameterType.Name, parm[i].Name);
                }
                Console.Write(")");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\n\t{0}", attribs[0].Description);
            }
        }
    }
    [Description("Clears the Current display Buffer")]
    static void Clear()
    {
        Console.Clear();
    }
    [Description("Quits out of the application")]
    static void Quit()
    {
        return;
    }

    static void writeHeader<T>(T data)
    {
        if (data == null)
        {
            Console.WriteLine("Unable to create Header from Null Object");
            return;
        }
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
        {
            Console.Write("{0, 8} ", prop.Name);
        }
        Console.WriteLine();
    }
    static void writeCollectionData<T>(IEnumerable<T> data)
    {
        if (data == null)
        {
            Console.WriteLine("Unable to write data from Null Object");
            return;
        }
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var i = 0;
        foreach (var t in data)
        {
            ++i;
            Console.Write("{0}: ", i);
            writeData(t, props);
            Console.WriteLine();
        }
    }
    static void writeData<T>(T data, IEnumerable<PropertyInfo> propertyInfo = null)
    {
        if (data == null)
        {
            Console.WriteLine("Unable to write data from Null Object");
            return;
        }
        var props = (propertyInfo != null) ? propertyInfo : typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in props)
        {
            Console.Write("{0, 8} ", prop.GetValue(data));
        }
    }

    static TimeSpan CalculateEta(DateTime startTime, int totalItems, int completeItems)
    {
        TimeSpan _eta = TimeSpan.MinValue;
        //	Avoid Divide by Zero Errors
        if (completeItems > 0)
        {
            int _itemduration = (int)DateTime.Now.Subtract(startTime).TotalMilliseconds / completeItems;
            _eta = TimeSpan.FromMilliseconds((double)((totalItems - completeItems) * _itemduration));
        }
        return _eta;
    }
    static void WriteExceptions(Exception e)
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Red;

        Trace.Write("Source:");
        Trace.Write(e.Source);
        Trace.WriteLine("\nMessage:");
        Trace.Write(e.Message);
        Trace.WriteLine("\nStack Trace:");
        Trace.Write(e.StackTrace);
        Trace.WriteLine("\nUser Defined Data:");
        foreach (System.Collections.DictionaryEntry de in e.Data)
        {
            Trace.WriteLine(string.Format("[{0}] :: {1}", de.Key, de.Value));
        }
        if (e.InnerException != null)
        {
            WriteExceptions(e.InnerException);
        }
        Console.ForegroundColor = ConsoleColor.White;
    }
    static string GetCurrentPath()
    {
        var asm = Assembly.GetExecutingAssembly();
        var fi = new FileInfo(asm.Location);
        return fi.DirectoryName;
    }
    static void HexDump(byte[] bytes)
    {
        for (int line = 0; line < bytes.Length; line += 16)
        {
            byte[] lineBytes = bytes.Skip(line).Take(16).ToArray();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("{0:x8} ", line);
            sb.Append(string.Join(" ", lineBytes.Select(b => b.ToString("x2")).ToArray()).PadRight(16 * 3));
            sb.Append(" ");
            sb.Append(new string(lineBytes.Select(b => b < 32 ? '.' : (char)b).ToArray()));
            Console.WriteLine(sb);
        }
    }
    static void CreateMethodCache()
    {
        _methods = _type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic).ToList();
    }
    static void AddTraceListeners()
    {
        TextWriterTraceListener CWriter = new TextWriterTraceListener(Console.Out);
        Trace.Listeners.Add(CWriter);
    }
}
