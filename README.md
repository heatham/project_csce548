# MatchTracker

MatchTracker is a layered .NET application designed to track video game matches and statistics. The system stores information about games, maps, characters, matches, and match statistics, and provides a web interface for creating, viewing, updating, and deleting this data.

The project demonstrates a **multi-layer software architecture** consisting of data, business, API, and client layers.

---

# Project Architecture

The application is divided into four main projects.

## MatchTracker.Data
Contains the data models and repository logic responsible for interacting with the PostgreSQL database.

Responsibilities include:

- Defining entity models
- Handling database connections
- Executing database queries
- Returning data objects to the business layer

---

## MatchTracker.Business
Implements the service layer and business logic used by the application.

Responsibilities include:

- Validating input data
- Applying business rules
- Coordinating between the API and the data layer

---

## MatchTracker.Api
Hosts the backend web API.

Responsibilities include:

- Defining HTTP endpoints
- Accepting client requests
- Calling the business layer
- Returning JSON responses

Example endpoints include:

`/games`  
`/maps`  
`/characters`  
`/matches`  
`/matchstats`

---

## MatchTracker.Web
A Blazor web application that provides the user interface for interacting with the system.

Responsibilities include:

- Displaying data from the API
- Allowing users to perform CRUD operations
- Communicating with the backend API using HTTP requests

---

# Features

The application supports the following functionality.

## Games
- Create a game
- View all games
- View a game by ID
- Update game name
- Delete a game

## Maps
- Create maps for a game
- View all maps
- View a map by ID
- View maps by game
- Update map name
- Delete a map

## Characters
- Create characters for a game
- View all characters
- View character by ID
- View characters by game
- Update character information
- Delete characters

## Matches
- Create match records
- View all matches
- View match by ID
- View matches by game
- Update match information
- Delete matches

## MatchStats
- Create match statistics
- View all match statistics
- View statistics by ID
- View statistics by match
- Update statistics
- Delete statistics

---

# Prerequisites

Before running the project, install the following software.

Required software:

- .NET SDK 8.0
- PostgreSQL
- Visual Studio 2022 or Visual Studio Code
- Git (optional but recommended)
- A modern web browser (Chrome, Edge, or Firefox)

---

# Getting the Project

There are two ways to download the repository.

## Option 1: Clone the Repository

Open a terminal or command prompt and run:

```
git clone <repository-url>
cd MatchTracker
```

## Option 2: Download the ZIP

1. Open the GitHub repository in a browser  
2. Click **Code**  
3. Click **Download ZIP**  
4. Extract the ZIP file  
5. Open the extracted folder in Visual Studio  

---

# Database Setup

The MatchTracker application uses PostgreSQL to store all data.

---

## Step 1: Install PostgreSQL

Download PostgreSQL from:

https://www.postgresql.org/download/

Run the installer and follow the setup steps.

Important configuration during installation:

- Username: postgres
- Port: 5432
- Create and remember the password

---

## Step 2: Create the Database

Open **pgAdmin** and create a new database named:

`matchtracker`

---

## Step 3: Create Tables

Open the Query Tool in pgAdmin and run the following SQL script.

```sql
CREATE TABLE Games (
    GameId SERIAL PRIMARY KEY,
    Name TEXT NOT NULL
);

CREATE TABLE Maps (
    MapId SERIAL PRIMARY KEY,
    GameId INT NOT NULL,
    Name TEXT NOT NULL
);

CREATE TABLE Characters (
    CharacterId SERIAL PRIMARY KEY,
    GameId INT NOT NULL,
    Name TEXT NOT NULL,
    Role TEXT
);

CREATE TABLE Matches (
    MatchId SERIAL PRIMARY KEY,
    GameId INT NOT NULL,
    MatchDate TIMESTAMP NOT NULL,
    QueueType TEXT NOT NULL,
    MapId INT,
    Result CHAR(1),
    DurationSec INT,
    Notes TEXT
);

CREATE TABLE MatchStats (
    StatId SERIAL PRIMARY KEY,
    MatchId INT NOT NULL,
    CharacterId INT,
    Kills INT,
    Deaths INT,
    Assists INT,
    Damage INT,
    Healing INT,
    ObjectiveTimeSec INT
);
```

---

## Step 4: Insert Sample Data

Run the following SQL commands to insert example records.

```sql
INSERT INTO Games(Name)
VALUES ('Marvel Rivals');

INSERT INTO Maps(GameId, Name)
VALUES (1, 'Tokyo 2099');

INSERT INTO Characters(GameId, Name, Role)
VALUES (1, 'Iron Man', 'Damage');

INSERT INTO Matches(GameId, MatchDate, QueueType, Result)
VALUES (1, NOW(), 'QuickPlay', 'W');

INSERT INTO MatchStats(MatchId, Kills, Deaths, Assists)
VALUES (1, 12, 3, 5);
```

---

# Configure the Database Connection

Open the file:

`MatchTracker.Api/appsettings.json`

Update the connection string.

Example:

```json
{
  "ConnectionStrings": {
    "MatchTrackerDb": "Host=localhost;Port=5432;Database=matchtracker;Username=postgres;Password=yourpassword"
  }
}
```

Replace `yourpassword` with the password you created during PostgreSQL installation.

---

# Build the Project

Open a terminal inside the repository folder and run:

```
dotnet restore
dotnet build
```

---

# Running the Backend API

Start the API server using:

```
dotnet run --project MatchTracker.Api
```

You should see something similar to:

```
Now listening on: http://localhost:5000
```

---

# Testing the API

Open a browser and visit:

http://localhost:5000/games

If the API is working correctly, it will return JSON data.

---

# Running the Web Application

Open a second terminal and run:

```
dotnet run --project MatchTracker.Web
```

The application will start and display a local web address such as:

https://localhost:7200

Open that address in a browser.

---

# Using the Web Interface

The web application provides pages for managing each data type.

Navigation routes include:

`/games`  
`/maps`  
`/characters`  
`/matches`  
`/matchstats`

Users can perform full CRUD operations from these pages.

---

# Verifying Deployment

The deployment is successful if:

- The API runs without errors
- The web application runs without errors
- API endpoints return JSON data
- Web pages load correctly
- CRUD operations update the database properly

---

# Troubleshooting

### API returns 404

Possible causes:

- API server not running  
- Incorrect route  
- Wrong port  

Verify that the API server is running.

---

### Web application loads but no data appears

Possible causes:

- API not running  
- Incorrect API Base URL  
- Empty database  

Verify the API URL in:

`MatchTracker.Web/appsettings.json`

---

### Database connection error

Possible causes:

- Incorrect username or password  
- Database not created  
- PostgreSQL service not running  

Check the connection string in the API configuration.

---

# Technologies Used

- C#
- .NET
- ASP.NET Core
- Blazor
- PostgreSQL
- REST API
- JSON

---

# Project Purpose

This project demonstrates how to build a full-stack application using a layered architecture including:

- Data layer
- Business logic layer
- Service/API layer
- Web client layer

The goal is to illustrate how these components interact to create a maintainable and scalable software system.
