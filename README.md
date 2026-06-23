# 🧠 SafeMind - Ecossistema Seguro para Neurodivergentes

## 📖 Sobre o Projeto
O **SafeMind** é uma plataforma digital inovadora e inclusiva projetada para oferecer segurança, acessibilidade e moderação para usuários neurodivergentes, profissionais de saúde e empresas. O sistema implementa fluxos robustos de validação documental, barreiras de idade e mascaramento de gatilhos (Trigger Warnings) em postagens.

Desenvolvido como projeto final para a disciplina de **Desenvolvimento de Sistemas (3º Semestre - UniCEUB)**.

## 🚀 Tecnologias Utilizadas
- **Back-end:** C# com .NET 10.0
- **Front-end:** HTML5, CSS3 e JavaScript (Vanilla JS)
- **Arquitetura:** Clean Architecture & SOLID Principles
- **Banco de Dados:** PostgreSQL (via Entity Framework Core ORM)
- **Containerização:** Docker & Docker Compose
- **Documentação de API:** OpenAPI / Scalar UI
- **Segurança:** Autenticação via JWT (JSON Web Tokens) e Hash BCrypt
- **Qualidade e Testes:** xUnit & Moq (Suíte de Testes Automatizados)

## 🏗️ Arquitetura (Clean Architecture)
O projeto foi rigorosamente modularizado para garantir escalabilidade e separação de responsabilidades:
- **Domain:** Núcleo do sistema. Contém as entidades polimórficas (`User`, `UsuarioNeurodivergente`, `Profissional`, `Empresa`) e regras de negócio puras.
- **Application:** Orquestração de casos de uso (Services), validações lógicas (RN01, RN03) e Data Transfer Objects (DTOs).
- **Infrastructure:** Persistência de dados, configurações do EF Core, Migrations e contexto do banco de dados.
- **WebAPI:** Ponto de entrada do sistema, contendo os Controllers RESTful, injeção de dependências, CORS e o Middleware global de tratamento de exceções.
- **Tests:** Camada isolada garantindo a integridade das regras lógicas.

## ⚙️ Como Executar o Projeto

### Pré-requisitos
- [Docker Desktop](https://www.docker.com/) rodando na máquina.
- SDK do [.NET 10.0](https://dotnet.microsoft.com/) instalado.

## 👥 Integrantes
- **Matheus Henrique da Silva** - Arquitetura, Regras de Negócio e Front-end
- **Gustavo Henrique** - APIs CRUD, Refatoração e Infraestrutura DB

### Passo a Passo


**1. Clone o repositório:**
```bash
git clone [https://github.com/mhsilvaworks/SafeMind.git](https://github.com/SEU_USUARIO/SafeMind.git)
cd SafeMind