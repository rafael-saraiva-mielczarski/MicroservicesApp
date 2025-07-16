using PlatformService.Entities;

namespace PlatformService.Data
{
    public interface IPlatformRepository
    {
        bool SaveChanges();

        IEnumerable<Platform> GetAllPlatforms();

        Platform GetPlatformById(Guid id);

        Platform GetPlatformByName(string name);

        void CreatePlatform(Platform platform);
    }
}
