using JobSearch.DataScraper.Data.Entities;

namespace JobSearch.DataScraper.Data.Repositories;

public interface IJobRepository
{
	public Task AddUniqueAsync(IEnumerable<JobEntity> models);
	public Task RemoveNonExistentAsync(IEnumerable<JobEntity> models);
	//todo: heck a few keys like job website id and job url, it should be enough to make it a unique combination
	public Task<bool> ExistsAsync(JobEntity entity);
	
}