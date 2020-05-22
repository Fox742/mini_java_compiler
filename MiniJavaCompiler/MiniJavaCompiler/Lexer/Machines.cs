using System.Collections.Generic;



namespace Lexer
{

    public abstract class BaseMachine:iMachine
    {

        protected List<char> RecognizedChain = new List<char>(); // Распознанная автоматом цепочка

        protected State CurrentState = State.Beginning;          // Текущее состояние

        protected TokenType MachineType;

        public virtual void Reset()
        {
            ResetRecognizedChain();
            CurrentState = State.Beginning;
        }

        public void ResetRecognizedChain()
        {
            RecognizedChain = new List<char>();
        }

        public int RecognizedChainLength
        {
            get
            {
                return RecognizedChain.Count;
            }
        }

        public State GetState()
        {
            return CurrentState;
        }

        public string Chain()
        {
            return new string(RecognizedChain.ToArray());
        }

        public bool Recognize()
        {
            return (CurrentState==State.Finishing);
        }

        public abstract void Step(char InputSymbol);

        public TokenType Type()
        {
            return MachineType;
        }

    }

    public class FixChainMachine:BaseMachine // Автомат, распознающий факсированные термы (Напрмиер, ключевые слова - 'class', 'while', 'for' и т.д.)
    {
        private string RecognizingChain; // Цепочка, которую должен распознать автомат. Если он не распознаёт такую цепочку - он должен перейти в состояние "ошибка"
                                         //        в отличие RecognizedChain базового класса, содержащей распознанную цепочку

        int ChainPointer;

        public FixChainMachine(TokenType _Type, string _RecognizingString)
        {
            ChainPointer = 0;
            MachineType = _Type;
            RecognizingChain = _RecognizingString;
        }

        private bool isDigit(char InputSymbol)
        {
            return ((InputSymbol >= '0') && (InputSymbol <= '9'));
        }

        private bool isLetter(char InputSymbol)
        {
            return (((InputSymbol >= 'a') && (InputSymbol <= 'z')) ||
                ((InputSymbol >= 'A') && (InputSymbol <= 'Z')));
        }

        public override void Step(char InputSymbol)
        {
            // 1. Распознаваемая цепочка не должна превышать длину шаблона (RecognizingChain)
            // 2. Последовательность символов во входной строке должна быть строго такой же как в шаблоне
            if (CurrentState!=State.Error)
            {
                if (ChainPointer < RecognizingChain.Length)
                {
                    if (RecognizingChain[ChainPointer] == InputSymbol)
                    {
                        ChainPointer++;
                        CurrentState = State.Current;
                        RecognizedChain.Add(InputSymbol);
                    }
                    else
                    {
                        CurrentState = State.Error;
                    }
                }
                else
                {
                    if (ChainPointer == RecognizingChain.Length)
                        CurrentState = State.Finishing;
                }
            }


        }

        public override void Reset()
        {
            ChainPointer = 0;
            base.Reset();
        }
    }



}