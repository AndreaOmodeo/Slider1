using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace Slider1
{
    class NumericFilenameComparer : IComparer<string>
    {
        static CaseInsensitiveComparer caseiComp = new CaseInsensitiveComparer();
        static char[] invalid_path_chars =  Path.GetInvalidPathChars();
        
        public int Compare(string x, string y)
        {
            try
            {
                string xName = Path.GetFileNameWithoutExtension(Remove(x, invalid_path_chars));
                string yName = Path.GetFileNameWithoutExtension(Remove(y, invalid_path_chars));
                string xNoDigits = RemoveDigits(xName);
                string yNoDigits = RemoveDigits(yName);

                if (xNoDigits.Length > 0 && xNoDigits == yNoDigits)
                {
                    int num1;
                    int num2;

                    string xdigits = Remove(xName, xNoDigits);
                    string ydigits = Remove(yName, xNoDigits);

                    if (int.TryParse(xdigits, out num1))
                        if (int.TryParse(ydigits, out num2))
                            return num1 - num2;
                }
            }
            catch
            {}
            return caseiComp.Compare(x, y);
        }

        private string RemoveDigits(string y)
        {
            return Remove(y, "0123456789");
        }

        private string Remove(string x, IEnumerable<char> xNoDigits)
        {
            string result = x;
            foreach (char c in xNoDigits)
            {
                result=result.Replace(new string (c,1), "");
            }
            return result;
        }
    }
}
