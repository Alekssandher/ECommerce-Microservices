# E-Commerce com Arquitetura de Microserviços

Sistema de e-commerce desenvolvido com arquitetura de microserviços para gerenciamento de estoque de produtos e vendas, implementando comunicação assíncrona e autenticação JWT.

## 🏗️ Arquitetura

O sistema é composto por 4 microserviços principais:

### 1. **API Gateway** (Port: 5001)
- Ponto de entrada único para todas as requisições
- Roteamento inteligente para os microserviços
- Rate limiting e cache
- Implementado com Ocelot

### 2. **AuthService** (Port: 5004)
- Autenticação e autorização de usuários
- Geração e validação de tokens JWT
- Gerenciamento de usuários (Cliente e Gerente)
- Criptografia de senhas com BCrypt

### 3. **SalesService** (Port: 5002)
- Gerenciamento de pedidos de venda
- Validação de estoque antes da confirmação
- Comunicação assíncrona com o serviço de estoque
- Controle de status de vendas (Pending, Confirmed, Canceled, Unauthorized)

### 4. **StockService** (Port: 5003)
- Gestão de produtos e estoque
- Controle de quantidade disponível e reservada
- Processamento de reservas e liberações de estoque
- CRUD de produtos

## 🛠️ Tecnologias Utilizadas

- **.NET 8**: Framework principal
- **Entity Framework Core**: ORM para acesso a dados
- **MySQL**: Banco de dados relacional
- **RabbitMQ**: Message broker para comunicação assíncrona
- **MassTransit**: Biblioteca para abstração do RabbitMQ
- **JWT**: Autenticação e autorização
- **Ocelot**: API Gateway
- **BCrypt.NET**: Criptografia de senhas
- **Serilog**: Sistema de logging
- **Docker & Docker Compose**: Containerização
- **xUnit**: Framework de testes unitários
- **Bogus**: Geração de dados para testes

## 🔄 Comunicação Entre Microserviços

O sistema utiliza eventos assíncronos via RabbitMQ:

### Fluxo de Venda:
1. **SaleCreated** → StockService reserva estoque
2. **SaleItemsReservedResponse** → SalesService confirma a venda
3. **SaleConfirmed** → StockService remove do estoque
4. **SaleCanceled** → StockService libera estoque reservado

### Eventos Implementados:
- `SaleCreated`: Novo pedido criado
- `SaleItemsReservedResponse`: Itens reservados com sucesso
- `SaleCreationFailed`: Falha na criação da venda
- `SaleConfirmed`: Venda confirmada
- `SaleCanceled`: Venda cancelada
- `StockReleased`: Estoque liberado
- `StockCanceled`: Cancelamento de estoque

## 🔐 Sistema de Autenticação

### Tipos de Usuário:
- **Client**: Pode realizar compras e consultar seus pedidos
- **Manager**: Pode gerenciar produtos, estoque e criar novos gerentes

### Endpoints de Autenticação:
- `POST /auth/login` - Login de usuários
- `POST /auth/register/user` - Registro de clientes
- `POST /auth/register/manager` - Registro de gerentes (requer autenticação de Manager)

## 📊 Funcionalidades Principais

### Gestão de Produtos (Manager):
- Cadastro de novos produtos
- Atualização de informações dos produtos
- Consulta de produtos e preços

### Gestão de Estoque (Manager):
- Consulta de estoque disponível e reservado
- Reserva e liberação de itens
- Remoção de estoque

### Gestão de Vendas (Client):
- Criação de pedidos
- Consulta de pedidos realizados
- Cancelamento de pedidos pendentes

## 🚀 Como Executar

### Pré-requisitos:
- Docker
- Docker Compose

### Executando com Docker:

1. **Clone o repositório:**
```bash
git clone <url-do-repositorio>
cd ecommerce-microservices
```

2. **Execute os containers:**
```bash
docker-compose up -d
```

3. **Aguarde os serviços ficarem prontos:**
- MySQL: Porta 3306
- RabbitMQ: Portas 5672 (AMQP) e 15672 (Management UI)
- API Gateway: Porta 5001

### URLs dos Serviços:

- **API Gateway**: http://localhost:5001
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)
- **AuthService**: http://localhost:5004
- **SalesService**: http://localhost:5002
- **StockService**: http://localhost:5003

## 📝 Endpoints da API

### Autenticação:
```http
POST /auth/login
POST /auth/register/user
POST /auth/register/manager (Manager only)
```

### Produtos:
```http
GET    /products                    # Listar produtos
GET    /products/{productId}        # Consultar produto
POST   /products                    # Criar produto (Manager only)
PUT    /products                    # Atualizar produto (Manager only)
```

### Estoque:
```http
GET    /stock/{productId}                      # Consultar estoque (Manager only)
GET    /stock/{productId}/available            # Consultar disponível (Manager only)
POST   /stock/{productId}/reserve/{quantity}   # Reservar estoque (Manager only)
POST   /stock/{productId}/release/{quantity}   # Liberar estoque (Manager only)
DELETE /stock/{productId}/remove/{quantity}    # Remover estoque (Manager only)
```

### Vendas:
```http
GET    /sales/{id}      # Consultar venda (Client only)
PATCH  /sales/{id}      # Atualizar venda (Client only)
DELETE /sales/{id}      # Cancelar venda (Client only)
```

## 🧪 Testes

O projeto inclui testes unitários abrangentes para:
- Mapeadores (Mappers)
- Modelos (Models)
- Serviços (Services)

### Executar testes:
```bash
dotnet test
```

### Cobertura de Testes:
- AuthService: Modelos, serviços JWT e mapeadores
- SalesService: Modelos, serviços de venda e mapeadores
- StockService: Modelos, serviços de produto/estoque e mapeadores

## 🗂️ Estrutura do Projeto

```
├── AuthService/                 # Microserviço de autenticação
├── Services/
│   ├── SalesService/           # Microserviço de vendas
│   └── StockService/           # Microserviço de estoque
├── Gateway/                    # API Gateway
├── Shared/                     # Bibliotecas compartilhadas
│   ├── Extensions/            # Extensions para JWT, RabbitMQ, etc.
│   ├── Messages/              # Eventos de comunicação
│   ├── Middlewares/           # Middleware global de exceções
│   └── ModelViews/            # DTOs e responses padronizadas
├── ECommerce-Microservices.Tests/ # Projetos de teste
├── docker-compose.yml         # Configuração Docker
└── README.md
```

## 📋 Bancos de Dados

Cada microserviço possui seu próprio banco de dados:
- `users_auth_service`: Dados de usuários e autenticação
- `sales_micro_service`: Dados de vendas e pedidos
- `stock_micro_service`: Dados de produtos e estoque

## 🔍 Monitoramento e Logs

- **Serilog**: Logging estruturado para todos os serviços
- **Health Checks**: Endpoint `/health` em cada serviço
- **RabbitMQ Management**: Interface web para monitoramento de filas

## 🛡️ Segurança

- Autenticação JWT com chaves simétricas
- Autorização baseada em roles (Client/Manager)
- Criptografia de senhas com BCrypt
- Rate limiting no API Gateway
- Validação de entrada em todos os endpoints

## 🎯 Boas Práticas Implementadas

- **Separation of Concerns**: Cada microserviço tem responsabilidade única
- **Event-Driven Architecture**: Comunicação assíncrona via eventos
- **Exception Handling**: Middleware global para tratamento de exceções
- **Logging**: Logs estruturados em todos os serviços
- **Testing**: Cobertura abrangente de testes unitários
- **Containerization**: Aplicação totalmente dockerizada
- **Database per Service**: Cada microserviço com seu próprio banco

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## 📞 Suporte

Para dúvidas ou suporte, entre em contato através das issues do GitHub.
