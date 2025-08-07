using Linkdev.TeamTrack.Core.Models;

namespace Linkdev.TeamTrack.Contract.Infrastructure.Interfaces
{
    public interface IUnitOfWork
    {
        IBaseRepository<Project , int> ProjectRepository { get; }
        IBaseRepository<ProjectTask , int> TaskRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
