using Spider.Persistence;

namespace Spider.Commands
{
    interface ICommand
    {
        void Execute(FileStore dataStore);
    }
}
