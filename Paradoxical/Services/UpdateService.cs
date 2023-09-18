using Paradoxical.Core;

namespace Paradoxical.Services;

public interface IUpdateService
{
    void Update(IEntity entity);
}

public class UpdateService : IUpdateService
{
    public IDataService Data { get; }

    public UpdateService(IDataService data)
    {
        Data = data;
    }

    public void Update(IEntity entity)
    {
        Data.Connection.Update(entity);
    }
}
