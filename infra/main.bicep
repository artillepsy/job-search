// =============================================================================
// Params
// =============================================================================
@description('The location of all resources')
param location string = resourceGroup().location
@description('Key Vault name')
param keyVaultName string = 'kv-data-${uniqueString(resourceGroup().id)}'
@description('Scraper container image. Built by Azure and passed through CLI')
param scraperImage string
@description('Database admin password. Passed through CLI')
@secure()
param dbPassword string

// =============================================================================
// Modules
// =============================================================================
@description('Create identity for the Scraper project')
module scraperIdentity './modules/identity.bicep' = {
  name: 'scraper-identity-deploy'
  params: {
    identityName: 'id-data-scraper'
    location: location
  }
}

@description('Create identity for the API project')
module apiIdentity './modules/identity.bicep' = {
  name: 'api-identity-deploy'
  params: {
    identityName: 'id-web-api'
    location: location
  }
}

@description('Create key vault for application data storage')
module keyVault './modules/keyvault.bicep' = {
  name: 'keyvault-deploy'
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
  name: 'api-identity-keyvault-rbac-deploy'
  scope: resourceGroup()
  params: {
    principalId: apiIdentity.outputs.principalId
    roleDefinitionId: keyVaultSecretsUser
  }
}

@description('Assign Scraper Identity to Key Vault')
module scraperKvRbac './modules/rbac.bicep' = {
  name: 'scraper-keyvault-rbac-deploy'
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
  name: 'storage-deploy'
  params: {
    location: location
    prefix: prefix
    dbAdminPassword: dbPassword
    serverName: storageServerName
  }
}

// =============================================================================
// Deploy Apps
// =============================================================================
var dbConnectionString = 'Host=${database.outputs.dbHost};Database=${database.outputs.dbName};Username=dbadmin;Password=${dbPassword};SSL Mode=Require;Trust Server Certificate=true'

module scraper './modules/scraper-app.bicep' = {
  name: 'scraper-deploy'
  dependsOn: [
    database
    foundation
  ]
  params: {
    location: location
    containerRegistryName: foundation.outputs.containerRegistryName
    environmentId: foundation.outputs.environmentId
    scraperImage: scraperImage
    dbConnectionString: dbConnectionString
  }
}

/* 
module api './modules/api-app.bicep' = {
  name: 'api-deploy'
  dependsOn: [
    scraper
    database
    foundation
  ]
  params: {
    location: location
    containerRegistryName: foundation.outputs.containerRegistryName
    environmentId: foundation.outputs.environmentId
    dbConnectionString: dbConnectionString
  }
} */

// =============================================================================
// Outputs
// =============================================================================
output apiIdentityId string = apiIdentity.outputs.identityId
output scraperIdentityId string = scraperIdentity.outputs.identityId
output keyVaultId string = keyVault.outputs.vaultId
output keyVaultName string = keyVaultName
output ContainerRegistryName string = foundation.outputs.containerRegistryName
output StorageServerName string = storageServerName
