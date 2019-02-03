# sheepIT

## Swagger

Address: http://localhost:5000/swagger/index.html

## Architecture

Project is divided into three primary modules: infrastructure, use cases and core.

### Infrastructure

It mainly contains technical code reused by all other modules.

It should be unrelated to this specific project (meaning it could in theory be used in other projects).

We should strive to have as much technical code moved from other modules to infrastructure, 
to keep them focused on logic instead of technical details.

### Use cases

Represents _specific_ functionalities related to specific endpoints (e. g. web app or public API).

Use cases should be structured in the same way as their enpoints, 
e. g. use cases related to web app should have similar structure as that web app.

Use cases should not reference one another. If some code needs reusing, 
it should either go to infrastructure or core modules.

### Core

Represents functionalities common to entire project. It includes domain 
code (e. g. entities, business rules, value objects, etc.), but also 
technical code related to this project (e. g. data access for specific entities).