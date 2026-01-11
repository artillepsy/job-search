param location string
param containerRegistryName string
param environmentId string
param dbConnectionString string
param image string
param identityId string

resource scraperJobs 'Microsoft.App/jobs@2023-05-01' = {
  name: 'job-scraper-careersinpoland'
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
      triggerType: 'Manual'
      replicaTimeout: 300
      replicaRetryLimit: 1
      // scheduleTriggerConfig: {
      //   cronExpression: config.cron
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
          image: image
          name: 'scraper'
          args: [
            '--scraper'
            'careersinpoland'
          ]
          env: [
            {
              name: 'ConnectionStrings__DefaultConnection'
              value: dbConnectionString
            }
          ]
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }
        }
      ]
    }
  }
}
