# mini_java_compiler
Учебный проект, сделанной мной во время обучения в ВУЗе. Компилятор подмножества языка `MiniJava`
Компилятор написан на языке `C#` c с использованием технологии WindowsForms и библиотеки `System.Reflection` для генерации исполняемого файла

Грамматику языка `MiniJava` смотрите в файле `Grammar.docx`.

На данный момент поддерживаются следующие возможности языка `MiniJava`:

`if`-Statement, `while`-Statement, `System.out.println(...)`, `new`-оператор, `{ Statement }`, `(Expression)`, `int`-type, `bool`-type, Целочисленные литералы, булевские литералы, вызов метода класса, `Classes`, `Object methods`, `formal method parameters`, 

Компилятор состоит из трёх частей: Лексический анализатор (Lexer), Синтаксический анализатор (Parser), кодогенератора (CodeGenerator),
каждая из которых является одним из трёх этапов компиляции:

1. Лексический анализатор с помощью конечных автоматов разбирает входящую цепочку на лексемы и выдаёт их список.
2. Синтаксический анализатор получает список лексемм программы и строит по нему синтаксическое дерево по грамматике языка Minijava 
и проверяет синтаксис на ошибки. Выходом этого этапа является абстрактное синтаксическое дерево (AST)
3. Кодогенератор обходит синтаксическое дерево и записывает машинные команды MSIL в исполняемый файл. NetFramework, путь 
к которому указывается в интерфейсе компилятора
