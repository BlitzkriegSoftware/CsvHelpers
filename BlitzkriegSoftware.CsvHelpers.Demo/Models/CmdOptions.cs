using System;
using CommandLine;

namespace BlitzkriegSoftware.CsvHelpers.Demo.Models
{
    public class CmdOptions
    {
        [Option('v', "verbose", Default = false, HelpText = "Enable Verbose")]
        public bool Verbose { get; set; }

        [Option('i',"Import", HelpText ="Import Filename")]
        public string ImportFilename { get; set; }

        [Option('o', "Output", HelpText ="Output Filename")]
        public string ExportFileName { get; set; }

        [Option('m', "MaxRows", HelpText = "Max Rows for Output (default: 10)")]
        public int MaxRows { get; set; } = 10;
    }
}
