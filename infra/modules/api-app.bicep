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
        targetPort: 8080
      }
    }
    template: {
      containers: [
        {
          name: 'api'
          image: 'mcr.microsoft.com/k8se/quickstart:latest'
          env: [
            { name: 'ASPNETCORE_URLS', value: 'http://+:8080' }
            { name: 'ConnectionStrings__DefaultConnection', value: dbConnectionString }
          ]
        }
      ]
      scale: {
        minReplicas: 0
        maxReplicas: 5
      }
    }
  }
}

resource apiAcrRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, containerRegistry.name, apiApp.name, 'AcrPull')
  scope: containerRegistry
  dependsOn: [
    apiApp // Assuming your container app is named apiContainerApp
  ]
  properties: {
    principalId: apiApp.identity.principalId
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-af7b-1100f57d5111')
    principalType: 'ServicePrincipal'
  }
}
