param location string
param containerRegistryName string
param environmentId string
param dbConnectionString string

resource scraperJob 'Microsoft.App/jobs@2023-05-01' = {
  name: 'scraper'
  location: location
  properties: {
    environmentId: environmentId
    configuration: {
      replicaTimeout: 300
      triggerType: 'Schedule'
      scheduleTriggerConfig: {
        cronExpression: '0 * * * *'
        parallelism: 1
        replicaCompletionCount: 1
      }
      registries: [
        {
          server: '${containerRegistryName}.azurecr.io'
          identity: 'system'
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'scraper'
          image: '${containerRegistryName}.azurecr.io/datascraper:latest'
          env: [{ name: 'ConnectionStrings__DefaultConnection', value: dbConnectionString }]
        }
      ]
    }
  }
}
