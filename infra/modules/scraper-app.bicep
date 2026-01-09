param location string
param containerRegistryName string
param environmentId string
param dbConnectionString string
param image string
param identityId string

resource scraperJob 'Microsoft.App/jobs@2023-05-01' = {
  name: 'scraper'
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${identityId}': {}
    }
  }
  properties: {
    environmentId: environmentId
    configuration: {
      replicaTimeout: 300
      triggerType: 'Manual' // Change to 'Schedule' for scheduled jobs
      // scheduleTriggerConfig: {
      //   cronExpression: '0 * * * *' // Runs every hour
      //   parallelism: 1
      //   replicaCompletionCount: 1
      // }
      registries: [
        {
          server: '${containerRegistryName}.azurecr.io'
          identity: identityId
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'scraper'
          image: image
          env: [
            { name: 'ASPNETCORE_URLS', value: 'http://+:8080' }
            { name: 'ConnectionStrings__DefaultConnection', value: dbConnectionString }
          ]
        }
      ]
    }
  }
}
