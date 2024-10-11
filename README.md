# Erray.EntitiesFiltering

### Simple library for filtering IQueryable collection in your ASP.NET application.
### In your HTTP request add "filters" parameter in the query string.
#### Format: <code>example?filters=Count.>=.4/IsDeleted.==.false</code>
#### Each filter is separated with / sign. Each part is separated with dot.
#### First part: name of entity`s property
#### Second part: operation (!=, ==, <, <=, >, >=)
#### Third part: value to compare