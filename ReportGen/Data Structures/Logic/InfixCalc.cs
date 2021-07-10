using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ReportGen
{
    public class InfixCalc : RPNCalc
    {

        #region Consts

        #endregion

        #region Variables

        private Queue<string> queue;
        private int precision;

        #endregion

        #region Constructor

        public InfixCalc()
        {
            this.precision = c_DefaultPrecision;
            this.queue = new Queue<string>();
        }

        public InfixCalc(int setPrecision)
        {
            this.precision = setPrecision;
            this.queue = new Queue<string>();
        }

        #endregion

        #region Examine Tokens

        private bool IsFuncStart(ref string str)
        {
            char c;

            if (str.Length > 0)
                c = str[0];
            else return false;

            if (c == c_Floor[0] ||
                c == c_Log10[0] ||
                c == c_Abs[0] ||
                c == c_Cos[0] ||
                c == c_Sin[0] ||
                c == c_Tan[0] ||
                c == c_Exp[0] ||
                c == c_Ordinal[0] ||
                c == c_ParO[0])
                return true;
            else
                return false;  
        }

        private bool IsBoolChar(ref string str, int index)
        {
            //If beyond possible indexes, return false.
            if (index > 4)
                return false;

            if (str.Length == 0)
                return false;

            char c = str[index];

            if (index < 4)
            {
                if (c == c_True[index] ||
                    c == c_False[index])
                    return true;
            }
            else if (index == 4)
            {
                if (c == c_False[index])
                    return true;
            }

            //Did not match a true/false char
            return false;
       }

        private bool IsOpChar(ref string str, int index)
        {
            if (str == string.Empty ||
                str == "" ||
                index > 1) return false;

            if (index < 1)
            {
                //Length 1 ops
                //If none are true, then false.
                if (str[index] == c_Plus[index] ||
                    str[index] == c_Minus[index] ||
                    str[index] == c_Mul[index] ||
                    str[index] == c_Div[index] ||
                    str[index] == c_Mod[index] ||
                    str[index] == c_Pow[index] ||
                    str[index] == c_GT[index] ||
                    str[index] == c_LT[index])
                    return true;
            }

            if (index < 2)
            {
                if (
                str[index] == c_GTOEQ[index] ||
                str[index] == c_LTOEQ[index] ||
                str[index] == c_Or[index] ||
                str[index] == c_Eq[index] ||
                str[index] == c_And[index] ||
                str[index] == c_NotEq[index])
                    return true;
            }

            return false;
        }

        private bool IsParClo(ref string str)
        {
            if (str[0] == c_ParClo[0])
                return true;
            else
                return false;
        }

        private int GetPrec(string token)
        {
            // CloseParenths  6
            // Funcs and (	  5
            //
            // Bool >> << ==  4
            // ^			  3 <----Right associative? The rest left.
            // * / &		  2
            // + -			  1
            //		Err		 -1

            if (token == c_Pow)
                return 3;

            if (token == c_Mul ||
                token == c_Div ||
                token == c_Mod)
                return 2;

            if (token == c_Plus ||
                token == c_Minus)
                return 1;

            if (token == c_GT ||
                token == c_LT ||
                token == c_Eq ||
                token == c_NotEq)
                return 4;

            if (IsFunc(token))
                return 5;

            if (token == c_ParClo)
                return 6;

            //Error No coresponding prescendence.
            return -1;
        }


        #endregion

        #region Parse Tokens

        protected bool ParseTokens(string exprs)
        {
            int timeOut = 0;
            ClearQueue();

            while (exprs.Length > 0)
            {
                if (exprs.Length == 0) break;

                //Queue any Unary Minus at string start.
                GetUnary(ref queue, ref exprs);
                if (exprs.Length == 0) break;

                //Queue any Number at string start.
                GetNum(ref queue, ref exprs);
                if (exprs.Length == 0) break;

                GetTextOrDate(ref queue, ref exprs);
                if (exprs.Length == 0) break;

                //Queue any true/false at string start.
                GetBool(ref queue, ref exprs);
                if (exprs.Length == 0) break;

                //Queue any Func at string start.
                GetFunc(ref queue, ref exprs);
                if (exprs.Length == 0) break;

                //Queue any operator at string start.
                GetOp(ref queue, ref exprs);
                if (exprs.Length == 0) break;

                //Queue any closing parentheses at string start.
                GetParClo(ref queue, ref exprs);
                if (exprs.Length == 0) break;

                timeOut++;

                if (timeOut == c_TimeOutTries)
                {
                    //Stuck token ss op, show format
                    if (IsOp(exprs))
                        throw new Exception("Format: (Value " + exprs + " Value)");

                    //Stuck token begins with dbl quote.
                    if (exprs[0] == c_QuoteDbl[0])
                        throw new Exception("( " + exprs + " ) needs a closing ( \" )");


                    throw new Exception("Partial token: (" + exprs + ")  Surround literals with \"\"");
                }
            }

            return true;
        }

        private void GetParClo(ref Queue<string> queue, ref string exprs)
        {
            TrimDelim(ref exprs);

            if (IsParClo(ref exprs))
            {
                string token = c_ParClo;
                queue.Enqueue(token);

                if (exprs.Length > 1)
                    exprs = exprs.Substring(c_ParClo.Length);
                else
                    exprs = string.Empty;
            }

        }

        private void GetOp(ref Queue<string> queue, ref string exprs)
        {
            TrimDelim(ref exprs);

            //If first char appears to be an OpChar...
            if (IsOpChar(ref exprs, 0))
            {
                for (int i = 0; i < exprs.Length; i++)
                {
                    if (!IsOpChar(ref exprs, i))
                    {
                        string token = exprs.Substring(0, i);

                        if (IsOp(token))
                        {
                            queue.Enqueue(token);
                            exprs = exprs.Substring(i);
                            return;
                        }
                        else
                            throw new Exception("Unexpected Op Token: " + token);
                    }
                }

                //If we get here, Op is all that remains in the string. Error.
               // throw new Exception("An operator must proceed a value: " + exprs);
            }
        }

        private void GetUnary(ref Queue<string> queue, ref string exprs)
        {
            TrimDelim(ref exprs);

            //The minus in an equation is always a unary.
            if (queue.Count == 0 && exprs[0] == c_Minus[0]) // 
            {
                queue.Enqueue(c_Neg);
                exprs = exprs.Substring(c_Minus.Length);
                return;
            }

            if (queue.Count > 0)
            {
                //If preceeding element is an op, and this is a negative.
                if (IsOp(queue.Last()) && exprs[0] == c_Minus[0])
                {
                    queue.Enqueue(c_Minus);
                    exprs = exprs.Substring(c_Minus.Length);
                    return;
                }
            }
        }

        private void GetNum(ref Queue<string> queue, ref string exprs)
        {
            TrimDelim(ref exprs);

            //Is the first char a num char? If not, don't search further.
            if (IsDigit(exprs[0]))
            {
                double num = double.NaN;
                bool hasDot = false;
                string token = string.Empty;

		        for (int i = 0; i < exprs.Length; i++)
		        {
                    //Check for double dots.
                    if (exprs[i] == c_Dot[0])
                    {
                        if (!hasDot)
                            hasDot = true;
                        else
                            //Double dot error.
                            throw new Exception("Cannot understand more than one decimal place in " + exprs);
                    }

			        if (!IsDigit(exprs[i]))
                    {
                        token = exprs.Substring(0, i);

                        //Is this token actually a number?                      

                        if (double.TryParse(token, out num))
                        {
                            exprs = exprs.Substring(i);
                            queue.Enqueue(token);
                            return;
                        }
                        else
                            throw new Exception("Unexpected number format in: " + token);
                    }

                    //If we get here, the entire string should be a number.
                    if (double.TryParse(exprs, out num))
                    {
                        token = exprs;
                        exprs = string.Empty;
                        queue.Enqueue(token);
                        return;
                    }
		        }	
            }
        }

        private void GetFunc(ref Queue<string> queue, ref string exprs)
        {
            TrimDelim(ref exprs);

            //Is the first 2 chars a Func char? If not, don't search further.
            if (IsFuncStart(ref exprs))
            {
                int parOPos = exprs.IndexOf(c_ParO) + 1;
                string token = exprs.Substring(0, parOPos);

                if (IsFunc(token))
                {
                    queue.Enqueue(token);
                    exprs = exprs.Substring(parOPos);
                    return;
                }
                else
                    return; //No error. Might be true/false

                //If we get here, error, string end is a Func.
                throw new Exception("A function must preceed a value: " + exprs);
            }
        }

        private void GetBool(ref Queue<string> queue, ref string exprs)
        {
            TrimDelim(ref exprs);

            bool isBool = true;
            string token = string.Empty;

            //Examine bool up to length 4.
            for (int i = 0; i < c_True.Length; i++)
            {
                if (!IsBoolChar(ref exprs, i))
                {
                    isBool = false;
                    return;
                }
            }

            if (isBool)
            {
                for (int i = 0; i < exprs.Length; i++)
                {
                    if (!IsBoolChar(ref exprs, i))
                    {
                        token = exprs.Substring(0, i);

                        if (IsBool(token))
                        {
                            exprs = exprs.Substring(i);
                            queue.Enqueue(token);
                            return;
                        }
                        else
                            throw new Exception("Unexpected Bool error in " + token);

                    }
                }

                //If we got here, take whole string as Bool.
                token = exprs;

                if (IsBool(token)) //Add if yes.
                {
                    exprs = string.Empty;
                    queue.Enqueue(token);
                    return;
                }
                else
                    throw new Exception("Unexpected Bool error in " + token);
            }
        }

        private void GetTextOrDate(ref Queue<string> queue, ref string exprs)
        {
            TrimDelim(ref exprs);

            //If the first char in the exprs string is a " then it's either a date, or a text literal.
            if (exprs[0] == c_QuoteDbl[0])
            {
                Regex regex = new Regex(c_QuoteCapRegex,
                                        RegexOptions.Singleline);
                Match match = regex.Match(exprs);

                //If we have a match, add it to the queue.
                if (match.Success)
                {
                    string token = match.Groups[1].Value;
                    queue.Enqueue(token);

                    int length = token.Length;
                    //remove token plus double quotes from exprs.
                    exprs = exprs.Substring(length);
                }
            }
          
        }

        #endregion

        #region Convert Infix to Postfix

        private string ConvertToRPN()
        {

            //Implement Shunting Yard Algorithm.
            Stack<string> opStack = new Stack<string>();
            string output = string.Empty;
            string tokenNew = string.Empty;
            string tokenPop = string.Empty;
            int tokenPopPrec = 0;
            int tokenNewPrec = 0;

            while (this.queue.Count > 0)
            {
                tokenNew = this.queue.Dequeue();

                #region Just Add values to string

                if (IsValue(tokenNew))
                {
                    //Just add numbers.
                    output += tokenNew + c_Delim;
                    continue;
                }

                #endregion // Just add nums

                #region Push all Funcs and ParOs onto the opStack.
                if (IsFunc(tokenNew))
                {
                    opStack.Push(tokenNew);
                    continue;
                }

                #endregion //Push all Single Variable Functions onto the stack.

                #region If Token is Operator Push or Add.
                if (IsOp(tokenNew))
                {
                    //While the opStack has tokens.
                    while (opStack.Count > 0)
                    {
                        //If there is an Op token at the top.
                        string peek = opStack.Peek();

                        if (IsOp(peek))
                        {
                            tokenPop = opStack.Pop();

                            tokenPopPrec = GetPrec(tokenPop);
                            tokenNewPrec = GetPrec(tokenNew);

                            //If op token's precedence is equal? Or greater than equal?
                            //to the op token popped off the op stack
                            // add popped token onto string.
                            if (tokenNewPrec <= tokenPopPrec)
                            {   //Add popped opStack token onto output str
                                output += tokenPop + c_Delim;
                                continue;
                            }
                            else//Popped precedence is higher, replace on the stack.
                            {
                                opStack.Push(tokenPop);
                                break; 
                            }
                        }
                        else
                            break; //There is a func on the top of the stack.

                    }

                    //Finally, Push new token onto opStack. 
                    opStack.Push(tokenNew);
                    continue;
                }

                #endregion

                #region If Closing Parenth

                if (tokenNew == c_ParClo)
                {
                    //While opStack has tokens
                    while (opStack.Count > 0)
                    {
                        if (IsFunc(opStack.Peek())) // If it's a func
                        {
                            //Break to add.
                            break;
                        }
                        else
                        {
                            //Pop Op tokens onto output string.
                            tokenPop = opStack.Pop();
                            output += tokenPop + c_Delim;
                        }
                    }

                    //Expect this is a Func or ParO, if it's not, we're missing an parOpen
                    if (!IsFunc(opStack.Peek()))
                        throw new Exception("Found closing parenth without and opening.");

                    //Add Func to output string. 
                    tokenPop = opStack.Pop();
                    if (tokenPop != c_ParO) //Discard ParO
                        output += tokenPop + c_Delim;
                }

                #endregion

            } // While the Queue has tokens to yard shunt.

            #region Add any remaining tokens in OpStack

            while (opStack.Count > 0)
            {
                tokenPop = opStack.Pop();

               // if (IsFunc(tokenPop) || tokenPop == c_ParClo)
                   // throw new Exception("Parentheses mismatch");

                output += tokenPop + c_Delim;
            }

            #endregion

            return output;

        }

        #endregion

        #region Evaluate

        public string Eval(string exprs)
        {
            exprs = RemoveInvalidChars(exprs);

            try
            {
                ParseTokens(exprs);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            //Return que as RPN format string.
            string rpnExprs = this.ConvertToRPN();

            return this.EvalRPN(rpnExprs);

        }

        #endregion

        #region Utility

        /// <summary>
        /// Clears the Queue
        /// </summary>
        private void ClearQueue()
        {
            this.queue = new Queue<string>();
        }

        private string RemoveInvalidChars(string exprs)
        {
            exprs = exprs.Replace('\n', ' ');
            exprs = exprs.Replace('\r', ' ');
            exprs = exprs.Replace('\t', ' ');
            //exprs = exprs.Insert(exprs.Length, "\n");
            return exprs;
        }

        #endregion

    }
}
