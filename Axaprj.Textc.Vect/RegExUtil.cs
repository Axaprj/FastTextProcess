using System.Text.RegularExpressions;

namespace Axaprj.Textc.Vect
{
    public static class RegExUtil
    {
        static readonly Regex rexClnSpaces = new Regex("\\s{2,}", RegexOptions.Compiled);
        static readonly Regex rexClnCommonEn = 
            new Regex("[^A-Za-z0-9(),.!?\'`\"/-]", RegexOptions.Compiled);
        static readonly Regex rexClnCommonEnCyr = 
            new Regex("[^A-Za-zА-Яа-яІіЇїЄєҐґЁё0-9(),.!?\'`\"/-]", RegexOptions.Compiled);
        static readonly Regex rexVe = new Regex("(?<lett>[A-Za-z]) ' ve", RegexOptions.Compiled);
        static readonly Regex rexRe = new Regex("(?<lett>[A-Za-z]) ' re", RegexOptions.Compiled);
        static readonly Regex rexD = new Regex("(?<lett>[A-Za-z]) ' d", RegexOptions.Compiled);
        static readonly Regex rexLL = new Regex("(?<lett>[A-Za-z]) ' ll", RegexOptions.Compiled);
        static readonly Regex rexS = new Regex("(?<lett>[A-Za-z]) ' s", RegexOptions.Compiled);

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
        
        static public string RegExClean(string src) =>
            CleanCommon(src, rexClnCommonEnCyr);

        static string CleanCommon(string str, Regex clean_rex)
        {
            //str = str.ToLower();
            str = clean_rex.Replace(str, " ");
            str = str
                .Replace("-", " - ") // add
                .Replace("/", " / ") // add
                .Replace("'", " ' ") // add
                .Replace("\"", " \" ") // add
                .Replace("`", " ' ") // add
                .Replace(".", " . ") // add
                .Replace(",", " , ")
                .Replace("!", " ! ")
                .Replace("(", " ( ")
                .Replace(")", " ) ")
                .Replace("?", " ? ");
            str = str.Replace("n ' t", "n't");
            str = rexVe.Replace(str, "${lett} 've");
            str = rexRe.Replace(str, "${lett} 're");
            str = rexD.Replace(str, "${lett} 'd");
            str = rexLL.Replace(str, "${lett} 'll");
            str = rexS.Replace(str, "${lett} 's");
            str = rexClnSpaces.Replace(str, " ");
            return str.Trim();
        }

    }
}
