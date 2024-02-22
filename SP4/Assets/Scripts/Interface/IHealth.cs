public interface IHealth
{
    //get health
    public int GetHealth();
    //health += amt;
    public void UpdateHealth(int amt);

    //for when hp <= 0;
    public void Die();

    //set hp = maxhp;
    public void InitHealth();
}
