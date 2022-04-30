
namespace Console
{
    public interface IConsoleCommand
    {
        public abstract string Name();
        public abstract string Description();
        
        public abstract bool IsHidden();
        public abstract void Execute(string[] args);
    }
}
