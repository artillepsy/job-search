param location string
param prefix string
param serverName string
@secure()
param dbAdminPassword string

// Create a PostgreSQL Server instance
resource postgresServer 'Microsoft.DBforPostgreSQL/flexibleServers@2023-03-01-preview' = {
  name: serverName
  location: location
  sku: {
    name: 'Standard_B1ms'
    tier: 'Burstable'
  }
  properties: {
    administratorLogin: 'dbadmin'
    administratorLoginPassword: dbAdminPassword
    version: '15'
    storage: {
      storageSizeGB: 32
    }
    backup: {
      backupRetentionDays: 7
      geoRedundantBackup: 'Disabled'
    }
  }
}

// Create a database with a name specified
resource jobSearchDatabase 'Microsoft.DBforPostgreSQL/flexibleServers/databases@2023-03-01-preview' = {
  parent: postgresServer
  name: '${prefix}db'
  properties: {
    charset: 'UTF8'
    collation: 'en_US.UTF8'
  }
}

output dbHost string = postgresServer.properties.fullyQualifiedDomainName
output dbName string = jobSearchDatabase.name
