// See https://aka.ms/new-console-template for more information

using CompressionTool.Models;
using System.Text;

string filePath = @"C:\Users\Pranay More\OneDrive\Desktop\.Net\Coding challenges\CompressionTool\Files\NormalFiles\TempFile.txt";

if (File.Exists(filePath))
{
    Console.WriteLine("File does exists");
    Console.WriteLine();
}
else
{
    Console.WriteLine("File does not exists");
    return;
}

using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

using StreamReader sr = new StreamReader(fs);

string content = sr.ReadToEnd();
Console.WriteLine(content);
Console.WriteLine();
Console.WriteLine($"The File content length is {content.Length}");


int[] occ = new int[128];

for (int i = 0; i < content.Length; i++)
{
    occ[content[i] - 1]++;
}

//for (int i = 0; i < 128; i++)
//{
//    if (occ[i] > 0)
//    {
//        Console.WriteLine($"{((char)(i + 1))} : {occ[i]}");
//    }
//}

PriorityQueue<Tree, int> pq = new PriorityQueue<Tree, int>();

int index = 0;
while (index < 128)
{
    if (occ[index] == 0)
    {
        index++;
        continue;
    }
    pq.Enqueue(new Tree(null, null, (char)(index + 1), occ[index],null), occ[index]);
    index++;
}

while (pq.Count > 1)
{
    Tree firstOne = pq.Dequeue();
    Tree secondOne = pq.Dequeue();

    int sum = firstOne.data + secondOne.data;
    Tree combinedTree = new Tree(firstOne, secondOne,null, sum,null);
    pq.Enqueue(combinedTree, sum);
}

Tree finalTree = pq.Peek();
StringBuilder sb = new StringBuilder();
string result="";

//AddBinary(finalTree, sb, 's');
//Console.WriteLine("s :" + result);
//char retrieveChar = FindChar(finalTree, "1001");
//Console.WriteLine($"The retrive char base on binary values is {retrieveChar}");
Console.WriteLine();
Console.WriteLine("The binaryContent is: ");
string binaryContent = ConvertToBinary(finalTree, content);
Console.WriteLine($"{binaryContent}");

Console.WriteLine();
Console.WriteLine("The originalContent is: ");
Console.WriteLine($"{ConvertBinaryToOriginal(finalTree, binaryContent)}");


//Creating new file to strore encrypted binaryy content
string[] splits = filePath.Split("/");
string fileName = splits[splits.Length - 1];
string binaryFilePath = @"C:\Users\Pranay More\OneDrive\Desktop\.Net\Coding challenges\CompressionTool\Files\BinaryFiles\TempFileBinary.txt";
File.WriteAllText(binaryFilePath, binaryContent);

















string ConvertToBinary(Tree tree, string content)
{

    int i = 0;
    StringBuilder binaryContent = new StringBuilder();

    while (i < content.Length)
    {
        StringBuilder sb = new StringBuilder();
        string binary = AddBinary(tree, sb, content[i]);
        binaryContent.Append(binary);
        i++;
    }
    return binaryContent.ToString();
}

string ConvertBinaryToOriginal(Tree tree, string binaryContent)
{
    StringBuilder originalContent = new StringBuilder();
    int i = 0;
    Tree curr = tree;

    while(i < binaryContent.Length)
    {
        if (binaryContent[i]=='0' && curr.left!=null)
        {
            curr = curr.left;
            if (curr.charVal != null)
            {
                originalContent.Append(curr.charVal);
                curr = tree;
            }
        }else if(binaryContent[i]=='1' && curr.right!=null)
        {
            curr = curr.right;
            if (curr.charVal != null)
            {
                originalContent.Append(curr.charVal);
                curr = tree;
            }
        }
        i++;
    }

    return originalContent.ToString();
}


string AddBinary(Tree tree, StringBuilder sb,char c)
{
    if (tree.charVal != null && tree.charVal == c)
    {
        tree.bitValue = sb.ToString();
        result = sb.ToString();
        return result;
    }
    else if (tree.charVal != null)
    {
        return result;
    }

    if (tree.left != null)
    {
        sb.Append('0');
        result = AddBinary(tree.left, sb, c);
        sb.Remove(sb.Length - 1, 1);
    }
    if (tree.right != null)
    {
        sb.Append('1');
        result = AddBinary(tree.right, sb, c);
        sb.Remove(sb.Length - 1, 1);
    }

    return result;
}

char FindChar(Tree tree, string s)
{
    int i = 0;
    while (i < s.Length)
    {
        if (s[i] == '0' && tree.left != null)
        {
            tree = tree.left;
        }else if(s[i] == '1' && tree.right != null)
        {
            tree = tree.right;
        }
        i++;
    }
    if (tree.charVal != null)
    {
        return (char)tree.charVal;
    }
    return '*';
}


//convert string to byte
byte[] ConvertBinaryStringToBytes(string binaryContent)
{
    int numBytes = (binaryContent.Length+7)/8;
    byte[] bytes = new byte[numBytes];
    int bitIndex = 0;

    for(int i=0; i<numBytes; i++) 
    {
        Byte b = 0;

        for(int bit = 0; bit < 8; bit++)
        {
            if (bitIndex<binaryContent.Length && binaryContent[bitIndex] == '1')
            {
                b |= (byte)(1 << (7-bit));
            }
            bitIndex++;
        }

        bytes[i] = b;
    }

    return bytes;
}


