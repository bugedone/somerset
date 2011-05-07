using Castle.ActiveRecord;
using Raven.Client;

namespace Spider.Commands
{
    class GenerateSchemaCommand : ICommand
    {
        public void Execute(IDocumentStore dataStore)
        {
            ActiveRecordStarter.GenerateCreationScripts("Cricket.sql");
        }

    }
}
