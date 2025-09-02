# E-Commerce Microservices

> 🚧 **Projeto em Desenvolvimento** - Este é um projeto em desenvolvimento ativo e algumas funcionalidades ainda estão sendo implementadas.

Um sistema de e-commerce construído com arquitetura de microsserviços usando .NET 9, focado em escalabilidade, resiliência e separação de responsabilidades.

## 📋 Visão Geral

Este projeto implementa um sistema de e-commerce distribuído usando o padrão de microsserviços, com comunicação assíncrona via RabbitMQ e API Gateway para roteamento de requisições.

### Arquitetura

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   API Gateway   │────│   Sales Service  │────│  Stock Service  │
│    (Ocelot)     │    │   (Vendas)       │    │   (Estoque)     │
└─────────────────┘    └──────────────────┘    └─────────────────┘
         │                        │                        │
         │                        └────────────────────────┤
         │                                  RabbitMQ       │
         └─────────────────────────────────────────────────┘
```

## 🛠️ Tecnologias Utilizadas

- **.NET 9** - Framework principal
- **Ocelot** - API Gateway
- **Entity Framework Core** - ORM
- **MySQL** - Banco de dados
- **MassTransit** - Message broker abstraction
- **RabbitMQ** - Sistema de mensageria
- **Docker** - Containerização (planejado)

## 🏗️ Estrutura do Projeto

```
ECommerce-Microservices/
├── Gateway/                    # API Gateway (Ocelot)
├── Services/
│   ├── SalesService/          # Serviço de Vendas
│   └── StockService/          # Serviço de Estoque
├── Shared/                    # Bibliotecas compartilhadas
│   └── Shared/
│       ├── DTOs/             # Data Transfer Objects
│       ├── Messages/         # Mensagens para comunicação
│       ├── Extensions/       # Extensões utilitárias
│       └── ModelViews/       # Modelos de resposta da API
└── ECommerce-Microservices.sln
```

## 🚀 Microsserviços

### API Gateway
- **Porta**: 5001 (HTTP) / 7018 (HTTPS)
- **Responsabilidade**: Roteamento de requisições, rate limiting, cache
- **Tecnologia**: Ocelot

### Sales Service
- **Porta**: 5002 (HTTP) / 7117 (HTTPS)
- **Responsabilidade**: Gerenciamento de vendas e pedidos
- **Banco de dados**: MySQL (`sales_micro_service`)
- **Funcionalidades**:
  - Criação de vendas
  - Confirmação de vendas
  - Cancelamento de vendas
  - Consulta de vendas

### Stock Service
- **Porta**: A ser definida
- **Responsabilidade**: Gerenciamento de estoque e produtos
- **Funcionalidades**:
  - Reserva de estoque
  - Liberação de estoque
  - Remoção de estoque
  - Consulta de disponibilidade

## 📨 Comunicação entre Serviços

### Mensagens (Event-Driven)

- **SaleCreated**: Dispara reserva de estoque
- **SaleConfirmed**: Remove estoque reservado
- **SaleCanceled**: Libera estoque reservado
- **StockReleased**: Confirma liberação de estoque
- **StockCanceled**: Confirma cancelamento de estoque

### Fluxo de Eventos

1. **Criação de Venda**: `SalesService` → `SaleCreated` → `StockService` (reserva estoque)
2. **Confirmação**: `SalesService` → `SaleConfirmed` → `StockService` (remove estoque)
3. **Cancelamento**: `SalesService` → `SaleCanceled` → `StockService` (libera estoque)

## ⚙️ Configuração e Execução

### Pré-requisitos

- .NET 9 SDK
- MySQL Server
- RabbitMQ
- Visual Studio 2022 ou VS Code

### Configuração do Banco de Dados

1. Configure a string de conexão MySQL:
```json
{
  "ConnectionStrings": {
    "mySql": "Server=localhost;Database=sales_micro_service;Uid=admin;Pwd=1234;"
  }
}
```

### Configuração do RabbitMQ

1. Configure as credenciais do RabbitMQ:
```json
{
  "RabbitMq": {
    "url": "amqp://localhost:5672",
    "user": "guest",
    "password": "guest"
  }
}
```

### Executar os Serviços

1. **API Gateway**:
```bash
cd Gateway
dotnet run
```

2. **Sales Service**:
```bash
cd Services/SalesService
dotnet run
```

3. **Stock Service**:
```bash
cd Services/StockService
dotnet run
```

## 📋 Status de Desenvolvimento

### ✅ Implementado
- [x] Estrutura básica dos microsserviços
- [x] API Gateway com Ocelot
- [x] Configuração do RabbitMQ
- [x] Modelos de dados básicos
- [x] Sistema de mensageria
- [x] Consumers para eventos
- [x] Configuração do Entity Framework

### 🔄 Em Desenvolvimento
- [ ] Implementação completa dos serviços
- [ ] Controllers REST
- [ ] Validações de negócio
- [ ] Tratamento de erros
- [ ] Configuração do Ocelot
- [ ] Testes unitários

### 📅 Planejado
- [ ] Authentication/Authorization
- [ ] Logging centralizado
- [ ] Monitoramento (Health Checks)
- [ ] Documentação da API (Swagger)
- [ ] Containerização (Docker)
- [ ] CI/CD Pipeline
- [ ] Testes de integração

## 🤝 Contribuição

Este projeto está em desenvolvimento ativo. Contribuições são bem-vindas!

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -am 'Adiciona nova feature'`)
4. Push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

**Nota**: Este README será atualizado conforme o desenvolvimento do projeto progride.
