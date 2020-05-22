using Collections = System.Collections.Generic;
using Reflect = System.Reflection;
using Emit = System.Reflection.Emit;
using System.Windows.Forms;
using System.IO;

namespace CodeGenerator
{
    public class Generator
    {
        private Node _Tree;
        private RichTextBox _RTB;

        System.Type CurrentTopStackType;

        Emit.ModuleBuilder ModBuilder;
        Emit.AssemblyBuilder AsmBuilder;

        Emit.ILGenerator AssemblerGenerator = null;
        Collections.List<Collections.Dictionary<string, Emit.LocalBuilder>> SymbolTable;

        Collections.Dictionary<string, Emit.TypeBuilder> TypeTable;

        Collections.Dictionary<string, System.Type> CreatedTypeTable;

        Collections.List<Node> FormalTypes = new Collections.List<Node>();

        Collections.List<Node> FormalIds = new Collections.List<Node>();

        Collections.List<Node> ExpListTypes = new Collections.List<Node>();

        Collections.List<Node> ExpListIds = new Collections.List<Node>();

        Collections.List<Collections.List<string>>
            ParametersStack =
            new Collections.List<Collections.List<string>>();

        Emit.TypeBuilder CurrentType;

        bool IsReadingExpList = false;

        public Generator()
        {

        }

        // Главная функция кодогенератора. Получает на вход синтаксическое дерево, обходит его и в хое его обхода с
        //     помощью библиотеки System.Reflection строит исполняемый файл на MSIL, который потом можно запускать вне компилятора (однако, на компьютере должна быть установлена .Net Framwork)
        //     Путь к файлу задаётся в интерфейсе компилятора
        public bool Proceed(Node Tree, RichTextBox RTB, string FileName)
        {
            _Tree = Tree;
            _RTB = RTB;
            InitFileName(FileName);

            try
            {

                Reflect.AssemblyName Name = new Reflect.AssemblyName(FileName);
                AsmBuilder = System.AppDomain.CurrentDomain.DefineDynamicAssembly(Name, Emit.AssemblyBuilderAccess.Save);
                ModBuilder= AsmBuilder.DefineDynamicModule(FileName);
                TypeTable = new Collections.Dictionary<string, Emit.TypeBuilder>();
                CreatedTypeTable = new Collections.Dictionary<string, System.Type>();
                SymbolTable = new Collections.List<Collections.Dictionary<string, Emit.LocalBuilder>>();

                BeforeCompile(Tree);
                CreateCode(Tree);

                AsmBuilder.Save(FileName);
                this.SymbolTable = null;
                this.AssemblerGenerator = null;
                this.TypeTable = null;

                File.Move(FileName, FileName + ".exe");
                File.Delete(FileName);

                return true;
            }
            catch (System.Exception Exc)
            {
                _RTB.Text = Exc.Message;

                return false;
            }
        }

        private void CompileStatement(Node ActiveNode)
        {
            if (ActiveNode.Nodes[0].TypeToken == TokenType.KW_IF)
            {
                Emit.Label ElseLabel = this.AssemblerGenerator.DefineLabel();

                Emit.Label EndIfElseLabel = this.AssemblerGenerator.DefineLabel();

                // Вычисляем логическое значение
                CreateCode(ActiveNode.Nodes[2]);
                // Если false -  переход на метку "ИНАЧЕ"
                this.AssemblerGenerator.Emit(Emit.OpCodes.Brfalse, ElseLabel);

                // Разбираем блок "ЕСЛИ"
                CreateCode(ActiveNode.Nodes[4]);

                // Переход на метку "Конец блока иначе"
                this.AssemblerGenerator.Emit(Emit.OpCodes.Br, EndIfElseLabel);

                // Ставим метку "ИНАЧЕ"
                this.AssemblerGenerator.MarkLabel(ElseLabel);

                // Разбираем блок "Иначе"
                CreateCode(ActiveNode.Nodes[6]);

                // Ставим метку "Конец блока иначе"
                this.AssemblerGenerator.MarkLabel(EndIfElseLabel);
            }

            if (ActiveNode.Nodes[0].TypeToken==TokenType.KW_WHILE)
            {
                Emit.Label TestCycleLabel = this.AssemblerGenerator.DefineLabel();
                Emit.Label EndCycleLabel = this.AssemblerGenerator.DefineLabel();

                // Начало цикла
                AssemblerGenerator.MarkLabel(TestCycleLabel);

                // Вычисляем логическое выражение
                CreateCode(ActiveNode.Nodes[2]);

                // Если false переходим на конец цикла
                this.AssemblerGenerator.Emit(Emit.OpCodes.Brfalse, EndCycleLabel);

                // Тело цикла
                CreateCode(ActiveNode.Nodes[4]);


                // Переход на начало цикла
                this.AssemblerGenerator.Emit(Emit.OpCodes.Br, TestCycleLabel);
                // Метка конца цикла
                this.AssemblerGenerator.MarkLabel(EndCycleLabel);

            }

            if (ActiveNode.Nodes[0].TypeToken == TokenType.KW_SYSTEM)
            {
                CreateCode(ActiveNode.Nodes[6]);

                this.AssemblerGenerator.Emit(Emit.OpCodes.Call, 
                    typeof(System.Convert).GetMethod("ToString", new System.Type[] { typeof(int) }));

                this.AssemblerGenerator.Emit(Emit.OpCodes.Call, typeof(System.Console).GetMethod("WriteLine", new System.Type[] { typeof(string) }));
            }

            if (ActiveNode.Nodes[0].TypeToken == TokenType.O_P_BRACKET)
            {
                OpenBlockVariables();
                CreateCode(ActiveNode.Nodes[1]);
                CloseBlockVariables();
            }

            if ((ActiveNode.Nodes[0].TypeToken == TokenType.ID) && (ActiveNode.Nodes[1].TypeToken == TokenType.OP_EQUAL))
            {
                CreateCode(ActiveNode.Nodes[2]);
                Store(ActiveNode.Nodes[0].Value);

            }

            if ((ActiveNode.Nodes[0].TypeToken == TokenType.ID) && (ActiveNode.Nodes[1].TypeToken == TokenType.O_S_BRACKET))
            {
                Load(ActiveNode.Nodes[0].Value);

                CreateCode(ActiveNode.Nodes[2]);

                CreateCode(ActiveNode.Nodes[5]);

                AssemblerGenerator.Emit(Emit.OpCodes.Stelem_I4);

            }

        }


        private int checkStackParams(string ParamName)
        {
            if (ParametersStack.Count>0)
            {
                for (int i = 0; i < ParametersStack[ParametersStack.Count - 1].Count; i++)
                    if (ParametersStack[ParametersStack.Count - 1][i]
                        ==ParamName)
                        return i;
            }

            return -1;
        }

        private void Store(string Id)
        {
            
            int Position;
            if ((Position=checkStackParams(Id))>=0)
            {
                AssemblerGenerator.Emit(Emit.OpCodes.Starg,Position+1);
            }
            else
            {
                Emit.LocalBuilder LocVar = VariableBuilder(Id);
                if (LocVar==null)
                {
                    throw new System.Exception("Использование необъявленной переменной.");
                }
                else
                {

                    AssemblerGenerator.Emit(Emit.OpCodes.Stloc, LocVar);
                }
            }
        }

        private void CompileProgramm(Node ActiveNode)
        {

            CreateCode(ActiveNode.Nodes[1]);
            CreateCode(ActiveNode.Nodes[0]);
        }

        private void CompileExp(Node ActiveNode)
        {
            if (ActiveNode.Nodes[0].IsTerminal)
            {
                if (ActiveNode.Nodes[0].TypeToken==TokenType.INTEGER_LITERAL)
                {

                    this.AssemblerGenerator.Emit(Emit.OpCodes.Ldc_I4, System.Convert.ToInt32(ActiveNode.Nodes[0].Value));
                }

                if (ActiveNode.Nodes[0].TypeToken == TokenType.KW_TRUE)
                {
                    this.AssemblerGenerator.Emit(Emit.OpCodes.Ldc_I4, 1);
                }

                if (ActiveNode.Nodes[0].TypeToken == TokenType.KW_FALSE)
                {
                    this.AssemblerGenerator.Emit(Emit.OpCodes.Ldc_I4, 0);
                }

                if ((ActiveNode.Nodes[0].TypeToken == TokenType.KW_NEW) && (ActiveNode.Nodes[1].TypeToken == TokenType.ID))
                {

                    if (!TypeTable.ContainsKey(ActiveNode.Nodes[1].Value))
                    {
                        throw new System.Exception("Не найден тип " + ActiveNode.Nodes[1].Value);
                    }
                    else
                    {
                        this.AssemblerGenerator.Emit(Emit.OpCodes.Newobj, TypeTable[ActiveNode.Nodes[1].Value].GetConstructor(System.Type.EmptyTypes));
                        CreateCode(ActiveNode.Nodes[4]);
                    }

                }

                if ((ActiveNode.Nodes[0].TypeToken == TokenType.KW_NEW) && (ActiveNode.Nodes[2].TypeToken == TokenType.O_S_BRACKET))
                {
                    CreateCode(ActiveNode.Nodes[3]);
                    this.AssemblerGenerator.Emit(Emit.OpCodes.Newarr, TypeByTypeId( ActiveNode.Nodes[1]));
                }

                if (ActiveNode.Nodes[0].TypeToken == TokenType.O_R_BRACKET)
                {
                    CreateCode(ActiveNode.Nodes[1]);
                   
                }

                if (ActiveNode.Nodes[0].TypeToken == TokenType.ID)
                {
                    Load(ActiveNode.Nodes[0].Value);

                }
            }
            else
            {

            }

            CreateCode(ActiveNode.Nodes[ActiveNode.Nodes.Count-1]);
        }

        private void Load(string Id)
        {
            int Position=checkStackParams(Id);

            if (Position >= 0)
            {
                AssemblerGenerator.Emit(Emit.OpCodes.Ldarg, Position+1);
            }
            else
            {

                Emit.LocalBuilder LocVar = VariableBuilder(Id);

                if (LocVar == null)
                {
                    throw new System.Exception("Использование необъявленной переменной.");
                }
                else
                {
                    CurrentTopStackType = LocVar.LocalType;
                    AssemblerGenerator.Emit(Emit.OpCodes.Ldloc, LocVar);

                }
            }
        }

        private void CompileExpLeft(Node ActiveNode)
        {
            if (ActiveNode.Nodes[0].IsTerminal)
            {
                if (ActiveNode.Nodes[0].TypeToken != TokenType.Epsilon)
                {
                    if (ActiveNode.Nodes[0].TypeToken == TokenType.O_R_BRACKET)
                    {
                        CreateCode(ActiveNode.Nodes[1]);
                        AssemblerGenerator.Emit(Emit.OpCodes.Ldelem_I4);
                        CreateCode(ActiveNode.Nodes[3]);

                    }

                    if ((ActiveNode.Nodes[0].TypeToken == TokenType.POINT) && (ActiveNode.Nodes[1].TypeToken == TokenType.ID))
                    {
                        System.Type TypeCurrentMethod = CurrentTopStackType;
                        IsReadingExpList = true;


                        CreateCode(ActiveNode.Nodes[3]);

                        // Создаём вызов метода
                        AssemblerGenerator.Emit(Emit.OpCodes.Callvirt,CurrentTopStackType.GetMethod(ActiveNode.Nodes[1].Value));

                        CreateCode(ActiveNode.Nodes[5]);
                    }

                }

            }
            else
            {
                if (ActiveNode.Nodes[0].TypeNTerminal == NonTerminal.Operation)
                {

                    CreateCode(ActiveNode.Nodes[1]);
                    CreateCode(ActiveNode.Nodes[0]);
                    CreateCode(ActiveNode.Nodes[2]);

                }
            }
        }

        private void CompileFormalList(Node ActiveNode)
        {
            if (ActiveNode.Nodes[0].TypeToken != TokenType.Epsilon)
            {
                FormalTypes.Add(ActiveNode.Nodes[0]);

                FormalIds.Add(ActiveNode.Nodes[1]);

                ParametersStack[ParametersStack.Count - 1].Add(ActiveNode.Nodes[1].Value);

                CreateCode(ActiveNode.Nodes[2]);
            }
        }

        private void CompileFormalRest(Node ActiveNode)
        {
            if (ActiveNode.Nodes[0].TypeToken != TokenType.Epsilon)
            {

                FormalTypes.Add(ActiveNode.Nodes[1]);
                FormalIds.Add(ActiveNode.Nodes[2]);

                ParametersStack[ParametersStack.Count - 1].Add(ActiveNode.Nodes[2].Value);
            }
        }

        private void OpenParametersBlock()
        {
            Collections.List<string> NewBlock = new Collections.List<string>();

            ParametersStack.Add(NewBlock);

        }

        private void CloseParametersBLock()
        {
            ParametersStack.RemoveAt(ParametersStack.Count-1);
        }


        private void CompileMethodDecl(Node ActiveNode)
        {

            OpenBlockVariables();
            OpenParametersBlock();
            // Разбираем список переменных
            CreateCode(ActiveNode.Nodes[4]);

            // Создаём объявление метода
            Emit.MethodBuilder Method;


            if (FormalIds.Count == 0)
            {
                Method = CurrentType.DefineMethod(ActiveNode.Nodes[2].Value, Reflect.MethodAttributes.Public,
                    TypeByTypeId(ActiveNode.Nodes[1]),
                    System.Type.EmptyTypes);


                System.Type Tst = System.Type.GetType(ActiveNode.Nodes[1].Nodes[0].Value);
            }
            else
            {

                Collections.List<System.Type> FormalParameters = new Collections.List<System.Type>();

                for (int i = 0; i < FormalIds.Count; i++)
                {

                    FormalParameters.Add(TypeByTypeId(FormalTypes[i]));
                }

                Method = CurrentType.DefineMethod(ActiveNode.Nodes[2].Value, Reflect.MethodAttributes.Public,
                        TypeByTypeId(ActiveNode.Nodes[1]),
                        FormalParameters.ToArray());
            }
            this.AssemblerGenerator = Method.GetILGenerator();
          
            // Обнуляем список формальных параметров
            FormalTypes = new Collections.List<Node>();
            FormalIds = new Collections.List<Node>();

            // Вызываем разбор объявлений переменных
            CreateCode(ActiveNode.Nodes[7]);
            

            // Вызываем разбор тела метода
            CreateCode(ActiveNode.Nodes[8]);
            CreateCode(ActiveNode.Nodes[10]);

            // Завершаем метод
            AssemblerGenerator.Emit(Emit.OpCodes.Ret);

            CloseBlockVariables();
            CloseParametersBLock();
        }


        private void CompileStatementIter(Node ActiveNode)
        {
            if (!ActiveNode.Nodes[0].IsTerminal)
            {
                CreateCode(ActiveNode.Nodes[0]);
                CreateCode(ActiveNode.Nodes[1]);
            }
        }

        private void OpenBlockVariables()
        {
            SymbolTable.Add(new Collections.Dictionary<string, Emit.LocalBuilder>());
        }

        private System.Type TypeByTypeId(Node TypeNode)
        {

            System.Type Result = null;
            if (!TypeNode.IsTerminal)
            {
                if (TypeNode.Nodes.Count > 1)
                {
                    // Массивы
                    if (TypeNode.Nodes[0].Value == "int")
                        return typeof(System.Int32[]);

                }
                else
                {
                    if (TypeNode.Nodes[0].Value == "int")
                        return System.Type.GetType("System.Int32");

                    if (TypeNode.Nodes[0].Value == "boolean")
                        return System.Type.GetType("System.Boolean");

                    if (CreatedTypeTable.ContainsKey(TypeNode.Nodes[0].Value))
                    {
                        return CreatedTypeTable[TypeNode.Nodes[0].Value];

                    }
                }
            }
            else
            {
                if (TypeNode.Value == "int")
                    return System.Type.GetType("System.Int32[]");
            }
            throw new System.Exception("Использование неопределённого типа");

        }
        

        private void CloseBlockVariables()
        {

            SymbolTable.RemoveAt(SymbolTable.Count-1);
        }

        private void AddVariable(string IdVar, Node TypeNode)
        {
            System.Type TypeVar = TypeByTypeId(TypeNode);
            SymbolTable[SymbolTable.Count - 1][IdVar]
                = this.AssemblerGenerator.DeclareLocal( TypeVar );

        }


        private Emit.LocalBuilder VariableBuilder(string Name)
        {
            for (int i = SymbolTable.Count - 1; i >= 0;i-- )
                if (SymbolTable[i].ContainsKey(Name))
                    return SymbolTable[i][Name];
                return null;
        }

        private void CompileOperation(Node ActiveNode)
        {
            Node OperationNode = ActiveNode.Nodes[0];

            if (OperationNode.TypeNTerminal == NonTerminal.Summ)
            {
                this.AssemblerGenerator.Emit(Emit.OpCodes.Add);
            }

            if (OperationNode.TypeNTerminal == NonTerminal.Substract)
            {
                this.AssemblerGenerator.Emit(Emit.OpCodes.Sub);
            }

            if (OperationNode.TypeNTerminal == NonTerminal.Multiply)
            {
                this.AssemblerGenerator.Emit(Emit.OpCodes.Mul);
            }

            if (OperationNode.TypeNTerminal == NonTerminal.Divide)
            {
                this.AssemblerGenerator.Emit(Emit.OpCodes.Div);
            }


            if (OperationNode.TypeNTerminal == NonTerminal.And)
            {
                this.AssemblerGenerator.Emit(Emit.OpCodes.And);
            }

            if (OperationNode.TypeNTerminal == NonTerminal.Or)
            {
                this.AssemblerGenerator.Emit(Emit.OpCodes.Or);
            }

            if (OperationNode.TypeNTerminal == NonTerminal.Not)
            {
                this.AssemblerGenerator.Emit(Emit.OpCodes.Not);
            }

            if (OperationNode.TypeNTerminal == NonTerminal.Equal)
            {
                this.AssemblerGenerator.Emit(Emit.OpCodes.Ceq);
            }

            if (OperationNode.TypeNTerminal == NonTerminal.NotEqual)
            {
                this.AssemblerGenerator.Emit(Emit.OpCodes.Ceq);
                this.AssemblerGenerator.Emit(Emit.OpCodes.Not);
            }

            if (OperationNode.TypeNTerminal == NonTerminal.Less)
            {
                this.AssemblerGenerator.Emit(Emit.OpCodes.Clt);
            }

            if (OperationNode.TypeNTerminal == NonTerminal.More)
            {
                this.AssemblerGenerator.Emit(Emit.OpCodes.Cgt);
            }



        }

        private void CompileVarDecl(Node ActiveNode)
        {
            AddVariable(ActiveNode.Nodes[1].Value, ActiveNode.Nodes[0]);
        }

        // Функция предкодогенерационного обхода синтаксического дерева
        //   Нужен для того, чтобы выполнить некоторую подготовительную работу. Например, задекларировать классы
        private void BeforeCompile(Node ActiveNode)
        {

            if (ActiveNode.IsTerminal)
            {

            }
            else
            {
                if (
                    (ActiveNode.TypeNTerminal == NonTerminal.MainClass) ||

                    (ActiveNode.TypeNTerminal == NonTerminal.ClassDecl))
                {


                    if (TypeTable.ContainsKey(ActiveNode.Nodes[1].Value))
                    {
                        throw new System.Exception("Переопределение класса " + ActiveNode.Nodes[1].Value);
                    }
                    else
                    {
                        Emit.TypeBuilder _TypeBuilder =
                            ModBuilder.DefineType(ActiveNode.Nodes[1].Value);

                        TypeTable[ActiveNode.Nodes[1].Value] = _TypeBuilder;

                        if (ActiveNode.TypeNTerminal != NonTerminal.MainClass)
                        _TypeBuilder.DefineDefaultConstructor(Reflect.MethodAttributes.Public);

                    }
                }


                foreach (Node Child in ActiveNode.Nodes)
                    BeforeCompile(Child);
            }

        }

        private void InitFileName(string FileName)
        {
            if (File.Exists(FileName+".exe"))
                File.Delete(FileName + ".exe");
        }


        private void CompileClassDecl(Node ActiveNode)
        {
            Emit.TypeBuilder _TypeBuilder = TypeTable[ActiveNode.Nodes[1].Value];
            CurrentType = _TypeBuilder;
            OpenBlockVariables();
            CreateCode(ActiveNode.Nodes[3]);
            CloseBlockVariables();
            CreateCode(ActiveNode.Nodes[4]);
            CreatedTypeTable[ActiveNode.Nodes[1].Value] = _TypeBuilder.CreateType();
        }

        private void CompileMainClass(Node ActiveNode)
        {
                Emit.TypeBuilder _TypeBuilder =
                    TypeTable[ActiveNode.Nodes[1].Value];

                Emit.MethodBuilder MethBuilder = _TypeBuilder.DefineMethod("Main", Reflect.MethodAttributes.Static, typeof(void), System.Type.EmptyTypes);
                this.AssemblerGenerator = MethBuilder.GetILGenerator();
                CurrentType = _TypeBuilder;
                OpenBlockVariables();
                CreateCode(ActiveNode.Nodes[14]);
                
            
                CreateCode(ActiveNode.Nodes[15]);
                CloseBlockVariables();
                // Отладочные команды
                AssemblerGenerator.Emit(Emit.OpCodes.Ldstr, "Programm was finished. Press Enter key to quit...");
                AssemblerGenerator.Emit(Emit.OpCodes.Call, 
                    typeof(System.Console).GetMethod("WriteLine", new System.Type[] { typeof(string) }));
                AssemblerGenerator.Emit(Emit.OpCodes.Call, 
                    typeof(System.Console).GetMethod("ReadLine", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, new System.Type[] { }, null));
                AssemblerGenerator.Emit(Emit.OpCodes.Pop);
                AssemblerGenerator.Emit(Emit.OpCodes.Ret);

                CreatedTypeTable[ActiveNode.Nodes[1].Value] = _TypeBuilder.CreateType();

                ModBuilder.CreateGlobalFunctions();
                AsmBuilder.SetEntryPoint(MethBuilder);
        }

        // Функция "CreateCode", получаюшая узел дерева и внутри себя вызывающая соответствующую узлу функцию компиляции.
        //     Таким образом в стеке вызовов функции компиляции будут чередоваться с вызовами функции CreateCode
        public void CreateCode(Node ActiveNode)
        {
            bool NotSkipChild = true;

            if (!ActiveNode.IsTerminal)
            {
                if (ActiveNode.TypeNTerminal == NonTerminal.MainClass)
                {
                    CompileMainClass(ActiveNode);
                    NotSkipChild = false;

                }

                if (ActiveNode.TypeNTerminal == NonTerminal.Programm)
                {
                    CompileProgramm(ActiveNode);
                    NotSkipChild = false;

                }

                if (ActiveNode.TypeNTerminal == NonTerminal.StatementIter)
                {
                    CompileStatementIter(ActiveNode);
                    NotSkipChild = false;
                }

                if (ActiveNode.TypeNTerminal == NonTerminal.Exp)
                {
                    CompileExp(ActiveNode);
                    NotSkipChild = false;
                }

                if (ActiveNode.TypeNTerminal == NonTerminal.ExpLeft)
                {
                    CompileExpLeft(ActiveNode);
                    NotSkipChild = false;
                }

                if (ActiveNode.TypeNTerminal == NonTerminal.Statement)
                {
                    CompileStatement(ActiveNode);
                    NotSkipChild = false;
                }

                if (ActiveNode.TypeNTerminal == NonTerminal.Operation)
                {
                    CompileOperation(ActiveNode);
                    NotSkipChild = false;
                }

                if (ActiveNode.TypeNTerminal == NonTerminal.MethodDecl)
                {
                    CompileMethodDecl(ActiveNode);
                    NotSkipChild = false;
                }

                if (ActiveNode.TypeNTerminal == NonTerminal.VarDecl)
                {
                    CompileVarDecl(ActiveNode);
                    NotSkipChild = false;
                }


                if (ActiveNode.TypeNTerminal == NonTerminal.FormalList)
                {
                    CompileFormalList(ActiveNode);
                    NotSkipChild = false;
                }

                if (ActiveNode.TypeNTerminal == NonTerminal.FormalRest)
                {
                    CompileFormalRest(ActiveNode);
                    NotSkipChild = false;
                }

                if (ActiveNode.TypeNTerminal == NonTerminal.ClassDecl)
                {

                    CompileClassDecl(ActiveNode);
                    NotSkipChild = false;
                }
                if (NotSkipChild)
                {
                    foreach (Node Child in ActiveNode.Nodes)
                    {
                        CreateCode(Child);
                    }
                }
            }
        }
}
}
