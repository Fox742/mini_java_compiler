using System.Collections.Generic;


public enum NonTerminal
{
    Programm,
    MainClass,
    ClassDecl,
    ClassDectIter,
    VarDecl,
    VarDeclIter,
    MethodDecl,
    MethodDeclIter,
    FormalList,
    FormalRestIter,
    FormalRest,
    Type,
    Statement,
    StatementIter,
    Exp,
    ExpList,
    ExpRestIter,
    ExpRest,
    Operation,
    Equal,
    Less,
    More,
    NotEqual,
    Summ,
    Substract,
    Multiply,
    Divide,
    And,
    Or,
    Not,
    ExpLeft

}

public static class Grammar
{
    // Класс грамматики языка. Грамматика состоит из списка правил, каждое из которых состоит из списка альтернатив
    //    Например, есть оператор ветвления if. Оператор if может выглядеть в коде как .
    //                                 if Statement
    //                                          Expression
    //    Так и:
    //                                 if Statement
    //                                          Expression
    //                                 Else
    //                                          Expression
    // Значит, у правила, соответствующего оператору if будет две альтернативы:
    //
    //     if Operator -> if Statement Expression                   Первая альтернатива
    //                 -> if Statement Expression else Expression   Вторая альтернатива


    
    public static List<Rule> Rules = new List<Rule>();

    public static void InitGrammar()
    {
        // В данной функции собираем все альтернативы для каждого правила

        Rule NewRule;
        Alternative NewAlternative;


        //--------Programm
        #region Programm
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.Programm);

        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add( new GrammarSymbol(NonTerminal.MainClass));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ClassDectIter));

        NewRule.Alternatives.Add(NewAlternative);
        Rules.Add(NewRule);
        #endregion
        //--------------------


        //---------- MainClass
        #region MainClass
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.MainClass);

        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_CLASS));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.ID));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_P_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_PUBLIC));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_STATIC));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_VOID));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_MAIN));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_R_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_STRING));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_S_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_S_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.ID));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_R_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_P_BRACKET));

        //--------------------------
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.VarDeclIter));
        //--------------------------

        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.StatementIter));
        
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_P_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_P_BRACKET));

        NewRule.Alternatives.Add(NewAlternative);
        Rules.Add(NewRule);
        #endregion
        //-----------------------


        //----------- ClassDecl
        #region ClassDecl
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.ClassDecl);

        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_CLASS));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.ID));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_P_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.VarDeclIter));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.MethodDeclIter));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_P_BRACKET));


        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();


        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_CLASS));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.ID));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_EXTENDS));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.ID));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_P_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.VarDeclIter));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.MethodDeclIter));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_P_BRACKET));


        NewRule.Alternatives.Add(NewAlternative);

        Rules.Add(NewRule);
        #endregion
        //-----------------------


        //----------- ClassDectIter
        #region ClassDeclIter
        NewRule = new Rule();

        NewRule.Left = new GrammarSymbol(NonTerminal.ClassDectIter);

        NewAlternative = new Alternative();

        NewRule.Alternatives.Add(NewAlternative);
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ClassDecl));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ClassDectIter));
        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.Epsilon));
        
        NewRule.Alternatives.Add(NewAlternative);

        Rules.Add(NewRule);
        #endregion
        //-----------------------
        
        
        //----------- VarDecl
        #region VarDecl
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.VarDecl);

        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Type));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.ID));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.SEMICOLON));

        NewRule.Alternatives.Add(NewAlternative);

    

        Rules.Add(NewRule);
        #endregion
        //-----------------------
        
        
        //----------- MethodDecl
        #region MethodDecl
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.MethodDecl);

        NewAlternative = new Alternative();


        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_PUBLIC));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Type));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.ID));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_R_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.FormalList));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_R_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_P_BRACKET));

        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.VarDeclIter));

        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.StatementIter));

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_RETURN));

        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.SEMICOLON));


        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_P_BRACKET));







        NewRule.Alternatives.Add(NewAlternative);

        
        Rules.Add(NewRule);
        #endregion
        //-----------------------
        
        
        //----------- MethodDeclIter
        #region MethodDeclIter
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.MethodDeclIter);

        NewAlternative = new Alternative();

        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.MethodDecl));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.MethodDeclIter));

        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.Epsilon));

        NewRule.Alternatives.Add(NewAlternative);

        Rules.Add(NewRule);
        #endregion
        //-----------------------
        
        
        //----------- FomalList
        #region FormalList
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.FormalList);

        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Type));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.ID));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.FormalRestIter));
      
        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.Epsilon));
        NewRule.Alternatives.Add(NewAlternative);

        Rules.Add(NewRule);
        #endregion
        //-----------------------
        
        
        //----------- FormalRestIter
        #region FormalRestIter
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.FormalRestIter);

        NewAlternative = new Alternative();

        NewRule.Alternatives.Add(NewAlternative);
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.FormalRest));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.FormalRestIter));

        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.Epsilon));
        NewRule.Alternatives.Add(NewAlternative);

        Rules.Add(NewRule);

        #endregion
        //-----------------------
        
        
        //----------- FormalRest
        #region FormalRest
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.FormalRest);

        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.COMMA));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Type));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.ID));


        NewRule.Alternatives.Add(NewAlternative);

        //NewAlternative = new Alternative();
        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.Epsilon));
        //NewRule.Alternatives.Add(NewAlternative);

        Rules.Add(NewRule);
        #endregion
        //-----------------------
        
        
        //----------- Type
        #region Type
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.Type);

        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_INT));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_S_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_S_BRACKET));      

        NewRule.Alternatives.Add(NewAlternative);



        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_BOOLEAN));

        NewRule.Alternatives.Add(NewAlternative);


        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_INT));

        NewRule.Alternatives.Add(NewAlternative);



        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.ID));

        NewRule.Alternatives.Add(NewAlternative);

        Rules.Add(NewRule);
        #endregion
        //-----------------------

        
        //----------- Statement
        #region Statement
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.Statement);




        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_P_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.StatementIter));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_P_BRACKET)); 

        NewRule.Alternatives.Add(NewAlternative);





        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_IF));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_R_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_R_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Statement));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_ELSE));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Statement));


        NewRule.Alternatives.Add(NewAlternative);




        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_WHILE));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_R_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_R_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Statement));
        NewRule.Alternatives.Add(NewAlternative);




        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_SYSTEM));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.POINT));

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_OUT));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.POINT));

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_PRINTLN));

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_R_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_R_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.SEMICOLON));
        NewRule.Alternatives.Add(NewAlternative);





        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.ID));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_EQUAL));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.SEMICOLON));


        NewRule.Alternatives.Add(NewAlternative);





        NewAlternative = new Alternative();
        
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.ID));

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_S_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_S_BRACKET));
        
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_EQUAL));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.SEMICOLON));

        NewRule.Alternatives.Add(NewAlternative);





        Rules.Add(NewRule);
        #endregion

        //-----------------------


        //----------- StatementIter
        #region StatementIter
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.StatementIter);

        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Statement));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.StatementIter));

        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.Epsilon));
        NewRule.Alternatives.Add(NewAlternative);

        Rules.Add(NewRule);
        #endregion
        //-----------------------
 
        
        //----------- Exp
        #region Exp
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.Exp);

        //NewAlternative = new Alternative();
        //NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));
        //NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Operation));
        //NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));
        //NewRule.Alternatives.Add(NewAlternative);

        //NewAlternative = new Alternative();
        //NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));
        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_S_BRACKET));
        //NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));
        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_S_BRACKET)); 
        //NewRule.Alternatives.Add(NewAlternative);

        //NewAlternative = new Alternative();
        //NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));
        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.POINT));
        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_LENGTH));
        //NewRule.Alternatives.Add(NewAlternative);

        //NewAlternative = new Alternative();
        //NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));
        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.POINT));
        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.ID));
        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_R_BRACKET));
        //NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ExpList));
        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_R_BRACKET));
        //NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.INTEGER_LITERAL));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ExpLeft));
        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_TRUE));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ExpLeft));
        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_FALSE));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ExpLeft));
        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.ID));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ExpLeft));
        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_THIS));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ExpLeft));
        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_NEW));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_INT));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_S_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_S_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ExpLeft));
        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_NEW));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.ID));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_R_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_R_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ExpLeft));
        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.NEGATION));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ExpLeft));

        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_R_BRACKET));


        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_R_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ExpLeft));

        NewRule.Alternatives.Add(NewAlternative);

        Rules.Add(NewRule);
        #endregion
        //-----------------------



        //----------- ExpLeft
        #region ExpLeft
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.ExpLeft);


        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Operation));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ExpLeft));
        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_S_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_S_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ExpLeft));
        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.POINT));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.KW_LENGTH));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ExpLeft));
        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.POINT));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.ID));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_R_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ExpList));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_R_BRACKET));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ExpLeft));
        NewRule.Alternatives.Add(NewAlternative);


        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.Epsilon));
        NewRule.Alternatives.Add(NewAlternative);

        Rules.Add(NewRule);
        #endregion
        //---------------




        //----------- ExpList
        #region ExpList
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.ExpList);

        NewAlternative = new Alternative();
      
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ExpRestIter));

        NewRule.Alternatives.Add(NewAlternative);
        
        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.Epsilon));
        NewRule.Alternatives.Add(NewAlternative);

        Rules.Add(NewRule);
        #endregion
        //-----------------------

        //----------- ExpRestIter
        #region ExpRestIter
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.ExpRestIter);

        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ExpRest));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.ExpRestIter));

        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.Epsilon));

        NewRule.Alternatives.Add(NewAlternative);

        Rules.Add(NewRule);
        #endregion
        //-----------------------

        
        //----------- ExpRest
        #region ExpRest
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.ExpRest);

        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.COMMA));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Exp));

        NewRule.Alternatives.Add(NewAlternative);


        Rules.Add(NewRule);
        #endregion
        //-----------------------


        //----------- VarDeclIter
        #region VarDeclIter
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.VarDeclIter);

        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.VarDecl));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.VarDeclIter));

        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();

        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.Epsilon));

        NewRule.Alternatives.Add(NewAlternative);


        Rules.Add(NewRule);
        #endregion
        //-----------------------


        //----------Operation
        #region Operation

        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.Operation);

        NewAlternative = new Alternative();

        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_EQUAL));
        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_EQUAL));

        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Equal));
        NewRule.Alternatives.Add(NewAlternative);
        
        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_C_BRACKET));
        


        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Less));
        NewRule.Alternatives.Add(NewAlternative);
        

        
        NewAlternative = new Alternative();
        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_C_BRACKET));
        
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.More));
        NewRule.Alternatives.Add(NewAlternative);
        
        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.NEGATION));
        
        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_EQUAL));
        
        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_EQUAL));

         NewAlternative = new Alternative();
         NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.NotEqual));
         NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();

        
        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_PLUS));
        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Summ));


        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();

                //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_MINUS));

                NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Substract));

        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();

        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_MULTIPLY));

                NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Multiply));

        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();

        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_SLASH));

                NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Divide));

        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();

        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_AND));

                NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.And));

        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();

        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.NEGATION));

                NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Not));

        NewRule.Alternatives.Add(NewAlternative);

        NewAlternative = new Alternative();

        //NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_OR));

        NewAlternative.Symbols.Add(new GrammarSymbol(NonTerminal.Or));

        NewRule.Alternatives.Add(NewAlternative);

        Rules.Add(NewRule);



        #endregion
        //-----------------------
        

        // Операции
        //----------
        #region Операции
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.Equal);

        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_EQUAL));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_EQUAL));
        NewRule.Alternatives.Add(NewAlternative);
        Rules.Add(NewRule);

        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.Less);
        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.O_C_BRACKET));
        NewRule.Alternatives.Add(NewAlternative);
        Rules.Add(NewRule);

        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.More);
        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.C_C_BRACKET));
        NewRule.Alternatives.Add(NewAlternative);
        Rules.Add(NewRule);

        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.NotEqual);
        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.NEGATION));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_EQUAL));
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_EQUAL));
        NewRule.Alternatives.Add(NewAlternative);

        Rules.Add(NewRule);
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.Summ);
        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_PLUS));
        NewRule.Alternatives.Add(NewAlternative);
        Rules.Add(NewRule);
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.Substract);
        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_MINUS));
        NewRule.Alternatives.Add(NewAlternative);
        Rules.Add(NewRule);
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.Multiply);
        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_MULTIPLY));
        NewRule.Alternatives.Add(NewAlternative);
        Rules.Add(NewRule);
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.Divide);
        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_SLASH));
        NewRule.Alternatives.Add(NewAlternative);
        Rules.Add(NewRule);
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.And);
        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_AND));
        NewRule.Alternatives.Add(NewAlternative);
        Rules.Add(NewRule);
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.Not);
        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.NEGATION));
        NewRule.Alternatives.Add(NewAlternative);
        Rules.Add(NewRule);
        NewRule = new Rule();
        NewRule.Left = new GrammarSymbol(NonTerminal.Or);
        NewAlternative = new Alternative();
        NewAlternative.Symbols.Add(new GrammarSymbol(TokenType.OP_OR));
        NewRule.Alternatives.Add(NewAlternative);
        Rules.Add(NewRule);


        #endregion
        //-----------


    }

}


public class GrammarSymbol
{
    bool _IsTerminal;

    public bool IsTerminal
    {
        get { return _IsTerminal; }
        set { _IsTerminal = value; }
    }


    NonTerminal _TypeNTerminal;

    public NonTerminal TypeNTerminal
    {
        get { return _TypeNTerminal; }
        set { _TypeNTerminal = value; }
    }

    TokenType _TypeToken;

    public TokenType TypeToken
    {
        get { return _TypeToken; }
        set { _TypeToken = value; }
    }

    public GrammarSymbol(NonTerminal NTerminal)
    {
        _IsTerminal = false;
        _TypeNTerminal = NTerminal;

    }

    public GrammarSymbol(TokenType Terminal)
    {
        _IsTerminal = true;
        _TypeToken = Terminal;

    }
}

public class Rule
{
    public GrammarSymbol Left;
    public List<Alternative> Alternatives = new List<Alternative>();


}

public class Alternative
{
    public List<GrammarSymbol> Symbols = new List<GrammarSymbol>();
}