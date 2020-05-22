using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Lexer
{
    public class LexAnalizer
    {
        List<BaseMachine> Recogniziers = new List<BaseMachine>();

        List<BaseMachine> FinishRecognizing;

        int CurrentString;

        public List<Token> Tokens = new List<Token>();

        string ProgrammText;

        List<int> LinespositionsMathcing;

        public LexAnalizer()
        {
            Recogniziers.Add(new IDMachines());

            Recogniziers.Add(new FixChainMachine(TokenType.KW_CLASS,"class"));

            Recogniziers.Add(new FixChainMachine(TokenType.KW_PUBLIC, "public"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_STATIC, "static"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_VOID, "void"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_MAIN, "main"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_STRING, "String"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_EXTENDS, "extends"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_RETURN, "return"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_INT, "int"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_BOOLEAN, "boolean"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_IF, "if"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_ELSE, "else"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_WHILE, "while"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_SYSTEM, "System"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_OUT, "out"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_PRINTLN, "println"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_LENGTH, "length"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_TRUE, "true"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_FALSE, "false"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_THIS, "this"));
            Recogniziers.Add(new FixChainMachine(TokenType.KW_NEW, "new"));
            Recogniziers.Add(new FixChainMachine(TokenType.SEMICOLON, ";"));
            Recogniziers.Add(new FixChainMachine(TokenType.O_R_BRACKET, "("));
            Recogniziers.Add(new FixChainMachine(TokenType.C_R_BRACKET, ")"));
            Recogniziers.Add(new FixChainMachine(TokenType.O_S_BRACKET, "["));
            Recogniziers.Add(new FixChainMachine(TokenType.C_S_BRACKET, "]"));
            Recogniziers.Add(new FixChainMachine(TokenType.O_P_BRACKET, "{"));
            Recogniziers.Add(new FixChainMachine(TokenType.C_P_BRACKET, "}"));
            Recogniziers.Add(new FixChainMachine(TokenType.COMMA, ","));
            Recogniziers.Add(new FixChainMachine(TokenType.POINT, "."));
            Recogniziers.Add(new FixChainMachine(TokenType.NEGATION, "!"));
            Recogniziers.Add(new FixChainMachine(TokenType.OP_AND, "&&"));
            Recogniziers.Add(new FixChainMachine(TokenType.O_C_BRACKET, "<"));
            Recogniziers.Add(new FixChainMachine(TokenType.C_C_BRACKET, ">"));
            Recogniziers.Add(new FixChainMachine(TokenType.OP_PLUS, "+"));
            Recogniziers.Add(new FixChainMachine(TokenType.OP_MULTIPLY, "*"));
            Recogniziers.Add(new FixChainMachine(TokenType.OP_EQUAL, "="));
            Recogniziers.Add(new FixChainMachine(TokenType.OP_MINUS, "-"));
            Recogniziers.Add(new FixChainMachine(TokenType.OP_SLASH, "/"));
            Recogniziers.Add(new FixChainMachine(TokenType.OP_OR, "||"));
            Recogniziers.Add(new IntLiteralMachines());
        }

        private void Step(char InputSymbol)
        {
            // Функция подаёт на каждый из автоматов символ, сравнивает состояния автомата до и после обработки символа автоматом и в случае, если автомат перешёл в конечное состояние - 
            //           добавляет этот автомат в массив автоматов, распознавших цепочку (FinishRecognizing)
            State Before, After;
            foreach (BaseMachine Machine in Recogniziers)
            {
                Before = Machine.GetState();
                Machine.Step(InputSymbol);
                After = Machine.GetState();

                if ((After == State.Finishing) && (Before != State.Finishing))
                    FinishRecognizing.Add(Machine);

            }
        }

        private BaseMachine FinishingAuto()
        {
            foreach (BaseMachine Machine in Recogniziers)
            {
                if (Machine.GetState() == State.Finishing)
                    return Machine;
            }
            return null;
        }

        private BaseMachine CurrentAuto()
        {
            foreach (BaseMachine Machine in Recogniziers)
            {
                if (Machine.GetState() == State.Current)
                    return Machine;
            }
            return null;
        }

        private void Reset()
        {
            foreach (BaseMachine Machine in Recogniziers)
                    Machine.Reset();
        }

        public void Print(DataGridView DGW)
        {
            foreach (Token OneToken in Tokens)
            {
                DGW.Rows.Add(OneToken.Type.ToString(),
                    "\""+OneToken.Value+"\"",
                    OneToken.StringNumber.ToString());
            }
        }

        public bool isSpace(char InputSymbol)
        {
            return ((InputSymbol == ' ') || (InputSymbol == '\t') ||
                (InputSymbol == '\r') || (InputSymbol == '\n'));
        }

        public bool IsBeginCurrent()
        {
            foreach (BaseMachine Machine in Recogniziers)
            {
                if ((Machine.GetState()==State.Beginning)||(Machine.GetState()==State.Current))
                return false;
            }
            return true;
        }

        public bool Proceed(string[] Programm, RichTextBox RTB)
        {
            RTB.Text = "";

           // Группируем пробельные символы. Вместо каждой из групп пробельных символов - мы ставим один пробел
            ProgrammText = TreatSpaces(Programm);
            
            try
            {
                // Собственно разбор
                Tokens = new List<Token>();
                Analysing(ProgrammText, RTB);
            }
            catch (Exception Exc)
            {
                return false;

            }
            return true;
        }

        private int RowByPosition(string InPutString, int Position)
        {
            // Определяем в какой строке в исходной программе раположен символ по его позиции в нормализованной цепочке
            int Result=0;

            // Символ находится в последней строке
            if (Position > LinespositionsMathcing[LinespositionsMathcing.Count - 1])
                Result = LinespositionsMathcing.Count - 1;
            else
            {
                // Символ находится в в первой строке
                if (Position < LinespositionsMathcing[0])
                    Result = 0;
                else
                {
                    // Идем по списку позиций, в которых начинаются новые строки и проверяем, между какой из позиций находится Position
                    for (int i = 0; i < LinespositionsMathcing.Count - 1; i++)
                    {
                        if ((LinespositionsMathcing[i] > Position) && (Position <= LinespositionsMathcing[i + 1]))
                        {
                            Result = i;
                            break;
                        }
                    }
                }
            }


            return Result;
        }

        private void Analysing(string ToAnalize, RichTextBox RTB)
        {
            int Pointer=0;
            int CurrentPointer;
            Token NewToken;
            char [] Temp;
            int RowNumber;

            while (Pointer < ToAnalize.Length)
            {
                // Отбрасываем пробельные символы перед термом
                while (isSpace((ToAnalize[Pointer]))&&(Pointer<ToAnalize.Length))
                    Pointer++;

                CurrentPointer = Pointer;

                // Список в котором мы будем сохранять, которые распознали цепочку
                FinishRecognizing = new List<BaseMachine>();
                Reset();

                // Получаем номер строки входного текста программы, на символ из которой указывает Pointer (не забываем, что мы сгруппировали пробелы, сохранив в каких позициях начинаются строки в фукции TreatSpaces)
                RowNumber = RowByPosition(ToAnalize,Pointer);
                while (!IsBeginCurrent())
                {
                    // Пока есть автоматы, находящиеся в состоянии "Начальное" или "Промежуточное" - читаем символ из текста программы (из которого выкинули дополнительный пробелы)
                    Step(ToAnalize[CurrentPointer]);
                    CurrentPointer++;

                    // Если мы дошли до конца нормализованного текста - надо подать нейтральный символ, чтобы все автоматы могли перейти из промежуточного состояния в конечное состояние
                    if (CurrentPointer == ToAnalize.Length)
                    {
                        Step('\0');
                        break;
                    }

                }

                if (FinishRecognizing.Count != 0)
                {
                    // Если есть автоматы, которые находятся в конечном состоянии, то у нас есть новый терм, который мы должны записать в выходной список
                    Temp = new char[FinishRecognizing[FinishRecognizing.Count - 1].RecognizedChainLength];
                    
                    // Создаём новый класс токена
                    NewToken = new Token();
                    NewToken.Type = FinishRecognizing[FinishRecognizing.Count - 1].Type();

                    // Заполняем его - запоминаем его тип, значение, номер строки в котором мы его встретили
                    ToAnalize.CopyTo(Pointer, Temp, 0, FinishRecognizing[FinishRecognizing.Count - 1].RecognizedChainLength);
                    NewToken.Value = new string(Temp);
                    NewToken.StringNumber = RowNumber;
                    Tokens.Add(NewToken);

                    // Передвигаем указатель на символ в разбираемом тексте
                    Pointer = Pointer + FinishRecognizing[FinishRecognizing.Count - 1].RecognizedChainLength;

                }
                else
                {
                    // Если нет автоматов, находящихся в конечном состояни, то это значит, что цепочка не распознана
                    Tokens = new List<Token>();
                    RTB.Text += "Синтаксическая ошибка в строке: " + RowNumber.ToString();
                    throw new System.Exception();
                }
            }
            RTB.Text += "Лексический анализ прошёл успешно!";

        }

        private string TreatSpaces(string [] Lines)
        {
            List<char> Result=new List<char>();
            bool SpaceMode = false;
            LinespositionsMathcing = new List<int>();

            for (int i = 0; i < Lines.Length; i++)
            {
                for (int j = 0; j < Lines[i].Length; j++)
                {
                    if (SpaceMode)
                    {
                        if (!isSpace(Lines[i][j]))
                        {
                            SpaceMode = false;
                        }
                    }
                    else
                    {
                        if (isSpace(Lines[i][j]))
                        {
                                SpaceMode = true;
                                Result.Add(' ');
                        }
                    }
                    if (!SpaceMode)
                        Result.Add(Lines[i][j]);
                }
                LinespositionsMathcing.Add(Result.Count);// Сохраняем в специальный массив текущую длину массива Results для того,
                // чтобы при разборе можно было бы определить - в какой строкне находится текущий символ. Нужно для вывода ошибок
            }
            return new string(Result.ToArray());
        }
    }
}
