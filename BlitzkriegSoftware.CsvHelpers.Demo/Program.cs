using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;
using Faker.Extensions;

namespace BlitzkriegSoftware.CsvHelpers.Demo
{
    class Program
    {
        private static DataTable dt = null;

        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection().AddLogging(o =>
            {
                o.SetMinimumLevel(LogLevel.Trace);
                o.AddConsole();
                o.AddDebug();
            }).BuildServiceProvider();

            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("Demo");

            CommandLine.Parser.Default.ParseArguments<Models.CmdOptions>(args)
                .WithParsed<Models.CmdOptions>(opts =>
                {
                    RunOptionsAndReturnExitCode(opts, logger);
                })
                .WithNotParsed<Models.CmdOptions>((errs) => HandleParseError(errs));
        }

        /// <summary>
        /// Do main logic e.g. command line swiches were ok
        /// </summary>
        /// <param name="opts">Options</param>
        private static void RunOptionsAndReturnExitCode(Models.CmdOptions opts, ILogger logger)
        {
            if (opts is null)
            {
                throw new ArgumentNullException(nameof(opts));
            }

            if (string.IsNullOrWhiteSpace(opts.ExportFileName) && string.IsNullOrWhiteSpace(opts.ImportFilename)) throw new ArgumentException("Either -i or -o must be specified");

            var csv = new CsvUtility(new CsvOptions(), logger);

            if (!string.IsNullOrWhiteSpace(opts.ExportFileName))
            {
                dt = CreateData(opts);
                csv.WriteCsv(opts.ExportFileName, HandleCsvLineWrite);
            }

            if (!string.IsNullOrWhiteSpace(opts.ImportFilename))
            {
                csv.ReadCsv(opts.ImportFilename, HandleCsvLineRead);
            }

            logger.LogInformation("Exiting...");
        }

        /// <summary>
        /// Handle Command Line Parsing Errors
        /// </summary>
        /// <param name="errors">(sic)</param>
        private static void HandleParseError(IEnumerable<Error> errors)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var e in errors) sb.AppendLine(e.Tag.ToString());
            Console.Error.WriteLine("{0}", sb.ToString());
        }

        public static void HandleCsvLineRead(object[] fields, CsvOptions options, ILogger logger)
        {
            string outline = fields.Join(options.FieldSeparator.ToString());
            logger.LogDebug($"Read: {outline}");
        }

        private static int row = 0;

        public static object[] HandleCsvLineWrite(CsvOptions options, ILogger logger)
        {
            var data = new List<string>();
            if (row < dt.Rows.Count)
            {
                foreach (var s in dt.Rows[row].ItemArray)
                {
                    data.Add(s.ToString());
                }
                row++;
            }
            return data.ToArray();
        }

        private static System.Data.DataTable CreateData(Models.CmdOptions opts)
        {
            var dice = new BlitzkriegSoftware.SecureRandomLibrary.SecureRandom();

            var d = new System.Data.DataTable();

            d.Columns.Add(new System.Data.DataColumn() { AllowDBNull = true, ColumnName = "Id", DataType = typeof(long), Unique = true });

            d.Columns.Add(new System.Data.DataColumn() { AllowDBNull = true, ColumnName = "IsDeleted", DataType = typeof(bool) });

            d.Columns.Add(new System.Data.DataColumn() { AllowDBNull = true, ColumnName = "Cash", DataType = typeof(decimal) });

            d.Columns.Add(new System.Data.DataColumn() { AllowDBNull = true, ColumnName = "Company", DataType = typeof(string), MaxLength = 512 });

            d.Columns.Add(new System.Data.DataColumn() { AllowDBNull = true, ColumnName = "Multiplex", DataType = typeof(Double) });

            for (int i = 0; i < opts.MaxRows; i++)
            {
                var ia = new Object[]
                {
                    (long) i,
                    (i % 2 == 0),
                    (decimal) ((double) dice.Next(1,99) / 100) + dice.Next(1,99),
                    Faker.Lorem.Word(),
                    (double) ((double) dice.Next(1,99) / 100) + dice.Next(100,999)
                };
                d.Rows.Add(ia);
            }

            return d;
        }

    }
}
