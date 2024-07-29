<h1 align="center">
  <br>
   API Service Quotes
  <br>
</h1>
<h4 align="center">Projeto Service Quote API desenvolvido com <a href="https://dotnet.microsoft.com/pt-br/apps/aspnet" target="_blank">ASP.NET Core</a>, utilizando serviços da <a href="https://aws.amazon.com/pt/?nc2=h_lg">AWS S3</a> e hospedagem e banco de dados da <a href="https://azure.microsoft.com/pt-br/" target="_blank">Azure</a></h4>

<div align='center'>

[![Build and deploy Azure Web App](https://github.com/evans-costa/backend-servicequotes/actions/workflows/deploy.yml/badge.svg)](https://github.com/evans-costa/backend-servicequotes/actions/workflows/deploy.yml)

![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![MicrosoftSQLServer](https://img.shields.io/badge/Microsoft%20SQL%20Server-CC2927?style=for-the-badge&logo=microsoft%20sql%20server&logoColor=white)
![AWS](https://img.shields.io/badge/AWS-%23FF9900.svg?style=for-the-badge&logo=amazon-aws&logoColor=white)
![Azure](https://img.shields.io/badge/azure-%230072C6.svg?style=for-the-badge&logo=microsoftazure&logoColor=white)

<p align="center">
  <a href="#introdução">Introdução</a> •
  <a href="#funcionalidades">Funcionalidades</a> •
  <a href="#motivação">Motivação</a> •
  <a href="#instalação">Instalação</a> •
  <a href="#configuração">Configuração</a> •
  <a href="#estrutura-do-projeto">Estrutura do Projeto</a> •
  <a href="#créditos">Créditos</a> •
  <a href="#licença">Licença</a>
</p>

</div>

## Introdução

Essa é uma API desenvolvida para o projeto Service Quote, usando .NET 8.0 com ASP.NET Core e Entity Framework Core, que permite criar e gerenciar clientes, produtos e cotações de serviços, emitindo arquivos em PDF para envio do orçamento consolidado ao cliente. A aplicação segue os princípios da Clean Architecture para facilitar melhor manutenção e possível escalabilidade do sistema, além de garantir a correta separação de responsabilidades.

O projeto está em produção usando os serviços da Azure, porém com acesso restrito devido ao free tier não ser tão generoso, pretendo disponibilizá-lo para maior acesso, mas enquanto isso você pode conferir uma demo hospedada no YouTube aqui.

## Funcionalidades

- Criar cotações (orçamentos) de serviços
- Criar produtos e obter produtos cadastrados
- Criar clientes e obter clientes cadastrados
- Paginação de resultados de clientes e produtos
- Geração de PDF com o orçamento consolidado (um template do documento gerado pode ser visto aqui)
- Integração com o serviço S3 da Amazon para armazenar os documentos gerados
  
## Motivação

Meu grande objetivo com esse projeto foi criar algo que "tocasse" o mundo real e que resolvesse um problema que via diariamente, nesse caso, foi o gerenciamento de orçamentos para serviços de eletricista de um familiar, como ele fazia tudo manualmente, em papel, e por vezes a cotação de materiais utilizados, o contato do cliente e o valor do orçamento se perdia, desenvolvi esse sistema para ajudá-lo a organizar o seu trabalho. Além disso, serviu para aprender uma linguagem nova e conceitos e paradigmas que essa linguagem trás, como POO, SOLID e Clean Architecture.

## Instalação

- Para instalar o projeto, você precisará ter o [.NET 8.0](https://dotnet.microsoft.com/pt-br/download) instalado na sua máquina. Opcionalmente você pode instalar o [Docker](https://www.docker.com/get-started/) com [Docker Compose](https://docs.docker.com/compose/install/) para rodar o serviço de banco de dados localmente. Da sua linha de comando:

  ```sh
  # Clone este repositório
  $ git clone https://github.com/evans-costa/backend-servicequotes.git

  # Entre na pasta do repositório clonado
  $ cd backend-servicequotes
  ```

- Restaure as dependências do projeto
  ```sh
  # Restaure as depedências
  $ dotnet restore
  ```

  ## Configuração

- Configure as variáveis de ambiente necessárias no arquivo `appsettings.Development.json` ou `secrets.json`:

	```appsettings.Development.json
    // appsettings.Development.json
    {
	  "ConnectionStrings": {
	    "DefaultConnection": "Server=localhost,1433;Database=<your-database-name>;User Id=sa;Password=<your-password>;TrustServerCertificate=True;"
	  },
	  "AWS": {
        "BucketName": "<your-bucket-name>",
        "ProfileName": "<your-profile-name>",
        "AccessKey": "<your-access-key-id>",
        "SecretKey": "<your-secret-key>",
        "Region": "<your-bucket-region>"
      }
   }	
   ```
    
   ```sh
    // secrets.json
    dotnet user-secrets init
    
    dotnet user-secrets set "ConnectionsStrings:DefaultConnection" "Server=localhost,1433;Database=<your-database-name>;User Id=sa;Password=<your-password>;TrustServerCertificate=True;"    

    dotnet user-secrets set "AWS:BucketName" "<your-bucket-name>"
    dotnet user-secrets set "AWS:ProfileName" "<your-profile-name>"
    dotnet user-secrets set "AWS:AccessKey" "<your-access-key-id>"
    dotnet user-secrets set "AWS:SecretKey" "<your-secret-key>"
    dotnet user-secrets set "AWS:Region" "<your-bucket-region>"
   ```
   > **📌 Nota:** </br>
   > Caso não tenha um usuário cadastrado na AWS para correta configuração dos perfis e resolução do usuário da AWS, por favor, siga a documentação do SDK para .NET [nesse link](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/welcome.html).

- Se você está usando o Docker, rode o seguinte comando, não esquecendo de usar a mesma senha definida nas variáveis de ambiente no arquivo `docker-compose.yml`:

  ```sh
  # Suba o container do SQL Server
  $ docker compose up -d
  ```
  > **📌 Nota:** </br>
   > Se você não estiver usando o Docker, precisará instalar e configurar o SQL Server Management disponível [nesse link](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16).

- Rode as migrações para criar o banco de dados e suas tabelas:

  ```sh
  # Rode as migrações
  $ dotnet ef database update
  ```

- Você agora está pronto para rodar o projeto, aperte F5 no teclado, caso esteja usando o Visual Studio 2022, ou use o comando:

  ```sh
  # Rode o projeto
  $ dotnet watch run --launch-profile https --project src\ServiceQuotes.API\
  ```

  Isso irá abrir a interface do Swagger no seu navegador, onde você poderá consultar e testar os endpoints da aplicação, bem como ver os DTO's usados e os exemplos de requisição e resposta. Como alternativa, você pode testar usando seu cliente API REST favorito como o [Insomnia](https://insomnia.rest/download) ou [Postman](https://www.postman.com/).

## Estrutra do projeto

### Diagrama UML

### Diagrama Banco de Dados

  
## TODO / Melhorias

### Funcionalidades

- [ ] Criar endpoints para atualização e deleção de clientes, produtos e cotações
- [ ] Fazer autenticação do usuário

### Melhorias técnicas

- [ ] Escrever testes para a aplicação (em andamento)
- [ ] Script para rodar as migrações
- [ ] Seed do banco de dados para propósitos de desenvolvimento

### Infraestrutura

- [ ] Configurar CI/CD para rodar os testes
  
## Licença

The MIT License (MIT) 2024 - Evandro Costa. Por favor, dê uma olhada no arquivo LICENSE para mais detalhes.