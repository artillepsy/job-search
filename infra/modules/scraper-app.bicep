param location string
param acrName string
param envId string
param dbConnectionString string

resource scraperJob 'Microsoft.App/jobs@2023-05-01' = {
  name: 'job-scraper'
  location: location
  properties: {
    environmentId: envId
    configuration: {
      replicaTimeout: 300 // 5 minutes max run time
      triggerType: 'Schedule'
      scheduleConfiguration: {
        cronExpression: '0 0 * * *' // Runs every day at midnight
      }
      registries: [
        {
          server: '${acrName}.azurecr.io'
          identity: 'system'
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'scraper'
          image: '${acrName}.azurecr.io/datascraper:latest'
          env: [{ name: 'ConnectionStrings__DefaultConnection', value: dbConnectionString }]
        }
      ]
    }
  }
}
