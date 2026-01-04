// =============================================================================
// params
// =============================================================================
@description('The location of all resources')
param location string = resourceGroup().location
param keyVaultName string = 'kv-data-${uniqueString(resourceGroup().id)}'

// =============================================================================
// modules
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
// outputs
// =============================================================================
output apiIdentityId string = apiIdentity.outputs.identityId
output scraperIdentityId string = scraperIdentity.outputs.identityId
output keyVaultId string = keyVault.outputs.vaultId
