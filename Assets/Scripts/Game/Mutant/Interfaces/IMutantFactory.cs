namespace Game.Mutant.Interfaces
{
    public interface IMutantFactory
    {
        MutantController GetEnemyFromPool(int hashCode);
        MutantController GetEnemyFromPool();
    }
}