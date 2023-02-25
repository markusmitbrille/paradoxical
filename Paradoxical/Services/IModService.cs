using Paradoxical.Model;

namespace Paradoxical.Services;

public interface IModService
{
    Mod Get();

    string GetPrefix();
    string GetModName();
}
