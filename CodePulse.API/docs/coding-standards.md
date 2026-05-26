## **.NET Coding Standards Guide (Practical + Enterprise)** 

A combined guide for ASP.NET Core / C# applications with practical and enterprise-grade standards. 

## **Naming Conventions** 

- Use PascalCase for methods/classes. 

- Use camelCase for variables and parameters. 

- Prefix interfaces with I. 

- Use meaningful names and avoid abbreviations. 

## **Method Standards** 

- Keep methods focused on a single responsibility. 

- Prefer early returns to reduce nesting. 

- Use async/await properly and suffix async methods with Async. 

- Avoid magic numbers. 

## **Clean Code** 

- Write self-explanatory code. 

- Validate nulls and inputs. 

- Use structured logging and meaningful exceptions. 

## **API Standards** 

- Use REST naming conventions. 

- Return proper status codes. 

- Use DTOs instead of entities. 

## **Entity Framework** 

- Use AsNoTracking for read-only queries. 

- Avoid N+1 queries. 

- Project only needed fields. 

## **Architecture** 

- Use dependency injection. 

- Keep controllers thin. 

- Follow SOLID principles. 

## **Code Review Checklist** 

- Readability 

- Performance 

- Security 

- Null handling 

- Testability 

## **Examples** 

```
public async Task<UserResponse> GetUserByIdAsync(int userId)
{
```

```
    if(userId <= 0)
        throw new ArgumentException(nameof(userId));
```

```
    return await _userRepository.GetByIdAsync(userId);
}
```

