# SafeMind - Rede Social para Neurodivergentes

## Sobre o Projeto
O SafeMind é uma plataforma de fóruns projetada para oferecer segurança e acessibilidade sensorial. 
Desenvolvido como projeto final para a disciplina de [Nome da Disciplina].

##  Tecnologias Utilizadas
- **Backend:** .NET 8 (C#)
- **Arquitetura:** Clean Architecture
- **Banco de Dados:** PostgreSQL
- **Containerização:** Docker & Docker Compose
- **Documentação API:** Swagger / OpenAPI
- **Testes:** xUnit / Moq

##  Arquitetura
O projeto segue os princípios da **Clean Architecture**, dividido em:
- **Domain:** Entidades e regras de negócio puras.
- **Application:** Casos de uso e interfaces.
- **Infrastructure:** Acesso a dados (EF Core) e integrações externas.
- **WebAPI:** Controllers e configuração de serviços.

##  Como Executar
1. Clone o repositório.
2. Certifique-se de ter o Docker instalado.
3. No terminal, execute: `docker-compose up -d`.
4. Acesse o Swagger em: `https://localhost:5001/swagger`.

## 👥 Integrantes
- Matheus Henrique da Silva
- Gustavo Henrique
