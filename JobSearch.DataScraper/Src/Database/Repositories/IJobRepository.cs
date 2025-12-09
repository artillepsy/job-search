using JobSearch.DataScraper.Database.Models;

namespace JobSearch.DataScraper.Database.Repositories;

public interface IJobRepository
{
	public Task AddRangeAsync(IEnumerable<JobModel> models);
	//todo: heck a few keys like job website id and job url, it should be enough to make it a unique combination
	public Task<bool> ExistsAsync(JobModel model);
	
}