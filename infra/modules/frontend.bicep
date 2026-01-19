param swaName string
param location string

resource swa 'Microsoft.Web/staticSites@2023-12-01' = {
  name: swaName
  location: location
  sku: {
    name: 'Free'
    tier: 'Free'
  }
  properties: {}
}

@description('The deployment token for the Static Web App')
output swaDeploymentToken string = swa.listSecrets().properties.apiKey
output swaName string = swa.name
output swaUrl string = 'https://${swa.properties.defaultHostname}'
