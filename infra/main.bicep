// =============================================================================
// Params
// =============================================================================
@description('The location of all resources')
param location string = resourceGroup().location
@description('Key Vault name')
param keyVaultName string = 'kv-data-${uniqueString(resourceGroup().id)}'
@description('Database admin password. Passed through CLI')
@secure()
param dbPassword string

// =============================================================================
// Modules
// =============================================================================
@description('Create identity for the API project')
module apiIdentity './modules/identity.bicep' = {
  name: 'apiIdentityDeployment'
  params: {
    identityName: 'id-web-api'
    location: location
  }
}

@description('Create identity for the Scraper project')
module scraperIdentity './modules/identity.bicep' = {
  name: 'scraperIdentityDeployment'
  params: {
    identityName: 'id-data-scraper'
    location: location
  }
}

@description('Create key vault for application data storage')
module keyVault './modules/keyvault.bicep' = {
  name: 'keyVaultDeployment'
  params: {
    vaultName: keyVaultName
    location: location
  }
}

// =============================================================================
// RBAC Assignments
// =============================================================================
@description('The role definition ID for Key Vault Secrets User (Azures built-in role)')
var keyVaultSecretsUser = '4633458b-17de-408a-b874-0445c86b69e6'

@description('Assign API Identity to Key Vault')
module apiIdentityKeyVaultRbac './modules/rbac.bicep' = {
  name: 'apiIdentityKeyVaultRbacDeployment'
  scope: resourceGroup()
  params: {
    principalId: apiIdentity.outputs.principalId
    roleDefinitionId: keyVaultSecretsUser
  }
}

@description('Assign Scraper Identity to Key Vault')
module scraperKvRbac './modules/rbac.bicep' = {
  name: 'scraperKvRbacDeployment'
  scope: resourceGroup()
  params: {
    principalId: scraperIdentity.outputs.principalId
    roleDefinitionId: keyVaultSecretsUser
  }
}

// =============================================================================
// Foundation
// =============================================================================
@description('Prefix for all resources')
var prefix = 'jobsearch'

module foundation './modules/foundation.bicep' = {
  name: 'foundation-deploy'
  params: {
    location: location
    prefix: prefix
  }
}

// =============================================================================
// Storage (PostgreSQL)
// =============================================================================
var storageServerName = '${prefix}-db-${uniqueString(resourceGroup().id)}'

module database './modules/storage.bicep' = {
  name: 'storageDeployment'
  params: {
    location: location
    prefix: 'jobsearch'
    dbAdminPassword: dbPassword
    serverName: storageServerName
  }
}

// =============================================================================
// Networking / Firewall
// =============================================================================
resource postgresServer 'Microsoft.DBforPostgreSQL/flexibleServers@2023-03-01-preview' existing = {
  name: storageServerName
}

resource allowAzure 'Microsoft.DBforPostgreSQL/flexibleServers/firewallRules@2023-03-01-preview' = {
  name: 'AllowAllAzureServices'
  parent: postgresServer
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// =============================================================================
// Deploy Apps
// =============================================================================
var dbConnectionString = 'Host=${database.outputs.dbHost};Database=jobsearch;Username=dbadmin;Password=${dbPassword};SSL Mode=Require;Trust Server Certificate=true'

module api './modules/api-app.bicep' = {
  name: 'api-deploy'
  params: {
    location: location
    environmentId: foundation.outputs.environmentId
    dbConnectionString: dbConnectionString
  }
}

module scraper './modules/scraper-app.bicep' = {
  name: 'scraper-deploy'
  params: {
    location: location
    containerRegistryName: foundation.outputs.containerRegistryName
    environmentId: foundation.outputs.environmentId
    dbConnectionString: dbConnectionString
  }
}

// =============================================================================
// Outputs
// =============================================================================
output apiIdentityId string = apiIdentity.outputs.identityId
output scraperIdentityId string = scraperIdentity.outputs.identityId
output keyVaultId string = keyVault.outputs.vaultId
output keyVaultName string = keyVaultName
output ContainerRegistryName string = foundation.outputs.containerRegistryName
