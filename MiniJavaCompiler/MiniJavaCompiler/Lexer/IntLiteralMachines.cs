using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexer
{
    class IntLiteralMachines:BaseMachine // Целочисленный литерал
    {

        private bool isDigit(char InputSymbol)
        {
            return ((InputSymbol >= '0') && (InputSymbol <= '9'));
        }

        public IntLiteralMachines()
        {
            MachineType = TokenType.INTEGER_LITERAL;
        }

        public override void Step(char InputSymbol)
        {
            // В целочисленном литерале могут быть только цифры, а если мы встречаем не цифру, то это означает, что либо целочисленный литерал закончился
            if (CurrentState != State.Error)
            {
                if (isDigit(InputSymbol))
                {
                    CurrentState = State.Current;
                    RecognizedChain.Add(InputSymbol);
                }
                else
                {
                    if (RecognizedChain.Count == 0)
                    {
                        CurrentState = State.Error;
                    }
                    else
                    {
                        CurrentState = State.Finishing;
                    }
                }
            }


        }

    }
}
