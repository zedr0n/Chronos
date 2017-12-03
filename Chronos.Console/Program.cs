using System;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Chronos.CrossCuttingConcerns.DependencyInjection;
using Chronos.Infrastructure.Logging;
using SimpleInjector;

using static System.Console;

namespace Chronos.Console
{
    public class Scripter
    {
        private readonly Container _container = new Container();

        public Scripter()
        {
            Initialise();
        }
        
        private void Initialise()
        {
            var root = new CompositionRoot().WriteWith().Persistent().Database("Chronos.ES.Console")
                .ReadWith().Persistent().Database("Chronos.Read.Console");
            root.ComposeApplication(_container);
            _container.Register<IDebugLog,DebugLog>(Lifestyle.Singleton);
            _container.Register<ChronosVisitor>();
            _container.RegisterCollection(typeof(IChronosListener),
                new[]
                {
                    typeof(GuidListener),
                    typeof(CreateCoinListener),
                    typeof(TrackCoinListener),
                    typeof(CreateBagListener),
                    typeof(AddAssetToBagListener)
                });
            _container.Verify();
        }

        public void RunVisitor(string text)
        {
            var inputStream = new AntlrInputStream(text);
            var lexer = new ChronosLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new ChronosParser(commonTokenStream);
 
            var context = parser.createCoin();
            if (context.exception != null)
                return;
            var visitor = _container.GetInstance<ChronosVisitor>();        
            visitor.Visit(context); 
        }
        
        public void RunListeners(string text)
        {
            var inputStream = new AntlrInputStream(text);
            var lexer = new ChronosLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new ChronosParser(commonTokenStream);

            var context = parser.command();
            
            foreach(var listener in _container.GetAllInstances<IChronosListener>())
                ParseTreeWalker.Default.Walk(listener, context);    
        }
    }
    
    class Program
    {
        static void Run()
        {
            string input;
            var scripter = new Scripter();
                
            // to type the EOF character and end the input: use CTRL+D, then press <enter>
            while ((input = ReadLine()) != "EOF")
            {
                if(input == "")
                   continue;
                var text = new StringBuilder();
                text.AppendLine(input);
                    
                scripter.RunListeners(text.ToString());
            }    
        }
        
        static void Main(string[] args)
        {
            WriteLine("Chronos.Console");
            while (true)
            {
                try
                {
                    Run();
                }
                catch (Exception ex)
                {
                    WriteLine("Error: " + ex);                
                } 
            }
        }
    }
}