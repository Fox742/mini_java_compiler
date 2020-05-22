
using System.Collections.Generic;  


    public class Node
    {
        public Node Parent = null;
        public List<Node>Nodes = new List<Node>();

        GrammarSymbol Symbol;


        public int TablePointer;

        int _TokensPointerBackUp;

        public int TokensPointerBackUp
        {
            get { return _TokensPointerBackUp; }
            set { _TokensPointerBackUp = value; }
        }

        public int ChildPointer;

        public bool IsTerminal
        {
            get { return Symbol.IsTerminal; }
            set { Symbol.IsTerminal=value; }
        }

        int _RuleIndex;

        public int RuleIndex
        {
            get { return _RuleIndex; }
            set { _RuleIndex = value; }
        }
        int _AlternativeIndex;

        public int AlternativeIndex
        {
            get { return _AlternativeIndex; }
            set { _AlternativeIndex = value; }
        }

        internal NonTerminal TypeNTerminal
        {
            get { return Symbol.TypeNTerminal; }
            set { Symbol.TypeNTerminal = value; }
        }

        public TokenType TypeToken
        {
            get { return Symbol.TypeToken; }
            set { Symbol.TypeToken = value; }
        }

        string _Value;

        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public Node(NonTerminal NTerminal)
        {

            Symbol = new GrammarSymbol(NTerminal);

            _RuleIndex = -1;
            _AlternativeIndex = -1;

        }

        public Node(TokenType Terminal)
        {

            Symbol = new GrammarSymbol(Terminal);

            _Value = Value;
        }
    }

