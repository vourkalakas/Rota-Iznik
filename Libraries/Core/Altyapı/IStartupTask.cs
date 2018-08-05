namespace Core.Altyapı
{
    public interface IStartupTask
    {
        void Execute();
        int Order { get; }
    }
}
