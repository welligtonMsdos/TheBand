# Backup do Core

O `TheBand.CoreWorker` exporta as tabelas `Cassette`, `CD`, `Concert` e `Vinyl` ao iniciar e depois a cada hora. Cada execução grava um arquivo `ddMMyyyyHHmmss.txt` na pasta `bkp` do diretório de trabalho da aplicação. Os campos são separados por `;`, e valores com ponto e vírgula, aspas ou quebra de linha são delimitados por aspas.

Configure a mesma conexão PostgreSQL usada pela Core API por meio da variável de ambiente `ConnectionStrings__CollectionConnection`. Por exemplo, no PowerShell:

```powershell
$env:ConnectionStrings__CollectionConnection = 'Host=localhost;Database=theband.Core;Username=usuario;Password=senha'
dotnet run --project TheBand.CoreWorker
```

Mantenha o processo em execução para os backups seguintes. Monte a pasta `bkp` em armazenamento persistente ao hospedar o worker em um contêiner ou serviço com disco temporário.

## Docker Compose

O serviço `core-worker` usa a conexão definida em `TheBand.CoreApi/.env` e monta `./bkp` do repositório em `/app/bkp` no contêiner. Para iniciar e ver os arquivos:

```powershell
docker compose up -d --build core-worker
Get-ChildItem .\bkp
Get-Content .\bkp\NOME_DO_ARQUIVO.txt
```

Dentro do contêiner, use `docker compose exec core-worker ls -l /app/bkp`. Para acompanhar a execução, use `docker compose logs -f core-worker`.
