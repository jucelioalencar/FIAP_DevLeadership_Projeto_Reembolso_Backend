# Script de deploy para Azure
# Execute este script para fazer deploy da infraestrutura no Azure

param(
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory=$true)]
    [string]$Location = "Brazil South",
    
    [Parameter(Mandatory=$true)]
    [string]$AppName
)

Write-Host "Iniciando deploy da infraestrutura Azure..." -ForegroundColor Green

# 1. Criar Resource Group
Write-Host "Criando Resource Group: $ResourceGroupName" -ForegroundColor Yellow
az group create --name $ResourceGroupName --location $Location

# 2. Criar App Service Plan
Write-Host "Criando App Service Plan..." -ForegroundColor Yellow
az appservice plan create --name "$AppName-plan --resource-group $ResourceGroupName --sku B1 --is-linux

# 3. Criar App Services para cada microsserviço
$services = @("gateway", "ingestion", "validation", "analysis", "notification")

foreach ($service in $services) {
    Write-Host "Criando App Service: $AppName-$service" -ForegroundColor Yellow
    az webapp create --resource-group $ResourceGroupName --plan "$AppName-plan" --name "$AppName-$service" --runtime "DOTNET|8.0"
}

# 4. Criar SQL Database
Write-Host "Criando SQL Server..." -ForegroundColor Yellow
az sql server create --name "$AppName-sql" --resource-group $ResourceGroupName --location $Location --admin-user "corisadmin" --admin-password "Coris@2024!"

Write-Host "Criando SQL Database..." -ForegroundColor Yellow
az sql db create --resource-group $ResourceGroupName --server "$AppName-sql" --name "CorisSeguros" --service-objective Basic

# 5. Criar Storage Account
Write-Host "Criando Storage Account..." -ForegroundColor Yellow
az storage account create --name "$AppName"storage --resource-group $ResourceGroupName --location $Location --sku Standard_LRS

# 6. Criar Service Bus Namespace
Write-Host "Criando Service Bus Namespace..." -ForegroundColor Yellow
az servicebus namespace create --name "$AppName-bus" --resource-group $ResourceGroupName --location $Location --sku Standard

# 7. Criar Key Vault
Write-Host "Criando Key Vault..." -ForegroundColor Yellow
az keyvault create --name "$AppName-vault" --resource-group $ResourceGroupName --location $Location

# 8. Criar Application Insights
Write-Host "Criando Application Insights..." -ForegroundColor Yellow
az monitor app-insights component create --app "$AppName-insights" --location $Location --resource-group $ResourceGroupName

Write-Host "Deploy da infraestrutura concluído!" -ForegroundColor Green
Write-Host "Próximos passos:" -ForegroundColor Cyan
Write-Host "1. Configure as connection strings nos App Services" -ForegroundColor White
Write-Host "2. Configure o Key Vault com os segredos" -ForegroundColor White
Write-Host "3. Faça deploy dos microsserviços" -ForegroundColor White



