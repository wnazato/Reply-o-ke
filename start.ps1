# Script de Configuração do Reply-O-Ke

Write-Host "=== Reply-O-Ke - Configuração Inicial ===" -ForegroundColor Green
Write-Host ""

# Verificar se a API Key do YouTube está configurada
$configPath = ".\Reply-o-ke\appsettings.json"
if (Test-Path $configPath) {
    $config = Get-Content $configPath | ConvertFrom-Json
    
    if ($config.YouTube.ApiKey -eq "YOUR_YOUTUBE_API_KEY_HERE") {
        Write-Host "⚠️  ATENÇÃO: API Key do YouTube não configurada!" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Para usar a funcionalidade de busca do YouTube, você precisa:" -ForegroundColor White
        Write-Host "1. Criar uma API Key no Google Cloud Console" -ForegroundColor White
        Write-Host "2. Habilitar a YouTube Data API v3" -ForegroundColor White
        Write-Host "3. Substituir 'YOUR_YOUTUBE_API_KEY_HERE' pela sua API Key em:" -ForegroundColor White
        Write-Host "   $configPath" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Tutorial: https://developers.google.com/youtube/v3/getting-started" -ForegroundColor Blue
        Write-Host ""
        
        $apiKey = Read-Host "Digite sua API Key do YouTube (ou pressione Enter para continuar sem configurar)"
        
        if (![string]::IsNullOrWhiteSpace($apiKey)) {
            $config.YouTube.ApiKey = $apiKey
            $config | ConvertTo-Json -Depth 10 | Set-Content $configPath
            Write-Host "✅ API Key configurada com sucesso!" -ForegroundColor Green
        } else {
            Write-Host "⚠️  Continuando sem API Key. A busca do YouTube não funcionará." -ForegroundColor Yellow
        }
    } else {
        Write-Host "✅ API Key do YouTube já configurada!" -ForegroundColor Green
    }
} else {
    Write-Host "❌ Arquivo de configuração não encontrado: $configPath" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=== Iniciando o aplicativo ===" -ForegroundColor Green
Write-Host ""
Write-Host "O Reply-O-Ke será iniciado em: https://localhost:7072" -ForegroundColor Cyan
Write-Host ""
Write-Host "Funcionalidades:" -ForegroundColor White
Write-Host "• Pagina inicial: Criar sessao de karaoke" -ForegroundColor White
Write-Host "• /join/{id}: Participantes adicionam musicas" -ForegroundColor White
Write-Host "• /host/{id}: Host gerencia fila e reproduz videos" -ForegroundColor White
Write-Host ""

# Executar o aplicativo
dotnet run --project .\Reply-o-ke\Reply-o-ke.csproj
