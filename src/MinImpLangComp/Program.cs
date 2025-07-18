using MinImpLangComp.LexerSpace;

namespace MinImpLangComp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test de lecture "lambda"
            string input = "var1 = 42 + 8;";
            var lexer = new Lexer(input);
            Token token;
            do
            {
                token = lexer.GetNextToken();
                Console.WriteLine(token);
            }while (token.Type != TokenType.EOF);

            Console.WriteLine("--------------------------------------------");

            //Test de lecture "complexe"
            string input2 = "var2 = (42 + 8) * 2;";
            var lexer2 = new Lexer(input2);
            Token token2;
            do
            {
                token2 = lexer2.GetNextToken();
                Console.WriteLine(token2);
            } while (token2.Type != TokenType.EOF);

            Console.WriteLine("--------------------------------------------");

            //Test gestion des nombres entier, décimaux, du . et des nombres "mal encodé"
            string input3 = "42 3.14 0.9 42. .32 . 4.3.2";
            var lexer3 = new Lexer(input3);
            Token token3;
            do
            {
                token3 = lexer3.GetNextToken();
                Console.WriteLine(token3);
            } while (token3.Type != TokenType.EOF);
        }
    }
}