# Biblioteca-Net
# 📚 Biblioteca MVC – Gestión de préstamos con ASP.NET Core

Aplicación web desarrollada con **ASP.NET Core MVC** para la gestión de una biblioteca.  
Permite consultar el catálogo públicamente y gestionar préstamos mediante autenticación con roles diferenciados.

El proyecto implementa lógica de negocio real: control de disponibilidad, renovaciones, devoluciones e histórico de préstamos.

---

## 🚀 Tecnologías utilizadas

- ASP.NET Core MVC  
- Entity Framework Core  
- SQL Server / LocalDB  
- ASP.NET Core Identity  
- Razor Views  
- C#  

---

## 👥 Roles y funcionalidades

### 🔓 Visitante (sin iniciar sesión)

- Consultar el catálogo de libros  
- Ver disponibilidad (**Disponible / No disponible**)  

---

### 👤 Usuario

- Ver sus préstamos activos  
- Consultar el historial completo (activos, renovados y devueltos)  
- Renovar préstamos (según límites establecidos)  

---

### 👨‍💼 Bibliotecario

- Acceso a panel de administración  
- Gestión del catálogo:
  - Añadir libros  
  - Editar libros  
  - Eliminar libros  
  - Ver detalles  
- Crear préstamos  
- Registrar devoluciones  
- Ver listado completo de préstamos  
- Filtrar por estado (**activo / devuelto**)  
- Control automático de disponibilidad de copias  

---

## 📖 Lógica de negocio implementada

- No permite realizar préstamos si no hay copias disponibles  
- Actualización automática de disponibilidad al prestar o devolver  
- Sistema de renovaciones con contador  
- Histórico completo de préstamos por usuario  
- Navegación y acceso diferenciados según rol  
- Catálogo público accesible sin autenticación  

---

## 🔑 Usuarios de prueba (Seed)

Al iniciar la aplicación se crean automáticamente:

**Bibliotecario**
email: "biblio@local.com"
password: "Biblio123!


**Usuario**
email: "usuario@local.com"
password: "Usuario123!"


---

## ⚙️ Cómo ejecutar el proyecto

1. Clonar el repositorio
2. Abrir la solución en **Visual Studio**
3. Ejecutar las migraciones (si es necesario):


4. Ejecutar la aplicación (**F5**)

La base de datos se creará automáticamente.

---

## 🧭 Flujo de navegación

- Página principal con catálogo público
- Redirección automática tras el login según el rol
- Enlaces dinámicos en el inicio según el usuario autenticado

---

## ⭐ Características destacadas

- Autenticación y autorización por roles con **ASP.NET Identity**
- Arquitectura MVC limpia
- Uso de **ViewModels** para las vistas
- Separación de responsabilidades (datos, lógica y presentación)
- Interfaz coherente y adaptada al contexto del usuario

---
## 🎯 Objetivo del proyecto

Proyecto desarrollado para consolidar conocimientos en:

- ASP.NET Core MVC  
- Entity Framework Core  
- Autenticación y autorización  
- Diseño de lógica de negocio real  
- Desarrollo de aplicaciones web completas en .NET  

---

## 📌 Estado del proyecto

Proyecto funcional y en evolución.












Entity Framework Core

Autenticación y autorización

Diseño de lógica de negocio real
