using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexer
{

    public enum State // Состояния автомата
    {
        Beginning,
        Finishing,
        Current,
        Error
    }

    interface iMachine // Интерфейс автомата
    {

        TokenType Type(); // Тип токена, распозаваемого автоматом

        string Chain();

        void Step(char InputSymbol); // Подача символа на вход

        bool Recognize(); // Распознал ли автомат цепочку, соответствующую типу своего токена

        void Reset();  // Перевод в состояние Beginning

        State GetState();   // Получаем состояние автомата

    }
}
