using System.Diagnostics;

namespace NetFramePeli1
{
    internal class Battle
    {
        Unit[] myTeam = new Unit[3];
        Unit[] enemyTeam = new Unit[3];

        public List<Attacks> attacksList { get; private set; }

        Random rng;

        public int mySelection = -1;
        public int enemySelection = -1;
        public int round = 0;
        public int hold = -1;
        public bool canUndo = false;
        public bool unitSelected = false;

        //Initializes the battle and creates the units and assigns them to their righ indexes
        public void InitBattle()
        {
            Unit scubaDiver1 = new Unit("Scuba Diver", 200, 20);
            Unit scubaDiver2 = new Unit("Scuba Diver", 200, 20);
            Unit DiveOfficer = new Unit("Dive Officer", 200, 25);
            Unit Shark1 = new Unit("Shark", 100, 15);
            Unit Shark2 = new Unit("Shark", 100, 15);
            Unit Kraken = new Unit("Kraken", 300, 8);

            myTeam[0] = scubaDiver1;
            myTeam[1] = scubaDiver2;
            myTeam[2] = DiveOfficer;
            enemyTeam[0] = Shark1;
            enemyTeam[1] = Shark2;
            enemyTeam[2] = Kraken;

            rng = new Random();

            attacksList = new List<Attacks>();

            //Start running battle logic, which runs until the end of the game
            BattleLogic();
        }

        public void BattleLogic()
        {
            //Run as long as there are players alive
            while (true)
            {
                Console.Clear();

                //if it's the firs round, write the starting message, else write the round number
                if (round == 0) WriteLine("Battle begins!\n", ConsoleColor.DarkRed);
                if (round != 0)
                {
                    Write($"Round ");
                    WriteLine(round.ToString(), ConsoleColor.Yellow);
                    Console.WriteLine();
                }
                else round++;

                //Print the teams
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

                    //If all the units in my team have attacked, set the attacked boolean to false
                    if (myTeam[0].attacked && myTeam[1].attacked && myTeam[2].attacked)
                    {
                        myTeam[0].attacked = false;
                        myTeam[1].attacked = false;
                        myTeam[2].attacked = false;
                        round++;
                    }


                    if (hold != -1)
                    {
                        mySelection = hold;

                        if (mySelection >= 0 && mySelection <= myTeam.Length - 1)
                        {
                            if (!myTeam[mySelection].isAlive)
                            {
                                WriteLine("Selected unit is not alive, try another one!", ConsoleColor.DarkRed);
                           }
                            else if (myTeam[mySelection].attacked == true)
                            {
                                WriteLine("Selected unit has already attacked this round!", ConsoleColor.DarkRed);
                            }
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

                Console.WriteLine();

                //Attack
                #region printAttacks
                Attack();
                WriteLine();
                WriteLine("YOU ATTACK:", ConsoleColor.Blue);
                Write(new Text(myTeam[mySelection].name, ConsoleColor.DarkCyan), new Text(" attacked "), new Text(enemyTeam[enemySelection].name, ConsoleColor.DarkYellow), new Text(", for "), new Text(myTeam[mySelection].damage.ToString(), ConsoleColor.Red), new Text(" damage!"));
                WriteLine();
                #endregion

                //AI's turn to attack
                AiTurn();
                Console.WriteLine();

                //Print the attack history
                PrintHistory();

                //Check if there are any units alive in any team
                #region aliveCheck
                if (!myTeam[0].isAlive && !myTeam[1].isAlive && !myTeam[2].isAlive)
                {
                    BattleEnd(1);
                    break;
                }
                else if (!enemyTeam[0].isAlive && !enemyTeam[1].isAlive && !enemyTeam[2].isAlive)
                {
                    BattleEnd(2);
                    break;
                }


                #endregion
                canUndo = true;

                //Ask, if the user wants to Undo his attack
                Undo();

                //Reset the Unit Selection variables
                enemySelection = -1;
                mySelection = -1;
            }
        }

        //Adds the attack to the attacksList
        public void Attack()
        {
            Attacks action = new Attacks(myTeam[mySelection], enemyTeam[enemySelection]);
            action.Attack();
            attacksList.Add(action);
        }

        //Prints the game status(player and enemy armies)
        public void PrintStatus()
        {
            Console.WriteLine("[------------------------ Status ---------------------------------]");
            WriteLine("Player Army", ConsoleColor.DarkBlue);
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
            for (int i = 0; i < enemyTeam.Length; i++)
            {
                if (!enemyTeam[i].isAlive)
                {
                    Console.SetCursorPosition(25, i + addRows);
                    WriteLine($"{i}) {enemyTeam[i].name} | {enemyTeam[i].HP}", ConsoleColor.DarkRed);
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
        
        //Prints the game message(The action you have to do)
        public void PrintMessage()
        {
            Console.WriteLine("[------------------------ Message --------------------------------]");
            WriteLine("Player turn!", ConsoleColor.Red);
            if (!unitSelected) WriteLine("Select unit from team (0-2)");
            else WriteLine("Select target (0-2)");
        }
        
        //Prints the history(previously done attacks)
        public void PrintHistory()
        {
            Console.SetCursorPosition(0, 17);
            Console.WriteLine("[------------------------ History --------------------------------]");
            //Checks through all the actions in the List<Attacks> and prints the attacks for everyone of them
            foreach (Attacks action in attacksList)
            {
                //Checks if the attack was done by friendly army, and if it was write in Blue and if not write in Red
                if (action.ReturnTeam() == "Friendly") WriteLine(action.PrintAttack(), ConsoleColor.Blue);
                else WriteLine(action.PrintAttack(), ConsoleColor.Red);
            }
        }
        
        //AI selects and attacks the target
        public void AiTurn()
        {
            WriteLine("AI ATTACKS: ", ConsoleColor.DarkRed);

            //Get random int between 0 and 2 representing the Units index
            int AIUnit = rng.Next(0, enemyTeam.Length);
            int AITarget = rng.Next(0, myTeam.Length);

            while (true)
            {
                //Check if the enemy own unit selection is alive, and if not, get random int again
                if (!enemyTeam[AIUnit].isAlive) AIUnit = rng.Next(0, enemyTeam.Length);

                //Check if the enemy target unit selection is alive, and if not, get random int again
                if (!myTeam[AITarget].isAlive) AITarget = rng.Next(0, myTeam.Length);

                //If both, the target and the own unit are alive, attack
                if (myTeam[AITarget].isAlive && enemyTeam[AIUnit].isAlive)
                {
                    Attacks action = new Attacks(enemyTeam[AIUnit], myTeam[AITarget]);
                    action.Attack();
                    attacksList.Add(action);
                    break;
                }
            }
            
            Write(new Text(enemyTeam[AIUnit].name, ConsoleColor.DarkYellow), new Text(" attacked "), new Text(myTeam[AITarget].name, ConsoleColor.DarkCyan), new Text(", for "), new Text(enemyTeam[AIUnit].damage.ToString(), ConsoleColor.Red), new Text(" damage"));
            Console.WriteLine();
        }

        //Prints the team that has won
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

        //Console.Write, but the color is changeable
        public void Write(string text = "", ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }

        //Console.Write, but writes it with green Background color
        public void WriteGreenBG(string text = "", ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write(text);
            Console.BackgroundColor = ConsoleColor.White;
            Console.ResetColor();
        }

        //Console.WriteLine, but the color is changeable
        public void WriteLine(string text = "", ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        //Console.WriteLine, but writes it with green Background color
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
            while(true)
            {
                WriteLine("Press ctrl + z to undo, or press enter to continue", ConsoleColor.DarkRed);
                if (attacksList.Count < 1) break;

                int lastIndex = attacksList.Count - 1;

                // Read the key that the user has pressed
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Check if the Ctrl+Z key combination has been pressed
                if (keyInfo.Modifiers == ConsoleModifiers.Control && keyInfo.Key == ConsoleKey.Z)
                {
                    // Ctrl+Z has been pressed, so do something here

                    //Console.WriteLine("Ctrl + z was pressed");

                    attacksList[lastIndex].UndoAttack();
                    attacksList.RemoveAt(lastIndex);
                    lastIndex--;
                    attacksList[lastIndex].UndoAttack();
                    attacksList.RemoveAt(lastIndex);

                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    break;
                }
            }
        }

        //Turns the ConsoleKeyInfo into an integer
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

    }
}
