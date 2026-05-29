# Instrucciones generales para GitHub Copilot

Este proyecto usa .NET, C#, ASP.NET Core, Razor Pages y Entity Framework Core.

## Idioma y nomenclatura

- Todos los nombres de clases, entidades, propiedades, métodos, variables, parámetros, interfaces, enums, archivos y namespaces deben estar en inglés.
- No usar nombres en español para identificadores de código.
- Los comentarios dentro del código deben estar en español.
- No agregar comentarios obvios. Comentar solo cuando ayude a entender una decisión, regla de negocio o comportamiento no evidente.
- Los mensajes visibles para el usuario pueden estar en español, salvo que el proyecto ya use otro idioma.
- Los mensajes de error, validaciones y todos los visibles para el usuario deben estar en español, salvo que el proyecto ya use otro idioma.

## Estilo de código

- Mantener el estilo existente del proyecto antes de proponer cambios.
- Usar nombres claros y descriptivos.
- Evitar abreviaturas innecesarias.
- No mezclar idiomas en el mismo identificador.
- Preferir código simple y legible antes que soluciones demasiado genéricas.
- No cambiar nombres existentes salvo que se pida explícitamente.

## .NET / C#

- Usar PascalCase para clases, propiedades, métodos, enums e interfaces.
- Usar camelCase para variables locales y parámetros.
- Prefijar interfaces con `I`, por ejemplo `IPatientService`.
- Usar `async`/`await` cuando corresponda.
- Los métodos asincrónicos deben terminar en `Async`.
- Evitar lógica de negocio compleja dentro de Razor Pages cuando pueda moverse a servicios.
- Usar inyección de dependencias para servicios y contextos.
- No acceder directamente a la base de datos desde la vista `.cshtml`.

## Entity Framework Core

- Las entidades deben tener nombres en inglés.
- Las propiedades de navegación deben tener nombres claros en inglés.
- No crear migraciones automáticamente salvo que se solicite.
- Cuando se modifique el modelo de datos, explicar si hace falta crear una migración.

## Respuestas de Copilot

- Cuando se pidan cambios de código, mostrar preferentemente solo el código necesario y no archivos completos, salvo que se solicite.
- Explicar brevemente qué se cambió y por qué.