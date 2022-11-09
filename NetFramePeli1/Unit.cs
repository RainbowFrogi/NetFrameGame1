namespace NetFramePeli1
{
    public struct Unit
    {
        public string name;
        public int HP;
        public int damage;
        public bool isAlive = true;

        public Unit(string name, int HP, int damage)
        {
            this.name = name;
            this.HP = HP;
            this.damage = damage;
        }

        public void Damage(int dealtDamage)
        {
            HP -= dealtDamage;

            isAlive = HP > 0 ? true : false;
        }

        public void Heal(int healAmount)
        {
            HP += healAmount;

            isAlive = HP > 0 ? true : false;
        }
    }
}
