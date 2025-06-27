# Memos_Project

**Memos_Project** is a C#/.NET console application that handles a set of diverse tasks:

1. Detecting duplicates in a large array of integers.
2. Retrieving Star Wars ships piloted by characters from the planet Kashyyyk.
3. Modeling a starship crew hierarchy with support for advanced navigation.


##  Project Structure & Features

### 1. Console Interface (`Program.cs`)

Presents a main loop menu offering three primary actions:

1. **Find duplicates** — handled via `HandleDuplicateTask()`
2. **Star Wars ship search** — handled via `FindShipsWithPilotFromKashyykAsync()`
3. **Crew hierarchy management** — handled via `HandleCrewHierarchyTask()`

Each menu option dispatches to a relevant service class and handles user input/output accordingly.


### 2. `DuplicatesService`

**Purpose**: Detect duplicate integers in a randomly generated array of 1,000,000 elements (range 1–100).  
**Key Methods:**

- `FillArray()` — populates the array with random integers.
- `GetDuplicates_HashSet()` — detects duplicates using a `HashSet`.
- `PrintDuplicates_HashSet()` — prints the found duplicates.
- `GetDuplicates_Dictionary()` — counts occurrences using a `Dictionary`.
- `PrintDuplicates_Dictionary()` — prints numbers and how often they appear.



### 3. `ApiService`

**Purpose**: Uses the Star Wars API (SWAPI) to identify ships with pilots from a specified planet (e.g. Kashyyyk).  
**Key Asynchronous Methods:**

- `GetJSON_AllPeopleAsync()`, `GetJSON_AllVehiclesAsync()`, `GetJSON_AllPlanetsAsync()` — handle paginated SWAPI calls.
- `GetPlanetNameAsync(url)`, `GetShipNameAsync(url)` — resolve names from URLs.
- `GetString_ShipsOfPilotsFromPlanetAsync(...)` — finds ships piloted by characters from a target planet.
- Additional LINQ-based methods and helper transformers for optimized processing.


### 4. `CrewHierarchy` & `CrewMember`

**Purpose**: Models the crew structure of the starship Enterprise (led by Captain Jean-Luc Picard) and provides navigation utilities.  
**Components:**

- `CrewMember` — defines a crew member with:
  - `Name`
  - `Commander` (superior officer)
  - `Subordinates` (list of direct reports)

- `CrewHierarchy` — builds a full crew hierarchy and offers:
  - Recursive structure from Captain → Officers → Specialists → Recruits
  - **Key Methods**:
    - `PrintCrewHierarchy(member, level)` — prints the tree-like hierarchy from a member downward.
    - `PrintHierarchyUpwards(member)` — walks the chain of command from a member up to the captain.
    - `FindCrewMemberByName(root, name)` — depth-first search by name.
    - `GetAllSubordinates(member)` — recursively retrieves all subordinates.
    - `GetInfectedUntilCaptain(Captain, name)` — returns a list of members up the chain (e.g., for infection tracking).
- **Design Benefits**:
    - Bidirectional links: commanders ↔ subordinates.
    - Recursion allows complex organization modeling.
    - Ready for extension with ranks, IDs, roles, etc.


#  Suggestions for Improvement

1. **API caching** — cache planet and ship name lookups to reduce redundant HTTP calls.
2. **Parallelization** — use `Task.WhenAll` to optimize asynchronous web requests.
3. **Unit testing** — introduce a test project to validate `DuplicatesService`, `CrewHierarchy`, and `ApiService`.
4. **User interface** — enhance UX with clear screen transitions, error handling, and navigation prompts.
5. **Export feature** — allow exporting crew hierarchy as JSON or XML.
6. **Loop detection** — add safety checks to prevent cyclic links in crew structure.
7. **Data enrichment** — extend `CrewMember` with attributes like `Rank`, `ID`, `Position`, etc.

## Technologies Used

- C# 13 / .NET 9
- `System.Text.Json` and `Newtonsoft.Json` for JSON processing
- [SWAPI.dev](https://swapi.dev) — Star Wars API
- Console-based input/output

## How to Run

```bash
git clone https://github.com/00DRUG/Memos_Project.git
cd Memos_Project
dotnet build
dotnet run
