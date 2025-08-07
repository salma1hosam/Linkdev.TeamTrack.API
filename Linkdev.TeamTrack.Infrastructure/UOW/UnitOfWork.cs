using Linkdev.TeamTrack.Contract.Infrastructure.Interfaces;
using Linkdev.TeamTrack.Core.Models;
using Linkdev.TeamTrack.Infrastructure.Data.Contexts;
using Linkdev.TeamTrack.Infrastructure.Repositories;

namespace Linkdev.TeamTrack.Infrastructure.UOW
{
    public class UnitOfWork(TeamTrackDbContext _dbContext) : IUnitOfWork
    {
        private IBaseRepository<Project, int> _projectRepository;
        private IBaseRepository<ProjectTask, int> _taskRepository;

        public IBaseRepository<Project, int> ProjectRepository
        {
            get
            {
                if (_projectRepository is null)
                    _projectRepository = new BaseRepository<Project, int>(_dbContext);
                return _projectRepository;
            }
        }

        public IBaseRepository<ProjectTask, int> TaskRepository
        {
            get
            {
                if (_taskRepository is null)
                    _taskRepository = new BaseRepository<ProjectTask, int>(_dbContext);
                return _taskRepository;
            }
        }

        public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
    }
}
