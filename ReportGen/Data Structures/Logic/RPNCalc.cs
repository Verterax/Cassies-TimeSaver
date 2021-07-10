using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReportGen
{

    public class RPNCalc
    {

        #region Consts

        public const int c_DefaultPrecision = 3;
        public const int c_TimeOutTries = 900;
        //Match must come at start string, start with " and end with "
        //containing any chars inbetween the first(? lazy operator) two "
        public const string c_QuoteCapRegex = "^(\".*?\")";
        public const string c_NoQuoteCapRegex = "^\"(.*?)\"";

        public const string c_Err = "Err";
        public const string c_QuoteDbl = "\"";

        public const string c_True = "true";
        public const string c_False = "false";
        public const double c_dblTrue = 1.0f;
        public const string c_strTrue = "1";

        public const string c_TagO = "<";
        public const string c_TagClo = ">";

        public const string c_Eq = "==";
        public const string c_NotEq = "!=";
        public const string c_GT = ">";
        public const string c_LT = "<";
        public const string c_GTOEQ = ">=";
        public const string c_LTOEQ = "<=";
        public const string c_And = "&&";
        public const string c_Or = "||";

        public const string c_Plus = "+";
        public const string c_Minus = "-";
        public const string c_Mul = "*";
        public const string c_Div = "/";
        public const string c_Mod = "%";
        public const string c_Pow = "^";
        public const string c_Delim = " ";
        public const string c_Dot = ".";

        #region Funcs

        public const string c_ParO = "(";
        public const string c_ParClo = ")";
        public const string c_Neg = "neg(";
        public const string c_Cos = "cos("; //Trig
        public const string c_Sin = "sin(";
        public const string c_Tan = "tan(";
        public const string c_Acos = "acos(";
        public const string c_Asin = "asin(";
        public const string c_Atan = "atan(";
        public const string c_Cosh = "cosh("; // Hyperbolic
        public const string c_Sinh = "sinh(";
        public const string c_Tanh = "tanh(";
        public const string c_Exp = "exp("; // Exponent
        public const string c_Log = "log("; // Roots
        public const string c_Log10 = "log10(";
        public const string c_Sqrt = "sqrt(";
        public const string c_Ceil = "ceil("; //Rounding
        public const string c_Floor = "floor(";
        public const string c_Abs = "abs("; //Absolute

        //Text functions
        public const string c_Ordinal = "ord("; // Ordinalize numbers 1st, 2nd, 3rd...

        #endregion // Funcs

        #endregion // Consts

        #region Variables
       
        Stack<string> stack;
        int precision;
        bool success;

        #endregion

        #region Constructor

        public RPNCalc()
        {
            stack = new Stack<string>();
            precision = c_DefaultPrecision;
            success = false;
        }

        #endregion

        #region Examine Tokens

        protected bool IsDigit(char c)
        {
            bool isNum = false;

            if (c > 47 && c < 58) isNum = true;
            if (c == '.') isNum = true;

            return isNum;
        }

        protected bool IsFunc(string token)
        {
            if (token == c_ParO ||
                token == c_Neg ||
                token == c_Cos ||      //Trig
                token == c_Sin ||
                token == c_Tan ||
                token == c_Acos ||
                token == c_Asin ||
                token == c_Atan ||
                token == c_Cosh ||     // Hyperbolic
                token == c_Sinh ||
                token == c_Tanh ||
                token == c_Exp ||      // Exponent
                token == c_Log ||      // Roots
                token == c_Log10 ||
                token == c_Sqrt ||
                token == c_Ceil ||     //Rounding
                token == c_Floor ||
                token == c_Abs ||  //Absolute
                token == c_Ordinal)  //return ordinal
                return true;
            else
                return false;
        }

        protected bool IsOp(string token)
        {
            if (
               token == c_TagO ||
               token == c_TagClo ||

               token == c_Eq ||
               token == c_NotEq ||
               token == c_GT ||
               token == c_LT ||
               token == c_GTOEQ ||
               token == c_LTOEQ ||
               token == c_And ||
               token == c_Or ||

               token == c_Plus ||
               token == c_Minus ||
               token == c_Mul ||
               token == c_Div ||
               token == c_Mod ||
               token == c_Pow)
                return true;
            else
                return false;
        }

        protected bool IsNum(string token)
        {
            double num = 0;

            if (double.TryParse(token, out num))
                return true;
            else
                return false;
        }

        protected bool IsTruth(string token)
        {
            if (token == c_Or ||
                token == c_And)
                return true;
            else
                return false;
        }

        protected bool IsBool(string token)
        {
            if (token == c_True ||
                token == c_False)
                return true;
            else
                return false;
        }

        //Text literals are surrounded by double quotes.
        protected bool IsText(string token)
        {
            bool textMatched = false;
            Regex regex = new Regex(c_NoQuoteCapRegex, RegexOptions.Singleline);
             textMatched = regex.IsMatch(token);

            if (textMatched)
                return true;
            else
                return false;
        }

        //Date literals are surrounded by double quotes.
        protected bool IsDate(string token)
        {
            bool parseSuccess = false;
            DateTime date = DateTime.MinValue;
            token = RemoveQuotes(token);
            parseSuccess = DateTime.TryParse(token, out date);

            if (parseSuccess)
                return true;
            else
                return false;
        }

        protected bool IsValue(string token)
        {
            if (IsNum(token) ||
                IsBool(token) ||
                IsText(token)) //Text matches same as date.
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Parse Tokens

        private string ReadToken(ref string str)
        {
            //Trim leading spaces.
            TrimDelim(ref str);

            if (str.Length > 0)
            {
                int tokenIndex = 0;
                string token = string.Empty;

                //If First char is a double quote, use regex capture.
                if (str[0] == c_QuoteDbl[0])
                {
                    Regex regex = new Regex(c_QuoteCapRegex, RegexOptions.Singleline);
                    Match match = regex.Match(str);

                    if (match.Success)
                    {
                        token = match.Groups[1].Value;
                        str = str.Substring(token.Length);
                        return token;
                    }
                    else
                        return "Missing \" in date/text";

                }
                else
                {
                    tokenIndex = str.IndexOf(c_Delim[0]);
                    token = string.Empty;

                    if (tokenIndex != -1)
                    {
                        token = str.Substring(0, tokenIndex);
                        str = str.Substring(tokenIndex);
                        return token;
                    }
                    else //All that remains is token.
                    {
                        token = str;
                        str = string.Empty;
                        return token;
                    }
                }
            }

            //Error
            return string.Empty;
        }

        protected string RemoveQuotes(string token)
        {
            Regex regex = new Regex(c_NoQuoteCapRegex, RegexOptions.Singleline);
            Match match = regex.Match(token);

            if (match.Success)
                return match.Groups[1].Value;
            else
                return token;
        }

        public string AddQ(string token)
        {
            return c_QuoteDbl + token + c_QuoteDbl;
        }

        #endregion

        #region Evaluate

        public string EvalRPN(string exprs)
        {
            this.success = false;

            string token = string.Empty;
            string left = string.Empty;
            string right = string.Empty;
            string result = string.Empty;
            int opCount = 0;

            try
            {
                while (exprs.Length > 0)
                {
                    token = this.ReadToken(ref exprs);

                    if (token == string.Empty)
                        break;

                    if (IsValue(token))
                    {
                        opCount++;
                        this.stack.Push(token);
                        continue;
                    }

                    if (IsFunc(token))
                    {
                        if (this.stack.Count == 0)
                        {
                            success = false;
                            return c_Err + " Missing number error.";
                        }

                        opCount++;
                        right = this.stack.Pop();
                        result = this.Func(token, right);
                        this.stack.Push(result);
                        continue;
                    }

                    if (IsOp(token))
                    {
                        if (this.stack.Count == 0)
                        {   //No token to get.
                            success = false;
                            return c_Err + " Missing number error.";
                        }

                        //Get right token.
                        right = this.stack.Pop();

                        if (this.stack.Count == 0)
                        {   //No token to get.
                            success = false;
                            return c_Err + " Missing number error.";
                        }

                        //Get left token.
                        left = this.stack.Pop();

                        opCount++;
                        result = Op(left, right, token);
                        this.stack.Push(result);
                        continue;
                    }

                    if (exprs == string.Empty ||
                        exprs == "")
                        break;
                    else
                        return c_Err + " Invalid token: " + token;             

                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            if (opCount > 0)
            {
                success = true;
            }
           
            if (this.success)
                return this.stack.Peek();
            else
                return string.Empty;
        }

        #endregion

        #region Calculate

        private string Func(string func, string right)
        {
            string result = c_Err;
            double num = 0;

            if (!double.TryParse(right, out num))
                return result;

            #region Negate/Return

            if (func == c_Neg)
            {
                num = (num * -1);
                return num.ToString();
            }

            if (func == c_ParO)
                return right;

            #endregion

            #region Trig

            if (func == c_Sin)
            {
                num = Math.Sin(num);
                return num.ToString();
            }

            if (func == c_Cos)
            {
                num = Math.Cos(num);
                return num.ToString();
            }

            if (func == c_Tan)
            {
                num = Math.Tan(num);
                return num.ToString();
            }


            #endregion

            #region Arc Trig

            if (func == c_Acos)
            {
                num = Math.Acos(num);
                return num.ToString();
            }

            if (func == c_Asin)
            {
                num = Math.Asin(num);
                return num.ToString();
            }

            if (func == c_Atan)
            {
                num = Math.Atan(num);
                return num.ToString();
            }


            #endregion

            #region Hyperbolic Trig

            if (func == c_Cosh)
            {
                num = Math.Cosh(num);
                return num.ToString();
            }

            if (func == c_Sinh)
            {
                num = Math.Sinh(num);
                return num.ToString();
            }

            if (func == c_Tanh)
            {
                num = Math.Tanh(num);
                return num.ToString();
            }


            #endregion

            #region Powers and Roots

            if (func == c_Sqrt)
            {
                num = Math.Sqrt(num);
                return num.ToString();
            }
            if (func == c_Log)
            {
                num = Math.Log(num);
                return num.ToString();
            }
            if (func == c_Log10)
            {
                num = Math.Log10(num);
                return num.ToString();
            }
            if (func == c_Exp)
            {
                num = Math.Exp(num);
                return num.ToString();
            }

            #endregion

            #region Rounding

            if (func == c_Ceil)
            {
                num = Math.Ceiling(num);
                return num.ToString();
            }

            if (func == c_Floor)
            {
                num = Math.Floor(num);
                return num.ToString();
            }

            if (func == c_Abs)
            {
                num = Math.Abs(num);
                return num.ToString();
            }

            #endregion

            #region Text Functions

            if (func == c_Ordinal)
            {
                return Ordinalize(num);
            }

            #endregion

            //Return Error No known function

            return c_Err + " Function " + func + " unknown.";
        }

        private string Op(string left, string right, string op)
        {
            double x = double.NaN;
            double y = double.NaN;

            bool xIsNum = false;
            bool yIsNum = false;
            double numResult;

            bool xBool = false;
            bool yBool = false;
            bool xIsBool = false;
            bool yIsBool = false;
            bool boolResult = false;

            DateTime xDate = DateTime.MinValue;
            DateTime yDate = DateTime.MinValue;
            bool xIsDate = false;
            bool yIsDate = false;
            DateTime dateResult = DateTime.MinValue;

            string xString = string.Empty;
            string yString = string.Empty;
            bool xIsString = false;
            bool yIsString = false;


            string result = string.Empty;

            #region Cast to appropriate data types.

            //Cast X
            #region Cast X.
            if (IsNum(left))
            {
                if (double.TryParse(left, out x))
                    xIsNum = true;
            }
            else if (IsBool(left))
            {
                xBool = BoolConvert(left);
                xIsBool = true;
            }
            else if (IsDate(left))
            {
                if (DateTime.TryParse(RemoveQuotes(left), out xDate))
                    xIsDate = true;
            }
            else //If this isn't a num, bool, or date, it must be a string literal.
            {
                left = RemoveQuotes(left);
                xString = left;
                xIsString = true;
            }
            #endregion

            //Cast Y
            #region Cast Y
            if (IsNum(right))
            {
                if (double.TryParse(right, out y))
                    yIsNum = true;
            }
            else if (IsBool(right))
            {
                yBool = BoolConvert(right);
                yIsBool = true;
            }
            else if (IsDate(right))
            {
                if (DateTime.TryParse(RemoveQuotes(right), out yDate))
                    yIsDate = true;
            }
            else //If this isn't a num, bool, or date, it must be a string literal.
            {
                right = RemoveQuotes(right);
                yString = right;
                yIsString = true;
            }
            #endregion //Cast y

            #endregion

            //Numbers
            #region Number Arithmetic

            //Number Arithmetic.
            if (xIsNum && yIsNum)
            {
                switch (op)
                {
                    case c_Plus:
                        result = (x + y).ToString();
                        return result;
                    case c_Minus:
                        result = (x - y).ToString();
                        return result;
                    case c_Mul:
                        result = (x * y).ToString();
                        return result;
                    case c_Div:
                        result = (x / y).ToString();
                        return result;
                    case c_Mod:
                        result = (x % y).ToString();
                        return result;
                    case c_Pow:
                        result = Math.Pow(x, y).ToString();
                        return result;
                }
            }

            #endregion

            //Dates
            #region DateMath

            if (xIsDate && yIsDate)
            {
                switch (op) //Only date date op is minus right now.
                {
                    case c_Minus:
                        numResult = (xDate - yDate).TotalDays;
                        return numResult.ToString();
                }
            } //Add and subtract days from date.
            else if ((xIsDate || yIsDate) && (xIsNum || yIsNum))
            {
                //If x is the date, then y is num.
                if (xIsDate)
                    switch (op)
                    {
                        case c_Plus:
                            dateResult = xDate.AddDays(y);
                            return dateResult.ToShortDateString();
                        case c_Minus:
                            dateResult = xDate.AddDays(-y);
                            return dateResult.ToShortDateString();
                    }
                else if (yIsDate) //Else y is the date, x is the num.
                {
                    switch (op)
                    {
                        case c_Plus:
                            dateResult = yDate.AddDays(x);
                            return dateResult.ToShortDateString();
                        case c_Minus:
                            dateResult = yDate.AddDays(-x);
                            return dateResult.ToShortDateString();
                    }
                }
            }


            #endregion

            //Bools
            #region Truth

            //Compare Numbers.
            if (xIsNum && yIsNum)
            {
                switch (op)
                {
                    case c_Eq:
                        boolResult = (x == y);
                        return BoolConvert(boolResult);
                    case c_NotEq:
                        boolResult = (x != y);
                        return BoolConvert(boolResult);
                    case c_GT:
                        boolResult = (x > y);
                        return BoolConvert(boolResult);
                    case c_LT:
                        boolResult = (x < y);
                        return BoolConvert(boolResult);
                    case c_GTOEQ:
                        boolResult = (x >= y);
                        return BoolConvert(boolResult);
                    case c_LTOEQ:
                        boolResult = (x <= y);
                        return BoolConvert(boolResult);
                }
            }
            else if (xIsDate && yIsDate) //Compare dates.
            {
                switch (op)
                {
                    case c_Eq:
                        boolResult = (xDate == yDate);
                        return BoolConvert(boolResult);
                    case c_NotEq:
                        boolResult = (xDate != yDate);
                        return BoolConvert(boolResult);
                    case c_GT:
                        boolResult = (xDate > yDate);
                        return BoolConvert(boolResult);
                    case c_LT:
                        boolResult = (xDate < yDate);
                        return BoolConvert(boolResult);
                    case c_GTOEQ:
                        boolResult = (xDate >= yDate);
                        return BoolConvert(boolResult);
                    case c_LTOEQ:
                        boolResult = (xDate <= yDate);
                        return BoolConvert(boolResult);
                }
            }
            else if (xIsString && yIsString)
            {
                switch (op)
                {
                    case c_Eq:
                        boolResult = (xString == yString);
                        return BoolConvert(boolResult);
                    case c_NotEq:
                        boolResult = (xString != yString);
                        return BoolConvert(boolResult);
                    case c_And:
                        return BoolConvert(false);
                    case c_Or:
                        return BoolConvert(false);
                }
            }
            else if (xIsBool && yIsBool) // AND OR
            {
                switch (op)
                {
                    case c_And:
                        boolResult = xBool && yBool;
                        return BoolConvert(boolResult);
                    case c_Or:
                        boolResult = xBool || yBool;
                        return BoolConvert(boolResult);
                    case c_Eq:
                        boolResult = (xBool == yBool);
                        return BoolConvert(boolResult);
                    case c_NotEq:
                        boolResult = (xBool != yBool);
                        return BoolConvert(boolResult);
                }
            }
            else if (xIsBool && yIsNum ||
                    xIsBool && yIsString)
            {
                if (yIsString)
                    yBool = yString == c_strTrue;
                else
                    yBool = y == c_dblTrue;

                switch (op)
                {
                    case c_Eq:
                        return BoolConvert(xBool == yBool);
                    case c_NotEq:
                        return BoolConvert(xBool != yBool);
                    case c_And:
                        return BoolConvert(xBool && yBool);
                    case c_Or:
                        return BoolConvert(xBool || yBool);
                }
            }
            else if (xIsNum && yIsBool ||
                    xIsString && yIsBool)
            {

                if (xIsString)
                    xBool = xString == c_strTrue;
                else
                    xBool = x == c_dblTrue;

                switch (op)
                {
                    case c_Eq:
                        return BoolConvert(xBool == yBool);
                    case c_NotEq:
                        return BoolConvert(xBool != yBool);
                    case c_And:
                        return BoolConvert(xBool && yBool);
                    case c_Or:
                        return BoolConvert(xBool || yBool);
                }
            }
            else if (xIsNum && yIsString)
            {
                switch (op)
                {
                    case c_Eq:
                        return BoolConvert(false);
                    case c_NotEq:
                        return BoolConvert(true);
                    case c_GT:
                        return BoolConvert(false);
                    case c_LT:
                        return BoolConvert(false);
                    case c_GTOEQ:
                        return BoolConvert(false);
                    case c_LTOEQ:
                        return BoolConvert(false);
                }
            }
            else if (xIsString && yIsNum)
            {
                switch (op)
                {
                    case c_Eq:
                        return BoolConvert(false);
                    case c_NotEq:
                        return BoolConvert(true);
                    case c_GT:
                        return BoolConvert(false);
                    case c_LT:
                        return BoolConvert(false);
                    case c_GTOEQ:
                        return BoolConvert(false);
                    case c_LTOEQ:
                        return BoolConvert(false);
                }
            }

            #endregion // Truth

            //Strings
            #region Concat

            if (xIsString || yIsString)
            {
                if (op == c_Plus)
                {
                    if (xIsString && yIsString)
                    {
                        result = xString + yString;
                        return result;
                    }

                    //String + Date
                    if (xIsString && yIsDate)
                    {
                        result = xString + yDate.ToShortDateString();
                        return result;
                    }
                    else if (xIsDate && yIsString)
                    {
                        result = xDate.ToShortDateString() + yString;
                        return result;
                    }

                    //String + number
                    if (xIsString && yIsNum)
                    {
                        result = xString + y.ToString();
                        return result;
                    }
                    else if (xIsNum && yIsString)
                    {
                        result = x.ToString() + yString;
                        return result;
                    }

                }
            }




            #endregion

            string xIs = string.Empty;
            string yIs = string.Empty;

            if (xIsNum) xIs = "a number [" + left + "]";
            else if (xIsBool) xIs = "true/false [" + left + "]";
            else if (xIsDate) xIs = "a date [" + left + "]";
            else if (xIsString) xIs = "text [" + left + "]";
            else 
                xIs = "Unknown value type: [" + left + "]";

            if (yIsNum) yIs = "a number [" + right + "]";
            else if (yIsBool) yIs = "true/false [" + right + "]";
            else if (yIsDate) yIs = "a date [" + right + "]";
            else if (yIsString) yIs = "text [" + right + "]";
            else yIs = "Unknown value type: [" + right + "]";

            return c_Err + " Undefined calculation: ( " +
                xIs + " " + op + " " + yIs + ")";

        }

        #endregion

        #region Cast To

        public double ToDouble()
        {
            string dbl = this.stack.Peek();
            double result = 0;
            if (!double.TryParse(dbl, out result))
                throw new Exception("Could not return " + dbl + " as double");
            return result;
        }

        public int ToInt()
        {
            string Integer = this.stack.Peek();
            int result = 0;
            if (!int.TryParse(Integer, out result))
                throw new Exception("Could not return " + Integer + " as int");
            return result;
        }

        #endregion

        #region Utility

        public void SetPrecision(int precision)
        {
            this.precision = precision;
        }

        protected void TrimDelim(ref string exprs)
        {
            if (exprs.Length > 0 && exprs[0] == c_Delim[0])
                exprs = exprs.Trim(c_Delim[0]);
        }

        private bool BoolConvert(string boolString)
        {
            if (boolString == c_True)
                return true;
            else if (boolString == c_False)
                return false;
            else
                throw new Exception("Expected truth value and got: " + boolString);
        }

        private string BoolConvert(bool Bool)
        {
            if (Bool) return c_True;
            else return c_False;
        }

        public void Clear()
        {
            this.stack = new Stack<string>();
        }

        #endregion

        #region Custom Functions

        private string Ordinalize(double baseNumber)
        {
            int num = (int)Math.Floor(baseNumber);

            string output = string.Empty;
            int iUnit = num % 10;

            if (num < 0)
                output = "";
            else if (num > 9 && num < 20)
                output = "th";
            else
            {
                switch (iUnit)
                {
                    case 0:
                        output = "th";
                        break;
                    case 1:
                        output = "st";
                        break;
                    case 2:
                        output = "nd";
                        break;  
                    case 3:
                        output = "rd";
                        break;
                    default:
                        output = "th";
                        break;
                }
            }

            return num.ToString() + output;
        }

        #endregion

    }
}
