using UniOrm.Model; 
namespace UniOrm.Application
{
    public interface IGodWorker
    {
        string WorkerName { get; set; }
        TypeDefinition G(string typeName);
        DefaultModuleManager ModuleManager { get; set; }
        void Run(params object[] parameters);
    }
}
