# 🐾 Pet Health API

> **FIAP — Challenge 2026 | Sprint 1 + Sprint de Observabilidade e Testes**
> Advanced Business Development with .NET


---

## 📋 Descrição do Projeto

A **Pet Health API** é uma API RESTful desenvolvida em **ASP.NET Core (.NET 8)** como parte do Challenge 2026 da FIAP, em parceria com a **CLYVO VET**.

O projeto resolve um problema real do mercado pet: a **fragmentação da jornada de saúde do animal**. Tutores esquecem vacinas, perdem histórico clínico e clínicas perdem recorrência por falta de acompanhamento preventivo.

A API centraliza **tutores, pets, vacinas, consultas e medicamentos**, formando a base da plataforma Pet Health — uma solução de cuidado contínuo e preventivo para pets.

Nesta sprint, o projeto evoluiu com **camadas de monitoramento, observabilidade e testes automatizados**: Health Checks, logging estruturado com correlação de requisições, distributed tracing e métricas customizadas com OpenTelemetry, uma camada de domínio testável, e suítes de testes unitários e de integração seguindo o padrão AAA.

### 🏗️ Tecnologias Utilizadas

| Tecnologia | Versão | Finalidade |
|---|---|---|
| ASP.NET Core | .NET 8 | Framework principal da API |
| Entity Framework Core | 8.0 | ORM para mapeamento das entidades |
| Oracle.EntityFrameworkCore | 8.21.121 | Driver de conexão com Oracle |
| Swashbuckle (Swagger) | 6.5.0 | Documentação OpenAPI |
| Oracle Database | — | Banco de dados (servidor FIAP) |
| Microsoft.Extensions.Diagnostics.HealthChecks | 10.0.11 | Health Checks nativos |
| AspNetCore.HealthChecks.Oracle | 9.0.0 | Health Check de conectividade com Oracle |
| Serilog.AspNetCore | 10.0.0 | Logging estruturado (console + arquivo) |
| OpenTelemetry.Extensions.Hosting | 1.18.0 | Tracing e métricas distribuídas |
| OpenTelemetry.Instrumentation.AspNetCore | 1.18.0 | Instrumentação automática de requisições HTTP |
| xUnit | 2.5.3 | Framework de testes |
| Moq | 4.20.72 | Mock de dependências |
| FluentAssertions | 8.10.0 | Assertivas fluentes nos testes |
| Microsoft.AspNetCore.Mvc.Testing | 8.0.11 | Testes de integração com WebApplicationFactory |

---

## 🗂️ Estrutura do Projeto

A solução foi reorganizada em `src/` (código de produção) e `tests/` (testes), seguindo o padrão de separação DDD-lite:

```
PetHealthAPI.sln
├── src/
│   └── PetHealthAPI/
│       ├── Controllers/
│       │   ├── TutoresController.cs
│       │   ├── PetsController.cs
│       │   ├── VacinasController.cs
│       │   ├── ConsultasController.cs
│       │   └── MedicamentosController.cs
│       ├── Data/
│       │   └── AppDbContext.cs
│       ├── Models/
│       │   ├── Tutor.cs
│       │   ├── Pet.cs
│       │   ├── Vacina.cs
│       │   ├── Consulta.cs
│       │   └── Medicamento.cs
│       ├── Dominio/
│       │   └── Validacoes/
│       │       └── PetValidador.cs        # Regras de negócio (domínio rico)
│       ├── Aplicacao/
│       │   └── Middlewares/
│       │       └── CorrelationIdMiddleware.cs
│       ├── Infraestrutura/
│       │   ├── Health/
│       │   │   └── ServicoExternoHealthCheck.cs
│       │   └── Observabilidade/
│       │       └── AplicacaoMetricas.cs   # Meter + Counter customizado
│       ├── appsettings.json
│       └── Program.cs
└── tests/
    ├── PetHealthAPI.Tests.Unit/
    │   └── Dominio/
    │       └── PetValidadorTests.cs
    └── PetHealthAPI.Tests.Integration/
        └── PetsEndpointsTests.cs
```

---

## 🗄️ Modelo de Banco de Dados

```
TB_PH_TUTOR (1) ──── (N) TB_PH_PET
                              │
              ┌───────────────┼──────────────────┐
              │               │                  │
        TB_PH_VACINA   TB_PH_CONSULTA   TB_PH_MEDICAMENTO
```

---

## 🩺 Monitoramento e Observabilidade

### Health Checks

O endpoint `/health` verifica a saúde da aplicação, a conectividade com o banco Oracle e a disponibilidade de um serviço externo simulado:

```
GET /health
```

Resposta (JSON detalhado por dependência):

```json
{
  "status": "Healthy",
  "checks": [
    {
      "nome": "oracle-database",
      "status": "Healthy",
      "duracaoMs": 12.5,
      "dados": {}
    },
    {
      "nome": "servico-externo",
      "status": "Healthy",
      "duracaoMs": 8.3,
      "dados": { "LatenciaMs": 22 }
    }
  ],
  "duracaoTotalMs": 21.1
}
```

- `oracle-database`: usa `AddOracle`, testando a conexão real com o banco configurado em `appsettings.json`.
- `servico-externo`: check customizado (`ServicoExternoHealthCheck`) que simula ~90% de disponibilidade, útil para demonstrar um cenário de falha controlada.

### Logging Estruturado (Serilog)

- Saída simultânea em **console** e **arquivo** (`logs/app-{data}.log`, rotação diária).
- Cada linha de log inclui um **Correlation ID** (`X-Correlation-ID`), propagado automaticamente pelo `CorrelationIdMiddleware` — gerado se o cliente não enviar um, e devolvido no header da resposta.
- Formato: `[HH:mm:ss NÍVEL] [CorrelationId] Mensagem`

### Tracing e Métricas (OpenTelemetry)

- **Tracing**: Span manual `CriarPetEndpoint` no `POST /api/pets`, com tags `pet.nome` e `pet.tutorId`, além da instrumentação automática do ASP.NET Core (spans por requisição HTTP).
- **Métricas**: Counter customizado `pets_criados_total` (unidade `{pets}`), com tag `status` (`sucesso` ou `erro_validacao`), expondo o volume de criações de pets por resultado.
- Exportação via `AddConsoleExporter` — os spans e métricas aparecem no console da aplicação em execução.

> **Como visualizar**: rode a API (veja seção de execução) e observe o console — cada requisição imprime o Span correspondente, e as métricas são exportadas periodicamente.

---

## 🧪 Testes Automatizados

O projeto conta com duas suítes de teste, organizadas por camada:

### Testes Unitários (`tests/PetHealthAPI.Tests.Unit`)

Cobrem a camada de **Domínio** (`PetValidador`), isoladamente, sem dependência de banco ou HTTP. Seguem o padrão **AAA** (Arrange, Act, Assert) com xUnit e FluentAssertions.

```bash
dotnet test tests/PetHealthAPI.Tests.Unit
```

### Testes de Integração (`tests/PetHealthAPI.Tests.Integration`)

Validam o fluxo HTTP completo usando `WebApplicationFactory<Program>`, cobrindo:
- `GET /health` → 200
- `POST /api/pets` com dados válidos → 201
- `POST /api/pets` com dados inválidos → 400

```bash
dotnet test tests/PetHealthAPI.Tests.Integration
```

### Rodar toda a solução de uma vez

```bash
dotnet test
```

> **Nota**: os testes de integração conectam no Oracle real (mesma connection string do `appsettings.json`) e assumem a existência de um Tutor com `id = 1` no banco.

---

## ⚠️ Limitação Conhecida

O endpoint `GET /api/tutores` pode retornar erro `500` (`JsonException: A possible object cycle was detected`) quando um tutor possui pets vinculados, devido à referência circular entre `Tutor.Pets` e `Pet.Tutor` na serialização JSON do Entity Framework. Correção sugerida: configurar `ReferenceHandler.Preserve` ou `[JsonIgnore]` na propriedade de navegação reversa. Não afeta os demais endpoints (`GET /api/pets`, `POST`, etc.).

---

## 🛣️ Documentação das Rotas

### 👤 Tutores — `/api/tutores`

| Método | Rota | Descrição | HTTP |
|--------|------|-----------|------|
| GET | `/api/tutores` | Lista todos os tutores | 200 |
| GET | `/api/tutores/{id}` | Busca tutor por ID | 200 / 404 |
| GET | `/api/tutores/email/{email}` | Busca tutor por email | 200 / 404 |
| GET | `/api/tutores/nome/{nome}` | Busca tutores por nome (parcial) | 200 |
| GET | `/api/tutores/{id}/pets` | Lista os pets de um tutor | 200 / 404 |
| POST | `/api/tutores` | Cadastra novo tutor | 201 / 400 |
| PUT | `/api/tutores/{id}` | Atualiza dados do tutor | 204 / 400 / 404 |
| DELETE | `/api/tutores/{id}` | Remove tutor | 204 / 404 |

### 🐶 Pets — `/api/pets`

| Método | Rota | Descrição | HTTP |
|--------|------|-----------|------|
| GET | `/api/pets` | Lista todos os pets | 200 |
| GET | `/api/pets/{id}` | Busca pet por ID (com histórico completo) | 200 / 404 |
| GET | `/api/pets/especie/{especie}` | Lista pets por espécie | 200 |
| GET | `/api/pets/nome/{nome}` | Busca pets por nome | 200 |
| GET | `/api/pets/castrados` | Lista apenas pets castrados | 200 |
| POST | `/api/pets` | Cadastra novo pet | 201 / 400 |
| PUT | `/api/pets/{id}` | Atualiza dados do pet | 204 / 400 / 404 |
| DELETE | `/api/pets/{id}` | Remove pet | 204 / 404 |

### 💉 Vacinas — `/api/vacinas`

| Método | Rota | Descrição | HTTP |
|--------|------|-----------|------|
| GET | `/api/vacinas` | Lista todas as vacinas | 200 |
| GET | `/api/vacinas/{id}` | Busca vacina por ID | 200 / 404 |
| GET | `/api/vacinas/pet/{petId}` | Lista vacinas de um pet | 200 / 404 |
| GET | `/api/vacinas/vencendo` | Vacinas com próxima dose em 30 dias | 200 |
| POST | `/api/vacinas` | Registra nova vacina | 201 / 400 |
| PUT | `/api/vacinas/{id}` | Atualiza dados da vacina | 204 / 400 / 404 |
| DELETE | `/api/vacinas/{id}` | Remove registro de vacina | 204 / 404 |

### 🏥 Consultas — `/api/consultas`

| Método | Rota | Descrição | HTTP |
|--------|------|-----------|------|
| GET | `/api/consultas` | Lista todas as consultas | 200 |
| GET | `/api/consultas/{id}` | Busca consulta por ID | 200 / 404 |
| GET | `/api/consultas/pet/{petId}` | Histórico clínico de um pet | 200 / 404 |
| GET | `/api/consultas/retornos` | Retornos agendados nos próximos 30 dias | 200 |
| POST | `/api/consultas` | Registra nova consulta | 201 / 400 |
| PUT | `/api/consultas/{id}` | Atualiza dados da consulta | 204 / 400 / 404 |
| DELETE | `/api/consultas/{id}` | Remove registro de consulta | 204 / 404 |

### 💊 Medicamentos — `/api/medicamentos`

| Método | Rota | Descrição | HTTP |
|--------|------|-----------|------|
| GET | `/api/medicamentos` | Lista todos os medicamentos | 200 |
| GET | `/api/medicamentos/{id}` | Busca medicamento por ID | 200 / 404 |
| GET | `/api/medicamentos/pet/{petId}` | Lista medicamentos de um pet | 200 / 404 |
| GET | `/api/medicamentos/ativos` | Lista medicamentos em uso ativo | 200 |
| POST | `/api/medicamentos` | Registra novo medicamento | 201 / 400 |
| PUT | `/api/medicamentos/{id}` | Atualiza dados do medicamento | 204 / 400 / 404 |
| DELETE | `/api/medicamentos/{id}` | Remove registro de medicamento | 204 / 404 |

### 🩺 Monitoramento

| Método | Rota | Descrição | HTTP |
|--------|------|-----------|------|
| GET | `/health` | Verifica saúde da API, Oracle e serviço externo | 200 / 503 |

---

## ⚙️ Instalação e Execução

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Git](https://git-scm.com/)
- Acesso à rede FIAP (VPN se necessário para o Oracle)

### Passo a Passo

**1. Clone o repositório**
```bash
git clone https://github.com/PedroClaes/PetHealthAPI.git
cd PetHealthAPI
```

**2. Restaure os pacotes NuGet de toda a solução**
```bash
dotnet restore
```

**3. Execute as Migrations para criar as tabelas no Oracle**
```bash
cd src/PetHealthAPI
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**4. Execute a aplicação** (a partir de `src/PetHealthAPI`)
```bash
dotnet run
```

**5. Acesse o Swagger**
```
http://localhost:5000
```

**6. Rode os testes** (a partir da raiz, onde está o `.sln`)
```bash
dotnet test
```

---

## 🔌 Configuração do Banco de Dados

A string de conexão está no arquivo `src/PetHealthAPI/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=rm556649;Password=280306;Data Source=oracle.fiap.com.br:1521/orcl;"
  }
}
```

---

## 📌 Retornos HTTP Utilizados

| Código | Significado | Quando ocorre |
|--------|-------------|---------------|
| 200 OK | Sucesso | GET com resultado |
| 201 Created | Criado com sucesso | POST bem-sucedido |
| 204 No Content | Sucesso sem corpo | PUT e DELETE bem-sucedidos |
| 400 Bad Request | Dados inválidos | Validação falhou ou dados inconsistentes |
| 404 Not Found | Não encontrado | Recurso inexistente no banco |
| 503 Service Unavailable | Indisponível | Health check reporta dependência fora do ar |

---

## 👥 Integrantes

| Nome | RM |
| Matheus Arazin de Oliveira | 556649 |
| Artur Pioli Silva| 565597 |
| Kevin Martins Campos| 563454 |
| Pedro Gabriel Claes| 566058 |

---

*Challenge 2026 — FIAP × CLYVO VET*
*"Cuidado contínuo para quem faz parte da família."*