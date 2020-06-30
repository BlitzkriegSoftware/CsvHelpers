using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace BlitzkriegSoftware.CsvHelpers
{
    public class CsvUtility
    {
        public delegate void HandleCsvLineRead(object[] fields, CsvOptions options, ILogger logger);
        public delegate object[] HandleCsvLineWrite(CsvOptions options, ILogger logger);

        private CsvUtility() { }

        private readonly CsvOptions _options;
        private readonly ILogger _logger;

        public CsvUtility(CsvOptions options, ILogger logger)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (!options.IsValid) throw new ArgumentException("Options are not valid, Field Separator is required");

            _options = options;
            _logger = logger;
        }

        public void ReadCsv(string filename, HandleCsvLineRead readLineHandler)
        {
            if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentNullException(nameof(filename));
            if (!File.Exists(filename)) throw new FileNotFoundException("Filename is required", filename);

            if (readLineHandler == null) throw new ArgumentNullException(nameof(readLineHandler));

            using (var reader = new StreamReader(filename))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] fields = line.Split(new char[] { _options.FieldSeparator }, StringSplitOptions.None);
                    if (!string.IsNullOrWhiteSpace(_options.Quote))
                    {
                        for (var i = 0; i < fields.Length; i++)
                        {
                            if (IsText(fields[i]))
                            {
                                fields[i] = fields[i].Replace(_options.Quote, "").Trim();
                            }
                        }
                    }
                    readLineHandler(fields, _options, _logger);
                }
            }
        }

        public void WriteCsv(string filename, HandleCsvLineWrite writelineHandler)
        {
            if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentNullException(nameof(filename));
            if (File.Exists(filename)) File.Delete(filename);

            if (writelineHandler == null) throw new ArgumentNullException(nameof(writelineHandler));

            using (var writer = new StreamWriter(filename))
            {
                while (true)
                {
                    var fields = writelineHandler(_options, _logger);

                    if ((fields == null) || (fields.Length <= 0)) break;

                    for (int i = 0; i < fields.Length; i++)
                    {
                        if (IsText(fields[i]) && !IsQuoted(fields[i]))
                        {
                            fields[i] = QuoteIt(fields[i]);
                        }
                        writer.Write(fields[i]);
                        if (i < (fields.Length-1))
                        {
                            writer.Write(_options.FieldSeparator);
                        }
                    }
                    writer.WriteLine();
                }
            }
        }

        #region "Type Inferers"

        public string QuoteIt(object s)
        {
            return _options.Quote + s.ToString() + _options.Quote;
        }

        public bool IsQuoted(object s)
        {
            if (!string.IsNullOrWhiteSpace(s?.ToString()))
            {
                return !(s is string t) ? false : t.Contains(_options.Quote);
            }
            else
            {
                return false;
            }
        }

        public static bool IsNumeric(object s)
        {
            if (string.IsNullOrWhiteSpace(s?.ToString())) return false;
            return Double.TryParse(s?.ToString(), out double _);
        }

        public static bool IsBool(object s)
        {
            if (string.IsNullOrWhiteSpace(s?.ToString())) return false;
            return bool.TryParse(s?.ToString(), out bool _);
        }

        public static bool IsText(object s)
        {
            return !IsNumeric(s) && !IsBool(s);
        }

        #endregion

    }
}
