# Password Validator API

API Web em .NET 8 que valida senhas de acordo com regras de segurança predefinidas.

**Input:** uma senha (string) · **Output:** um booleano indicando se é válida.
---
## Regras de validação

| Regra | Descrição |
|-------|-----------|
| Comprimento mínimo | 9 ou mais caracteres |
| Dígito | Ao menos 1 dígito |
| Letra minúscula | Ao menos 1 letra minúscula |
| Letra maiúscula | Ao menos 1 letra maiúscula |
| Caractere especial | Ao menos 1 dos seguintes: `!@#$%^&*()-+` |
| Sem repetição | Não pode ter caracteres repetidos |
| Sem espaços | Espaços em branco não são válidos |

---

## Como executar o projeto

**Pré-requisito:** [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

```bash
cd PasswordValidatorApi
dotnet run
```

O Swagger estará disponível em `(https://localhost:7254/swagger/index.html)`.

### Exemplo de requisição

```bash
curl -X POST https://localhost:7254/validate-password \
  -H "Content-Type: application/json" \
  -d '{"password": "AbTp9!fok"}'
```

### Como rodar os testes

```bash
dotnet test
```

---

## Detalhes da solução

### Arquitetura

A solução foi organizada com separação de responsabilidades em camadas:

```
PasswordValidatorApi/
+-- Controllers/PasswordController.cs   ? Recebe a requisição e devolve a resposta HTTP
+-- Models/PasswordRequest.cs           ? DTO de entrada (propriedade Password como required)
+-- Validators/IPasswordValidator.cs    ? Interface/contrato de validação
+-- Validators/PasswordValidator.cs     ? Implementação das regras de validação
+-- Program.cs                          ? Configuração da aplicação e injeção de dependência
+-- ProgramPartial.cs                   ? Classe partial para viabilizar testes de integração

PasswordValidatorApi.Tests/
+-- PasswordValidatorUnitTests.cs       ? Testes unitários isolados do validador
+-- PasswordValidatorIntegrationTests.cs ? Testes de integração do pipeline HTTP completo
```

### Racional das decisões

**Por que usar uma interface para o validador?**
Criei a `IPasswordValidator` para separar o controller da lógica de validação. A ideia é que o controller não precisa saber *como* a senha é validada, só precisa chamar o método e receber o resultado. Se no futuro as regras mudarem ou precisar de outra implementação, é só trocar no `Program.cs` sem mexer no controller. Além disso, essa separação facilita bastante na hora de escrever testes.

**Por que variáveis booleanas com nomes descritivos?**
Poderia ter colocado tudo direto no `return`, mas aí o código ficaria difícil de ler e de debugar. Com cada regra em uma variável separada (`hasMinimumLength`, `hasDigit`, etc.), fica fácil de entender o que cada verificação faz só de bater o olho, e na hora de debugar dá para ver exatamente qual regra está falhando.

**Por que Regex para validar as regras?**
Usei expressões regulares para verificar se a senha contém dígito, letra maiúscula, letra minúscula e caractere especial. O Regex deixa essa verificação bem direta e curta, em uma linha você expressa exatamente o que está procurando (por exemplo, `[A-Z]` para maiúscula, `\d` para dígito). Principalmente para o caractere especial, o Regex facilita bastante porque consigo listar todos os permitidos (`!@#$%^&*()-+`) num único padrão, sem precisar tratar cada um separadamente.

**Por que HashSet para detectar caracteres repetidos?**
Precisava verificar se algum caractere aparece mais de uma vez. A forma mais simples que pensei foi usar um `HashSet<char>`: ele não aceita duplicatas, então conforme percorro a senha, se o `Add` retorna `false` é porque o caractere já apareceu. Isso resolve o problema percorrendo a string uma vez só.

**Por que 200 e 422?**
Senha válida retorna `200 OK`, senha inválida retorna `422 Unprocessable Entity`. Escolhi o 422 porque a requisição em si está correta (o JSON está bem formado), o problema é que o conteúdo não atende às regras. Quando a propriedade `Password` não é enviada, o próprio ASP.NET Core já retorna `400 Bad Request` por conta do `required` no DTO.

**Por que a classe `ProgramPartial`?**
Como o `Program.cs` usa top-level statements, a classe `Program` fica interna por padrão. Precisei criar essa classe `partial` para que os testes de integração conseguissem acessar o tipo `Program` e usar o `WebApplicationFactory` para subir a API em memória durante os testes.

**Por que dois tipos de teste?**
Fiz testes unitários para validar a lógica do `PasswordValidator` de forma isolada, cada regra tem seu teste, além de cobrir todos os exemplos do enunciado e todos os caracteres especiais. E fiz testes de integração para garantir que a API funciona de ponta a ponta: que o endpoint recebe o JSON, chama o validador e retorna o status code e o body corretos.

### Premissas assumidas

**Caracteres repetidos são case-sensitive:** O desafio pede que a senha não possua caracteres repetidos, mas não especifica se `A` e `a` devem ser tratados como o mesmo caractere. Assumi que são caracteres **diferentes**, já que possuem valores distintos na tabela ASCII/Unicode. Faz sentido para senhas, onde maiúscula e minúscula são coisas diferentes. Isso também é consistente com o exemplo do enunciado: `"AbTp9!foA"` é inválida porque o `A` maiúsculo aparece duas vezes, não porque tem `A` e `a`.
