using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace NetFramePeli1
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            Battle game = new Battle();

            game.InitBattle();
        }
    }
}