// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");

while (true)
{
    Console.WriteLine("Provide Expression");
    string exp = Console.ReadLine();
    double output = EvaluateExpression(exp);
    Console.WriteLine(output);
}





static double EvaluateExpression(string expression)
{
    List<string> postfix = ConvertToPostFix(expression);
    return EvaluatePostFix(postfix);
}


// converting to postfix
static List<string> ConvertToPostFix(string expression)
{
    Stack<char> operators = new Stack<char>();
    List<string> output = new List<string>();
    StringBuilder number = new StringBuilder(); 

    for(int i=0;i<expression.Length; i++)
    {
        char c = expression[i];

        if(char.IsDigit(c) || c == '.')
        {
            number.Append(c);
        }
        else
        {
            if (number.Length > 0)
            {
                output.Add(number.ToString());
                number.Clear();
            }
            if (c == '(')
            {
                operators.Push(c);
            }else if(c == ')')
            {
                while(operators.Count>0 && operators.Peek() != '(')
                {
                    output.Add(operators.Pop().ToString());
                }
                if(operators.Count == 0 || operators.Pop()!='(')
                {
                    throw new Exception("mismatched expression");
                }
            }else if (IsOperator(c))
            {
                while(operators.Count>0 && Precedence(operators.Peek()) >= Precedence(c))
                   output.Add(operators.Pop().ToString());
                operators.Push(c);
            }
        }
    }

    if(number.Length > 0)
    {
        output.Add(number.ToString());
    }

    while(operators.Count > 0)
    {
        char op = operators.Pop();

        if (op == '(' || op == ')') throw new Exception("mismatched expression.");

        output.Add(op.ToString());
    }

    return output;
}

static double EvaluatePostFix(List<string> postfix)
{
    foreach(string val in postfix)
    {
        Console.Write(val + " ");
    }
    Console.WriteLine();

    Stack<double> stack = new Stack<double>();

    foreach(string token in postfix)
    {
        if(double.TryParse(token, out double num))
        {
            stack.Push(num);
        }
        else if (IsOperator(token[0]))
        {
            if (stack.Count < 2) throw new Exception("invalid expression");

            double b = stack.Pop();
            double a = stack.Pop();

            stack.Push(ApplyOperator(a, b, token[0]));
        }
    }

    if (stack.Count != 1) throw new Exception("invalid expression");

    return stack.Pop();
}



static bool IsOperator(char c) => c == '+' || c == '-' || c == '/' || c == '*';

static int Precedence(char op)
{
    return op switch
    {
        '+' or '-' => 1,
        '*' or '/' => 2,
        _ => 0
    };
}

static double ApplyOperator(double a, double b, char op)
{
    return op switch
    {
        '+' => a + b,
        '-' => a - b,
        '*' => a * b,
        '/' => b != 0 ? a / b : throw new DivideByZeroException(),
        _ => throw new InvalidOperationException($"Unknown operator: {op}")
    };
}