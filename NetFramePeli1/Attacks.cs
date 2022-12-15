using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFramePeli1
{
    internal class Attacks
    {
        private Unit source;
        private Unit target;
        
        //Constructor
        public Attacks(Unit source, Unit target)
        {
            this.source = source;
            this.target = target;
        }

        //Deals the damage of the source to the destination
        public void Attack()
        {
            source.attacked = true;
            target.Damage(source.damage);
        }

        //Heals the target for the amount of the source damage
        public void UndoAttack()
        {
            source.attacked = false;
            target.Heal(source.damage);
        }

        //Returns the attack that happened as a string
        public string PrintAttack()
        {
            string whoAttacked = $"{source.name} attacked {target.name} and dealt {source.damage} damage!";
            return whoAttacked;
        }

        //Returns the team as a string
        public string ReturnTeam()
        {
            if(source.name == "Scuba Diver" || source.name == "Dive Officer") 
            {
                return "Friendly";
            }
            else
            {
                return "Enemy";
            }
            
        }

        public void WriteLine(string text = "", ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
