using System.Text.RegularExpressions;

namespace Axaprj.Textc.Vect
{
    public static class RegExUtil
    {
        static readonly string SP = " ";
        static readonly Regex rexNumSp = new Regex(
            @" [0-9]*(?: \. [0-9]*)? ", RegexOptions.Compiled);
        static readonly Regex rexNumLetter = new Regex(
            @"(?<num>[0-9])(?<lett>[A-Za-zА-Яа-яІіЇїЄєҐґЁё])", RegexOptions.Compiled);

        public static string Prepare(string s)
        {
            var res = s;
            // split digit-letter by space
            res = rexNumLetter.Replace(res,
                (m) => m.Groups["num"] + SP + m.Groups["lett"]);
            // remove spaces from fractional numbers 
            res = rexNumSp.Replace(res,
                (m) => SP + m.Value.Replace(SP, string.Empty) + SP);
            return res;
        }
    }
}
