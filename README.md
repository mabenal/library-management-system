# Library Management System

This project is a Library Management System designed to manage library resources efficiently. It includes both a client-side application built with Angular and a server-side application built with .NET Core.

## Project Structure

### Client

The client-side application is built using Angular. It includes various components, services, and modules to handle the user interface and interactions.

- **Angular**: A platform for building mobile and desktop web applications.
- **Karma**: A test runner for JavaScript.
- **Docker**: Used for containerizing the application.

### Server

The server-side application is built using .NET Core. It includes various controllers, services, and models to handle the backend logic and database interactions.

- **.NET Core**: A cross-platform framework for building modern, cloud-based, internet-connected applications.
- **Entity Framework Core**: An ORM framework for .NET.
- **Swashbuckle**: Used for generating Swagger documentation for the API.
- **XUnit**: A testing framework for .NET.

## Getting Started

### Prerequisites

- Node.js
- Angular CLI
- .NET Core SDK
- Docker

### Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/your-repo/library-management-system.git
    ```

2. Navigate to the client directory and install dependencies:
    ```sh
    cd client
    npm install
    npm run build
    ```

3. Navigate to the server directory and install dependencies:
    ```sh
    cd ../server
    dotnet restore
    ```

### Running the Application

#### Client

To run the client-side application, navigate to the  directory and run:
```sh
ng serve