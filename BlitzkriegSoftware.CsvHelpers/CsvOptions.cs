using System;

namespace BlitzkriegSoftware.CsvHelpers
{
    public class CsvOptions
    {

        private char _fieldseparator = ',';

        /// <summary>
        /// Required: Field Separator
        /// </summary>
        public char FieldSeparator
        {
            get
            {
                return _fieldseparator;
            }
            set
            {
                _fieldseparator = value;
            }
        }

        private string _quote = "\"";

        /// <summary>
        /// Not Required But Handy
        /// </summary>
        public string Quote
        {
            get
            {
                return _quote;
            }
            set
            {
                _quote = value;
            }
        }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.FieldSeparator.ToString());
            }
        }
    }
}
