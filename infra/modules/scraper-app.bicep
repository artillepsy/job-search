param location string
param containerRegistryName string
param environmentId string
param dbConnectionString string
param image string
param identityId string

var scraperConfigs = [
  {
    name: 'careersinpoland'
    cron: '0/10 * * * *' // Every 10 minutes
  }
]

resource scraperJobs 'Microsoft.App/jobs@2023-05-01' = [
  for config in scraperConfigs: {
    name: 'job-scraper-${config.name}'
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
        triggerType: 'Schedule'
        replicaTimeout: 1800 // 30 minutes max execution time
        replicaRetryLimit: 1
        scheduleTriggerConfig: {
          cronExpression: config.cron
        }
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
              config.name
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
]
