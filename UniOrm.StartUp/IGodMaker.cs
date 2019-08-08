using UniOrm.Model;

namespace UniOrm.Application
{
    public interface IGodMaker
    {
        TypeDefinition G(string typeName);

        void Run(params object[] parameters);
    }
}
