using Raven.Client;

namespace Spider.Commands
{
    interface ICommand
    {
        void Execute(IDocumentStore dataStore);
    }
}
