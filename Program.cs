﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace RedeemCodeGen
{
    class Program
    {
        private sealed class Argument
        {
            [Option('c', "count", Required = true, HelpText = "要生成的RedeemCode数量")]
            public int count
            {
                get;
                set;
            }

            [Option('t', "type", Required = true, HelpText = "输入RedeemCode类型，1 = char, 0 =  num, 2 = mix")]
            public int type
            {
                get;
                set;
            }

            [Option('l', "len", Required = true, HelpText = "生成的RedeemCode长度")]
            public int length
            {
                get;
                set;
            }

            [Option('p', "prefix", Required = true, HelpText = "前缀")]
            public string prefix
            {
                get;
                set;
            }

            [Option('s', "suffix", Required = true, HelpText = "后缀")]
            public string suffix
            {
                get;
                set;
            }

            [Option('o', "out put file", Required = true, HelpText = "输入的文件路径.")]
            public string path
            {
                get;
                set;
            }
        }
        static void Main(string[] args)
        {
            var result = CommandLine.Parser.Default.ParseArguments<Argument>(args);

            if(!result.Errors.Any())
            {
                var codes = RedeemCodeGen.CreateCode(result.Value.count, result.Value.type, result.Value.length, result.Value.prefix, result.Value.suffix);

                var file = new FileStream(result.Value.path, FileMode.CreateNew);
                var stream = new StreamWriter(file);
                for (int i = 0; i < codes.Length; i++)
                {
                    stream.WriteLine(codes[i]);
                }
                stream.Close();
                stream.Dispose();

                file.Close();
                file.Dispose();
            }
        }
    }
}