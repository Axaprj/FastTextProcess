using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Axaprj.Textc.Vect
{
    public static class ExpressionUtil
    {
        /// <summary>
        /// Get named value from Takenet.Textc.Expression 
        /// </summary>
        public static object GetTokenValue(this Takenet.Textc.Expression expr, string name)
        {
            foreach (var token in expr.Tokens)
            {
                if (token != null)
                {
                    if (token.Type.Name == name)
                        return token.Value;
                }
            }
            return string.Empty;
        }
    }
}
