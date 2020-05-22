
using Lexer;
using Parser;
using CodeGenerator;
using System.Windows.Forms;

public class Compiler
{
    LexAnalizer MiniJavaLexer=new LexAnalizer();
    SynAnalizier MiniJavaParser = new SynAnalizier();
    Generator CodeCreator = new Generator();

    bool Result;

    public void Compile (string [] Programm, DataGridView DGW, RichTextBox RTB, TreeView TW, TextBox FileName)
    {
        Result = true;
        try
        {
            TW.Nodes.Clear();
            DGW.Rows.Clear();
            RTB.Text = "";


            MiniJavaLexer = new LexAnalizer();
            MiniJavaParser = new SynAnalizier();

            //---------- Лексический анализ
            Result = MiniJavaLexer.Proceed(Programm, RTB);
            if (!Result) throw new System.Exception();
            MiniJavaLexer.Print(DGW);

            // -------- Синтаксический анализ
            Result = MiniJavaParser.Proceed(MiniJavaLexer.Tokens, RTB);
            if (!Result) throw new System.Exception();
            MiniJavaParser.Print(TW);

            // -------- Генерация кода
            CodeCreator.Proceed(MiniJavaParser.Tree,RTB,FileName.Text);

        }
        catch (System.Exception Exc)
        {

        }
    }

}