param location string
param acrName string
param envId string
param dbConnectionString string

resource apiApp 'Microsoft.App/containerApps@2023-05-01' = {
  name: 'job-api'
  location: location
  properties: {
    managedEnvironmentId: envId
    configuration: {
      ingress: {
        external: true // Allows you to access it from the web
        targetPort: 8080
      }
    }
    template: {
      containers: [
        {
          name: 'api'
          image: '${acrName}.azurecr.io/jobapi:latest'
          env: [{ name: 'ConnectionStrings__DefaultConnection', value: dbConnectionString }]
        }
      ]
      scale: {
        minReplicas: 0 // <--- Scale to zero for $0 idle cost!
        maxReplicas: 5
      }
    }
  }
}
