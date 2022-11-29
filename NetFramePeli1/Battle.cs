using System.Drawing;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;

namespace NetFramePeli1
{
    internal class Battle
    {
        Unit[] myTeam = new Unit[3];
        Unit[] enemyTeam = new Unit[3];

        Random rng;

        public int mySelection = -1;
        public int enemySelection = -1;
        public int round = 0;
        public int hold = -1;
        public bool unitSelected = false;
        public void InitBattle()
        {
            Unit scubaDiver = new Unit("Scuba Diver", 200, 20);
            Unit DiveOfficer = new Unit("Dive Officer", 200, 25);
            Unit Shark = new Unit("Shark", 100, 15);
            Unit Kraken = new Unit("Kraken", 300, 8);

            myTeam[0] = scubaDiver;
            myTeam[1] = scubaDiver;
            myTeam[2] = DiveOfficer;
            enemyTeam[0] = Shark;
            enemyTeam[1] = Shark;
            enemyTeam[2] = Kraken;

            rng = new Random();

            

            BattleLogic();
        }

        public void BattleLogic()
        {
            WriteLine("Battle begins!\n", ConsoleColor.DarkRed);
            //PrintTeams();

            while (true)
            {
                if (round != 0)
                {
                    Write($"Round ");
                    WriteLine(round.ToString(), ConsoleColor.Yellow);
                    Console.WriteLine();
                }
                round++;
                
                
                /*Console.WriteLine("Player's turn: Choose a unit by giving a number:");

                //FriendlyPrinting
                for (int i = 0; i < myTeam.Length; i++)
                {
                    if (myTeam[i].isAlive)
                    {
                        Write($"{i}: {myTeam[i].name}", ConsoleColor.DarkCyan);
                        WriteLine($"|{myTeam[i].HP.ToString()} HP", ConsoleColor.Red);
                    }
                    else WriteLine($"{i}: {myTeam[i].name}", ConsoleColor.Red);
                }
                Console.WriteLine();*/

                PrintStatus();
                PrintMessage();

                //FriendlySelection
                while (true)
                {
                    Console.Write("Select a unit: ");
                    mySelection = -1;
                    hold = -1;

                    ConsoleKeyInfo UserInput = Console.ReadKey();
                    hold = keyToNumber(UserInput.Key.ToString());

                    if (hold != -1)
                    {
                        mySelection = hold;

                        if (mySelection >= 0 && mySelection <= myTeam.Length - 1)
                        {
                            if (!myTeam[mySelection].isAlive) WriteLine("Selected unit is not alive, try another one!", ConsoleColor.DarkRed);
                            else break;
                        }
                        else WriteLine("Selected unit doesn't exist", ConsoleColor.DarkRed);
                    }
                    else WriteLine("Input was not a number", ConsoleColor.DarkRed);
                }
                Console.Clear();

                Write($"Round ");
                WriteLine(round.ToString(), ConsoleColor.Yellow);
                Console.WriteLine();

                PrintStatus();
                PrintMessage();
                PrintHistory();

                /*Console.WriteLine("OLD THINGS DOWN FROM HERE");

                Console.WriteLine("");
                Console.WriteLine("Choose target from enemy team:");

                //EnemyPrinting
                for (int i = 0; i < enemyTeam.Length; i++)
                {
                    if (enemyTeam[i].isAlive)
                    {
                        Write($"{i}: {enemyTeam[i].name}", ConsoleColor.DarkYellow);
                        WriteLine($"|{enemyTeam[i].HP.ToString()} HP", ConsoleColor.Red);
                    }
                    else WriteLine($"{i}: {enemyTeam[i].name}", ConsoleColor.Red);
                }
                Console.WriteLine();*/

                //EnemySelection
                while (true)
                {
                    Console.Write("Give a number: ");
                    enemySelection = -1;
                    hold = -1;

                    ConsoleKeyInfo UserInput = Console.ReadKey();
                    hold = keyToNumber(UserInput.Key.ToString());

                    if (hold != -1)
                    {
                        enemySelection = hold;

                        if (enemySelection >= 0 && enemySelection <= enemyTeam.Length - 1)
                        {
                            if (!enemyTeam[enemySelection].isAlive) WriteLine("Selected unit is not alive, try another one!", ConsoleColor.DarkRed);
                            else break;
                        }
                        else WriteLine("Selected unit doesn't exist", ConsoleColor.DarkRed);
                    }
                    else WriteLine("Input was not a number", ConsoleColor.DarkRed);
                }

                //Attack
                #region printAttacks
                enemyTeam[enemySelection].Damage(myTeam[mySelection].damage);
                WriteLine();
                WriteLine("YOU ATTACK:", ConsoleColor.Blue);
                Write(new Text(myTeam[mySelection].name, ConsoleColor.DarkCyan), new Text(" attacked "), new Text(enemyTeam[enemySelection].name, ConsoleColor.DarkYellow), new Text(", for "), new Text(myTeam[mySelection].damage.ToString(), ConsoleColor.Red), new Text(" damage!"));
                WriteLine();
                #endregion
                #region aliveCheck
                if (!myTeam[0].isAlive && !myTeam[1].isAlive)
                {
                    BattleEnd(1);
                    break;
                }
                else if (!enemyTeam[0].isAlive && !enemyTeam[1].isAlive)
                {
                    BattleEnd(2);
                    break;
                }
                #endregion

                Undo();
                enemySelection = -1;
                mySelection = -1;
            }
        }

        public void PrintStatus()
        {

            Console.WriteLine("[------------------------ Status ---------------------------------]");
            WriteLine("Player Army", ConsoleColor.DarkBlue);
            //mySelection = -1;
            for (int i = 0; i < myTeam.Length; i++)
            {
                if (!myTeam[i].isAlive)
                {
                    WriteLine($"{i}) {myTeam[i].name} | {myTeam[i].HP}", ConsoleColor.DarkRed);
                }
                else if (myTeam[i].isAlive && mySelection == i)
                {
                    WriteGreenBG($"{i}) {myTeam[i].name} | ");
                    WriteLine($"{myTeam[i].HP}", ConsoleColor.DarkRed);
                }
                else
                {
                    Write($"{i}) {myTeam[i].name} | ");
                    WriteLine($"{myTeam[i].HP}", ConsoleColor.DarkRed);
                }
            }

            Console.SetCursorPosition(25, 3);
            WriteLine("Enemy Army", ConsoleColor.DarkRed);
            int addRows = 4;
            //enemySelection = -1;
            for (int i = 0; i < enemyTeam.Length; i++)
            {
                if (!enemyTeam[i].isAlive)
                {
                    Console.SetCursorPosition(25, i + addRows);
                    WriteLine($"{i}) {enemyTeam[i].name} | {myTeam[i].HP}", ConsoleColor.DarkRed);
                }
                else if (enemyTeam[i].isAlive && enemySelection == i)
                {
                    Console.SetCursorPosition(25, i + addRows);
                    WriteGreenBG($"{i}) {enemyTeam[i].name} | ");
                    WriteLine($"{enemyTeam[i].HP}", ConsoleColor.DarkRed);
                }
                else
                {
                    Console.SetCursorPosition(25, i + addRows);
                    Write($"{i}) {enemyTeam[i].name} | ");
                    WriteLine($"{enemyTeam[i].HP}", ConsoleColor.DarkRed);
                }
            }
            Console.SetCursorPosition(0, 7);
        }
        public void PrintMessage()
        {
            Console.WriteLine("[------------------------ Message --------------------------------]");
            WriteLine("Player turn!", ConsoleColor.Red);
            if (!unitSelected) WriteLine("Select unit from team (0-2)");
            else WriteLine("Select target (0-2)");
        }
        public void PrintHistory()
        {
            Console.WriteLine("[------------------------ History --------------------------------]");
        }
        
        public void AiTurn()
        {
            WriteLine("AI ATTACKS: ", ConsoleColor.DarkRed);

            int AIUnit = rng.Next(0, enemyTeam.Length);
            int AITarget = rng.Next(0, myTeam.Length);

            while (true)
            {
                if (!enemyTeam[AIUnit].isAlive) AIUnit = rng.Next(0, enemyTeam.Length);

                if (!myTeam[AITarget].isAlive) AITarget = rng.Next(0, myTeam.Length);

                if (myTeam[AITarget].isAlive && enemyTeam[AIUnit].isAlive)
                {
                    myTeam[AITarget].Damage(enemyTeam[AIUnit].damage);

                    break;
                }
            }
            
            Write(new Text(enemyTeam[AIUnit].name, ConsoleColor.DarkYellow), new Text(" attacked "), new Text(myTeam[AITarget].name, ConsoleColor.DarkCyan), new Text(", for "), new Text(enemyTeam[AIUnit].damage.ToString(), ConsoleColor.Red), new Text(" damage"));
            Console.WriteLine();

            Console.WriteLine("Press any key to advance to the next round");
            Console.ReadKey();
            Console.Clear();
        }

        public void BattleEnd(int teamWon)
        {
            int unitsAlive = 0;

            switch (teamWon)
            {
                case 1:
                    foreach (Unit enemy in enemyTeam)
                    {
                        if (enemy.isAlive) unitsAlive++;
                    }
                    WriteLine($"The enemy has won with {unitsAlive} unit(s) left!");
                    break;

                case 2:
                    foreach (Unit ally in myTeam)
                    {
                        if (ally.isAlive) unitsAlive++;
                    }
                    WriteLine($"You have won with {unitsAlive} unit(s) left!");
                    break;
            }
        }

        public void Write(string text = "", ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }
        public void WriteGreenBG(string text = "", ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write(text);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ResetColor();
        }

        public void WriteLine(string text = "", ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public void WriteLineGreenBG(string text = "", ConsoleColor color = ConsoleColor.White, int jee = 3)
        {
            Console.ForegroundColor = color;
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ResetColor();
        }

        public void Write(params Text[] texts)
        {
            foreach (Text text in texts)
            {
                Console.ForegroundColor = text.color;
                Console.Write(text.str);
            }
            Console.ResetColor();
            Console.WriteLine();
        }

        public void Undo()
        {
            Console.WriteLine("Write 'undo', if you want to undo the attack!");
            if (Console.ReadLine() == "undo")
            {
                enemyTeam[enemySelection].Heal(myTeam[mySelection].damage);
                WriteLine();
                WriteLine("Undid the last attack!", ConsoleColor.DarkRed);
                WriteLine();
                round--;
                Console.WriteLine("Press any key to advance to the next round");
                Console.ReadKey();
                Console.Clear();
            }
            //AiTurn();
            else AiTurn();
            #region aliveCheck
            if (!myTeam[0].isAlive && !myTeam[1].isAlive)
            {
                BattleEnd(1);
            }
            else if (!enemyTeam[0].isAlive && !enemyTeam[1].isAlive)
            {
                BattleEnd(2);
            }
            #endregion
        }

        public static int keyToNumber(string key)
        {
            if (key.Length == 1) return -1;

            int keyToNumberInt = 0;
            switch (key)
            {
                case "D0":
                    keyToNumberInt = 0;
                    return keyToNumberInt;

                case "D1":
                    keyToNumberInt = 1;
                    return keyToNumberInt;

                case "D2":
                    keyToNumberInt = 2;
                    return keyToNumberInt;
            }
            return -1;
        }

        /*public static void WriteAt(string s, int x, int y, ConsoleColor color = ConsoleColor.White)
        {
            try
            {
                Console.ForegroundColor = color;
                Console.SetCursorPosition(origCol + x, origRow + y);
                Console.Write(s);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }*/
    }
}
