using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexer
{
    public class IDMachines:BaseMachine // Автомат, распознающий идентификаторы
    {
        private bool isDigit(char InputSymbol)
        {
            return ((InputSymbol >= '0') && (InputSymbol <= '9'));
        }

        private bool isLetter(char InputSymbol)
        {
            return (((InputSymbol>='a')&&(InputSymbol<='z'))||
                ((InputSymbol >= 'A') && (InputSymbol <= 'Z')));
        }

        private bool isUnderLine(char InputSymbol)
        {
            return (InputSymbol=='_');
        }

        public IDMachines()
        {
            MachineType = TokenType.ID;
        }

        public override void Step(char InputSymbol)
        {
            // Идентификаторы могут содержать буквы и цифры, но первый символ не может быть цифрой (иначе это не идентификатор, а целочисленное значение)
            if (CurrentState != State.Error)
            {
                if (CurrentState == State.Beginning)
                {
                    if (isLetter(InputSymbol))
                    {
                        RecognizedChain.Add(InputSymbol);
                        CurrentState = State.Current;
                    }
                    else
                        CurrentState = State.Error;
                }
                else
                {
                    if ((isDigit(InputSymbol)) ||
                        (isLetter(InputSymbol))
                        || (isUnderLine(InputSymbol)))
                    {
                        RecognizedChain.Add(InputSymbol);
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
