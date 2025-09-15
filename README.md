# Yet Another To-Do App

A simple to-do application built as a trial task.
This project demonstrates a full-stack setup with separate frontend and backend services,
containerized using Docker for easy deployment and development.
- **Backend**: ASP.NET Core 9 API
- **Frontend**: Vue 3 + Vite
- **Database**: PostgreSQL

Why this stack?
- **ASP.NET Core**: a powerful and flexible framework for building web APIs in C#, a language I enjoy working with.
- **Vue 3 + Vite**: offers modern, fast, and efficient frontend development,
allowing for rapid prototyping and an excellent developer experience. It is also the frontend framework I'm the most familiar with.
- **PostgreSQL**: a robust and widely-used relational database, ideal for handling structured data reliably.


## Running the app

1. Clone the repository
```bash
git clone -b variant-2 https://github.com/anviks/yet-another-todo-app.git
cd yet-another-todo-app
```

2. Copy `.env.example` to `.env` for both backend and frontend, and adjust settings if needed
```bash
cp backend/.env.example backend/.env
cp frontend/.env.example frontend/.env
```

3. Start all services
```bash
docker compose up -d --build
```

4. Access the app
- Frontend: http://localhost:8080
- Backend: http://localhost:5000/api/todo

5. View logs (optional)
```bash
docker compose logs -f backend
docker compose logs -f frontend
```

6. Seed database with random to-do tasks (optional)
```bash
docker compose exec backend dotnet /app/seeder/YetAnotherTodoApp.Seeder.dll --count [count]
```


## Docker notes

- **Backend**
  - **Container port**: 80
  - **Host port**: 5000
  - **Container port configuration**: `ASPNETCORE_URLS=http://+:80` (set in `docker-compose.yml` under `environment`)
  - **Changing the host port**:
    - Update the `ports` mapping in `docker-compose.yml`
    - Update `VITE_BACKEND_URL` in `frontend/.env` to match the new host port
    
- **Frontend**
  - **Container port**: 80 (default Nginx port)
  - **Host port**: 8080
  - **Changing the host port**:
    - Update the `ports` mapping in `docker-compose.yml`
    - Update `ALLOWEDORIGINS__n` in `backend/.env` to permit requests from the new frontend URL

