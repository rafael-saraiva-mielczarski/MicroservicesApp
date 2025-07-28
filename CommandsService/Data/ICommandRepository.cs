using CommandsService.Entities;

namespace CommandsService.Data
{
    public interface ICommandRepository
    {
        bool SaveChanges();

        IEnumerable<Platform> GetAllPlatforms();

        void CreatePlatform(Platform platform);

        bool PlatformExists(Guid platformId);

        bool ExternalPlatformExists(Guid externalPlatformId);


        IEnumerable<Command> GetCommandsForPlatform(Guid platformId);

        Command GetCommand(Guid platformId, Guid commandId);
        
        void CreateCommand(Guid platformId, Command command);
    }
}
