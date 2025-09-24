using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Linkdev.TeamTrack.Contract.DTOs.ProjectDtos;
using Linkdev.TeamTrack.Contract.Infrastructure.Interfaces;
using Linkdev.TeamTrack.Core.Models;
using Linkdev.TeamTrack.Core.Responses;
using Linkdev.TeamTrack.Infrastructure.ElasticSearch.Indexes;
using Linkdev.TeamTrack.Infrastructure.Extensions;

namespace Linkdev.TeamTrack.Infrastructure.ElasticSearch
{
    public class ProjectElasticService(ElasticsearchClient _elasticsearchClient) : IProjectElasticService
    {
        private const string indexName = "projects";
        public async Task IndexProjectAsync(Project project)
        {
            var projectElasticDocument = new ProjectElasticDocument
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                ProjectStatus = (int)project.ProjectStatus,
                ProjectManagerId = project.ProjectManagerId,
                CreatedDate = project.CreatedDate,
                IsActive = project.IsActive,
                Tasks = project.Tasks.Select(T => new ProjectTaskElasticNestedIndex
                {
                    Id = T.Id,
                    Title = T.Title,
                    AssignedUserId = T.AssignedUserId,
                    ProjectId = T.ProjectId
                }).ToList()
            };
            var response = await _elasticsearchClient.IndexAsync(projectElasticDocument, i => i.Index(indexName).Id(projectElasticDocument.Id));
            if (response.IsValidResponse)
                Console.WriteLine($"Indexed project {projectElasticDocument.Name} into index {indexName}");
            else
                Console.WriteLine($"Failed to index project {projectElasticDocument.Name}: {response.ElasticsearchServerError?.Error.Reason}");
        }

        public async Task<PaginatedResponse<GetAllProjectsDto>> SearchProjectAsync(ProjectFilterParams projectFilterParams, string userId)
        {
            var mustQueries = new List<Query>()
            {
                new TermQuery
                {
                    Field = nameof(ProjectElasticDocument.IsActive).ToCamelCase(),
                    Value = true
                },
                new BoolQuery
                {
                    Should =
                    [
                        new TermQuery
                        {
                            Field = Field.FromString($"{nameof(ProjectElasticDocument.ProjectManagerId)}.keyword".ToCamelCase()),
                            Value = userId
                        }
                        //new NestedQuery
                        //{
                        //    Path = /*nameof(ProjectElasticDocument.Tasks)*/"tasks",
                        //    Query = new TermQuery
                        //    {
                        //         Field = /*Field.FromString($"{nameof(ProjectElasticDocument.Tasks)}.{nameof(ProjectTaskElasticNestedIndex.AssignedUserId)}")*/"tasks.assignedUserId",
                        //         Value = userId
                        //    }
                        //}
                    ],
                    MinimumShouldMatch = 1
                }
            };

            if (!string.IsNullOrEmpty(projectFilterParams.Name))
            {
                mustQueries.Add(new MatchQuery
                {
                    Field = nameof(ProjectElasticDocument.Name).ToCamelCase(),
                    Query = projectFilterParams.Name.ToLower()
                });
            }

            if (projectFilterParams.ProjectStatus.HasValue)
            {
                mustQueries.Add(new TermQuery
                {
                    Field = nameof(ProjectElasticDocument.ProjectStatus).ToCamelCase(),
                    Value = projectFilterParams.ProjectStatus.Value
                });
            }

            var searchResponse = await _elasticsearchClient.SearchAsync<ProjectElasticDocument>(s =>
            s.Index(indexName)
             .From((projectFilterParams.PageNumber - 1) * projectFilterParams.PageSize)
             .Size(projectFilterParams.PageSize)
             .Query(q => q.Bool(b => b.Must(mustQueries)))
             .Sort(sort => sort.Field(nameof(ProjectElasticDocument.CreatedDate).ToCamelCase(), SortOrder.Desc)));

            var data = searchResponse.Documents.Select(projectDoc => new GetAllProjectsDto
            {
                Name = projectDoc.Name,
                CreatedDate = projectDoc.CreatedDate,
                ProjectStatus = projectDoc.ProjectStatus.ToString(),
                ProjectManagerId = projectDoc.ProjectManagerId
            }).ToList();

            var paginatedResponse = new PaginatedResponse<GetAllProjectsDto>
            {
                TotalCount = (int)searchResponse.Total,
                PageNumber = projectFilterParams.PageNumber,
                PageSize = projectFilterParams.PageSize,
                Data = data
            };

            return paginatedResponse;
        }
    }
}
