
using System.Collections.Generic;
using System.Windows.Forms;

namespace Parser
{

    public class SynAnalizier
    {

        public Node Tree = null;

        List<Token> _Tokens;

        int TokenPointer;



        public bool Proceed(List<Token> Tokens, RichTextBox RTB)
        {
            _Tokens = Tokens;
            TokenPointer = 0;

            BuildTree();

            if (TokenPointer == Tokens.Count)
            {
                RTB.Text += "\nСинтаксический анализ прошёл успешно!";

                return true;
            }
            else
            {
                RTB.Text = "Синтаксическая ошибка в строке: " + _Tokens[TokenPointer].StringNumber.ToString();

                return false;
            }
        }

        public void Print(TreeView TV)
        {
            if (Tree.IsTerminal)
            {

            }
            else
            {
                TV.Nodes.Add(new TreeNode(Tree.TypeNTerminal.ToString()));
                PrintNode(TV.Nodes[0],Tree);
            }

            TV.ExpandAll();
        }


        private void PrintNode (TreeNode Parent, Node ToPrint)
        {
            foreach (Node OneNode in ToPrint.Nodes)
            {
                if (OneNode.IsTerminal)
                {
                    Parent.Nodes.Add(new TreeNode(OneNode.TypeToken.ToString()+"  |  \""+OneNode.Value+"\""));
                }
                else
                {
                    Parent.Nodes.Add(new TreeNode(OneNode.TypeNTerminal.ToString()));
                    PrintNode(Parent.Nodes[Parent.Nodes.Count-1],OneNode);
                }
            }
        }

        private void BuildTree()
        {
            // Строим синтаксическое дерево сверзу вниз: Берём терминал нетерминал "Программа", разворачиваем правила и смотрим, покрыли ли мы все термы программы

            /*
             * Берём узел "Programm", вызываем ApplyAltrernative И получаем дочерние узла, в которые разворачивается узел Programm
             *    по первой альтернативе правила грамматике, сответствующего нетерминалу Programm
             *    Дочерние узлы прикрепляем к узлу Programm и проходим по ним. Нетерминальные дочерние узлы рекурсивно
             *    разворачиваем по грамматическому правилу, а терминальные проверяем на несовпадение
             *    Если есть несовпадение, то возвращаемся к родительскому узлу (в нашем примере - Programm) и берём другую альтернативу для развёртки узла
             *    В случае если мы перебрали все альтернативы и у нас во всех есть несовпадение - то значит, что в программе есть ошибка
             */

            bool NoCoincidence = false;            

            Tree = new Node(NonTerminal.Programm);
            Node ActiveNode = Tree;

            // Пока не перебрали все токены, полученные при лексическом анализе
            while ((TokenPointer<_Tokens.Count)&&(ActiveNode!=null))
            {
                // Узел терминальный. Мы должны проверить есть ли такой узел в программе
                if (ActiveNode.IsTerminal)
                {
                    if ((ActiveNode.TypeToken != _Tokens[TokenPointer].Type) &&
                        (ActiveNode.TypeToken != TokenType.Epsilon))
                    {
                        // Узла нет - значит нужно применять другую альтернативу к нетерминалу, из которого данный терминал выведен
                        NoCoincidence = true;
                    }
                    else
                    {
                        if (ActiveNode.TypeToken != TokenType.Epsilon)
                        {
                            // Записываем в узел значение (например, если узел "целочисленный литерал" - нам нужно сохранить значение этого литерала)
                            ActiveNode.Value = _Tokens[TokenPointer].Value;

                            // Смещаем указатель на следующий терм программы
                            TokenPointer++;
                        }
                    }

                    if (ActiveNode.Parent != null)
                        ActiveNode = ActiveNode.Parent;

                }
                else
                { // Узел нетерминальный. Нам нужно либо его развернуть, либо вернуться на родительский узел, если есть несовпадение в братьях-нетерминалах

                    if (ActiveNode.Nodes.Count > 0)
                    {

                        // Несовпадения нет
                        if (!NoCoincidence)
                        {
                            // Обошли всех детей узла - надо выйти из него (потому что мы построили уже всё поддерево этого узла)
                            if (ActiveNode.ChildPointer >= ActiveNode.Nodes.Count-1)
                            {
                                ActiveNode = ActiveNode.Parent;
                            }
                            else
                            {
                                // Активным узлом становится очередной ребёнок
                                ActiveNode.ChildPointer++;
                                ActiveNode = ActiveNode.Nodes[ActiveNode.ChildPointer];
                            }
                        }
                        else
                        {
                            // Обнаружено несовпадение. Надо применить другое правило к этому узлу. Вызываем ApplyNode, возвращаем старое значение TakenPointera,
                            //      указывающего на терм программы во время, когда мы начинали обрабатывать 
                            //  текущий узел (часть альтернативных правил могут содержать одни и те же термы вначале) - например, оператор if ... и if ... else ...
                            //    содержат в качестве первого нетерминала if. Соответственно, мы должнв будем заново обработать терминал if
                            if (ActiveNode.AlternativeIndex <
                                Grammar.Rules[ActiveNode.RuleIndex].Alternatives.Count)
                            {
                                ApplyAlternative(ActiveNode);
                                NoCoincidence = false;
                                TokenPointer = ActiveNode.TokensPointerBackUp;
                                ActiveNode = ActiveNode.Nodes[ActiveNode.ChildPointer];
                                
                            }
                            else
                            {
                                // Альтернативы исчерпались - мы должны вернуться на родительский узел, чтобы применить к нему другую альтернативу правила
                                ActiveNode = ActiveNode.Parent;
                            }
                        }
                    }
                    else
                    {

                        ActiveNode.TokensPointerBackUp = TokenPointer;
                        ApplyAlternative(ActiveNode);
                        ActiveNode = ActiveNode.Nodes[ActiveNode.ChildPointer];
                    }
                }
            }



        }

       private void ApplyAlternative(Node ActiveNode)
       {
            // Применяем альтернативу
            // Если неизвестно правило, ищем его
            if (ActiveNode.RuleIndex == -1)
            {
                ActiveNode.RuleIndex = FindRuleToApply(ActiveNode.TypeNTerminal);
            }

            if (ActiveNode.AlternativeIndex == -1)
                ActiveNode.AlternativeIndex = 0;

            // По полю RuleIndex и AlternativeIndex берём альтернативу правила и сохраняем термы, хранящиеся в альтернативе и сохранем в список Children
            List<GrammarSymbol> Children = Grammar.Rules[ActiveNode.RuleIndex].
                Alternatives[ActiveNode.AlternativeIndex].Symbols;

            ActiveNode.ChildPointer = 0;
            ActiveNode.Nodes = new List<Node>();

           // Добавляем детей в узел
            foreach(GrammarSymbol GS in Children)
            {
                if (GS.IsTerminal)
                {
                    ActiveNode.Nodes.Add(new Node(GS.TypeToken));
                }
                else
                {
                    ActiveNode.Nodes.Add(new Node(GS.TypeNTerminal));
                }

            }

            // ПРиписываем детям в качестве родителя активный узел
            foreach (Node Child in ActiveNode.Nodes)
                Child.Parent = ActiveNode;

            // Увеличиваем счётчик альтернатив, чтобы в следующий раз помнить какие альтернативы мы применяли
            ActiveNode.AlternativeIndex++;
       }

        private int FindRuleToApply(NonTerminal  TT)
        {
            for (int i = 0; i < Grammar.Rules.Count; i++)
                if (Grammar.Rules[i].Left.TypeNTerminal == TT)
                    return i;

            return -1;
        }

        private bool AllAlternativesTryied(int RuleNumber,int AlternativeNumber)
        {
            return AlternativeNumber >= Grammar.Rules[RuleNumber].Alternatives.Count;
        }

    }


}