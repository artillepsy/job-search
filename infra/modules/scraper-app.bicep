param location string
param containerRegistryName string
param environmentId string
param dbConnectionString string
param scraperImage string = 'mcr.microsoft.com/k8se/quickstart:latest'

// Get a reference to the existing ACR to assign permissions
resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-07-01' existing = {
  name: containerRegistryName
}

resource scraperJob 'Microsoft.App/jobs@2023-05-01' = {
  name: 'scraper'
  location: location
  identity: {
    type: 'SystemAssigned'
  } // Enable Identity
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
          identity: 'system'
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'scraper'
          image: scraperImage
          env: [
            { name: 'ASPNETCORE_URLS', value: 'http://+:8080' }
            { name: 'ConnectionStrings__DefaultConnection', value: dbConnectionString }
          ]
        }
      ]
    }
  }
}

// // Grant the Scraper permission to pull from ACR
// resource runerAcrPull 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
//   name: guid(scraperJob.id, containerRegistry.id, 'AcrPull')
//   scope: containerRegistry
//   properties: {
//     principalId: scraperJob.identity.principalId
//     roleDefinitionId: subscriptionResourceId(
//       'Microsoft.Authorization/roleDefinitions',
//       '7f951dda-4ed3-4680-af7b-1100f57d5111'
//     ) // AcrPull Role ID
//     principalType: 'ServicePrincipal'
//   }
// }

resource acrRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, containerRegistry.name, scraperJob.name, 'AcrPull')
  scope: containerRegistry
  dependsOn: [
    scraperJob
  ]
  properties: {
    principalId: scraperJob.identity.principalId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-af7b-1100f57d5111')
    principalType: 'ServicePrincipal'
  }
}
