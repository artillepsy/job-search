param location string
param containerRegistryName string
param environmentId string
param dbConnectionString string

// Reference existing ACR for permissions
resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-07-01' existing = {
  name: containerRegistryName
}

resource apiApp 'Microsoft.App/containerApps@2023-05-01' = {
  name: 'api'
  location: location
  identity: {
    type: 'SystemAssigned'
  } // Enable Identity
  properties: {
    managedEnvironmentId: environmentId
    configuration: {
      registries: [
        {
          server: '${containerRegistryName}.azurecr.io'
          identity: 'system'
        }
      ]
      ingress: {
        external: true
        targetPort: 80
      }
    }
    template: {
      containers: [
        {
          name: 'api'
          image: 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
          env: [{ name: 'ConnectionStrings__DefaultConnection', value: dbConnectionString }]
        }
      ]
      scale: {
        minReplicas: 0
        maxReplicas: 5
      }
    }
  }
}

// Static Role Assignment: Allows API to pull from ACR
resource apiAcrPull 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(apiApp.id, containerRegistry.id, 'AcrPull')
  scope: containerRegistry
  properties: {
    principalId: apiApp.identity.principalId
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      '7f951dda-4ed3-4680-af7b-1100f57d5111'
    ) // AcrPull Role ID
    principalType: 'ServicePrincipal'
  }
}
